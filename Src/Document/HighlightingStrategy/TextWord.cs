// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TextWord.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	public enum TextWordType {
		Word,
		Space,
		Tab
	}
	
	/// <summary>
	/// This class represents single words with color information, two special versions of a word are
	/// spaces and tabs.
	/// </summary>
	public class TextWord
	{
		HighlightColor  color;
		LineSegment     line;
		IDocument       document;
		
		int          offset;
		int          length;
		
		public sealed class SpaceTextWord : TextWord
		{
			public SpaceTextWord()
			{
				length = 1;
			}
			
			public SpaceTextWord(HighlightColor color)
			{
				length = 1;
				base.SyntaxColor = color;
			}
			
			public override Font GetFont(FontContainer fontContainer)
			{
				return null;
			}
			
			public override TextWordType Type {
				get {
					return TextWordType.Space;
				}
			}
			public override bool IsWhiteSpace {
				get {
					return true;
				}
			}
		}
		
		public sealed class TabTextWord : TextWord
		{
			public TabTextWord()
			{
				length = 1;
			}
			public TabTextWord(HighlightColor color)
			{
				length = 1;
				base.SyntaxColor = color;
			}
			
			public override Font GetFont(FontContainer fontContainer)
			{
				return null;
			}
			
			public override TextWordType Type {
				get {
					return TextWordType.Tab;
				}
			}
			public override bool IsWhiteSpace {
				get {
					return true;
				}
			}
		}
		
		static TextWord spaceWord = new SpaceTextWord();
		static TextWord tabWord   = new TabTextWord();
		
		bool hasDefaultColor;
		
		public static TextWord Space {
			get {
				return spaceWord;
			}
		}
		
		public static TextWord Tab {
			get {
				return tabWord;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public int Length {
			get {
				return length;
			}
		}
		
		/// <summary>
		/// Splits the <paramref name="word"/> into two parts: the part before <paramref name="pos"/> is assigned to
		/// the reference parameter <paramref name="word"/>, the part after <paramref name="pos"/> is returned.
		/// </summary>
		public static TextWord Split(ref TextWord word, int pos)
		{
			#if DEBUG
			if (word.Type != TextWordType.Word)
				throw new ArgumentException("word.Type must be Word");
			if (pos <= 0)
				throw new ArgumentOutOfRangeException("pos", pos, "pos must be > 0");
			if (pos >= word.Length)
				throw new ArgumentOutOfRangeException("pos", pos, "pos must be < word.Length");
			#endif
			TextWord after = new TextWord(word.document, word.line, word.offset + pos, word.length - pos, word.color, word.hasDefaultColor);
			word = new TextWord(word.document, word.line, word.offset, pos, word.color, word.hasDefaultColor);
			return after;
		}
		
		public bool HasDefaultColor {
			get {
				return hasDefaultColor;
			}
		}
		
		public virtual TextWordType Type {
			get {
				return TextWordType.Word;
			}
		}
		
		public string Word {
			get {
				if (document == null) {
					return String.Empty;
				}
				return document.GetText(line.Offset + offset, length);
			}
		}
		
		public virtual Font GetFont(FontContainer fontContainer)
		{
			return color.GetFont(fontContainer);
		}
		
		public Color Color {
			get {
				if (color == null)
					return Color.Black;
				else
					return color.Color;
			}
		}
		
		public bool Bold {
			get {
				if (color == null)
					return false;
				else
					return color.Bold;
			}
		}
		
		public bool Italic {
			get {
				if (color == null)
					return false;
				else
					return color.Italic;
			}
		}
		
		public HighlightColor SyntaxColor {
			get {
				return color;
			}
			set {
				Debug.Assert(value != null);
				color = value;
			}
		}
		
		public virtual bool IsWhiteSpace {
			get {
				return false;
			}
		}
		
		protected TextWord()
		{
		}
		
		// TAB
		public TextWord(IDocument document, LineSegment line, int offset, int length, HighlightColor color, bool hasDefaultColor)
		{
			Debug.Assert(document != null);
			Debug.Assert(line != null);
			Debug.Assert(color != null);
			
			this.document = document;
			this.line  = line;
			this.offset = offset;
			this.length = length;
			this.color = color;
			this.hasDefaultColor = hasDefaultColor;
		}
		
		/// <summary>
		/// Converts a <see cref="TextWord"/> instance to string (for debug purposes)
		/// </summary>
		public override string ToString()
		{
			return "[TextWord: Word = " + Word + ", Color = " + Color + "]";
		}
	}
}
