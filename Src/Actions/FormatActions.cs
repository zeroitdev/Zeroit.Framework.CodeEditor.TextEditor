﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="FormatActions.cs" company="">
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
using System.Text;
using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor.Actions 
{
	public abstract class AbstractLineFormatAction : AbstractEditAction
	{
		protected TextArea textArea;
		abstract protected void Convert(IDocument document, int startLine, int endLine);
		
		public override void Execute(TextArea textArea)
		{
			this.textArea = textArea;
			textArea.BeginUpdate();
			textArea.Document.UndoStack.StartUndoGroup();
			if (textArea.SelectionManager.HasSomethingSelected) {
				foreach (ISelection selection in textArea.SelectionManager.SelectionCollection) {
					Convert(textArea.Document, selection.StartPosition.Y, selection.EndPosition.Y);
				}
			} else {
				Convert(textArea.Document, 0, textArea.Document.TotalNumberOfLines - 1);
			}
			textArea.Document.UndoStack.EndUndoGroup();
			textArea.Caret.ValidateCaretPos();
			textArea.EndUpdate();
			textArea.Refresh();
		}
	}
	
	public abstract class AbstractSelectionFormatAction : AbstractEditAction
	{
		protected TextArea textArea;
		abstract protected void Convert(IDocument document, int offset, int length);
		
		public override void Execute(TextArea textArea)
		{
			this.textArea = textArea;
			textArea.BeginUpdate();
			if (textArea.SelectionManager.HasSomethingSelected) {
				foreach (ISelection selection in textArea.SelectionManager.SelectionCollection) {
					Convert(textArea.Document, selection.Offset, selection.Length);
				}
			} else {
				Convert(textArea.Document, 0, textArea.Document.TextLength);
			}
			textArea.Caret.ValidateCaretPos();
			textArea.EndUpdate();
			textArea.Refresh();
		}
	}
	
	public class RemoveLeadingWS : AbstractLineFormatAction
	{
		protected override void Convert(IDocument document, int y1, int y2) 
		{
			for (int i = y1; i < y2; ++i) {
				LineSegment line = document.GetLineSegment(i);
				int removeNumber = 0;
				for (int x = line.Offset; x < line.Offset + line.Length && Char.IsWhiteSpace(document.GetCharAt(x)); ++x) {
					++removeNumber;
				}
				if (removeNumber > 0) {
					document.Remove(line.Offset, removeNumber);
				}
			}
		}
	}
	
	public class RemoveTrailingWS : AbstractLineFormatAction
	{
		protected override void Convert(IDocument document, int y1, int y2) 
		{
			for (int i = y2 - 1; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				int removeNumber = 0;
				for (int x = line.Offset + line.Length - 1; x >= line.Offset && Char.IsWhiteSpace(document.GetCharAt(x)); --x) {
					++removeNumber;
				}
				if (removeNumber > 0) {
					document.Remove(line.Offset + line.Length - removeNumber, removeNumber);
				}
			}
		}
	}
	
	
	public class ToUpperCase : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length).ToUpper();
			document.Replace(startOffset, length, what);
		}
	}
	
	public class ToLowerCase : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length).ToLower();
			document.Replace(startOffset, length, what);
		}
	}
	
	public class InvertCaseAction : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			StringBuilder what = new StringBuilder(document.GetText(startOffset, length));
			
			for (int i = 0; i < what.Length; ++i) {
				what[i] = Char.IsUpper(what[i]) ? Char.ToLower(what[i]) : Char.ToUpper(what[i]);
			}
			
			document.Replace(startOffset, length, what.ToString());
		}
	}
	
	public class CapitalizeAction : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			StringBuilder what = new StringBuilder(document.GetText(startOffset, length));
			
			for (int i = 0; i < what.Length; ++i) {
				if (!Char.IsLetter(what[i]) && i < what.Length - 1) {
					what[i + 1] = Char.ToUpper(what[i + 1]);
				}
			}
			document.Replace(startOffset, length, what.ToString());
		}
		
	}
	
	public class ConvertTabsToSpaces : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length);
			string spaces = new string(' ', document.TextEditorProperties.TabIndent);
			document.Replace(startOffset, length, what.Replace("\t", spaces));
		}
	}
	
	public class ConvertSpacesToTabs : AbstractSelectionFormatAction
	{
		protected override void Convert(IDocument document, int startOffset, int length)
		{
			string what = document.GetText(startOffset, length);
			string spaces = new string(' ', document.TextEditorProperties.TabIndent);
			document.Replace(startOffset, length, what.Replace(spaces, "\t"));
		}
	}
	
	public class ConvertLeadingTabsToSpaces : AbstractLineFormatAction
	{
		protected override void Convert(IDocument document, int y1, int y2) 
		{
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				
				if(line.Length > 0) {
					// count how many whitespace characters there are at the start
					int whiteSpace = 0;
					for(whiteSpace = 0; whiteSpace < line.Length && Char.IsWhiteSpace(document.GetCharAt(line.Offset + whiteSpace)); whiteSpace++) {
						// deliberately empty
					}
					if(whiteSpace > 0) {
						string newLine = document.GetText(line.Offset,whiteSpace);
						string newPrefix = newLine.Replace("\t",new string(' ', document.TextEditorProperties.TabIndent));
						document.Replace(line.Offset,whiteSpace,newPrefix);
					}
				}
			}
		}
	}
	
	public class ConvertLeadingSpacesToTabs : AbstractLineFormatAction
	{
		protected override void Convert(IDocument document, int y1, int y2) 
		{
			for (int i = y2; i >= y1; --i) {
				LineSegment line = document.GetLineSegment(i);
				if(line.Length > 0) {
					// note: some users may prefer a more radical ConvertLeadingSpacesToTabs that
					// means there can be no spaces before the first character even if the spaces
					// didn't add up to a whole number of tabs
					string newLine = TextUtilities.LeadingWhiteSpaceToTabs(document.GetText(line.Offset,line.Length), document.TextEditorProperties.TabIndent);
					document.Replace(line.Offset,line.Length,newLine);
				}
			}
		}
	}

	/// <summary>
	/// This is a sample editaction plugin, it indents the selected area.
	/// </summary>
	public class FormatBuffer : AbstractLineFormatAction
	{
		protected override void Convert(IDocument document, int startLine, int endLine)
		{
			document.FormattingStrategy.IndentLines(textArea, startLine, endLine);
		}
	}
}
