﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2012 Michael Winsor
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
// Created: Tuesday, March 13, 2012 1:02:50 PM
// 
#endregion

using System.Drawing;
using D3D = SharpDX.Direct3D11;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// Settings to describe a texture structure.
	/// </summary>
	public interface ITextureSettings		
        : IImageSettings
	{
		#region Properties.
		/// <summary>
		/// Property to set or return the unordered access view format.
		/// </summary>
		/// <remarks>This changes how the texture is accessed in an unordered access view in a shader.
		/// <para>If this value is set to anything other than Unknown, then an unordered access view will be created for the texture.  If the value is 
		/// left as Unknown, then no unordered access view will be created for the texture.</para>
		/// <para>Textures using an unordered access view can only use a typed (e.g. int, uint, float) format that belongs to the same group as the format assigned to the texture, 
		/// or R32_UInt/Int/Float (but only if the texture format is 32 bit).  Any other format will raise an exception.  Note that if the format is not set to R32_UInt/Int/Float, 
		/// then write-only access will be given to the UAV.</para> 
        /// <para>To check to see if a format is supported for UAV, use the <see cref="GorgonLibrary.Graphics.GorgonVideoDevice.SupportsUnorderedAccessViewFormat">GorgonVideoDevice.SupportsUnorderedAccessViewFormat</see> 
        /// method to determine if the format is supported.</para>
		/// <para>The default value is Unknown.</para>
		/// </remarks>
		BufferFormat UnorderedAccessViewFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return whether this is a cube texture.
		/// </summary>
		/// <remarks>When setting this value to TRUE, ensure that the <see cref="GorgonLibrary.Graphics.IImageSettings.ArrayCount">ArrayCount</see> property is set to a multiple of 6.
		/// <para>This only applies to 2D textures.  All other textures will return FALSE.  The default value is FALSE.</para></remarks>
		bool IsTextureCube
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the multisampling count/quality for the texture.
		/// </summary>
		/// <remarks>This only applies to 2D textures.  The default value is a count of 1, and a quality of 0 (no multisampling).
		/// <para>Note that multisampled textures cannot have sub resources (e.g. mipmaps), so the <see cref="GorgonLibrary.Graphics.IImageSettings.MipCount">MipCount</see> should be set to 1.</para>
		/// </remarks>
		GorgonMultisampling Multisampling
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the usage for the texture.
		/// </summary>
		/// <remarks>The default value is Default.</remarks>
		BufferUsage Usage
		{
			get;
			set;
		}
		#endregion
	}
	
	/// <summary>
	/// Settings for a 1D texture.
	/// </summary>
	public sealed class GorgonTexture1DSettings
		: ITextureSettings
	{
		#region Constructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonTexture1DSettings"/> class.
		/// </summary>
		public GorgonTexture1DSettings()
		{
			Width = 0;
			Format = BufferFormat.Unknown;
			ArrayCount = 1;
			MipCount = 1;
		    Usage = BufferUsage.Default;
		}
		#endregion

		#region ITextureSettings Members
		#region Properties.
		/// <summary>
		/// Property to return the type of image data.
		/// </summary>
		public ImageType ImageType
		{
			get
			{
				return ImageType.Image1D;
			}
		}

		/// <summary>
		/// Property to set or return whether this is a cube texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 2D textures.  This value is always FALSE.</remarks>
		bool ITextureSettings.IsTextureCube
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the width of a texture.
		/// </summary>
		/// <value></value>
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the height of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This applies to 2D and 3D textures only.  This value always returns 1.</remarks>
		int IImageSettings.Height
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the depth of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This applies to 3D textures only.  This value always returns 1.</remarks>
		int IImageSettings.Depth
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the format of a texture.
		/// </summary>
		/// <value></value>
        /// <remarks>
        /// When loading a texture from a file, leave this as Unknown to get the file format from the source file.
        /// <para>This sets the format of the texture data. To reinterpret the format of the data inside of a shader, create a new <see cref="GorgonLibrary.Graphics.GorgonShaderView">GorgonShaderView</see> and assign it to the texture.</para></remarks>
        public BufferFormat Format
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the unordered access view format.
		/// </summary>
		/// <remarks>This changes how the texture is accessed in an unordered access view in a shader.
		/// <para>If this value is set to anything other than Unknown, then an unordered access view will be created for the texture.  If the value is 
		/// left as Unknown, then no unordered access view will be created for the texture.</para>
        /// <para>Textures using an unordered access view can only use a typed (e.g. int, uint, float) format that belongs to the same group as the format assigned to the texture, 
        /// or R32_UInt/Int/Float (but only if the texture format is 32 bit).  Any other format will raise an exception.  Note that if the format is not set to R32_UInt/Int/Float, 
        /// then write-only access will be given to the UAV.</para> 
        /// <para>To check to see if a format is supported for UAV, use the <see cref="GorgonLibrary.Graphics.GorgonVideoDevice.SupportsUnorderedAccessViewFormat">GorgonVideoDevice.SupportsUnorderedAccessViewFormat</see> 
        /// method to determine if the format is supported.</para>
        /// <para>The default value is Unknown.</para>
		/// </remarks>
		public BufferFormat UnorderedAccessViewFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the number of textures there are in a texture array.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value for this setting is 1.</remarks>
		public int ArrayCount
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the number of mip maps in a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value for this setting is 1.</remarks>
		public int MipCount
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the multisampling count/quality for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 2D textures.  This will always returns a count of 1, and a quality of 0 (no multisampling).</remarks>
		GorgonMultisampling ITextureSettings.Multisampling
		{
			get
			{
				return new GorgonMultisampling(1, 0);
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the usage for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value is Default.</remarks>
		public BufferUsage Usage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return whether the size of the texture is a power of 2 or not.
		/// </summary>
		public bool IsPowerOfTwo
		{
			get
			{
				return ((Width == 0) || (Width & (Width - 1)) == 0);
			}
		}
		#endregion

        #region Methods.
        /// <summary>
        /// Function to clone the current 1D texture settings.
        /// </summary>
        /// <returns>A clone of the image settings object.</returns>
        public IImageSettings Clone()
        {
            return new GorgonTexture1DSettings
            {
                Width = Width,
                Format = Format,
                ArrayCount = ArrayCount,
                MipCount = MipCount,
				UnorderedAccessViewFormat = UnorderedAccessViewFormat,
                Usage = Usage
            };
        }
        #endregion
        #endregion
    }

	/// <summary>
	/// Settings for a 2D texture.
	/// </summary>
	public sealed class GorgonTexture2DSettings
		: ITextureSettings
	{
		#region Properties.
		/// <summary>
		/// Property to set or return the size of the texture.
		/// </summary>
		public Size Size
		{
			get
			{
				return new Size(Width, Height);
			}
			set
			{
				Width = value.Width;
				Height = value.Height;
			}
		}
		#endregion

		#region Constructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonTexture2DSettings"/> class.
		/// </summary>
		public GorgonTexture2DSettings()
		{
			Width = 0;
			Height = 0;
			Format = BufferFormat.Unknown;
			MipCount = 1;
			ArrayCount = 1;
			Multisampling = new GorgonMultisampling(1, 0);
		    ShaderView = null;
			UnorderedAccessViewFormat = BufferFormat.Unknown;
			Usage = BufferUsage.Default;
		}
		#endregion

		#region ITextureSettings Members
		#region Properties.
		/// <summary>
		/// Property to return the type of image data.
		/// </summary>
		public ImageType ImageType
		{
			get
			{
				return IsTextureCube ? ImageType.ImageCube : ImageType.Image2D;
			}
		}

		/// <summary>
		/// Property to set or return whether this is a cube texture.
		/// </summary>
		/// <value></value>
		/// <remarks>When setting this value to TRUE, ensure that the <see cref="GorgonLibrary.Graphics.GorgonTexture2DSettings.ArrayCount">ArrayCount</see> property is set to a multiple of 6.
		/// <para>This only applies to 2D textures.  All other textures will return FALSE.  The default value is FALSE.</para></remarks>
		public bool IsTextureCube
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the width of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>When loading a file, leave as 0 to use the width from the file source.</remarks>
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the height of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// When loading a file, leave as 0 to use the height from the file source.
		/// </remarks>
		public int Height
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the depth of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This applies to 3D textures only and will always return 1.</remarks>
		int IImageSettings.Depth
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the format of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// When loading a texture from a file, leave this as Unknown to get the file format from the source file.
        /// <para>This sets the format of the texture data. To reinterpret the format of the data inside of a shader, create a new <see cref="GorgonLibrary.Graphics.GorgonShaderView">GorgonShaderView</see> and assign it to the texture.</para></remarks>
		public BufferFormat Format
		{
			get;
			set;
		}

        /// <summary>
        /// Property to set or return the shader view.
        /// </summary>
        /// <remarks>This changes how the texture is sampled/viewed in a shader.  When this value is set to NULL (Nothing in VB.Net) the view format is taken from the texture format.
        /// <para>The default value is NULL.</para>
        /// </remarks>
        public GorgonShaderView? ShaderView
        {
            get;
            set;
        }

		/// <summary>
		/// Property to set or return the unordered access view format.
		/// </summary>
		/// <remarks>This changes how the texture is accessed in an unordered access view in a shader.
		/// <para>If this value is set to anything other than Unknown, then an unordered access view will be created for the texture.  If the value is 
		/// left as Unknown, then no unordered access view will be created for the texture.</para>
        /// <para>Textures using an unordered access view can only use a typed (e.g. int, uint, float) format that belongs to the same group as the format assigned to the texture, 
        /// or R32_UInt/Int/Float (but only if the texture format is 32 bit).  Any other format will raise an exception.  Note that if the format is not set to R32_UInt/Int/Float, 
        /// then write-only access will be given to the UAV.</para> 
        /// <para>To check to see if a format is supported for UAV, use the <see cref="GorgonLibrary.Graphics.GorgonVideoDevice.SupportsUnorderedAccessViewFormat">GorgonVideoDevice.SupportsUnorderedAccessViewFormat</see> 
        /// method to determine if the format is supported.</para>
        /// <para>The default value is Unknown.</para>
		/// </remarks>
		public BufferFormat UnorderedAccessViewFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the number of textures there are in a texture array.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value is 1.</remarks>
		public int ArrayCount
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the number of mip maps in a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>To have the system generate mipmaps for you, set this value to 0.  The default value for this setting is 1.</remarks>
		public int MipCount
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the multisampling count/quality for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 2D textures.  The default value is a count of 1, and a quality of 0 (no multisampling).
		/// <para>Note that multisampled textures cannot have sub resources (e.g. mipmaps), so the <see cref="GorgonLibrary.Graphics.GorgonTexture2DSettings.MipCount">MipCount</see> should be set to 1.</para>
		/// </remarks>
		public GorgonMultisampling Multisampling
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the usage for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value is Default.</remarks>
		public BufferUsage Usage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return whether the size of the texture is a power of 2 or not.
		/// </summary>
		public bool IsPowerOfTwo
		{
			get
			{
				return ((Width == 0) || (Width & (Width - 1)) == 0) &&
						((Height == 0) || (Height & (Height - 1)) == 0);
			}
		}
		#endregion

        #region Methods.
        /// <summary>
        /// Function to clone the current 1D texture settings.
        /// </summary>
        /// <returns>A clone of the image settings object.</returns>
        public IImageSettings Clone()
        {
            return new GorgonTexture2DSettings
	            {
		            Width = Width,
		            Height = Height,
		            Format = Format,
		            ArrayCount = ArrayCount,
		            MipCount = MipCount,
		            IsTextureCube = IsTextureCube,
		            UnorderedAccessViewFormat = UnorderedAccessViewFormat,
		            Multisampling = Multisampling,                
		            Usage = Usage
	            };
        }
        #endregion
		#endregion
	}

	/// <summary>
	/// Settings for a 3D texture.
	/// </summary>
	public sealed class GorgonTexture3DSettings
		: ITextureSettings
	{
		#region Constructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonTexture3DSettings"/> class.
		/// </summary>
		public GorgonTexture3DSettings()
		{
			Width = 0;
			Height = 0;
			Depth = 0;
			Format = BufferFormat.Unknown;
			MipCount = 1;
			UnorderedAccessViewFormat = BufferFormat.Unknown;
			Usage = BufferUsage.Default;
		}
		#endregion

		#region ITextureSettings Members
		#region Properties.
		/// <summary>
		/// Property to return the type of image data.
		/// </summary>
		public ImageType ImageType
		{
			get
			{
				return ImageType.Image3D;
			}
		}

		/// <summary>
		/// Property to set or return whether this is a cube texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 2D textures.  This value will always return FALSE.</remarks>
		bool ITextureSettings.IsTextureCube
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the width of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>When loading a file, leave as 0 to use the width from the file source.</remarks>
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the height of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// When loading a file, leave as 0 to use the height from the file source.
		/// <para>This applies to 2D and 3D textures only.</para></remarks>
		public int Height
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the depth of a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>When loading a file, leave as 0 to use the width from the depth source.</remarks>
		public int Depth
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the format of a texture.
		/// </summary>
		/// <value></value>
        /// <remarks>
        /// When loading a texture from a file, leave this as Unknown to get the file format from the source file.
        /// <para>This sets the format of the texture data. To reinterpret the format of the data inside of a shader, create a new <see cref="GorgonLibrary.Graphics.GorgonShaderView">GorgonShaderView</see> and assign it to the texture.</para></remarks>
        public BufferFormat Format
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the unordered access view format.
		/// </summary>
		/// <remarks>This changes how the texture is accessed in an unordered access view in a shader.
		/// <para>If this value is set to anything other than Unknown, then an unordered access view will be created for the texture.  If the value is 
		/// left as Unknown, then no unordered access view will be created for the texture.</para>
        /// <para>Textures using an unordered access view can only use a typed (e.g. int, uint, float) format that belongs to the same group as the format assigned to the texture, 
        /// or R32_UInt/Int/Float (but only if the texture format is 32 bit).  Any other format will raise an exception.  Note that if the format is not set to R32_UInt/Int/Float, 
        /// then write-only access will be given to the UAV.</para> 
        /// <para>To check to see if a format is supported for UAV, use the <see cref="GorgonLibrary.Graphics.GorgonVideoDevice.SupportsUnorderedAccessViewFormat">GorgonVideoDevice.SupportsUnorderedAccessViewFormat</see> 
        /// method to determine if the format is supported.</para>
        /// <para>The default value is Unknown.</para>
		/// </remarks>
		public BufferFormat UnorderedAccessViewFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the number of textures there are in a texture array.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 1D and 2D textures.  This value will always return 1.</remarks>
		int IImageSettings.ArrayCount
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the number of mip maps in a texture.
		/// </summary>
		/// <value></value>
		/// <remarks>To have the system generate mipmaps for you, set this value to 0.  The default value for this setting is 1.</remarks>
		public int MipCount
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the multisampling count/quality for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>This only applies to 2D textures.  This value will always return a count of 1, and a quality of 0 (no multisampling).</remarks>
		GorgonMultisampling ITextureSettings.Multisampling
		{
			get
			{
				return new GorgonMultisampling(1, 0);
			}
			set
			{
			}
		}

		/// <summary>
		/// Property to set or return the usage for the texture.
		/// </summary>
		/// <value></value>
		/// <remarks>The default value is Default.</remarks>
		public BufferUsage Usage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return whether the size of the texture is a power of 2 or not.
		/// </summary>
		public bool IsPowerOfTwo
		{
			get
			{
				return ((Width == 0) || (Width & (Width - 1)) == 0) &&
						((Height == 0) || (Height & (Height - 1)) == 0) &&
						((Depth == 0) || (Depth & (Depth - 1)) == 0);
			}
		}
		#endregion

        #region Methods.
        /// <summary>
        /// Function to clone the current 1D texture settings.
        /// </summary>
        /// <returns>A clone of the image settings object.</returns>
        public IImageSettings Clone()
        {
            return new GorgonTexture3DSettings
	            {
		            Width = Width,
		            Height = Height,
		            Depth = Depth,
		            Format = Format,
		            MipCount = MipCount,
		            UnorderedAccessViewFormat = UnorderedAccessViewFormat,                
		            Usage = Usage
	            };
        }
        #endregion
		#endregion
	}
}
