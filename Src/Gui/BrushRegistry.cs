// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="BrushRegistry.cs" company="">
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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Zeroit.Framework.CodeEditor.TextEditor
{
	/// <summary>
	/// Contains brushes/pens for the text editor to speed up drawing. Re-Creation of brushes and pens
	/// seems too costly.
	/// </summary>
	public class BrushRegistry
	{
		static Hashtable brushes = new Hashtable();
		static Hashtable pens    = new Hashtable();
		static Hashtable dotPens = new Hashtable();
		
		public static Brush GetBrush(Color color)
		{
			if (!brushes.Contains(color)) {
				Brush newBrush = new SolidBrush(color);
				brushes.Add(color, newBrush);
				return newBrush;
			}
			return brushes[color] as Brush;
		}
		
		public static Pen GetPen(Color color)
		{
			if (!pens.Contains(color)) {
				Pen newPen = new Pen(color);
				pens.Add(color, newPen);
				return newPen;
			}
			return pens[color] as Pen;
		}
		
		public static Pen GetDotPen(Color bgColor, Color fgColor)
		{
			bool containsBgColor = dotPens.Contains(bgColor);
			if (!containsBgColor || !((Hashtable)dotPens[bgColor]).Contains(fgColor)) {
				if (!containsBgColor) {
					dotPens[bgColor] = new Hashtable();
				}
				
				HatchBrush hb = new HatchBrush(HatchStyle.Percent50, bgColor, fgColor);
				Pen newPen = new Pen(hb);
				((Hashtable)dotPens[bgColor])[fgColor] = newPen;
				return newPen;
			}
			return ((Hashtable)dotPens[bgColor])[fgColor] as Pen;
		}
	}
}
