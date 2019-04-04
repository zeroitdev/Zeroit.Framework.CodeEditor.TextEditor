// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="FoldMarker.cs" company="">
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
	public enum FoldType {
		Unspecified,
		MemberBody,
		Region,
		TypeBody
	}
	
	public class FoldMarker : AbstractSegment, IComparable
	{
		bool      isFolded = false;
		string    foldText = "...";
		FoldType  foldType = FoldType.Unspecified;
		IDocument document = null;
		int startLine = -1, startColumn, endLine = -1, endColumn;
		
		static void GetPointForOffset(IDocument document, int offset, out int line, out int column)
		{
			if (offset > document.TextLength) {
				line = document.TotalNumberOfLines + 1;
				column = 1;
			} else if (offset < 0) {
				line = -1;
				column = -1;
			} else {
				line = document.GetLineNumberForOffset(offset);
				column = offset - document.GetLineSegment(line).Offset;
			}
		}
		
		public FoldType FoldType {
			get { return foldType; }
			set { foldType = value; }
		}
		
		public int StartLine {
			get {
				if (startLine < 0) {
					GetPointForOffset(document, offset, out startLine, out startColumn);
				}
				return startLine;
			}
		}
		
		public int StartColumn {
			get {
				if (startLine < 0) {
					GetPointForOffset(document, offset, out startLine, out startColumn);
				}
				return startColumn;
			}
		}
		
		public int EndLine {
			get {
				if (endLine < 0) {
					GetPointForOffset(document, offset + length, out endLine, out endColumn);
				}
				return endLine;
			}
		}
		
		public int EndColumn {
			get {
				if (endLine < 0) {
					GetPointForOffset(document, offset + length, out endLine, out endColumn);
				}
				return endColumn;
			}
		}
		
		public override int Offset {
			get { return base.Offset; }
			set {
				base.Offset = value;
				startLine = -1; endLine = -1;
			}
		}
		public override int Length {
			get { return base.Length; }
			set {
				base.Length = value;
				endLine = -1;
			}
		}
		
		public bool IsFolded {
			get {
				return isFolded;
			}
			set {
				isFolded = value;
			}
		}
		
		public string FoldText {
			get {
				return foldText;
			}
		}
		
		public string InnerText {
			get {
				return document.GetText(offset, length);
			}
		}
		
		public FoldMarker(IDocument document, int offset, int length, string foldText, bool isFolded)
		{
			this.document = document;
			this.offset   = offset;
			this.length   = length;
			this.foldText = foldText;
			this.isFolded = isFolded;
		}
		
		public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn) : this(document, startLine, startColumn, endLine, endColumn, FoldType.Unspecified)
		{
		}
		
		public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn, FoldType foldType)  : this(document, startLine, startColumn, endLine, endColumn, foldType, "...")
		{
		}
		
		public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn, FoldType foldType, string foldText) : this(document, startLine, startColumn, endLine, endColumn, foldType, foldText, false)
		{
		}
		
		public FoldMarker(IDocument document, int startLine, int startColumn, int endLine, int endColumn, FoldType foldType, string foldText, bool isFolded)
		{
			this.document = document;
			
			startLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(startLine, 0));
			ISegment startLineSegment = document.GetLineSegment(startLine);
			
			endLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(endLine, 0));
			ISegment endLineSegment   = document.GetLineSegment(endLine);
			
			// Prevent the region from completely disappearing
			if (string.IsNullOrEmpty(foldText)) {
				foldText = "...";
			}
			
			this.FoldType = foldType;
			this.foldText = foldText;
			this.offset = startLineSegment.Offset + Math.Min(startColumn, startLineSegment.Length);
			this.length = (endLineSegment.Offset + Math.Min(endColumn, endLineSegment.Length)) - this.offset;
			this.isFolded = isFolded;
		}
		
		public int CompareTo(object o)
		{
			if (!(o is FoldMarker)) {
				throw new ArgumentException();
			}
			FoldMarker f = (FoldMarker)o;
			if (offset != f.offset) {
				return offset.CompareTo(f.offset);
			}
			
			return length.CompareTo(f.length);
		}
	}
}
