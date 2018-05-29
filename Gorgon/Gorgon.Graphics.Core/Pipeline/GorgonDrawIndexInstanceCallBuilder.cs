﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2018 Michael Winsor
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
// Created: May 23, 2018 1:44:30 PM
// 
#endregion

using System;
using Gorgon.Graphics.Core.Properties;

namespace Gorgon.Graphics.Core
{
    /// <summary>
    /// A builder class used to create or update draw calls using fluent calls.
    /// </summary>
    public class GorgonDrawIndexInstanceCallBuilder
        : GorgonDrawCallBuilderCommon<GorgonDrawIndexInstanceCallBuilder, GorgonDrawIndexInstanceCall>
    {
        #region Methods.
        /// <summary>
        /// Function to create a new draw call.
        /// </summary>
        /// <returns>A new draw call.</returns>
        protected override GorgonDrawIndexInstanceCall OnCreate()
        {
            return new GorgonDrawIndexInstanceCall();
        }

        /// <summary>
        /// Function to reset the properties of the draw call to the draw call passed in.
        /// </summary>
        /// <param name="drawCall">The draw call to copy from.</param>
        /// <returns>The fluent builder interface.</returns>
        protected override GorgonDrawIndexInstanceCallBuilder OnResetTo(GorgonDrawIndexInstanceCall drawCall)
        {
            DrawCall.IndexStart = drawCall.IndexStart;
            DrawCall.BaseVertexIndex = drawCall.BaseVertexIndex;
            DrawCall.IndexCountPerInstance = drawCall.IndexCountPerInstance;
            DrawCall.StartInstanceIndex = drawCall.StartInstanceIndex;
            DrawCall.InstanceCount = drawCall.InstanceCount;
            DrawCall.IndexBuffer = drawCall.IndexBuffer;
            return this;
        }

        /// <summary>
        /// Function to clear the draw call.
        /// </summary>
        /// <returns>The fluent builder interface.</returns>
        protected override GorgonDrawIndexInstanceCallBuilder OnClear()
        {
            DrawCall.IndexStart = 0;
            DrawCall.BaseVertexIndex = 0;
            DrawCall.IndexCountPerInstance = 0;
            DrawCall.StartInstanceIndex = 0;
            DrawCall.InstanceCount = 0;
            DrawCall.IndexBuffer = null;
            return this;
        }

        /// <summary>
        /// Function to update the properties of the draw call from the working copy to the final copy.
        /// </summary>
        /// <param name="finalCopy">The object representing the finalized copy.</param>
        /// <returns></returns>
        protected override void OnUpdate(GorgonDrawIndexInstanceCall finalCopy)
        {
            finalCopy.IndexStart = DrawCall.IndexStart;
            finalCopy.BaseVertexIndex = DrawCall.BaseVertexIndex;
            finalCopy.IndexCountPerInstance = DrawCall.IndexCountPerInstance;
            finalCopy.StartInstanceIndex = DrawCall.StartInstanceIndex;
            finalCopy.InstanceCount = DrawCall.InstanceCount;
            finalCopy.IndexBuffer = DrawCall.IndexBuffer;
        }

        /// <summary>
        /// Function to assign an index buffer to the draw call.
        /// </summary>
        /// <param name="buffer">The buffer to assign.</param>
        /// <param name="indexStart">The first index in the index buffer to render.</param>
        /// <param name="indexCountPerInstance">The number of indices to render, per instance.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="indexStart"/> parameter is less than 0.
        /// <para>-or-</para>
        /// <para>Thrown when the <paramref name="indexCountPerInstance"/> parameter is less than 1.</para>
        /// </exception>
        public GorgonDrawIndexInstanceCallBuilder IndexBuffer(GorgonIndexBuffer buffer, int indexStart, int indexCountPerInstance)
        {
            if (indexStart < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), Resources.GORGFX_ERR_INDEX_TOO_SMALL);
            }

            if (indexCountPerInstance < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCountPerInstance), Resources.GORGFX_ERR_INDEX_COUNT_TOO_SMALL);
            }

            DrawCall.IndexStart = indexStart;
            DrawCall.IndexCountPerInstance = indexCountPerInstance;
            DrawCall.IndexBuffer = buffer;
            return this;
        }

        /// <summary>
        /// Function to set the first index, and the number of indices to render in the draw call.
        /// </summary>
        /// <param name="indexStart">The first index in the index buffer to render.</param>
        /// <param name="indexCountPerInstance">The number of indices to render, per instance.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="indexStart"/> parameter is less than 0.
        /// <para>-or-</para>
        /// <para>Thrown when the <paramref name="indexCountPerInstance"/> parameter is less than 1.</para>
        /// </exception>
        public GorgonDrawIndexInstanceCallBuilder IndexRange(int indexStart, int indexCountPerInstance)
        {
            if (indexStart < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexStart), Resources.GORGFX_ERR_INDEX_TOO_SMALL);
            }

            if (indexCountPerInstance < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCountPerInstance), Resources.GORGFX_ERR_INDEX_COUNT_TOO_SMALL);
            }

            DrawCall.IndexStart = indexStart;
            DrawCall.IndexCountPerInstance = indexCountPerInstance;
            return this;
        }

        /// <summary>
        /// Function to set the base vertex index.
        /// </summary>
        /// <param name="baseVertexIndex">The base vertex index to set.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="baseVertexIndex"/> parameter is less than 0.</exception>
        public GorgonDrawIndexInstanceCallBuilder BaseVertexIndex(int baseVertexIndex)
        {
            if (baseVertexIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(baseVertexIndex), Resources.GORGFX_ERR_VERTEX_INDEX_TOO_SMALL);
            }

            DrawCall.BaseVertexIndex = baseVertexIndex;

            return this;
        }

        /// <summary>
        /// Function to set the starting instance index, and the number of instances to draw.
        /// </summary>
        /// <param name="startInstanceIndex">The starting index for the the first instance.</param>
        /// <param name="instanceCount">The number of instances to draw.</param>
        /// <returns>The fluent builder interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="startInstanceIndex"/> parameter is less than 0.
        /// <para>-or-</para>
        /// <para>Thrown when the <paramref name="instanceCount"/> parameter is less than 1.</para>
        /// </exception>
        public GorgonDrawIndexInstanceCallBuilder InstanceRange(int startInstanceIndex, int instanceCount)
        {
            if (startInstanceIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startInstanceIndex), Resources.GORGFX_ERR_INSTANCE_START_INVALID);
            }

            if (instanceCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceCount), Resources.GORGFX_ERR_INSTANCE_COUNT_INVALID);
            }

            DrawCall.StartInstanceIndex = startInstanceIndex;
            DrawCall.InstanceCount = instanceCount;
            return this;
        }
        #endregion

        #region Constructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonDrawIndexInstanceCallBuilder"/> class.
        /// </summary>
        public GorgonDrawIndexInstanceCallBuilder()
            : base(new GorgonDrawIndexInstanceCall())
        {

        }
        #endregion
    }
}
