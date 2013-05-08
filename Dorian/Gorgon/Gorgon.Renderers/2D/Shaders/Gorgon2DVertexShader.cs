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
// Created: Monday, April 02, 2012 12:08:22 PM
// 
#endregion

using GorgonLibrary.Graphics;

namespace GorgonLibrary.Renderers
{
	/// <summary>
	/// A vertex shader state for the 2D renderer.
	/// </summary>
	public class Gorgon2DVertexShaderState
		: GorgonVertexShaderState
	{
		#region Variables.
		private Gorgon2D _gorgon2D = null;				// The 2D interface that owns this object.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the default vertex shader.
		/// </summary>
		internal GorgonVertexShader DefaultVertexShader
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the current vertex shader.
		/// </summary>
		public override GorgonVertexShader Current
		{
			get
			{
				return Graphics.Shaders.VertexShader.Current == DefaultVertexShader ? null : Graphics.Shaders.VertexShader.Current;
			}
			set
			{
				if (((Graphics.Shaders.VertexShader.Current != value) && (value != null)) || ((value == null) && (Graphics.Shaders.VertexShader.Current != DefaultVertexShader)))
				{
					_gorgon2D.RenderObjects();

					if (value == null)
						Graphics.Shaders.VertexShader.Current = DefaultVertexShader;
					else
						Graphics.Shaders.VertexShader.Current = value;

					// Assign buffers.
					Graphics.Shaders.VertexShader.ConstantBuffers[0] = _gorgon2D.ProjectionViewBuffer;
				}
			}
		}

		/// <summary>
		/// Property to return the list of constant buffers for the vertex shader.
		/// </summary>
		public override GorgonShaderState<GorgonVertexShader>.ShaderConstantBuffers ConstantBuffers
		{
			get
			{
				return Graphics.Shaders.VertexShader.ConstantBuffers;
			}
		}

		/// <summary>
		/// Property to return the sampler states.
		/// </summary>
		/// <remarks>On a SM2_a_b device, and while using a Vertex Shader, setting a sampler will raise an exception.</remarks>
		/// <exception cref="System.InvalidOperationException">Thrown when the current video device is a SM2_a_b device.</exception>
		public override GorgonShaderState<GorgonVertexShader>.TextureSamplerState TextureSamplers
		{
			get
			{
				return Graphics.Shaders.VertexShader.TextureSamplers;
			}
		}

		/// <summary>
		/// Property to return the list of textures for the shaders.
		/// </summary>
		/// <remarks>On a SM2_a_b device, and while using a Vertex Shader, setting a texture will raise an exception.</remarks>
		/// <exception cref="System.InvalidOperationException">Thrown when the current video device is a SM2_a_b device.</exception>
		public override GorgonShaderState<GorgonVertexShader>.ShaderResourceViews Resources
		{
			get
			{
				return Graphics.Shaders.VertexShader.Resources;
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to perform clean up on the shader state.
		/// </summary>
		internal void CleanUp()
		{
			if (DefaultVertexShader != null)
				DefaultVertexShader.Dispose();

			DefaultVertexShader = null;
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="Gorgon2DVertexShaderState"/> class.
		/// </summary>
		/// <param name="gorgon2D">The gorgon 2D interface that created this object.</param>
		internal Gorgon2DVertexShaderState(Gorgon2D gorgon2D)
			: base(gorgon2D.Graphics)
		{
			_gorgon2D = gorgon2D;

			if (DefaultVertexShader == null)
			{
#if DEBUG
				DefaultVertexShader = Graphics.Shaders.CreateShader<GorgonVertexShader>("Default_Basic_Vertex_Shader", "GorgonVertexShader", "#GorgonInclude \"Gorgon2DShaders\"", true);
#else
				DefaultVertexShader = Graphics.Shaders.CreateShader<GorgonVertexShader>("Default_Basic_Vertex_Shader", "GorgonVertexShader", "#GorgonInclude \"Gorgon2DShaders\"", true);
#endif
			}
		}
		#endregion
	}
}
