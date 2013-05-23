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
// Created: Thursday, December 15, 2011 1:44:18 PM
// 
#endregion

using System;
using System.ComponentModel;
using GorgonLibrary.Diagnostics;
using DX = SharpDX;
using D3D = SharpDX.Direct3D11;
using GorgonLibrary.IO;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// A constant buffer for shaders.
	/// </summary>
	/// <remarks>Constant buffers are used to send groups of scalar values to a shader.  The buffer is just a block of allocated memory that is written to by one of the various Write methods.
	/// <para>Typically, the user will define a value type that matches a constant buffer layout.  Then, if the value type uses nothing but blittable types, the user can then write the entire 
	/// value type structure to the constant buffer.  If the value type contains more complex types, such as arrays, then the user can write each item in the value type to a variable in the constant 
	/// buffer.  Please note that the names for the variables in the value type and the shader do -not- have to match, although, for the sake of clarity, it is a good idea that they do.</para>
	/// <para>In order to write to a constant buffer, the user must <see cref="GorgonLibrary.Graphics.GorgonBaseBuffer.Lock">lock</see> the buffer beforehand, and unlock it when done.  Failure to do so will result in an exception.</para>
	/// <para>Constant buffers follow very specific rules, which are explained at http://msdn.microsoft.com/en-us/library/windows/desktop/bb509632(v=vs.85).aspx </para>
	/// <para>When passing a value type to the constant buffer, ensure that the type has a System.Runtime.InteropServices.StructLayout attribute assigned to it, and that the layout is explicit.  Also, the size of the 
	/// value type must be a multiple of 16, so padding variables may be required.</para>
	/// </remarks>
	public sealed class GorgonConstantBuffer
		: GorgonBaseBuffer
	{
		#region Properties.
		/// <summary>
		/// Property to return the settings.
		/// </summary>
		public new GorgonConstantBufferSettings Settings
		{
			get
			{
				return (GorgonConstantBufferSettings)base.Settings;
			}
		}
		#endregion

		#region Methods.
        /// <summary>
        /// Function to retrieve the staging buffer for this buffer.
        /// </summary>
        /// <returns>
        /// The staging buffer for this buffer.
        /// </returns>
        protected override GorgonBaseBuffer GetStagingBufferImpl()
        {
            throw new NotSupportedException("Constant buffers do not support staging usage.");
        }
        
		/// <summary>
		/// Function to initialize the buffer.
		/// </summary>
		/// <param name="value">Value used to initialize the buffer.</param>
		protected override void InitializeImpl(GorgonDataStream value)
		{
			if (D3DResource != null)
			{
				D3DResource.Dispose();
				D3DResource = null;
			}

			var desc = new D3D.BufferDescription
				{
					BindFlags = D3D.BindFlags.ConstantBuffer,
					CpuAccessFlags = D3DCPUAccessFlags,
					OptionFlags = D3D.ResourceOptionFlags.None,
					SizeInBytes = SizeInBytes,
					StructureByteStride = 0,
					Usage = D3DUsage
				};

			if (value != null)
			{
				long position = value.Position;

				using(var dxStream = new DX.DataStream(value.BasePointer, value.Length - position, true, true))
				{
					D3DResource = new D3D.Buffer(Graphics.D3DDevice, dxStream, desc);
				}
			}
			else
			{
				D3DResource = new D3D.Buffer(Graphics.D3DDevice, desc);
			}

			GorgonRenderStatistics.ConstantBufferCount++;
			GorgonRenderStatistics.ConstantBufferSize += ((D3D.Buffer)D3DResource).Description.SizeInBytes;
		}

		/// <summary>
		/// Function to clean up the resource object.
		/// </summary>		
		protected override void CleanUpResource()
		{
			// If we're bound with a pixel or vertex shader, then unbind.
			Graphics.Shaders.Unbind(this);

		    if (IsLocked)
		    {
		        Unlock();
		    }

		    if (D3DResource == null)
		    {
		        return;
		    }

		    GorgonRenderStatistics.ConstantBufferCount--;
		    GorgonRenderStatistics.ConstantBufferSize -= D3DBuffer.Description.SizeInBytes;

		    D3DResource.Dispose();
		    D3DResource = null;

            Gorgon.Log.Print("Destroyed {0} {1}.", LoggingLevel.Verbose, GetType().FullName, Name);
		}

		/// <summary>
		/// Function to update the buffer.
		/// </summary>
		/// <param name="stream">Stream containing the data used to update the buffer.</param>
		/// <param name="offset">Offset, in bytes, into the buffer to start writing at.</param>
		/// <param name="size">The number of bytes to write.</param>
		protected override void UpdateImpl(GorgonDataStream stream, int offset, int size)
		{
			Graphics.Context.UpdateSubresource(
				new DX.DataBox
				    {
					DataPointer = stream.PositionPointer,
					RowPitch = 0,
					SlicePitch = 0
				}, 
				D3DResource);
		}

        /// <summary>
        /// Function to return this buffer as a staging buffer.
        /// </summary>
        /// <typeparam name="T">Type of buffer.</typeparam>
        /// <returns>The new staging buffer.</returns>
        /// <remarks>This is not applicable to constant buffers.</remarks>
        /// <exception cref="System.NotSupportedException">This method is not supported for constant buffers.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new T GetStagingBuffer<T>()
            where T : GorgonBaseBuffer
        {
            throw new NotSupportedException("Constant buffers do not support staging usage.");
        }
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonConstantBuffer"/> class.
		/// </summary>
		/// <param name="graphics">Graphics interface that owns this buffer.</param>
		/// <param name="name">Name of the constant buffer.</param>
		/// <param name="settings">Settings for the buffer.</param>
		internal GorgonConstantBuffer(GorgonGraphics graphics, string name, IBufferSettings settings)
			: base(graphics, name, settings)
		{
		}
		#endregion
	}
}
