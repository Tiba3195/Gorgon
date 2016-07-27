﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2016 Michael Winsor
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
// Created: July 25, 2016 12:40:16 AM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Gorgon.Core;
using Gorgon.Graphics.Properties;
using Gorgon.Native;
using Gorgon.Reflection;
using D3D11 = SharpDX.Direct3D11;

namespace Gorgon.Graphics
{
	/// <summary>
	/// Provides the necessary information required to set up a vertex buffer.
	/// </summary>
	public class GorgonVertexBufferInfo
		: IGorgonCloneable<GorgonVertexBufferInfo>
	{
		#region Properties.
		/// <summary>
		/// Property to set or return the intend
		/// </summary>
		public D3D11.ResourceUsage Usage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the size of the buffer, in bytes.
		/// </summary>
		/// <remarks>
		/// This value should be larger than 0, or else an exception will be thrown when the buffer is created.
		/// </remarks>
		public int SizeInBytes
		{
			get;
			set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to create a <see cref="GorgonVertexBufferInfo"/> based on the type representing a vertex.
		/// </summary>
		/// <typeparam name="T">The type of data representing a vertex. This must be a value type.</typeparam>
		/// <param name="count">The number of vertices to store in the buffer.</param>
		/// <param name="usage">[Optional] The usage parameter for the vertex buffer.</param>
		/// <returns>A new <see cref="GorgonVertexBufferInfo"/> to use when creating a <see cref="GorgonVertexBuffer"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="count"/> parameter is less than 1.</exception>
		/// <exception cref="GorgonException">Thrown when the type specified by <typeparamref name="T"/> is not safe for use with native functions (see <see cref="GorgonReflectionExtensions.IsFieldSafeForNative"/>).
		/// <para>-or-</para>
		/// <para>Thrown when the type specified by <typeparamref name="T"/> does not contain any public members.</para>
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method is offered as a convenience to simplify the creation of the required info for a <see cref="GorgonVertexBuffer"/>. It will automatically determine the size of a vertex based on the size 
		/// of a type specified by <typeparamref name="T"/> and fill in the <see cref="SizeInBytes"/> with the correct size.
		/// </para>
		/// <para>
		/// This method requires that the type passed by <typeparamref name="T"/> have its members decorated with the <see cref="InputElementAttribute"/>. This is used to determine which members of the 
		/// type are to be used in determining the size of the type.
		/// </para>
		/// </remarks>
		/// <seealso cref="GorgonReflectionExtensions.IsFieldSafeForNative"/>
		/// <seealso cref="GorgonReflectionExtensions.IsSafeForNative(Type)"/>
		/// <seealso cref="GorgonReflectionExtensions.IsSafeForNative(Type,out IReadOnlyList{FieldInfo})"/>
		public static GorgonVertexBufferInfo CreateFromType<T>(int count, D3D11.ResourceUsage usage = D3D11.ResourceUsage.Default)
			where T : struct
		{
			if (count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			List<Tuple<FieldInfo, InputElementAttribute>> fields = GorgonInputLayout.GetFieldInfoList(typeof(T));

			if (fields.Count == 0)
			{
				throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GORGFX_ERR_VERTEX_NO_FIELDS, typeof(T).FullName));
			}
			
			return new GorgonVertexBufferInfo
			       {
				       Usage = usage,
				       SizeInBytes = count * DirectAccess.SizeOf<T>()
			       };
		}

		/// <summary>
		/// Function to create a <see cref="GorgonVertexBufferInfo"/> based on a <see cref="GorgonInputLayout"/> and the intended slot for vertex data.
		/// </summary>
		/// <param name="layout">The <see cref="GorgonInputLayout"/> to evaluate.</param>
		/// <param name="count">The number of vertices to store in the buffer.</param>
		/// <param name="slot">The intended slot to use for the vertex data.</param>
		/// <param name="usage">[Optional] The usage parameter for the vertex buffer.</param>
		/// <returns>A new <see cref="GorgonVertexBufferInfo"/> to use when creating a <see cref="GorgonVertexBuffer"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="count"/> parameter is less than 1.
		/// <para>-or-</para>
		/// <para>Thrown when the <paramref name="slot"/> is not present in the <paramref name="layout"/>.</para>
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method is offered as a convenience to simplify the creation of the required info for a <see cref="GorgonVertexBuffer"/>. It will automatically determine the size of a vertex based on the size 
		/// of the specified <paramref name="slot"/> in the <paramref name="layout"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="GorgonInputLayout"/>
		public static GorgonVertexBufferInfo CreateFromInputLayout(GorgonInputLayout layout, int slot, int count, D3D11.ResourceUsage usage = D3D11.ResourceUsage.Default)
		{
			if (layout == null)
			{
				throw new ArgumentNullException(nameof(layout));
			}

			if (count < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			int sizeInBytes = layout.GetSlotSize(slot);

			if (sizeInBytes < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(slot));
			}

			return new GorgonVertexBufferInfo
			       {
				       Usage = usage,
				       SizeInBytes = sizeInBytes * count
			       };
		}

		/// <summary>
		/// Function to clone an object.
		/// </summary>
		/// <returns>The cloned object.</returns>
		public GorgonVertexBufferInfo Clone()
		{
			return new GorgonVertexBufferInfo
			       {
				       Usage = Usage,
					   SizeInBytes = SizeInBytes
			       };
		}
		#endregion

		#region Constructor/Finalizer.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonVertexBufferInfo"/> class.
		/// </summary>
		public GorgonVertexBufferInfo()
		{
			Usage = D3D11.ResourceUsage.Default;
		}
		#endregion
	}
}
