// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="HighlightingDefinitionParser.cs" company="">
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
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	public static class HighlightingDefinitionParser
	{
		public static DefaultHighlightingStrategy Parse(SyntaxMode syntaxMode, XmlReader xmlReader)
		{
			return Parse(null, syntaxMode, xmlReader);
		}

		public static DefaultHighlightingStrategy Parse(DefaultHighlightingStrategy highlighter, SyntaxMode syntaxMode, XmlReader xmlReader)
		{
			if (syntaxMode == null)
				throw new ArgumentNullException("syntaxMode");
			if (xmlReader == null)
				throw new ArgumentNullException("xmlTextReader");
			try {
				List<ValidationEventArgs> errors = null;
				XmlReaderSettings settings = new XmlReaderSettings();
				Stream shemaStream = typeof(HighlightingDefinitionParser).Assembly.GetManifestResourceStream("Zeroit.Framework.CodeEditor.TextEditor.Resources.Mode.xsd");
				settings.Schemas.Add("", new XmlTextReader(shemaStream));
				settings.Schemas.ValidationEventHandler += delegate(object sender, ValidationEventArgs args) {
					if (errors == null) {
						errors = new List<ValidationEventArgs>();
					}
					errors.Add(args);
				};
				settings.ValidationType = ValidationType.Schema;
				XmlReader validatingReader = XmlReader.Create(xmlReader, settings);

				XmlDocument doc = new XmlDocument();
				doc.Load(validatingReader);
				
				if (highlighter == null)
					highlighter = new DefaultHighlightingStrategy(doc.DocumentElement.Attributes["name"].InnerText);
				
				if (doc.DocumentElement.HasAttribute("extends")) {
					KeyValuePair<SyntaxMode, ISyntaxModeFileProvider> entry = HighlightingManager.Manager.FindHighlighterEntry(doc.DocumentElement.GetAttribute("extends"));
					if (entry.Key == null) {
						throw new HighlightingDefinitionInvalidException("Cannot find referenced highlighting source " + doc.DocumentElement.GetAttribute("extends"));
					} else {
						highlighter = Parse(highlighter, entry.Key, entry.Value.GetSyntaxModeFile(entry.Key));
						if (highlighter == null) return null;
					}
				}
				if (doc.DocumentElement.HasAttribute("extensions")) {
					highlighter.Extensions = doc.DocumentElement.GetAttribute("extensions").Split(new char[] { ';', '|' });
				}
				
				XmlElement environment = doc.DocumentElement["Environment"];
				if (environment != null) {
					foreach (XmlNode node in environment.ChildNodes) {
						if (node is XmlElement) {
							XmlElement el = (XmlElement)node;
							if (el.Name == "Custom") {
								highlighter.SetColorFor(el.GetAttribute("name"), el.HasAttribute("bgcolor") ? new HighlightBackground(el) : new HighlightColor(el));
							} else {
								highlighter.SetColorFor(el.Name, el.HasAttribute("bgcolor") ? new HighlightBackground(el) : new HighlightColor(el));
							}
						}
					}
				}
				
				// parse properties
				if (doc.DocumentElement["Properties"]!= null) {
					foreach (XmlElement propertyElement in doc.DocumentElement["Properties"].ChildNodes) {
						highlighter.Properties[propertyElement.Attributes["name"].InnerText] =  propertyElement.Attributes["value"].InnerText;
					}
				}
				
				if (doc.DocumentElement["Digits"]!= null) {
					highlighter.DigitColor = new HighlightColor(doc.DocumentElement["Digits"]);
				}
				
				XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("RuleSet");
				foreach (XmlElement element in nodes) {
					highlighter.AddRuleSet(new HighlightRuleSet(element));
				}
				
				xmlReader.Close();
				
				if (errors != null) {
					StringBuilder msg = new StringBuilder();
					foreach (ValidationEventArgs args in errors) {
						msg.AppendLine(args.Message);
					}
					throw new HighlightingDefinitionInvalidException(msg.ToString());
				} else {
					return highlighter;
				}
			} catch (Exception e) {
				throw new HighlightingDefinitionInvalidException("Could not load mode definition file '" + syntaxMode.FileName + "'.\n", e);
			}
		}
	}
}
