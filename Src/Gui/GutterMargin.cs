// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="GutterMargin.cs" company="">
//    This program is for creating a Code Editor control.
//    Copyright ©  2017  Zeroit Dev Technologies
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//    You can contact me at zeroitdevnet@gmail.com or zeroitdev@outlook.com
// </copyright>
// <summary></summary>
// ***********************************************************************


using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor
{
	/// <summary>
	/// This class views the line numbers and folding markers.
	/// </summary>
	public class GutterMargin : AbstractMargin, IDisposable
	{
		StringFormat numberStringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
		
		public static Cursor RightLeftCursor;
		
		static GutterMargin()
		{
			Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("Zeroit.Framework.CodeEditor.TextEditor.Resources.RightArrow.cur");
			if (cursorStream == null) throw new Exception("could not find cursor resource");
			RightLeftCursor = new Cursor(cursorStream);
			cursorStream.Close();
		}
		
		public void Dispose()
		{
			numberStringFormat.Dispose();
		}
		
		public override Cursor Cursor {
			get {
				return RightLeftCursor;
			}
		}
		
		public override Size Size {
			get {
				return new Size((int)(textArea.TextView.WideSpaceWidth
				                      * Math.Max(3, (int)Math.Log10(textArea.Document.TotalNumberOfLines) + 1)),
				                -1);
			}
		}
		
		public override bool IsVisible {
			get {
				return textArea.TextEditorProperties.ShowLineNumbers;
			}
		}
		
		public GutterMargin(TextArea textArea) : base(textArea)
		{
			numberStringFormat.LineAlignment = StringAlignment.Far;
			numberStringFormat.FormatFlags   = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox |
				StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
		}
		
		public override void Paint(Graphics g, Rectangle rect, Color BackColor)
		{
			if (rect.Width <= 0 || rect.Height <= 0) {
				return;
			}
			HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
			int fontHeight = textArea.TextView.FontHeight;
			Brush fillBrush = textArea.Enabled ? BrushRegistry.GetBrush(/*lineNumberPainterColor.BackgroundColor*/ BackColor) : SystemBrushes.InactiveBorder;
			Brush drawBrush = BrushRegistry.GetBrush(lineNumberPainterColor.Color);
			for (int y = 0; y < (DrawingPosition.Height + textArea.TextView.VisibleLineDrawingRemainder) / fontHeight + 1; ++y) {
				int ypos = drawingPosition.Y + fontHeight * y  - textArea.TextView.VisibleLineDrawingRemainder;
				Rectangle backgroundRectangle = new Rectangle(drawingPosition.X, ypos, drawingPosition.Width, fontHeight);
				if (rect.IntersectsWith(backgroundRectangle)) {
					g.FillRectangle(fillBrush, backgroundRectangle);
					int curLine = textArea.Document.GetFirstLogicalLine(textArea.Document.GetVisibleLine(textArea.TextView.FirstVisibleLine) + y);
					
					if (curLine < textArea.Document.TotalNumberOfLines) {
						g.DrawString((curLine + 1).ToString(),
						             lineNumberPainterColor.GetFont(TextEditorProperties.FontContainer),
						             drawBrush,
						             backgroundRectangle,
						             numberStringFormat);
					}
				}
			}
		}

		public override void HandleMouseDown(Point mousepos, MouseButtons mouseButtons)
		{
			TextLocation selectionStartPos;

			textArea.SelectionManager.selectFrom.where = WhereFrom.Gutter;
			int realline = textArea.TextView.GetLogicalLine(mousepos.Y);
			if (realline >= 0 && realline < textArea.Document.TotalNumberOfLines) {
				// shift-select
				if((Control.ModifierKeys & Keys.Shift) != 0) {
					if(!textArea.SelectionManager.HasSomethingSelected && realline != textArea.Caret.Position.Y) {
						if (realline >= textArea.Caret.Position.Y)
						{ // at or below starting selection, place the cursor on the next line
							// nothing is selected so make a new selection from cursor
							selectionStartPos = textArea.Caret.Position;
							// whole line selection - start of line to start of next line
							if (realline < textArea.Document.TotalNumberOfLines - 1)
							{
								textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new TextLocation(0, realline + 1)));
								textArea.Caret.Position = new TextLocation(0, realline + 1);
							}
							else
							{
								textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new TextLocation(textArea.Document.GetLineSegment(realline).Length + 1, realline)));
								textArea.Caret.Position = new TextLocation(textArea.Document.GetLineSegment(realline).Length + 1, realline);
							}
						}
						else
						{ // prior lines to starting selection, place the cursor on the same line as the new selection
							// nothing is selected so make a new selection from cursor
							selectionStartPos = textArea.Caret.Position;
							// whole line selection - start of line to start of next line
							textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new TextLocation(selectionStartPos.X, selectionStartPos.Y)));
							textArea.SelectionManager.ExtendSelection(new TextLocation(selectionStartPos.X, selectionStartPos.Y), new TextLocation(0, realline));
							textArea.Caret.Position = new TextLocation(0, realline);
						}
					}
					else
					{
						// let MouseMove handle a shift-click in a gutter
						MouseEventArgs e = new MouseEventArgs(mouseButtons, 1, mousepos.X, mousepos.Y, 0);
						textArea.RaiseMouseMove(e);
					}
				} else { // this is a new selection with no shift-key
					// sync the textareamousehandler mouse location
					// (fixes problem with clicking out into a menu then back to the gutter whilst
					// there is a selection)
					textArea.mousepos = mousepos;

					selectionStartPos = new TextLocation(0, realline);
					textArea.SelectionManager.ClearSelection();
					// whole line selection - start of line to start of next line
					if (realline < textArea.Document.TotalNumberOfLines - 1)
					{
						textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, selectionStartPos, new TextLocation(selectionStartPos.X, selectionStartPos.Y + 1)));
						textArea.Caret.Position = new TextLocation(selectionStartPos.X, selectionStartPos.Y + 1);
					}
					else
					{
						textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document, new TextLocation(0, realline), new TextLocation(textArea.Document.GetLineSegment(realline).Length + 1, selectionStartPos.Y)));
						textArea.Caret.Position = new TextLocation(textArea.Document.GetLineSegment(realline).Length + 1, selectionStartPos.Y);
					}
				}
			}
		}
	}
}
