// ***********************************************************************
// Assembly         : Zeroit.Framework.CodeEditor.TextEditor
// Author           : ZEROIT
// Created          : 01-03-2019
//
// Last Modified By : ZEROIT
// Last Modified On : 01-26-2019
// ***********************************************************************
// <copyright file="CheckedList.cs" company="">
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
using System.Threading;

namespace Zeroit.Framework.CodeEditor.TextEditor.Util
{
	/// <summary>
	/// A IList{T} that checks that it is only accessed on the thread that created it, and that
	/// it is not modified while an enumerator is running.
	/// </summary>
	sealed class CheckedList<T> : IList<T>
	{
		readonly int threadID;
		readonly IList<T> baseList;
		int enumeratorCount;
		
		public CheckedList() : this(new List<T>()) {}
		
		public CheckedList(IList<T> baseList)
		{
			if (baseList == null)
				throw new ArgumentNullException("baseList");
			this.baseList = baseList;
			this.threadID = Thread.CurrentThread.ManagedThreadId;
		}
		
		void CheckRead()
		{
			if (Thread.CurrentThread.ManagedThreadId != threadID)
				throw new InvalidOperationException("CheckList cannot be accessed from this thread!");
		}
		
		void CheckWrite()
		{
			if (Thread.CurrentThread.ManagedThreadId != threadID)
				throw new InvalidOperationException("CheckList cannot be accessed from this thread!");
			if (enumeratorCount != 0)
				throw new InvalidOperationException("CheckList cannot be written to while enumerators are active!");
		}
		
		public T this[int index] {
			get {
				CheckRead();
				return baseList[index];
			}
			set {
				CheckWrite();
				baseList[index] = value;
			}
		}
		
		public int Count {
			get {
				CheckRead();
				return baseList.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				CheckRead();
				return baseList.IsReadOnly;
			}
		}
		
		public int IndexOf(T item)
		{
			CheckRead();
			return baseList.IndexOf(item);
		}
		
		public void Insert(int index, T item)
		{
			CheckWrite();
			baseList.Insert(index, item);
		}
		
		public void RemoveAt(int index)
		{
			CheckWrite();
			baseList.RemoveAt(index);
		}
		
		public void Add(T item)
		{
			CheckWrite();
			baseList.Add(item);
		}
		
		public void Clear()
		{
			CheckWrite();
			baseList.Clear();
		}
		
		public bool Contains(T item)
		{
			CheckRead();
			return baseList.Contains(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			CheckRead();
			baseList.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item)
		{
			CheckWrite();
			return baseList.Remove(item);
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			CheckRead();
			return Enumerate();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			CheckRead();
			return Enumerate();
		}
		
		IEnumerator<T> Enumerate()
		{
			CheckRead();
			try {
				enumeratorCount++;
				foreach (T val in baseList) {
					yield return val;
					CheckRead();
				}
			} finally {
				enumeratorCount--;
				CheckRead();
			}
		}
	}
}
