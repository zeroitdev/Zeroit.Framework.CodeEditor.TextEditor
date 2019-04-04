// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="IFormattingStrategy.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// This interface handles the auto and smart indenting and formating
	/// in the document while  you type. Language bindings could overwrite this 
	/// interface and define their own indentation/formating.
	/// </summary>
	public interface IFormattingStrategy
	{
		/// <summary>
		/// This function formats a specific line after <code>ch</code> is pressed.
		/// </summary>
		void FormatLine(TextArea textArea, int line, int caretOffset, char charTyped);
		
		/// <summary>
		/// This function sets the indentation level in a specific line
		/// </summary>
		/// <returns>
		/// The target caret position (length of new indentation).
		/// </returns>
		int IndentLine(TextArea textArea, int line);
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		void IndentLines(TextArea textArea, int begin, int end);
		
		/// <summary>
		/// Finds the offset of the opening bracket in the block defined by offset skipping
		/// brackets in strings and comments.
		/// </summary>
		/// <param name="document">The document to search in.</param>
		/// <param name="offset">The offset of an position in the block or the offset of the closing bracket.</param>
		/// <param name="openBracket">The character for the opening bracket.</param>
		/// <param name="closingBracket">The character for the closing bracket.</param>
		/// <returns>Returns the offset of the opening bracket or -1 if no matching bracket was found.</returns>
		int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket);
		
		/// <summary>
		/// Finds the offset of the closing bracket in the block defined by offset skipping
		/// brackets in strings and comments.
		/// </summary>
		/// <param name="document">The document to search in.</param>
		/// <param name="offset">The offset of an position in the block or the offset of the opening bracket.</param>
		/// <param name="openBracket">The character for the opening bracket.</param>
		/// <param name="closingBracket">The character for the closing bracket.</param>
		/// <returns>Returns the offset of the closing bracket or -1 if no matching bracket was found.</returns>
		int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket);
	}
}
