// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TipPainterTools.cs" company="">
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
using System.Windows.Forms;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	class TipPainterTools
	{
		const int SpacerSize = 4;
		
		private TipPainterTools()
		{
			
		}
		public static Size GetDrawingSizeHelpTipFromCombinedDescription(Control control,
		                                                      Graphics graphics,
		                                                      Font font,
		                                                      string countMessage,
		                                                      string description)
		{
			string basicDescription = null;
			string documentation = null;

			if (IsVisibleText(description)) {
	     		string[] splitDescription = description.Split(new char[] { '\n' }, 2);
						
				if (splitDescription.Length > 0) {
					basicDescription = splitDescription[0];
					
					if (splitDescription.Length > 1) {
						documentation = splitDescription[1].Trim();
					}
				}
			}
			
			return GetDrawingSizeDrawHelpTip(control, graphics, font, countMessage, basicDescription, documentation);
		}
		
		public static Size DrawHelpTipFromCombinedDescription(Control control,
		                                                      Graphics graphics,
		                                                      Font font,
		                                                      string countMessage,
		                                                      string description)
		{
			string basicDescription = null;
			string documentation = null;

			if (IsVisibleText(description)) {
	     		string[] splitDescription = description.Split
	     			(new char[] { '\n' }, 2);
						
				if (splitDescription.Length > 0) {
					basicDescription = splitDescription[0];
					
					if (splitDescription.Length > 1) {
						documentation = splitDescription[1].Trim();
					}
				}
			}
			
			return DrawHelpTip(control, graphics, font, countMessage,
			            basicDescription, documentation);
 		}
		
		// btw. I know it's ugly.
		public static Rectangle DrawingRectangle1;
		public static Rectangle DrawingRectangle2;
		
		public static Size GetDrawingSizeDrawHelpTip(Control control,
		                               Graphics graphics, Font font,
		                               string countMessage,
		                               string basicDescription,
		                               string documentation)
		{
			if (IsVisibleText(countMessage)     ||
			    IsVisibleText(basicDescription) ||
			    IsVisibleText(documentation)) {
				// Create all the TipSection objects.
				CountTipText countMessageTip = new CountTipText(graphics, font, countMessage);
				
				TipSpacer countSpacer = new TipSpacer(graphics, new SizeF(IsVisibleText(countMessage) ? 4 : 0, 0));
				
				TipText descriptionTip = new TipText(graphics, font, basicDescription);
				
				TipSpacer docSpacer = new TipSpacer(graphics, new SizeF(0, IsVisibleText(documentation) ? 4 : 0));
				
				TipText docTip = new TipText(graphics, font, documentation);
				
				// Now put them together.
				TipSplitter descSplitter = new TipSplitter(graphics, false,
				                                           descriptionTip,
				                                           docSpacer
				                                           );
				
				TipSplitter mainSplitter = new TipSplitter(graphics, true,
				                                           countMessageTip,
				                                           countSpacer,
				                                           descSplitter);
				
				TipSplitter mainSplitter2 = new TipSplitter(graphics, false,
				                                           mainSplitter,
				                                           docTip);
				
				// Show it.
				Size size = TipPainter.GetTipSize(control, graphics, mainSplitter2);
				DrawingRectangle1 = countMessageTip.DrawingRectangle1;
				DrawingRectangle2 = countMessageTip.DrawingRectangle2;
				return size;
			}
			return Size.Empty;
		}
		public static Size DrawHelpTip(Control control,
		                               Graphics graphics, Font font,
		                               string countMessage,
		                               string basicDescription,
		                               string documentation)
		{
			if (IsVisibleText(countMessage)     ||
			    IsVisibleText(basicDescription) ||
			    IsVisibleText(documentation)) {
				// Create all the TipSection objects.
				CountTipText countMessageTip = new CountTipText(graphics, font, countMessage);
				
				TipSpacer countSpacer = new TipSpacer(graphics, new SizeF(IsVisibleText(countMessage) ? 4 : 0, 0));
				
				TipText descriptionTip = new TipText(graphics, font, basicDescription);
				
				TipSpacer docSpacer = new TipSpacer(graphics, new SizeF(0, IsVisibleText(documentation) ? 4 : 0));
				
				TipText docTip = new TipText(graphics, font, documentation);
				
				// Now put them together.
				TipSplitter descSplitter = new TipSplitter(graphics, false,
				                                           descriptionTip,
				                                           docSpacer
				                                           );
				
				TipSplitter mainSplitter = new TipSplitter(graphics, true,
				                                           countMessageTip,
				                                           countSpacer,
				                                           descSplitter);
				
				TipSplitter mainSplitter2 = new TipSplitter(graphics, false,
				                                           mainSplitter,
				                                           docTip);
				
				// Show it.
				Size size = TipPainter.DrawTip(control, graphics, mainSplitter2);
				DrawingRectangle1 = countMessageTip.DrawingRectangle1;
				DrawingRectangle2 = countMessageTip.DrawingRectangle2;
				return size;
			}
			return Size.Empty;
		}
		
		static bool IsVisibleText(string text)
		{
			return text != null && text.Length > 0;
		}
	}
}
