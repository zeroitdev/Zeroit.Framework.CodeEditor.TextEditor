// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="SpanStack.cs" company="">
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

namespace Zeroit.Framework.CodeEditor.TextEditor.Document
{
	/// <summary>
	/// A stack of Span instances. Works like Stack&lt;Span&gt;, but can be cloned quickly
	/// because it is implemented as linked list.
	/// </summary>
	public sealed class SpanStack : ICloneable, IEnumerable<Span>
	{
		internal sealed class StackNode
		{
			public readonly StackNode Previous;
			public readonly Span Data;
			
			public StackNode(StackNode previous, Span data)
			{
				this.Previous = previous;
				this.Data = data;
			}
		}
		
		StackNode top = null;
		
		public Span Pop()
		{
			Span s = top.Data;
			top = top.Previous;
			return s;
		}
		
		public Span Peek()
		{
			return top.Data;
		}
		
		public void Push(Span s)
		{
			top = new StackNode(top, s);
		}
		
		public bool IsEmpty {
			get {
				return top == null;
			}
		}
		
		public SpanStack Clone()
		{
			SpanStack n = new SpanStack();
			n.top = this.top;
			return n;
		}
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		public Enumerator GetEnumerator()
		{
			return new Enumerator(new StackNode(top, null));
		}
		IEnumerator<Span> IEnumerable<Span>.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public struct Enumerator : IEnumerator<Span>
		{
			StackNode c;
			
			internal Enumerator(StackNode node)
			{
				c = node;
			}
			
			public Span Current {
				get {
					return c.Data;
				}
			}
			
			object System.Collections.IEnumerator.Current {
				get {
					return c.Data;
				}
			}
			
			public void Dispose()
			{
				c = null;
			}
			
			public bool MoveNext()
			{
				c = c.Previous;
				return c != null;
			}
			
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}
	}
}
