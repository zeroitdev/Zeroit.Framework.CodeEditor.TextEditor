// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="IInsightDataProvider.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Gui.InsightWindow
{
	public interface IInsightDataProvider
	{
		/// <summary>
		/// Tells the insight provider to prepare its data.
		/// </summary>
		/// <param name="fileName">The name of the edited file</param>
		/// <param name="textArea">The text area in which the file is being edited</param>
		void SetupDataProvider(string fileName, TextArea textArea);
		
		/// <summary>
		/// Notifies the insight provider that the caret offset has changed.
		/// </summary>
		/// <returns>Return true to close the insight window (e.g. when the
		/// caret was moved outside the region where insight is displayed for).
		/// Return false to keep the window open.</returns>
		bool CaretOffsetChanged();
		
		/// <summary>
		/// Gets the text to display in the insight window.
		/// </summary>
		/// <param name="number">The number of the active insight entry.
		/// Multiple insight entries might be multiple overloads of the same method.</param>
		/// <returns>The text to display, e.g. a multi-line string where
		/// the first line is the method definition, followed by a description.</returns>
		string GetInsightData(int number);
		
		/// <summary>
		/// Gets the number of available insight entries, e.g. the number of available
		/// overloads to call.
		/// </summary>
		int InsightDataCount {
			get;
		}
		
		/// <summary>
		/// Gets the index of the entry to initially select.
		/// </summary>
		int DefaultIndex {
			get;
		}
	}
}
