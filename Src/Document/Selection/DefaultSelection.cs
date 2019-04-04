// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="DefaultSelection.cs" company="">
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
using System.Diagnostics;
using System.Drawing;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// Default implementation of the <see cref="Zeroit.Framework.CodeEditor.TextEditor.Document.ISelection"/> interface.
	/// </summary>
	public class DefaultSelection : ISelection
	{
		IDocument document;
		bool      isRectangularSelection;
		TextLocation     startPosition;
		TextLocation     endPosition;
		
		public TextLocation StartPosition {
			get {
				return startPosition;
			}
			set {
				DefaultDocument.ValidatePosition(document, value);
				startPosition = value;
			}
		}
		
		public TextLocation EndPosition {
			get {
				return endPosition;
			}
			set {
				DefaultDocument.ValidatePosition(document, value);
				endPosition = value;
			}
		}
		
		public int Offset {
			get {
				return document.PositionToOffset(startPosition);
			}
		}
		
		public int EndOffset {
			get {
				return document.PositionToOffset(endPosition);
			}
		}
		
		public int Length {
			get {
				return EndOffset - Offset;
			}
		}
		
		/// <value>
		/// Returns true, if the selection is empty
		/// </value>
		public bool IsEmpty {
			get {
				return startPosition == endPosition;
			}
		}
		
		/// <value>
		/// Returns true, if the selection is rectangular
		/// </value>
		// TODO : make this unused property used.
		public bool IsRectangularSelection {
			get {
				return isRectangularSelection;
			}
			set {
				isRectangularSelection = value;
			}
		}
		
		/// <value>
		/// The text which is selected by this selection.
		/// </value>
		public string SelectedText {
			get {
				if (document != null) {
					if (Length < 0) {
						return null;
					}
					return document.GetText(Offset, Length);
				}
				return null;
			}
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="DefaultSelection"/>
		/// </summary>
		public DefaultSelection(IDocument document, TextLocation startPosition, TextLocation endPosition)
		{
			DefaultDocument.ValidatePosition(document, startPosition);
			DefaultDocument.ValidatePosition(document, endPosition);
			Debug.Assert(startPosition <= endPosition);
			this.document      = document;
			this.startPosition = startPosition;
			this.endPosition   = endPosition;
		}
		
		/// <summary>
		/// Converts a <see cref="DefaultSelection"/> instance to string (for debug purposes)
		/// </summary>
		public override string ToString()
		{
			return String.Format("[DefaultSelection : StartPosition={0}, EndPosition={1}]", startPosition, endPosition);
		}
		public bool ContainsPosition(TextLocation position)
		{
			if (this.IsEmpty)
				return false;
			return startPosition.Y < position.Y && position.Y  < endPosition.Y ||
				startPosition.Y == position.Y && startPosition.X <= position.X && (startPosition.Y != endPosition.Y || position.X <= endPosition.X) ||
				endPosition.Y == position.Y && startPosition.Y != endPosition.Y && position.X <= endPosition.X;
		}
		
		public bool ContainsOffset(int offset)
		{
			return Offset <= offset && offset <= EndOffset;
		}
	}
}
