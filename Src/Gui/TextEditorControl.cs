// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="TextEditorControl.cs" company="">
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

using Zeroit.Framework.CodeEditor.TextEditor.Document;

namespace Zeroit.Framework.CodeEditor.TextEditor
{
	/// <summary>
	/// This class is used for a basic text area control
	/// </summary>
	[ToolboxBitmap("Zeroit.Framework.CodeEditor.TextEditor.Resources.CodeView.bmp")]
	[ToolboxItem(true)]
	public class CodeView : TextEditorControlBase
	{

	    public enum LanguageMode
	    {
	        ASPxHtml,
            Bat,
            Boo,
            Coco,
            Cpp,
            Csharp,
            Html,
            Java,
            Javascript,
            Patch,
            Php,
            Tex,
            VBNet,
            Xml
	        
        }


        private bool split = false;
		protected Panel textAreaPanel     = new Panel();
		TextAreaControl primaryTextArea;
		Splitter        textAreaSplitter  = null;
		TextAreaControl secondaryTextArea = null;
        
		PrintDocument   printDocument = null;

	    private LanguageMode language = LanguageMode.Csharp;

	    public bool Split
	    {
	        get { return split; }
	        set
	        {
	            if (value)
	            {
	                Splitted();
	            }

                split = value;
	            
	            Invalidate();
	        }
	    }

	    public LanguageMode Language
	    {
	        get { return language; }
	        set
	        {
                language = value;

	            switch (value)
	            {
	                case LanguageMode.ASPxHtml:
	                    SetHighlighting("ASP/XHTML");
                        break;
	                case LanguageMode.Bat:
	                    SetHighlighting("BAT");
                        break;
	                case LanguageMode.Boo:
	                    SetHighlighting("Boo");
                        break;
	                case LanguageMode.Coco:
	                    SetHighlighting("Coco");
                        break;
	                case LanguageMode.Cpp:
	                    SetHighlighting("C++.NET");
                        break;
	                case LanguageMode.Csharp:
	                    SetHighlighting("C#");
                        break;
	                case LanguageMode.Html:
	                    SetHighlighting("HTML");
                        break;
	                case LanguageMode.Java:
	                    SetHighlighting("Java");
                        break;
	                case LanguageMode.Javascript:
	                    SetHighlighting("JavaScript");
                        break;
	                case LanguageMode.Patch:
	                    SetHighlighting("Patch");
                        break;
	                case LanguageMode.Php:
	                    SetHighlighting("PHP");
                        break;
	                case LanguageMode.Tex:
	                    SetHighlighting("TeX");
                        break;
	                case LanguageMode.VBNet:
	                    SetHighlighting("VBNET");
                        break;
	                case LanguageMode.Xml:
	                    SetHighlighting("XML");
                        break;
	                default:
	                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
	            }

	            Invalidate();
	        }
	    }

        [TypeConverter(typeof(ExpandableObjectConverter))]
	    public TextAreaControl MainArea
	    {
	        get { return primaryTextArea; }
	        set
	        {
                primaryTextArea = value;
	            Invalidate();
	        }
	    }

	    [TypeConverter(typeof(ExpandableObjectConverter))]
	    public TextAreaControl SecondaryArea
	    {
	        get { return secondaryTextArea; }
	        set
	        {
	            secondaryTextArea = value;
	            Invalidate();
	        }
	    }

        [Browsable(false)]
		public PrintDocument PrintDocument {
			get {
				if (printDocument == null) {
					printDocument = new PrintDocument();
					printDocument.BeginPrint += new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  += new PrintPageEventHandler(this.PrintPage);
				}
				return printDocument;
			}
		}
		
		TextAreaControl activeTextAreaControl;
		
		public override TextAreaControl ActiveTextAreaControl {
			get {
				return activeTextAreaControl;
			}
		}
		
