// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="IndentFoldingStrategy.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// A simple folding strategy which calculates the folding level
	/// using the indent level of the line.
	/// </summary>
	public class IndentFoldingStrategy : IFoldingStrategy
	{
		public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
		{
			List<FoldMarker> l = new List<FoldMarker>();
			Stack<int> offsetStack = new Stack<int>();
			Stack<string> textStack = new Stack<string>();
			//int level = 0;
			//foreach (LineSegment segment in document.LineSegmentCollection) {
			//	
			//}
			return l;
		}
		
		int GetLevel(IDocument document, int offset)
		{
			int level = 0;
			int spaces = 0;
			for (int i = offset; i < document.TextLength; ++i) {
				char c = document.GetCharAt(i);
				if (c == '\t' || (c == ' ' && ++spaces == 4)) {
					spaces = 0;
					++level;
				} else {
					break;
				}
			}
			return level;
		}
	}
}
