﻿// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="RtfWriter.cs" company="">
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
using System.Text;

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	public class RtfWriter
	{
		static Dictionary<string, int> colors;
		static int           colorNum;
		static StringBuilder colorString;
		
		public static string GenerateRtf(TextArea textArea)
		{
			colors = new Dictionary<string, int>();
			colorNum = 0;
			colorString = new StringBuilder();
			
			
			StringBuilder rtf = new StringBuilder();
			
			rtf.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1031");
			BuildFontTable(textArea.Document, rtf);
			rtf.Append('\n');
			
			string fileContent = BuildFileContent(textArea);
			BuildColorTable(textArea.Document, rtf);
			rtf.Append('\n');
			rtf.Append(@"\viewkind4\uc1\pard");
			rtf.Append(fileContent);
			rtf.Append("}");
			return rtf.ToString();
		}
		
		static void BuildColorTable(IDocument doc, StringBuilder rtf)
		{
			rtf.Append(@"{\colortbl ;");
			rtf.Append(colorString.ToString());
			rtf.Append("}");
		}
		
		static void BuildFontTable(IDocument doc, StringBuilder rtf)
		{
			rtf.Append(@"{\fonttbl");
			rtf.Append(@"{\f0\fmodern\fprq1\fcharset0 " + doc.TextEditorProperties.Font.Name + ";}");
			rtf.Append("}");
		}
		
		static string BuildFileContent(TextArea textArea)
		{
			StringBuilder rtf = new StringBuilder();
			bool firstLine = true;
			Color curColor = Color.Black;
			bool  oldItalic = false;
			bool  oldBold   = false;
			bool  escapeSequence = false;
			
			foreach (ISelection selection in textArea.SelectionManager.SelectionCollection) {
				int selectionOffset    = textArea.Document.PositionToOffset(selection.StartPosition);
				int selectionEndOffset = textArea.Document.PositionToOffset(selection.EndPosition);
				for (int i = selection.StartPosition.Y; i <= selection.EndPosition.Y; ++i) {
					LineSegment line = textArea.Document.GetLineSegment(i);
					int offset = line.Offset;
					if (line.Words == null) {
						continue;
					}
					
					foreach (TextWord word in line.Words) {
						switch (word.Type) {
							case TextWordType.Space:
								if (selection.ContainsOffset(offset)) {
									rtf.Append(' ');
								}
								++offset;
								break;
							
							case TextWordType.Tab:
								if (selection.ContainsOffset(offset)) {
									rtf.Append(@"\tab");
								}
								++offset;
								escapeSequence = true;
								break;
							
							case TextWordType.Word:
								Color c = word.Color;
								
								if (offset + word.Word.Length > selectionOffset && offset < selectionEndOffset) {
									string colorstr = c.R + ", " + c.G + ", " + c.B;
									
									if (!colors.ContainsKey(colorstr)) {
										colors[colorstr] = ++colorNum;
										colorString.Append(@"\red" + c.R + @"\green" + c.G + @"\blue" + c.B + ";");
									}
									if (c != curColor || firstLine) {
										rtf.Append(@"\cf" + colors[colorstr].ToString());
										curColor = c;
										escapeSequence = true;
									}
									
									if (oldItalic != word.Italic) {
										if (word.Italic) {
											rtf.Append(@"\i");
										} else {
											rtf.Append(@"\i0");
										}
										oldItalic = word.Italic;
										escapeSequence = true;
									}
									
									if (oldBold != word.Bold) {
										if (word.Bold) {
											rtf.Append(@"\b");
										} else {
											rtf.Append(@"\b0");
										}
										oldBold = word.Bold;
										escapeSequence = true;
									}
									
									if (firstLine) {
										rtf.Append(@"\f0\fs" + (textArea.TextEditorProperties.Font.Size * 2));
										firstLine = false;
									}
									if (escapeSequence) {
										rtf.Append(' ');
										escapeSequence = false;
									}
									string printWord;
									if (offset < selectionOffset) {
										printWord = word.Word.Substring(selectionOffset - offset);
									} else if (offset + word.Word.Length > selectionEndOffset) {
										printWord = word.Word.Substring(0, (offset + word.Word.Length) - selectionEndOffset);
									} else {
										printWord = word.Word;
									}
									
									rtf.Append(printWord.Replace(@"\", @"\\").Replace("{", "\\{").Replace("}", "\\}"));
								}
								offset += word.Length;
								break;
						}
					}
					if (offset < selectionEndOffset) {
						rtf.Append(@"\par");
					}
					rtf.Append('\n');
				}
			}
			
			return rtf.ToString();
		}
	}
}
