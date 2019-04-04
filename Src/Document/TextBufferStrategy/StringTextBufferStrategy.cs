// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="StringTextBufferStrategy.cs" company="">
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
using System.IO;
using System.Text;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// Simple implementation of the ITextBuffer interface implemented using a
	/// string.
	/// Only for fall-back purposes.
	/// </summary>
	public class StringTextBufferStrategy : ITextBufferStrategy
	{
		string storedText = "";
		
		public int Length {
			get {
				return storedText.Length;
			}
		}
		
		public void Insert(int offset, string text)
		{
			if (text != null) {
				storedText = storedText.Insert(offset, text);
			}
		}
		
		public void Remove(int offset, int length)
		{
			storedText = storedText.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string text)
		{
			Remove(offset, length);
			Insert(offset, text);
		}
		
		public string GetText(int offset, int length)
		{
			if (length == 0) {
				return "";
			}
			if (offset == 0 && length >= storedText.Length) {
				return storedText;
			}
			return storedText.Substring(offset, Math.Min(length, storedText.Length - offset));
		}
		
		public char GetCharAt(int offset)
		{
			if (offset == Length) {
				return '\0';
			}
			return storedText[offset];
		}
		
		public void SetContent(string text)
		{
			storedText = text;
		}
		
		public StringTextBufferStrategy()
		{
		}
		
		public static ITextBufferStrategy CreateTextBufferFromFile(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new System.IO.FileNotFoundException(fileName);
			}
			StringTextBufferStrategy s = new StringTextBufferStrategy();
			s.SetContent(Util.FileReader.ReadFileContent(fileName, Encoding.Default));
			return s;
		}
	}
}
