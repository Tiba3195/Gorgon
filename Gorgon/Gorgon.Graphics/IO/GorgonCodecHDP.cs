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
// Created: Tuesday, February 19, 2013 9:13:28 PM
// 
#endregion

using System.Collections.Generic;
using GorgonLibrary.Graphics;
using GorgonLibrary.Graphics.Properties;
using SharpDX.WIC;

namespace GorgonLibrary.IO
{
    /// <summary>
    /// A codec to handle read/writing of high defintion photo files.
    /// </summary>
    /// <remarks>A codec allows for reading and/or writing of data in an encoded format.  Users may inherit from this object to define their own 
    /// image formats, or use one of the predefined image codecs available in Gorgon.
    /// <para>This format requires that the Windows Imaging Components are installed on the system.</para>
    /// </remarks>
    public sealed class GorgonCodecHDP
        : GorgonCodecWIC
    {
        #region Variables.
		// Supported formats.
		private readonly BufferFormat[] _supportedFormats = {
																BufferFormat.R8G8B8A8_UIntNormal,
																BufferFormat.B8G8R8A8_UIntNormal,
																BufferFormat.B8G8R8X8_UIntNormal,
																BufferFormat.R16G16B16A16_UIntNormal,
																BufferFormat.R32G32B32A32_Float,
																BufferFormat.R32G32B32A32_UInt
	                                                        };
		private float _imageQuality = 1.0f;         // Image quality for lossy compressed images.
        #endregion

        #region Properties.

		/// <summary>
		/// Property to return the data formats for the image.
		/// </summary>
	    public override IEnumerable<BufferFormat> SupportedFormats
	    {
		    get
		    {
			    return _supportedFormats;
		    }
	    }

	    /// <summary>
        /// Property to set or return whether lossless compression should be used to encode the image.
        /// </summary>
        /// <remarks>
        /// Set the property to TRUE to avoid lossy compression which may introduce artifacts to the image.  Setting this value to TRUE 
        /// will cause the <see cref="P:GorgonLibrary.IO.GorgonCodecHDP.ImageQuality">ImageQuality</see> property to be ignored.
        /// <para>This property is only used when encoding the image.</para>
        /// <para>The default value is FALSE.</para>
        /// </remarks>
        public bool UseLosslessCompression
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return the quality of an image compressed with lossy compression.
        /// </summary>
        /// <remarks>
        /// Use this property to control the fidelity of an image compressed with lossy compression.  0.0f will give the 
        /// lowest quality and 1.0f will give the highest.  If the <see cref="P:GorgonLibrary.IO.GorgonCodecHDP.UseLosslessCompression">UseLosslessCompression</see> 
        /// property is set to TRUE, then this property will be ignored.
        /// <para>This property is only used when encoding the image.</para>
        /// <para>The default value is 1.0f.</para>
        /// </remarks>
        public float ImageQuality
        {
            get
            {
                return _imageQuality;
            }
            set
            {
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                if (value > 1.0f)
                {
                    value = 1.0f;
                }

                _imageQuality = value;
            }
        }
        #endregion

        #region Methods.
        /// <summary>
        /// Function to set custom encoding options.
        /// </summary>
        /// <param name="frame">Frame encoder to use.</param>
        internal override void SetFrameOptions(BitmapFrameEncode frame)
        {
            frame.Options.LossLess = UseLosslessCompression;
            frame.Options.ImageQuality = _imageQuality;
        }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonCodecHDP"/> class.
        /// </summary>
        public GorgonCodecHDP()
            : base("HDP", Resources.GORGFX_IMAGE_HDP_CODEC_DESC, new[] { "hdp", "wmp" }, ContainerFormatGuids.Wmp)
        {
        }
        #endregion
    }
}
