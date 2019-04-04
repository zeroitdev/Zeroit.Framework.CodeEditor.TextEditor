// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TipSplitter.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	class TipSplitter: TipSection
	{
		bool         isHorizontal;
		float     [] offsets;
		TipSection[] tipSections;
		
		public TipSplitter(Graphics graphics, bool horizontal, params TipSection[] sections): base(graphics)
		{
			Debug.Assert(sections != null);
			
			isHorizontal = horizontal;
			offsets = new float[sections.Length];
			tipSections = (TipSection[])sections.Clone();	
		}
		
		public override void Draw(PointF location)
		{
			if (isHorizontal) {
				for (int i = 0; i < tipSections.Length; i ++) {
					tipSections[i].Draw
						(new PointF(location.X + offsets[i], location.Y));
				}
			} else {
				for (int i = 0; i < tipSections.Length; i ++) {
					tipSections[i].Draw
						(new PointF(location.X, location.Y + offsets[i]));
				}
			}
		}
		
		protected override void OnMaximumSizeChanged()
		{
			base.OnMaximumSizeChanged();
			
			float currentDim = 0;
			float otherDim = 0;
			SizeF availableArea = MaximumSize;
			
			for (int i = 0; i < tipSections.Length; i ++) {
				TipSection section = (TipSection)tipSections[i];
			
				section.SetMaximumSize(availableArea);
				
				SizeF requiredArea = section.GetRequiredSize();
				offsets[i] = currentDim;

				// It's best to start on pixel borders, so this will
				// round up to the nearest pixel. Otherwise there are
				// weird cutoff artifacts.
				float pixelsUsed;
				
				if (isHorizontal) {
					pixelsUsed  = (float)Math.Ceiling(requiredArea.Width);
					currentDim += pixelsUsed;
					
					availableArea.Width = Math.Max
						(0, availableArea.Width - pixelsUsed);
					
					otherDim = Math.Max(otherDim, requiredArea.Height);
				} else {
					pixelsUsed  = (float)Math.Ceiling(requiredArea.Height);
					currentDim += pixelsUsed;
					
					availableArea.Height = Math.Max
						(0, availableArea.Height - pixelsUsed);
					
					otherDim = Math.Max(otherDim, requiredArea.Width);
				}
			}
			
			foreach (TipSection section in tipSections) {
				if (isHorizontal) {
					section.SetAllocatedSize(new SizeF(section.GetRequiredSize().Width, otherDim));
				} else {
					section.SetAllocatedSize(new SizeF(otherDim, section.GetRequiredSize().Height));
				}
			}

			if (isHorizontal) {
				SetRequiredSize(new SizeF(currentDim, otherDim));
			} else {
				SetRequiredSize(new SizeF(otherDim, currentDim));
			}
		}
	}
}
