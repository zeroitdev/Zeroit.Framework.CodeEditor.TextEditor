// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="FontContainer.cs" company="">
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
using System.Drawing;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// This class is used to generate bold, italic and bold/italic fonts out
	/// of a base font.
	/// </summary>
	public class FontContainer
	{
		Font defaultFont;
		Font regularfont, boldfont, italicfont, bolditalicfont;
		
		/// <value>
		/// The scaled, regular version of the base font
		/// </value>
		public Font RegularFont {
			get {
				return regularfont;
			}
		}
		
		/// <value>
		/// The scaled, bold version of the base font
		/// </value>
		public Font BoldFont {
			get {
				return boldfont;
			}
		}
		
		/// <value>
		/// The scaled, italic version of the base font
		/// </value>
		public Font ItalicFont {
			get {
				return italicfont;
			}
		}
		
		/// <value>
		/// The scaled, bold/italic version of the base font
		/// </value>
		public Font BoldItalicFont {
			get {
				return bolditalicfont;
			}
		}
		
		static float twipsPerPixelY;
		
		public static float TwipsPerPixelY {
			get {
				if (twipsPerPixelY == 0) {
					using (Bitmap bmp = new Bitmap(1,1)) {
						using (Graphics g = Graphics.FromImage(bmp)) {
							twipsPerPixelY = 1440 / g.DpiY;
						}
					}
				}
				return twipsPerPixelY;
			}
		}
		
		/// <value>
		/// The base font
		/// </value>
		public Font DefaultFont {
			get {
				return defaultFont;
			}
			set {
				// 1440 twips is one inch
				float pixelSize = (float)Math.Round(value.SizeInPoints * 20 / TwipsPerPixelY);
				
				defaultFont    = value;
				regularfont    = new Font(value.FontFamily, pixelSize * TwipsPerPixelY / 20f, FontStyle.Regular);
				boldfont       = new Font(regularfont, FontStyle.Bold);
				italicfont     = new Font(regularfont, FontStyle.Italic);
				bolditalicfont = new Font(regularfont, FontStyle.Bold | FontStyle.Italic);
			}
		}
		
		public static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		public FontContainer(Font defaultFont)
		{
			this.DefaultFont = defaultFont;
		}
	}
}