		protected void SetActiveTextAreaControl(TextAreaControl value)
		{
			if (activeTextAreaControl != value) {
				activeTextAreaControl = value;
				
				if (ActiveTextAreaControlChanged != null) {
					ActiveTextAreaControlChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActiveTextAreaControlChanged;
		
		public CodeView()
		{
			SetStyle(ControlStyles.ContainerControl, true);
			
			textAreaPanel.Dock = DockStyle.Fill;
			
			Document = (new DocumentFactory()).CreateDocument();
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
			
			primaryTextArea  = new TextAreaControl(this);
            primaryTextArea.HScrollBar.Visible = false;
            primaryTextArea.VScrollBar.Visible = false;

            activeTextAreaControl = primaryTextArea;
			primaryTextArea.TextArea.GotFocus += delegate {
				SetActiveTextAreaControl(primaryTextArea);
			};
			primaryTextArea.Dock = DockStyle.Fill;
			textAreaPanel.Controls.Add(primaryTextArea);
			InitializeTextAreaControl(primaryTextArea);
			Controls.Add(textAreaPanel);
			ResizeRedraw = true;
			Document.UpdateCommited += new EventHandler(CommitUpdateRequested);
			OptionsChanged();

		    SetHighlighting("C#");
		}
		
		protected virtual void InitializeTextAreaControl(TextAreaControl newControl)
		{
		}
		
		public override void OptionsChanged()
		{
			primaryTextArea.OptionsChanged();
			if (secondaryTextArea != null) {
				secondaryTextArea.OptionsChanged();
			}
		}
		
		public void Splitted()
		{
			if (secondaryTextArea == null) {
				secondaryTextArea = new TextAreaControl(this);
				secondaryTextArea.Dock = DockStyle.Bottom;
				secondaryTextArea.Height = Height / 2;
				
				secondaryTextArea.TextArea.GotFocus += delegate {
					SetActiveTextAreaControl(secondaryTextArea);
				};
				
				textAreaSplitter =  new Splitter();
				textAreaSplitter.BorderStyle = BorderStyle.FixedSingle ;
				textAreaSplitter.Height = 8;
				textAreaSplitter.Dock = DockStyle.Bottom;
				textAreaPanel.Controls.Add(textAreaSplitter);
				textAreaPanel.Controls.Add(secondaryTextArea);
				InitializeTextAreaControl(secondaryTextArea);
				secondaryTextArea.OptionsChanged();
			} else {
				SetActiveTextAreaControl(primaryTextArea);
				
				textAreaPanel.Controls.Remove(secondaryTextArea);
				textAreaPanel.Controls.Remove(textAreaSplitter);
				
				secondaryTextArea.Dispose();
				textAreaSplitter.Dispose();
				secondaryTextArea = null;
				textAreaSplitter  = null;
			}
		}
		
		[Browsable(false)]
		public bool EnableUndo {
			get {
				return Document.UndoStack.CanUndo;
			}
		}
		
		[Browsable(false)]
		public bool EnableRedo {
			get {
				return Document.UndoStack.CanRedo;
			}
		}

		public void Undo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanUndo) {
				BeginUpdate();
				Document.UndoStack.Undo();
				
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public void Redo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanRedo) {
				BeginUpdate();
				Document.UndoStack.Redo();
				
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public void SetHighlighting(string name)
		{
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(name);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (printDocument != null) {
					printDocument.BeginPrint -= new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  -= new PrintPageEventHandler(this.PrintPage);
					printDocument = null;
				}
				Document.UndoStack.ClearAll();
				Document.UpdateCommited -= new EventHandler(CommitUpdateRequested);
				if (textAreaPanel != null) {
					if (secondaryTextArea != null) {
						secondaryTextArea.Dispose();
						textAreaSplitter.Dispose();
						secondaryTextArea = null;
						textAreaSplitter  = null;
					}
					if (primaryTextArea != null) {
						primaryTextArea.Dispose();
					}
					textAreaPanel.Dispose();
					textAreaPanel = null;
				}
			}
			base.Dispose(disposing);
		}
		
		#region Update Methods
		public override void EndUpdate()
		{
			base.EndUpdate();
			Document.CommitUpdate();
			if (!IsInUpdate) {
				ActiveTextAreaControl.Caret.OnEndUpdate();
			}
		}
		
		void CommitUpdateRequested(object sender, EventArgs e)
		{
			if (IsInUpdate) {
				return;
			}
			foreach (TextAreaUpdate update in Document.UpdateQueue) {
				switch (update.TextAreaUpdateType) {
					case TextAreaUpdateType.PositionToEnd:
						this.primaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.PositionToLineEnd:
					case TextAreaUpdateType.SingleLine:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.SinglePosition:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						}
						break;
					case TextAreaUpdateType.LinesBetween:
						this.primaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						}
						break;
					case TextAreaUpdateType.WholeTextArea:
						this.primaryTextArea.TextArea.Invalidate();
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.Invalidate();
						}
						break;
				}
			}
			Document.UpdateQueue.Clear();
//			this.primaryTextArea.TextArea.Update();
//			if (this.secondaryTextArea != null) {
//				this.secondaryTextArea.TextArea.Update();
//			}
		}
		#endregion
		
