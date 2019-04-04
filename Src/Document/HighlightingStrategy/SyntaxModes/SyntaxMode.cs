// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="SyntaxMode.cs" company="">
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
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	public class SyntaxMode
	{
		string   fileName;
		string   name;
		string[] extensions;
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string[] Extensions {
			get {
				return extensions;
			}
			set {
				extensions = value;
			}
		}
		
		public SyntaxMode(string fileName, string name, string extensions)
		{
			this.fileName   = fileName;
			this.name       = name;
			this.extensions = extensions.Split(';', '|', ',');
		}
		
		public SyntaxMode(string fileName, string name, string[] extensions)
		{
			this.fileName = fileName;
			this.name = name;
			this.extensions = extensions;
		}
		
		public static List<SyntaxMode> GetSyntaxModes(Stream xmlSyntaxModeStream)
		{
			XmlTextReader reader = new XmlTextReader(xmlSyntaxModeStream);
			List<SyntaxMode> syntaxModes = new List<SyntaxMode>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						switch (reader.Name) {
							case "SyntaxModes":
								string version = reader.GetAttribute("version");
								if (version != "1.0") {
									throw new HighlightingDefinitionInvalidException("Unknown syntax mode file defininition with version " + version);
								}
								break;
							case "Mode":
								syntaxModes.Add(new SyntaxMode(reader.GetAttribute("file"), 
								                               reader.GetAttribute("name"),
								                               reader.GetAttribute("extensions")));
								break;
							default:
								throw new HighlightingDefinitionInvalidException("Unknown node in syntax mode file :" + reader.Name);
						}
						break;
				}
			}
			reader.Close();
			return syntaxModes;
		}
		public override string ToString() 
		{
			return String.Format("[SyntaxMode: FileName={0}, Name={1}, Extensions=({2})]", fileName, name, String.Join(",", extensions));
		}
	}
}
