// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="MarkerStrategy.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// Manages the list of markers and provides ways to retrieve markers for specific positions.
	/// </summary>
	public sealed class MarkerStrategy
	{
		List<TextMarker> textMarker = new List<TextMarker>();
		IDocument document;
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		public IEnumerable<TextMarker> TextMarker {
			get {
				return textMarker.AsReadOnly();
			}
		}
		
		public void AddMarker(TextMarker item)
		{
			markersTable.Clear();
			textMarker.Add(item);
		}
		
		public void InsertMarker(int index, TextMarker item)
		{
			markersTable.Clear();
			textMarker.Insert(index, item);
		}
		
		public void RemoveMarker(TextMarker item)
		{
			markersTable.Clear();
			textMarker.Remove(item);
		}
		
		public void RemoveAll(Predicate<TextMarker> match)
		{
			markersTable.Clear();
			textMarker.RemoveAll(match);
		}
		
		public MarkerStrategy(IDocument document)
		{
			this.document = document;
			document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
		}
		
		Dictionary<int, List<TextMarker>> markersTable = new Dictionary<int, List<TextMarker>>();
		
		public List<TextMarker> GetMarkers(int offset)
		{
			if (!markersTable.ContainsKey(offset)) {
				List<TextMarker> markers = new List<TextMarker>();
				for (int i = 0; i < textMarker.Count; ++i) {
					TextMarker marker = (TextMarker)textMarker[i];
					if (marker.Offset <= offset && offset <= marker.EndOffset) {
						markers.Add(marker);
					}
				}
				markersTable[offset] = markers;
			}
			return markersTable[offset];
		}
		
		public List<TextMarker> GetMarkers(int offset, int length)
		{
			int endOffset = offset + length - 1;
			List<TextMarker> markers = new List<TextMarker>();
			for (int i = 0; i < textMarker.Count; ++i) {
				TextMarker marker = (TextMarker)textMarker[i];
				if (// start in marker region
				    marker.Offset <= offset && offset <= marker.EndOffset ||
				    // end in marker region
				    marker.Offset <= endOffset && endOffset <= marker.EndOffset ||
				    // marker start in region
				    offset <= marker.Offset && marker.Offset <= endOffset ||
				    // marker end in region
				    offset <= marker.EndOffset && marker.EndOffset <= endOffset
				   )
				{
					markers.Add(marker);
				}
			}
			return markers;
		}
		
		public List<TextMarker> GetMarkers(TextLocation position)
		{
			if (position.Y >= document.TotalNumberOfLines || position.Y < 0) {
				return new List<TextMarker>();
			}
			LineSegment segment = document.GetLineSegment(position.Y);
			return GetMarkers(segment.Offset + position.X);
		}
		
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			// reset markers table
			markersTable.Clear();
			document.UpdateSegmentListOnDocumentChange(textMarker, e);
		}
	}
}
