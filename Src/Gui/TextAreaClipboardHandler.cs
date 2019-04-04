﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TextAreaClipboardHandler.cs" company="">
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Zeroit.Framework.CodeEditor.TextEditor.Document;
using Zeroit.Framework.CodeEditor.TextEditor.Util;

namespace Zeroit.Framework.CodeEditor.TextEditor
{
	public class TextAreaClipboardHandler
	{
		TextArea textArea;
		
		public bool EnableCut {
			get {
				return textArea.EnableCutOrPaste; //textArea.SelectionManager.HasSomethingSelected;
			}
		}
		
		public bool EnableCopy {
			get {
				return true; //textArea.SelectionManager.HasSomethingSelected;
			}
		}
		
		public bool EnablePaste {
			get {
				if (!textArea.EnableCutOrPaste)
					return false;
				try {
					return Clipboard.ContainsText();
				} catch (ExternalException) {
					return false;
				}
			}
		}
		
		public bool EnableDelete {
			get {
				return textArea.SelectionManager.HasSomethingSelected && !textArea.SelectionManager.SelectionIsReadonly;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}
		
		public TextAreaClipboardHandler(TextArea textArea)
		{
			this.textArea = textArea;
			textArea.SelectionManager.SelectionChanged += new EventHandler(DocumentSelectionChanged);
		}
		
		void DocumentSelectionChanged(object sender, EventArgs e)
		{
//			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}

		const string LineSelectedType = "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy
		
		bool CopyTextToClipboard(string stringToCopy, bool asLine)
		{
			if (stringToCopy.Length > 0) {
				DataObject dataObject = new DataObject();
				dataObject.SetData(DataFormats.UnicodeText, true, stringToCopy);
				if (asLine) {
					MemoryStream lineSelected = new MemoryStream(1);
					lineSelected.WriteByte(1);
					dataObject.SetData(LineSelectedType, false, lineSelected);
				}
				// Default has no highlighting, therefore we don't need RTF output
				if (textArea.Document.HighlightingStrategy.Name != "Default") {
					dataObject.SetData(DataFormats.Rtf, RtfWriter.GenerateRtf(textArea));
				}
				OnCopyText(new CopyTextEventArgs(stringToCopy));
				
				SafeSetClipboard(dataObject);
				return true;
			} else {
				return false;
			}
		}
		
		// Code duplication: TextAreaClipboardHandler.cs also has SafeSetClipboard
		[ThreadStatic] static int SafeSetClipboardDataVersion;
		
		static void SafeSetClipboard(object dataObject)
		{
			// Work around ExternalException bug. (SD2-426)
			// Best reproducable inside Virtual PC.
			int version = unchecked(++SafeSetClipboardDataVersion);
			try {
				Clipboard.SetDataObject(dataObject, true);
			} catch (ExternalException) {
				Timer timer = new Timer();
				timer.Interval = 100;
				timer.Tick += delegate {
					timer.Stop();
					timer.Dispose();
					if (SafeSetClipboardDataVersion == version) {
						try {
							Clipboard.SetDataObject(dataObject, true, 10, 50);
						} catch (ExternalException) { }
					}
				};
				timer.Start();
			}
		}

		bool CopyTextToClipboard(string stringToCopy)
		{
			return CopyTextToClipboard(stringToCopy, false);
		}
		
		public void Cut(object sender, EventArgs e)
		{
			if (textArea.SelectionManager.HasSomethingSelected) {
				if (CopyTextToClipboard(textArea.SelectionManager.SelectedText)) {
					if (textArea.SelectionManager.SelectionIsReadonly)
						return;
					// Remove text
					textArea.BeginUpdate();
					textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
					textArea.SelectionManager.RemoveSelectedText();
					textArea.EndUpdate();
				}
			} else if (textArea.Document.TextEditorProperties.CutCopyWholeLine) {
				// No text was selected, select and cut the entire line
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset), textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset + lineWhereCaretIs.TotalLength));
				if (CopyTextToClipboard(caretLineText, true)) {
					if (textArea.SelectionManager.SelectionIsReadonly)
						return;
					// remove line
					textArea.BeginUpdate();
					textArea.Caret.Position = textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset);
					textArea.SelectionManager.RemoveSelectedText();
					textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr)));
					textArea.EndUpdate();
				}
			}
		}
		
		public void Copy(object sender, EventArgs e)
		{
			if (!CopyTextToClipboard(textArea.SelectionManager.SelectedText) && textArea.Document.TextEditorProperties.CutCopyWholeLine) {
				// No text was selected, select the entire line, copy it, and then deselect
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				CopyTextToClipboard(caretLineText, true);
			}
		}
		
		public void Paste(object sender, EventArgs e)
		{
			if (!textArea.EnableCutOrPaste) {
				return;
			}
			// Clipboard.GetDataObject may throw an exception...
			for (int i = 0;; i++) {
				try {
					IDataObject data = Clipboard.GetDataObject();
					if (data == null)
						return;
					bool fullLine = data.GetDataPresent(LineSelectedType);
					if (data.GetDataPresent(DataFormats.UnicodeText)) {
						string text = (string)data.GetData(DataFormats.UnicodeText);
						if (text.Length > 0) {
							textArea.Document.UndoStack.StartUndoGroup();
							try {
								if (textArea.SelectionManager.HasSomethingSelected) {
									textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
									textArea.SelectionManager.RemoveSelectedText();
								}
								if (fullLine) {
									int col = textArea.Caret.Column;
									textArea.Caret.Column = 0;
									if (!textArea.IsReadOnly(textArea.Caret.Offset))
										textArea.InsertString(text);
									textArea.Caret.Column = col;
								} else {
									// textArea.EnableCutOrPaste already checked readonly for this case
									textArea.InsertString(text);
								}
							} finally {
								textArea.Document.UndoStack.EndUndoGroup();
							}
						}
					}
					return;
				} catch (ExternalException) {
					// GetDataObject does not provide RetryTimes parameter
					if (i > 5) throw;
				}
			}
		}
		
		public void Delete(object sender, EventArgs e)
		{
			new Zeroit.Framework.CodeEditor.TextEditor.Actions.Delete().Execute(textArea);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			new Zeroit.Framework.CodeEditor.TextEditor.Actions.SelectWholeDocument().Execute(textArea);
		}
		
		protected virtual void OnCopyText(CopyTextEventArgs e)
		{
			if (CopyText != null) {
				CopyText(this, e);
			}
		}
		
		public event CopyTextEventHandler CopyText;
	}
	
	public delegate void CopyTextEventHandler(object sender, CopyTextEventArgs e);
	public class CopyTextEventArgs : EventArgs
	{
		string text;
		
		public string Text {
			get {
				return text;
			}
		}
		
		public CopyTextEventArgs(string text)
		{
			this.text = text;
		}
	}
}