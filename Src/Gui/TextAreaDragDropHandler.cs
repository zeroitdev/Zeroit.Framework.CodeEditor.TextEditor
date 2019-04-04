// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TextAreaDragDropHandler.cs" company="">
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
using System.Windows.Forms;

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor
{
	public class TextAreaDragDropHandler
	{
		public static Action<Exception> OnDragDropException = ex => MessageBox.Show(ex.ToString());
		
		TextArea textArea;
		
		public void Attach(TextArea textArea)
		{
			this.textArea = textArea;
			textArea.AllowDrop = true;
			
			textArea.DragEnter += MakeDragEventHandler(OnDragEnter);
			textArea.DragDrop  += MakeDragEventHandler(OnDragDrop);
			textArea.DragOver  += MakeDragEventHandler(OnDragOver);
		}
		
		/// <summary>
		/// Create a drag'n'drop event handler.
		/// Windows Forms swallows unhandled exceptions during drag'n'drop, so we report them here.
		/// </summary>
		static DragEventHandler MakeDragEventHandler(DragEventHandler h)
		{
			return (sender, e) => {
				try {
					h(sender, e);
				} catch (Exception ex) {
					OnDragDropException(ex);
				}
			};
		}
		
		static DragDropEffects GetDragDropEffect(DragEventArgs e)
		{
			if ((e.AllowedEffect & DragDropEffects.Move) > 0 &&
			    (e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Move) > 0) {
				return DragDropEffects.Move;
			} else if ((e.AllowedEffect & DragDropEffects.Copy) > 0) {
				return DragDropEffects.Copy;
			}
			return DragDropEffects.None;
		}
		
		protected void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(string))) {
				e.Effect = GetDragDropEffect(e);
			}
		}
		
		
		void InsertString(int offset, string str)
		{
			textArea.Document.Insert(offset, str);
			
			textArea.SelectionManager.SetSelection(new DefaultSelection(textArea.Document,
			                                                            textArea.Document.OffsetToPosition(offset),
			                                                            textArea.Document.OffsetToPosition(offset + str.Length)));
			textArea.Caret.Position = textArea.Document.OffsetToPosition(offset + str.Length);
			textArea.Refresh();
		}
		
		protected void OnDragDrop(object sender, DragEventArgs e)
		{
			Point p = textArea.PointToClient(new Point(e.X, e.Y));
			
			if (e.Data.GetDataPresent(typeof(string))) {
				textArea.BeginUpdate();
				textArea.Document.UndoStack.StartUndoGroup();
				try {
					int offset = textArea.Caret.Offset;
					if (textArea.IsReadOnly(offset)) {
						// prevent dragging text into readonly section
						return;
					}
					if (e.Data.GetDataPresent(typeof(DefaultSelection))) {
						ISelection sel = (ISelection)e.Data.GetData(typeof(DefaultSelection));
						if (sel.ContainsPosition(textArea.Caret.Position)) {
							return;
						}
						if (GetDragDropEffect(e) == DragDropEffects.Move) {
							if (SelectionManager.SelectionIsReadOnly(textArea.Document, sel)) {
								// prevent dragging text out of readonly section
								return;
							}
							int len = sel.Length;
							textArea.Document.Remove(sel.Offset, len);
							if (sel.Offset < offset) {
								offset -= len;
							}
						}
					}
					textArea.SelectionManager.ClearSelection();
					InsertString(offset, (string)e.Data.GetData(typeof(string)));
					textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				} finally {
					textArea.Document.UndoStack.EndUndoGroup();
					textArea.EndUpdate();
				}
			}
		}
		
		protected void OnDragOver(object sender, DragEventArgs e)
		{
			if (!textArea.Focused) {
				textArea.Focus();
			}
			
			Point p = textArea.PointToClient(new Point(e.X, e.Y));
			
			if (textArea.TextView.DrawingPosition.Contains(p.X, p.Y)) {
				TextLocation realmousepos= textArea.TextView.GetLogicalPosition(p.X - textArea.TextView.DrawingPosition.X,
				                                                                p.Y - textArea.TextView.DrawingPosition.Y);
				int lineNr = Math.Min(textArea.Document.TotalNumberOfLines - 1, Math.Max(0, realmousepos.Y));
				
				textArea.Caret.Position = new TextLocation(realmousepos.X, lineNr);
				textArea.SetDesiredColumn();
				if (e.Data.GetDataPresent(typeof(string)) && !textArea.IsReadOnly(textArea.Caret.Offset)) {
					e.Effect = GetDragDropEffect(e);
				} else {
					e.Effect = DragDropEffects.None;
				}
			} else {
				e.Effect = DragDropEffects.None;
			}
		}
	}
}
