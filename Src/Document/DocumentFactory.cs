// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="DocumentFactory.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// This interface represents a container which holds a text sequence and
	/// all necessary information about it. It is used as the base for a text editor.
	/// </summary>
	public class DocumentFactory
	{
		/// <remarks>
		/// Creates a new <see cref="IDocument"/> object. Only create
		/// <see cref="IDocument"/> with this method.
		/// </remarks>
		public IDocument CreateDocument()
		{
			DefaultDocument doc = new DefaultDocument();
			doc.TextBufferStrategy  = new GapTextBufferStrategy();
			doc.FormattingStrategy  = new DefaultFormattingStrategy();
			doc.LineManager         = new LineManager(doc, null);
			doc.FoldingManager      = new FoldingManager(doc, doc.LineManager);
			doc.FoldingManager.FoldingStrategy       = null; //new ParserFoldingStrategy();
			doc.MarkerStrategy      = new MarkerStrategy(doc);
			doc.BookmarkManager     = new BookmarkManager(doc, doc.LineManager);
			return doc;
		}
		
		/// <summary>
		/// Creates a new document and loads the given file
		/// </summary>
		public IDocument CreateFromTextBuffer(ITextBufferStrategy textBuffer)
		{
			DefaultDocument doc = (DefaultDocument)CreateDocument();
			doc.TextContent = textBuffer.GetText(0, textBuffer.Length);
			doc.TextBufferStrategy = textBuffer;
			return doc;
		}
		
		/// <summary>
		/// Creates a new document and loads the given file
		/// </summary>
		public IDocument CreateFromFile(string fileName)
		{
			IDocument document = CreateDocument();
			document.TextContent = Util.FileReader.ReadFileContent(fileName, Encoding.Default);
			return document;
		}
	}
}
