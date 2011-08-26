﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2011 Michael Winsor
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Created: Monday, June 27, 2011 8:59:49 AM
// 
#endregion

using System;
using GorgonLibrary;
using GorgonLibrary.Collections;

namespace GorgonLibrary.IO
{
	/// <summary>
	/// A collection of files available from the file system.
	/// </summary>
	/// <remarks>Users should be aware that file names in this collection are NOT case sensitive.</remarks>
	public class GorgonFileSystemFileEntryCollection
		: GorgonBaseNamedObjectCollection<GorgonFileSystemFileEntry>
	{
		#region Variables.
		private GorgonFileSystemDirectory _parent = null;					// File system directory parent.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to set or return a file system file entry by name.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>The file system file entry requested.</returns>
		public GorgonFileSystemFileEntry this[string fileName]
		{
			get
			{
				return GetItem(GorgonPath.RemoveIllegalFilenameChars(fileName));
			}
		}

		/// <summary>
		/// Property to return a file system file entry by index.
		/// </summary>
		/// <param name="index">Index of the file system file entry.</param>
		/// <returns>The file system file entry specified.</returns>
		public GorgonFileSystemFileEntry this[int index]
		{
			get
			{				
				return GetItem(index);
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to remove a file from the list of files.
		/// </summary>
		/// <param name="file">File to remove.</param>
		internal void Remove(GorgonFileSystemFileEntry file)
		{
			RemoveItem(file);
		}

		/// <summary>
		/// Function to add a file system file entry to the collection.
		/// </summary>
		/// <param name="fileEntry">File entry to add.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="fileEntry"/> parameter is NULL (or Nothing in VB.NET).</exception>
		/// <exception cref="System.ArgumentException">Thrown when the <paramref name="fileEntry"/> parameter already exists in the collection.</exception>
		internal void Add(GorgonFileSystemFileEntry fileEntry)
		{
			if (fileEntry == null)
				throw new ArgumentNullException("fileEntry");

			if (Contains(fileEntry.Name))
				throw new ArgumentException("The file entry '" + fileEntry.Name + "' already exists in this collection.", "fileEntry");

			fileEntry.Directory = _parent;
			AddItem(fileEntry);
		}

		/// <summary>
		/// Function to clear all directories from this collection.
		/// </summary>
		internal void Clear()
		{
			foreach (GorgonFileSystemFileEntry fileEntry in this)
				fileEntry.Directory = null;
			ClearItems();
		}
			
		/// <summary>
		/// Function to return whether an item with the specified name exists in this collection.
		/// </summary>
		/// <param name="name">Name of the item to find.</param>
		/// <returns>TRUE if found, FALSE if not.</returns>
		public override bool Contains(string name)
		{
			return base.Contains(GorgonPath.RemoveIllegalFilenameChars(name));
		}

		/// <summary>
		/// Function to return the index of a file entry name.
		/// </summary>
		/// <param name="fileName">Name of the file to return an index for.</param>
		/// <returns>The index of the directory or -1 if it could not be found.</returns>
		public int IndexOf(string fileName)
		{
			fileName = GorgonPath.RemoveIllegalFilenameChars(fileName);
			return IndexOf(this[fileName]);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonFileSystemFileEntryCollection"/> class.
		/// </summary>
		/// <param name="parent">The parent directory that owns this collection.</param>
		/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="parent"/> argument is NULL.</exception>
		internal GorgonFileSystemFileEntryCollection(GorgonFileSystemDirectory parent)			
			: base(false)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			_parent = parent;
		}
		#endregion
	}
}
