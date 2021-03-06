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
// Created: Saturday, October 12, 2013 9:10:02 PM
// 
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using GorgonLibrary.Graphics.Fonts;
using GorgonLibrary.IO;
using SlimMath;
using GorgonLibrary.Math;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// A brush that paints the font glyphs using a gradient that follows a specific path.
	/// </summary>
	public class GorgonGlyphPathGradientBrush
		: GorgonGlyphBrush
	{
		#region Properties.
		/// <summary>
		/// Property to return the type of brush.
		/// </summary>
		public override GlyphBrushType BrushType
		{
			get
			{
				return GlyphBrushType.PathGradient;
			}
		}

		/// <summary>
		/// Property to set or return the wrapping mode for the gradient fill.
		/// </summary>
		public WrapMode WrapMode
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return the points for the path to follow in the gradient fill.
		/// </summary>
		public IList<Vector2> Points
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the list of blending factors for the gradient falloff.
		/// </summary>
		public IList<float> BlendFactors
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to return the list of blending positions for the gradient falloff.
		/// </summary>
		public IList<float> BlendPositions
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the color of the center point in the gradient.
		/// </summary>
		public GorgonColor CenterColor
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the center point position in the gradient.
		/// </summary>
		public Vector2 CenterPoint
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the focus point for the gradient falloff.
		/// </summary>
		public Vector2 FocusScales
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return the interpolation values for the gradient.
		/// </summary>
		public IList<GorgonGlyphBrushInterpolator> Interpolation
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the surrounding colors for the gradient path.
		/// </summary>
		public IList<GorgonColor> SurroundColors
		{
			get;
			private set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to convert this brush to the equivalent GDI+ brush type.
		/// </summary>
		/// <returns>
		/// The GDI+ brush type for this object.
		/// </returns>
		internal override Brush ToGDIBrush()
		{
			var result = new PathGradientBrush(Points.Select(item => new PointF(item.X, item.Y)).ToArray(), WrapMode);

			var blend = new Blend(BlendFactors.Count.Max(BlendPositions.Count).Max(1));
			
			if (Interpolation.Count > 2)
			{
				var interpColors = new ColorBlend(Interpolation.Count);

				for (int i = 0; i < Interpolation.Count; i++)
				{
					interpColors.Colors[i] = Interpolation[i].Color;
					interpColors.Positions[i] = Interpolation[i].Weight;
				}

				result.InterpolationColors = interpColors;
			}

			for (int i = 0; i < blend.Factors.Length; i++)
			{
				if (i < BlendFactors.Count)
				{
					blend.Factors[i] = BlendFactors[i];
				}

				if (i < BlendPositions.Count)
				{
					blend.Positions[i] = BlendPositions[i];
				}
			}
			
			result.Blend = blend;
			result.CenterColor = CenterColor;
			result.CenterPoint = CenterPoint;
			result.FocusScales = FocusScales;
			
			result.SurroundColors = SurroundColors.Select(item => item.ToColor()).ToArray();

			return result;
		}

		/// <summary>
		/// Function to write the brush elements out to a chunked file.
		/// </summary>
		/// <param name="chunk">Chunk writer used to persist the data.</param>
		internal override void Write(GorgonChunkWriter chunk)
		{
			chunk.Begin("BRSHDATA");
			chunk.Write(BrushType);
			chunk.Write(WrapMode);
			chunk.Write(Points.Count);
			
			foreach (Vector2 point in Points)
			{
				chunk.Write(point);
			}

			chunk.Write(BlendFactors.Count);

			foreach (float factor in BlendFactors)
			{
				chunk.Write(factor);
			}

			chunk.Write(BlendPositions.Count);

			foreach (float position in BlendPositions)
			{
				chunk.Write(position);
			}

			chunk.Write(CenterColor);
			chunk.Write(CenterPoint);
			chunk.Write(FocusScales);

			chunk.Write(Interpolation.Count);

			foreach (GorgonGlyphBrushInterpolator interpolator in Interpolation)
			{
				interpolator.WriteChunk(chunk);
			}

			chunk.Write(SurroundColors.Count);

			foreach (GorgonColor color in SurroundColors)
			{
				chunk.Write(color);
			}

			chunk.End();
		}

		/// <summary>
		/// Function to read the brush elements in from a chunked file.
		/// </summary>
		/// <param name="chunk">Chunk reader used to read the data.</param>
		internal override void Read(GorgonChunkReader chunk)
		{
			Points.Clear();
			BlendPositions.Clear();
			BlendFactors.Clear();
			Interpolation.Clear();
			SurroundColors.Clear();

			WrapMode = chunk.Read<WrapMode>();
			int counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				Points.Add(chunk.Read<Vector2>());
			}

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				BlendFactors.Add(chunk.ReadFloat());
			}

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				BlendPositions.Add(chunk.ReadFloat());
			}

			CenterColor = chunk.Read<GorgonColor>();
			CenterPoint = chunk.Read<Vector2>();
			FocusScales = chunk.Read<Vector2>();

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				Interpolation.Add(new GorgonGlyphBrushInterpolator(chunk));
			}

			counter = chunk.ReadInt32();

			for (int i = 0; i < counter; i++)
			{
				SurroundColors.Add(chunk.Read<GorgonColor>());
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonGlyphPathGradientBrush"/> class.
		/// </summary>
		public GorgonGlyphPathGradientBrush()
		{
			Points = new List<Vector2>();
			BlendFactors = new List<float>();
			BlendPositions = new List<float>();
			Interpolation = new List<GorgonGlyphBrushInterpolator>();
			SurroundColors = new List<GorgonColor>();
		}
		#endregion
	}
}
