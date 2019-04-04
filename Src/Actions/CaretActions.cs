// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="CaretActions.cs" company="">
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
using System.Collections.Generic;
using System.Drawing;

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor.Actions
{
	public class CaretLeft : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation position = textArea.Caret.Position;
			List<FoldMarker> foldings = textArea.Document.FoldingManager.GetFoldedFoldingsWithEnd(position.Y);
			FoldMarker justBeforeCaret = null;
			foreach (FoldMarker fm in foldings) {
				if (fm.EndColumn == position.X) {
					justBeforeCaret = fm;
					break; // the first folding found is the folding with the smallest Startposition
				}
			}
			
			if (justBeforeCaret != null) {
				position.Y = justBeforeCaret.StartLine;
				position.X = justBeforeCaret.StartColumn;
			} else {
				if (position.X > 0) {
					--position.X;
				} else if (position.Y  > 0) {
					LineSegment lineAbove = textArea.Document.GetLineSegment(position.Y - 1);
					position = new TextLocation(lineAbove.Length, position.Y - 1);
				}
			}
			
			textArea.Caret.Position = position;
			textArea.SetDesiredColumn();
		}
	}
	
	public class CaretRight : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			LineSegment curLine = textArea.Document.GetLineSegment(textArea.Caret.Line);
			TextLocation position = textArea.Caret.Position;
			List<FoldMarker> foldings = textArea.Document.FoldingManager.GetFoldedFoldingsWithStart(position.Y);
			FoldMarker justBehindCaret = null;
			foreach (FoldMarker fm in foldings) {
				if (fm.StartColumn == position.X) {
					justBehindCaret = fm;
					break;
				}
			}
			if (justBehindCaret != null) {
				position.Y = justBehindCaret.EndLine;
				position.X = justBehindCaret.EndColumn;
			} else { // no folding is interesting
				if (position.X < curLine.Length || textArea.TextEditorProperties.AllowCaretBeyondEOL) {
					++position.X;
				} else if (position.Y + 1 < textArea.Document.TotalNumberOfLines) {
					++position.Y;
					position.X = 0;
				}
			}
			textArea.Caret.Position = position;
			textArea.SetDesiredColumn();
		}
	}
	
	public class CaretUp : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation position = textArea.Caret.Position;
			int lineNr = position.Y;
			int visualLine = textArea.Document.GetVisibleLine(lineNr);
			if (visualLine > 0) {
				Point pos = new Point(textArea.TextView.GetDrawingXPos(lineNr, position.X),
				                      textArea.TextView.DrawingPosition.Y + (visualLine - 1) * textArea.TextView.FontHeight - textArea.TextView.TextArea.VirtualTop.Y);
				textArea.Caret.Position = textArea.TextView.GetLogicalPosition(pos);
				textArea.SetCaretToDesiredColumn();
			}
//			if (textArea.Caret.Line  > 0) {
//				textArea.SetCaretToDesiredColumn(textArea.Caret.Line - 1);
//			}
		}
	}
	
	public class CaretDown : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation position = textArea.Caret.Position;
			int lineNr = position.Y;
			int visualLine = textArea.Document.GetVisibleLine(lineNr);
			if (visualLine < textArea.Document.GetVisibleLine(textArea.Document.TotalNumberOfLines)) {
				Point pos = new Point(textArea.TextView.GetDrawingXPos(lineNr, position.X),
				                      textArea.TextView.DrawingPosition.Y
				                      + (visualLine + 1) * textArea.TextView.FontHeight
				                      - textArea.TextView.TextArea.VirtualTop.Y);
				textArea.Caret.Position = textArea.TextView.GetLogicalPosition(pos);
				textArea.SetCaretToDesiredColumn();
			}
//			if (textArea.Caret.Line + 1 < textArea.Document.TotalNumberOfLines) {
//				textArea.SetCaretToDesiredColumn(textArea.Caret.Line + 1);
//			}
		}
	}
	
	public class WordRight : CaretRight
	{
		public override void Execute(TextArea textArea)
		{
			LineSegment line   = textArea.Document.GetLineSegment(textArea.Caret.Position.Y);
			TextLocation oldPos = textArea.Caret.Position;
			TextLocation newPos;
			if (textArea.Caret.Column >= line.Length) {
				newPos = new TextLocation(0, textArea.Caret.Line + 1);
			} else {
				int nextWordStart = TextUtilities.FindNextWordStart(textArea.Document, textArea.Caret.Offset);
				newPos = textArea.Document.OffsetToPosition(nextWordStart);
			}
			
			// handle fold markers
			List<FoldMarker> foldings = textArea.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
			foreach (FoldMarker marker in foldings) {
				if (marker.IsFolded) {
					if (oldPos.X == marker.StartColumn && oldPos.Y == marker.StartLine) {
						newPos = new TextLocation(marker.EndColumn, marker.EndLine);
					} else {
						newPos = new TextLocation(marker.StartColumn, marker.StartLine);
					}
					break;
				}
			}
			
			textArea.Caret.Position = newPos;
			textArea.SetDesiredColumn();
		}
	}
	
	public class WordLeft : CaretLeft
	{
		public override void Execute(TextArea textArea)
		{
			TextLocation oldPos = textArea.Caret.Position;
			if (textArea.Caret.Column == 0) {
				base.Execute(textArea);
			} else {
				LineSegment line   = textArea.Document.GetLineSegment(textArea.Caret.Position.Y);
				
				int prevWordStart = TextUtilities.FindPrevWordStart(textArea.Document, textArea.Caret.Offset);
				
				TextLocation newPos = textArea.Document.OffsetToPosition(prevWordStart);
				
				// handle fold markers
				List<FoldMarker> foldings = textArea.Document.FoldingManager.GetFoldingsFromPosition(newPos.Y, newPos.X);
				foreach (FoldMarker marker in foldings) {
					if (marker.IsFolded) {
						if (oldPos.X == marker.EndColumn && oldPos.Y == marker.EndLine) {
							newPos = new TextLocation(marker.StartColumn, marker.StartLine);
						} else {
							newPos = new TextLocation(marker.EndColumn, marker.EndLine);
						}
						break;
					}
				}
				textArea.Caret.Position = newPos;
				textArea.SetDesiredColumn();
			}
			
			
		}
	}
	
	public class ScrollLineUp : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.AutoClearSelection = false;
			
			textArea.MotherTextAreaControl.VScrollBar.Value = Math.Max(textArea.MotherTextAreaControl.VScrollBar.Minimum,
			                                                           textArea.VirtualTop.Y - textArea.TextView.FontHeight);
		}
	}
	
	public class ScrollLineDown : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.AutoClearSelection = false;
			textArea.MotherTextAreaControl.VScrollBar.Value = Math.Min(textArea.MotherTextAreaControl.VScrollBar.Maximum,
			                                                           textArea.VirtualTop.Y + textArea.TextView.FontHeight);
		}
	}
}
