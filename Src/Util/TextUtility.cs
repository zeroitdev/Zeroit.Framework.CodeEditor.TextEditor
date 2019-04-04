// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TextUtility.cs" company="">
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

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	public class TextUtility
	{
		
		public static bool RegionMatches(IDocument document, int offset, int length, string word)
		{
			if (length != word.Length || document.TextLength < offset + length) {
				return false;
			}
			
			for (int i = 0; i < length; ++i) {
				if (document.GetCharAt(offset + i) != word[i]) {
					return false;
				}
			}
			return true;
		}
		
		public static bool RegionMatches(IDocument document, bool casesensitive, int offset, int length, string word)
		{
			if (casesensitive) {
				return RegionMatches(document, offset, length, word);
			}
			
			if (length != word.Length || document.TextLength < offset + length) {
				return false;
			}
			
			for (int i = 0; i < length; ++i) {
				if (Char.ToUpper(document.GetCharAt(offset + i)) != Char.ToUpper(word[i])) {
					return false;
				}
			}
			return true;
		}
		
		public static bool RegionMatches(IDocument document, int offset, int length, char[] word)
		{
			if (length != word.Length || document.TextLength < offset + length) {
				return false;
			}
			
			for (int i = 0; i < length; ++i) {
				if (document.GetCharAt(offset + i) != word[i]) {
					return false;
				}
			}
			return true;
		}
		
		public static bool RegionMatches(IDocument document, bool casesensitive, int offset, int length, char[] word)
		{
			if (casesensitive) {
				return RegionMatches(document, offset, length, word);
			}
			
			if (length != word.Length || document.TextLength < offset + length) {
				return false;
			}
			
			for (int i = 0; i < length; ++i) {
				if (Char.ToUpper(document.GetCharAt(offset + i)) != Char.ToUpper(word[i])) {
					return false;
				}
			}
			return true;
		}
	}
}
