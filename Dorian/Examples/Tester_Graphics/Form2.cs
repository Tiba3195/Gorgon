﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SlimMath;
using GorgonLibrary;
using GorgonLibrary.UI;
using GorgonLibrary.IO;
using GorgonLibrary.Graphics;
using SharpDX.WIC;

namespace Tester_Graphics
{
	public partial class Form2 : Form
	{
		private GorgonGraphics _graphics = null;
		private GorgonSwapChain _swap = null;

		private bool Idle()
		{
			_swap.Clear(Color.White);

			_swap.Flip();
			return true;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			try
			{
				_graphics = new GorgonGraphics();
				_swap = _graphics.Output.CreateSwapChain("Screen", new GorgonSwapChainSettings()
				{
					IsWindowed = true,
					Width = 800,
					Height = 600,
					Format = BufferFormat.R8G8B8A8_UIntNormal
				});

				using (Image image = Image.FromFile(@"d:\images\OSUsers_512x512.gif"))
				{
					Image[] images = new Image[GorgonImageData.GetDepthSliceCount(4, 4)];
					for (int i = 0; i < images.Length; i++)
					{
						images[i] = image.Clone() as Image;
					}
					using (var texture = _graphics.Textures.Create3DTextureFromGDIImage("Test", images, new GorgonGDIOptions()
						{
							MipCount = 4,
							Depth = 2
						}))
					{
						texture.Save(@"D:\unpak\textureUpload.dds", ImageFileFormat.DDS);
					}
				}

				//Image[] images = new Image[

				/*using (GorgonTexture2D texture = _graphics.Textures.FromFile<GorgonTexture2D>("Test", @"D:\images\OSUsers.jpg"))
				{
					using (WICLoad wic = new WICLoad())
					{
						using (FileStream stream = File.Open(@"D:\unpak\wictest.png", FileMode.Create, FileAccess.Write, FileShare.None))
						{
							wic.SavePNGToStream(texture, stream);
						}
					}
				}*/

				Gorgon.ApplicationIdleLoopMethod = Idle;
			}
			catch (Exception ex)
			{
				GorgonDialogs.ErrorBox(this, ex);
			}
		}

		public Form2()
		{
			InitializeComponent();
		}
	}
}