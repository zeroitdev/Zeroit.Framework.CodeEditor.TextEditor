// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TipPainter.cs" company="">
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


using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	static class TipPainter
	{
		const float HorizontalBorder = 2;
		const float VerticalBorder   = 1;
		static RectangleF workingArea = RectangleF.Empty;
		
		//static StringFormat centerTipFormat = CreateTipStringFormat();
		
		public static Size GetTipSize(Control control, Graphics graphics, Font font, string description)
		{
			return GetTipSize(control, graphics, new TipText (graphics, font, description));
		}
		
		public static Size GetTipSize(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			if (workingArea == RectangleF.Empty) {
				Form ownerForm = control.FindForm();
				if (ownerForm.Owner != null) {
					ownerForm = ownerForm.Owner;
				}
				
				workingArea = Screen.GetWorkingArea(ownerForm);
			}
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - HorizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - VerticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
				TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(HorizontalBorder * 2,
				                      VerticalBorder   * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			return tipSize;
		}
		
		public static Size DrawTip(Control control, Graphics graphics, Font font, string description)
		{
			return DrawTip(control, graphics, new TipText (graphics, font, description));
		}
		
		public static Size DrawTip(Control control, Graphics graphics, TipSection tipData)
		{
			Size tipSize = Size.Empty;
			SizeF tipSizeF = SizeF.Empty;
			
			PointF screenLocation = control.PointToScreen(Point.Empty);
			
			if (workingArea == RectangleF.Empty) {
				Form ownerForm = control.FindForm();
				if (ownerForm.Owner != null) {
					ownerForm = ownerForm.Owner;
				}
				
				workingArea = Screen.GetWorkingArea(ownerForm);
			}
	
			SizeF maxLayoutSize = new SizeF(workingArea.Right - screenLocation.X - HorizontalBorder * 2,
			                                workingArea.Bottom - screenLocation.Y - VerticalBorder * 2);
			
			if (maxLayoutSize.Width > 0 && maxLayoutSize.Height > 0) {
				graphics.TextRenderingHint =
				TextRenderingHint.AntiAliasGridFit;
				
				tipData.SetMaximumSize(maxLayoutSize);
				tipSizeF = tipData.GetRequiredSize();
				tipData.SetAllocatedSize(tipSizeF);
				
				tipSizeF += new SizeF(HorizontalBorder * 2,
				                      VerticalBorder   * 2);
				tipSize = Size.Ceiling(tipSizeF);
			}
			
			if (control.ClientSize != tipSize) {
				control.ClientSize = tipSize;
			}
			
			if (tipSize != Size.Empty) {
				Rectangle borderRectangle = new Rectangle
				(Point.Empty, tipSize - new Size(1, 1));
				
				RectangleF displayRectangle = new RectangleF
				(HorizontalBorder, VerticalBorder,
				 tipSizeF.Width - HorizontalBorder * 2,
				 tipSizeF.Height - VerticalBorder * 2);
				
				// DrawRectangle draws from Left to Left + Width. A bug? :-/
				graphics.DrawRectangle(SystemPens.WindowFrame,
				                       borderRectangle);
				tipData.Draw(new PointF(HorizontalBorder, VerticalBorder));
			}
			return tipSize;
		}
	}
}
