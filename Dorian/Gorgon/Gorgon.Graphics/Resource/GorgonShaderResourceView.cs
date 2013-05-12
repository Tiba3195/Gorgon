﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2013 Michael Winsor
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
// Created: Saturday, May 11, 2013 1:56:53 PM
// 
#endregion

using System;
using GorgonLibrary.Graphics.Properties;

namespace GorgonLibrary.Graphics
{
    /// <summary>
    /// A shader resource view to allow access to resource data inside of a shader.
    /// </summary>
    public struct GorgonShaderView
        : IEquatable<GorgonShaderView>
    {
        #region Variables.
        /// <summary>
        /// The format of the resource view.
        /// </summary>
        public readonly BufferFormat Format;
        /// <summary>
        /// Information about the assigned format.
        /// </summary>
        public readonly GorgonBufferFormatInfo.GorgonFormatData FormatInfo;
        #endregion

        #region Methods.
        /// <summary>
        /// Function to return whether two instances are equal.
        /// </summary>
        /// <param name="left">Left instance to compare.</param>
        /// <param name="right">Right instance to compare.</param>
        /// <returns>TRUE if equal, FALSE if not.</returns>
        public static bool Equals(ref GorgonShaderView left, ref GorgonShaderView right)
        {
            return left.Format == right.Format;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is GorgonShaderView)
            {
                return Equals((GorgonShaderView)obj);
            }
            return base.Equals(obj);
        }
        
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 281.GenerateHash(Format);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(Resources.GORGFX_SHADER_VIEW_TOSTR, Format);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">Left instance to compare.</param>
        /// <param name="right">Right instance to compare.</param>
        /// <returns>TRUE if equal, FALSE if not.</returns>
        public static bool operator ==(GorgonShaderView left, GorgonShaderView right)
        {
            return Equals(ref left, ref right);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left">Left instance to compare.</param>
        /// <param name="right">Right instance to compare.</param>
        /// <returns>TRUE if not equal, FALSE if equal.</returns>
        public static bool operator !=(GorgonShaderView left, GorgonShaderView right)
        {
            return !Equals(ref left, ref right);
        }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonShaderView"/> struct.
        /// </summary>
        /// <param name="format">The format of the resource view.</param>
        public GorgonShaderView(BufferFormat format)
        {
            Format = format;
            FormatInfo = format != BufferFormat.Unknown ? GorgonBufferFormatInfo.GetInfo(format) : null;
        }
        #endregion

        #region IEquatable<GorgonShaderView> Implementation.
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(GorgonShaderView other)
        {
            return Equals(ref this, ref other);
        }
        #endregion
    }
}
