// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="RedBlackTreeIterator.cs" company="">
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
using System.Collections.Generic;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	internal struct RedBlackTreeIterator<T> : IEnumerator<T>
	{
		internal RedBlackTreeNode<T> node;
		
		internal RedBlackTreeIterator(RedBlackTreeNode<T> node)
		{
			this.node = node;
		}
		
		public bool IsValid {
			get { return node != null; }
		}
		
		public T Current {
			get {
				if (node != null)
					return node.val;
				else
					throw new InvalidOperationException();
			}
		}
		
		object System.Collections.IEnumerator.Current {
			get {
				return this.Current;
			}
		}
		
		void IDisposable.Dispose()
		{
		}
		
		void System.Collections.IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
		
		public bool MoveNext()
		{
			if (node == null)
				return false;
			if (node.right != null) {
				node = node.right.LeftMost;
			} else {
				RedBlackTreeNode<T> oldNode;
				do {
					oldNode = node;
					node = node.parent;
					// we are on the way up from the right part, don't output node again
				} while (node != null && node.right == oldNode);
			}
			return node != null;
		}
		
		public bool MoveBack()
		{
			if (node == null)
				return false;
			if (node.left != null) {
				node = node.left.RightMost;
			} else {
				RedBlackTreeNode<T> oldNode;
				do {
					oldNode = node;
					node = node.parent;
					// we are on the way up from the left part, don't output node again
				} while (node != null && node.left == oldNode);
			}
			return node != null;
		}
	}
}
