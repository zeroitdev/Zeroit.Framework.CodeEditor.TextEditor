// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="ITextEditorProperties.cs" company="">
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
using System.Text;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	public interface ITextEditorProperties
	{
		bool AutoInsertCurlyBracket { // is wrapped in text editor control
			get;
			set;
		}
		
		bool HideMouseCursor { // is wrapped in text editor control
			get;
			set;
		}
		
		bool IsIconBarVisible { // is wrapped in text editor control
			get;
			set;
		}
		
		bool AllowCaretBeyondEOL {
			get;
			set;
		}
		
		bool ShowMatchingBracket { // is wrapped in text editor control
			get;
			set;
		}
		
		bool CutCopyWholeLine {
			get;
			set;
		}

		System.Drawing.Text.TextRenderingHint TextRenderingHint { // is wrapped in text editor control
			get;
			set;
		}
		
		bool MouseWheelScrollDown {
			get;
			set;
		}
		
		bool MouseWheelTextZoom {
			get;
			set;
		}
		
		string LineTerminator {
			get;
			set;
		}
		
		LineViewerStyle LineViewerStyle { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowInvalidLines { // is wrapped in text editor control
			get;
			set;
		}
		
		int VerticalRulerRow { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowSpaces { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowTabs { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowEOLMarker { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ConvertTabsToSpaces { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowHorizontalRuler { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowVerticalRuler { // is wrapped in text editor control
			get;
			set;
		}
		
		Encoding Encoding {
			get;
			set;
		}
		
		bool EnableFolding { // is wrapped in text editor control
			get;
			set;
		}
		
		bool ShowLineNumbers { // is wrapped in text editor control
			get;
			set;
		}
		
		/// <summary>
		/// The width of a tab.
		/// </summary>
		int TabIndent { // is wrapped in text editor control
			get;
			set;
		}
		
		/// <summary>
		/// The amount of spaces a tab is converted to if ConvertTabsToSpaces is true.
		/// </summary>
		int IndentationSize {
			get;
			set;
		}
		
		IndentStyle IndentStyle { // is wrapped in text editor control
			get;
			set;
		}
		
		DocumentSelectionMode DocumentSelectionMode {
			get;
			set;
		}
		
		Font Font { // is wrapped in text editor control
			get;
			set;
		}
		
		FontContainer FontContainer {
			get;
		}
		
		BracketMatchingStyle  BracketMatchingStyle { // is wrapped in text editor control
			get;
			set;
		}
		
		bool SupportReadOnlySegments {
			get;
			set;
		}
	}
}
