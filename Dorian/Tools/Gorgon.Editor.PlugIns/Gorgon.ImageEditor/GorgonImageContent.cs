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
// Created: Monday, October 21, 2013 8:08:24 PM
// 
#endregion

using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using GorgonLibrary.Editor.ImageEditorPlugIn.Properties;
using GorgonLibrary.Graphics;
using GorgonLibrary.IO;
using GorgonLibrary.Renderers;

namespace GorgonLibrary.Editor.ImageEditorPlugIn
{
    /// <summary>
    /// Image content.
    /// </summary>
    class GorgonImageContent
        : ContentObject, IImageEditorContent
    {
        #region Variables.
	    private bool _disposed;													// Flag to indicate that the object was disposed.
        private GorgonImageContentPanel _contentPanel;                          // Panel used to display the content.
        private GorgonSwapChain _swap;											// The swap chain to display our texture.
        private IImageSettings _imageSettings;                                  // The current settings for the image.
        #endregion

        #region Properties.
		/// <summary>
		/// Property to return the renderer interface.
		/// </summary>
		[Browsable(false)]
	    public Gorgon2D Renderer
	    {
		    get;
		    private set;
	    }

        /// <summary>
        /// Property to return whether this content has properties that can be manipulated in the properties tab.
        /// </summary>
        [Browsable(false)]
        public override bool HasProperties
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Property to return the type of content.
        /// </summary>
        [Browsable(false)]
        public override string ContentType
        {
            get
            {
                return Resources.GORIMG_CONTENT_TYPE;
            }
        }

        /// <summary>
        /// Property to return whether the content object supports a renderer interface.
        /// </summary>
        [Browsable(false)]
        public override bool HasRenderer
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Property to set or return the format for the image.
        /// </summary>
        [TypeConverter(typeof(BufferFormatTypeConverter))]
        public BufferFormat ImageFormat
        {
            get
            {
                return _imageSettings.Format;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the width of the image.
        /// </summary>
        public int Width
        {
            get
            {
                return _imageSettings.Width;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the height of the image.
        /// </summary>
        public int Height
        {
            get
            {
                return _imageSettings.Height;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the depth for a volume texture.
        /// </summary>
        public int Depth
        {
            get
            {
                return _imageSettings.Depth;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the number of mip maps in the image.
        /// </summary>
        public int MipCount
        {
            get
            {
                return _imageSettings.MipCount;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the number of array indices in the image.
        /// </summary>
        public int ArrayCount
        {
            get
            {
                return _imageSettings.ArrayCount;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to set or return the type of image.
        /// </summary>
        public ImageType ImageType
        {
            get
            {
                return _imageSettings.ImageType;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Property to return the image codec used for this image.
        /// </summary>
        [Browsable(false)]
        public GorgonImageCodec Codec
        {
            get;
            private set;
        }
        #endregion

        #region Methods.
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	    protected override void Dispose(bool disposing)
	    {
		    if (!_disposed)
		    {
			    if (Image != null)
			    {
				    Image.Dispose();
			    }

			    if (Renderer != null)
			    {
				    Renderer.Dispose();
			    }

			    if (_swap != null)
			    {
				    _swap.Dispose();
			    }

			    _swap = null;
				Renderer = null;
			    Image = null;
		    }

		    _disposed = true;

		    base.Dispose(disposing);
	    }

	    /// <summary>
        /// Function to persist the content data to a stream.
        /// </summary>
        /// <param name="stream">Stream that will receive the data.</param>
        protected override void OnPersist(Stream stream)
        {
        }

        /// <summary>
        /// Function to read the content data from a stream.
        /// </summary>
        /// <param name="stream">Stream containing the content data.</param>
        protected override void OnRead(Stream stream)
        {
            Image = GorgonImageData.FromStream(stream, (int)stream.Length, Codec);
            _imageSettings = Image.Settings;

            if (Graphics.VideoDevice.SupportedFeatureLevel == DeviceFeatureLevel.SM2_a_b)
            {
                // If we're running a feature level 9 device, then we have to disable these items
                // because we cannot save an image.
                DisableProperty("Width", true);
                DisableProperty("Height", true);
                DisableProperty("Depth", true);
                DisableProperty("ImageType", true);
                DisableProperty("ImageFormat", true);
                DisableProperty("ArrayCount", true);
                DisableProperty("MipCount", true);
                return;
            }

            if (!Codec.SupportsArray)
            {
                DisableProperty("ArrayCount", true);
            }

            if (!Codec.SupportsDepth)
            {
                DisableProperty("Depth", true);
            }

            if (!Codec.SupportsMipMaps)
            {
                DisableProperty("MipCount", true);
            }

            if (Codec.SupportedFormats.Count() < 2)
            {
                DisableProperty("ImageFormat", true);
            }

            if (_imageSettings.ImageType == ImageType.Image1D)
            {
                DisableProperty("Height", true);
                DisableProperty("Depth", true);
            }

            if ((_imageSettings.ImageType == ImageType.Image2D)
                || (_imageSettings.ImageType == ImageType.ImageCube))
            {
                DisableProperty("Depth", true);
            }
        }

        /// <summary>
        /// Function called when the content is being initialized.
        /// </summary>
        /// <returns>
        /// A control to place in the primary interface window.
        /// </returns>
        protected override ContentPanel OnInitialize()
        {
	        _contentPanel = new GorgonImageContentPanel
	                        {
		                        Content = this
	                        };

	        _swap = Graphics.Output.CreateSwapChain("TextureDisplay",
	                                                new GorgonSwapChainSettings
	                                                {
														Window = _contentPanel.panelTextureDisplay,
														Format = BufferFormat.R8G8B8A8_UIntNormal
	                                                });

			Renderer = Graphics.Output.Create2DRenderer(_swap);

	        _contentPanel.CreateResources();

            return _contentPanel;
        }

		/// <summary>
		/// Function to draw the interface for the content editor.
		/// </summary>
	    public override void Draw()
		{
			Renderer.Target = _swap;
			_contentPanel.Draw();

			Renderer.Render(1);
		}

	    /// <summary>
        /// Function to retrieve a thumbnail image for the content plug-in.
        /// </summary>
        /// <returns>
        /// The image for the thumbnail of the content.
        /// </returns>
        /// <remarks>
        /// The size of the thumbnail should be set to 128x128.
        /// </remarks>
        public override Image GetThumbNailImage()
        {
            Image resultImage;
            float aspect = (float)Image[0].Width / Image[0].Height;
            var newSize = new Size(128, 128);

			// If this device can't support loading the image, then show
			// a thumbnail that will indicate that we can't load it.
	        if ((Image[0].Width >= Graphics.Textures.MaxWidth)
	            || (Image[0].Height >= Graphics.Textures.MaxHeight)
	            || (Image[0].Depth >= Graphics.Textures.MaxDepth))
	        {
		        return (Image)Resources.invalid_image_128x128.Clone();
	        }

			// Ensure that we support the image format as well.
	        switch (Image.Settings.ImageType)
	        {
			    case ImageType.Image1D:
			        if (!Graphics.VideoDevice.Supports1DTextureFormat(Image.Settings.Format))
			        {
						return (Image)Resources.invalid_image_128x128.Clone();
			        }
			        break;
				case ImageType.Image2D:
				case ImageType.ImageCube:
			        if (!Graphics.VideoDevice.Supports2DTextureFormat(Image.Settings.Format))
			        {
						return (Image)Resources.invalid_image_128x128.Clone();
			        }
			        break;
				case ImageType.Image3D:
			        if (!Graphics.VideoDevice.Supports3DTextureFormat(Image.Settings.Format))
			        {
						return (Image)Resources.invalid_image_128x128.Clone();
			        }
			        break;
				default:
					return (Image)Resources.invalid_image_128x128.Clone();
	        }

            // Resize our image.
            if ((Image[0].Width != 128)
                || (Image[0].Height != 128))
            {
                if (aspect > 1.0f)
                {
                    newSize.Height = (int)(128 / aspect);
                }
                else
                {
                    newSize.Width = (int)(128 * aspect);
                }

                Image.Resize(newSize.Width, newSize.Height, false, ImageFilter.Fant);
            }

            try
            {
                resultImage = Image[0].ToGDIImage();
            }
            catch (GorgonException gex)
            {
                // If we get format not supported, then we should just return a 
                // generic picture.  There's no reason to make a big deal about it.
                if (gex.ResultCode != GorgonResult.FormatNotSupported)
                {
                    throw;
                }

                // Return a generic picture.
                resultImage = (Image)Resources.image_128x128.Clone();
            }

            return resultImage;
        }

		/// <summary>
		/// Function to load the image from a file system file entry.
		/// </summary>
		/// <param name="fileName">The file name of the file to load.</param>
		/// <param name="stream">The stream containing the file.</param>
	    public void Load(string fileName, Stream stream)
	    {
			if (!Codec.IsReadable(stream))
			{
				throw new GorgonException(GorgonResult.CannotRead, string.Format(Resources.GORIMG_CODEC_CANNOT_READ, fileName, Codec.CodecDescription));
			}

			OnRead(stream);
	    }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonImageContent"/> class.
        /// </summary>
        /// <param name="name">Name of the content.</param>
        /// <param name="codec">The codec used for the image.</param>
        public GorgonImageContent(string name, GorgonImageCodec codec)
            : base(name)
        {
            HasThumbnail = true;
	        Codec = codec;

            // Make a default, empty, 2D settings object so we have something to work with.
            _imageSettings = new GorgonTexture2DSettings();
        }
        #endregion

        #region IImageEditorContent Members

        /// <summary>
        /// Property to return the image held in the content object.
        /// </summary>
        [Browsable(false)]
        public GorgonImageData Image
        {
            get;
            private set;
        }
        #endregion
    }
}