		#region Printing routines
		int          curLineNr = 0;
		float        curTabIndent = 0;
		StringFormat printingStringFormat;
		
		void BeginPrint(object sender, PrintEventArgs ev)
		{
			curLineNr = 0;
			printingStringFormat = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
			
			// 100 should be enough for everyone ...err ?
			float[] tabStops = new float[100];
			for (int i = 0; i < tabStops.Length; ++i) {
				tabStops[i] = TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth;
			}
			
			printingStringFormat.SetTabStops(0, tabStops);
		}
		
		void Advance(ref float x, ref float y, float maxWidth, float size, float fontHeight)
		{
			if (x + size < maxWidth) {
				x += size;
			} else {
				x  = curTabIndent;
				y += fontHeight;
			}
		}
		
		// btw. I hate source code duplication ... but this time I don't care !!!!
		float MeasurePrintingHeight(Graphics g, LineSegment line, float maxWidth)
		{
			float xPos = 0;
			float yPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0;
			FontContainer fontContainer = TextEditorProperties.FontContainer;
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, maxWidth, primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, maxWidth, TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * primaryTextArea.TextArea.TextView.GetWidth(' ');
//						}
						SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(maxWidth, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, maxWidth, drawingSize.Width, fontHeight);
						break;
				}
			}
			return yPos + fontHeight;
		}
		
		void DrawLine(Graphics g, LineSegment line, float yPos, RectangleF margin)
		{
			float xPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0 ;
			
			FontContainer fontContainer = TextEditorProperties.FontContainer;
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, margin.Width, primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, margin.Width, TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * primaryTextArea.TextArea.TextView.GetWidth(' ');
//						}
						g.DrawString(word.Word, word.GetFont(fontContainer), BrushRegistry.GetBrush(word.Color), xPos + margin.X, yPos);
						SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(margin.Width, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, margin.Width, drawingSize.Width, fontHeight);
						break;
				}
			}
		}
		
		void PrintPage(object sender, PrintPageEventArgs ev)
		{
			Graphics g = ev.Graphics;
			float yPos = ev.MarginBounds.Top;
			
			while (curLineNr < Document.TotalNumberOfLines) {
				LineSegment curLine  = Document.GetLineSegment(curLineNr);
				if (curLine.Words != null) {
					float drawingHeight = MeasurePrintingHeight(g, curLine, ev.MarginBounds.Width);
					if (drawingHeight + yPos > ev.MarginBounds.Bottom) {
						break;
					}
					
					DrawLine(g, curLine, yPos, ev.MarginBounds);
					yPos += drawingHeight;
				}
				++curLineNr;
			}
			
			// If more lines exist, print another page.
			ev.HasMorePages = curLineNr < Document.TotalNumberOfLines;
		}
        #endregion

        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);


            ScrollBarsRevealed();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);


            ScrollBarsRevealed();
        }

	    private void ScrollBarsRevealed()
	    {
	        Graphics g = CreateGraphics();
	        SizeF fs = g.MeasureString(Text, Font);
	        primaryTextArea.HScrollBar.Visible = fs.Width > (Width - primaryTextArea.HScrollBar.Height);

	        primaryTextArea.VScrollBar.Visible =
	            ((float) this.Document.TotalNumberOfLines / (int) this.Height) > (0.0536585365853659);

	        if (secondaryTextArea != null)
	        {
	            secondaryTextArea.HScrollBar.Visible = fs.Width > (Width - primaryTextArea.HScrollBar.Height);

	            secondaryTextArea.VScrollBar.Visible =
	                ((float)this.Document.TotalNumberOfLines / (int)this.Height) > (0.0536585365853659);

            }

        }

	}
}
