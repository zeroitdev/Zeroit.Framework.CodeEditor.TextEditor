// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="DeferredEventList.cs" company="">
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
	/// A list of events that are fired after the line manager has finished working.
	/// </summary>
	struct DeferredEventList
	{
		internal List<LineSegment> removedLines;
		internal List<TextAnchor> textAnchor;
		
		public void AddRemovedLine(LineSegment line)
		{
			if (removedLines == null)
				removedLines = new List<LineSegment>();
			removedLines.Add(line);
		}
		
		public void AddDeletedAnchor(TextAnchor anchor)
		{
			if (textAnchor == null)
				textAnchor = new List<TextAnchor>();
			textAnchor.Add(anchor);
		}
		
		public void RaiseEvents()
		{
			// removedLines is raised by the LineManager
			if (textAnchor != null) {
				foreach (TextAnchor a in textAnchor) {
					a.RaiseDeleted();
				}
			}
		}
	}
}
