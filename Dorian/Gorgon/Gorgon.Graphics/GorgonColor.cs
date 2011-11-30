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
// Created: Friday, September 02, 2011 6:32:30 AM
// 
#endregion

using System;
using System.Drawing;
using GorgonLibrary.Math;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// 4 component (Red, Green, Blue, and Alpha) color value.
	/// </summary>
	public struct GorgonColor
		: IEquatable<GorgonColor>
	{
		#region Variables.
		/// <summary>
		/// Alpha channel.
		/// </summary>
		public float Alpha;
		/// <summary>
		/// Red color channel.
		/// </summary>
		public float Red;
		/// <summary>
		/// Green color channel.
		/// </summary>
		public float Green;
		/// <summary>
		/// Blue color channel.
		/// </summary>
		public float Blue;
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return a SharpDX color4 type.
		/// </summary>
		internal SharpDX.Color4 SharpDXColor4
		{
			get
			{
				return new SharpDX.Color4(Red, Green, Blue, Alpha);
			}
		}

		/// <summary>
		/// Property to return a SharpDX color3 type.
		/// </summary>
		internal SharpDX.Color3 SharpDXColor3
		{
			get
			{
				return new SharpDX.Color3(Red, Green, Blue);
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is GorgonColor)
				return Equals((GorgonColor)obj);

			return base.Equals(obj);
		}

		/// <summary>
		/// Function to compare two colors for equality.
		/// </summary>
		/// <param name="left">Left color to compare.</param>
		/// <param name="right">Right color to compare.</param>
		/// <returns>TRUE if equal, FALSE if not.</returns>
		public static bool Equals(ref GorgonColor left, ref GorgonColor right)
		{
			return (GorgonMathUtility.EqualFloat(left.Red, right.Red)) && (GorgonMathUtility.EqualFloat(left.Green, right.Green)) && (GorgonMathUtility.EqualFloat(left.Blue, right.Blue)) && (GorgonMathUtility.EqualFloat(left.Alpha, right.Alpha));
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return Alpha.GetHashCode() ^ Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Color Value: Alpha={0}, Red={1}, Green={2}, Blue={3}", Alpha, Red, Green, Blue);
		}

		/// <summary>
		/// Function to perform linear interpolation between two colors.
		/// </summary>
		/// <param name="start">Starting color.</param>
		/// <param name="end">Ending color.</param>
		/// <param name="weight">Value between 0 and 1.0f to indicate weight.</param>
		/// <param name="outColor">The resulting color.</param>
		public static void Lerp(ref GorgonColor start, ref GorgonColor end, float weight, out GorgonColor outColor)
		{
			outColor.Alpha = start.Alpha + ((end.Alpha - start.Alpha) * weight);
			outColor.Red = start.Red + ((end.Red - start.Red) * weight);
			outColor.Green = start.Red + ((end.Red - start.Red) * weight);
			outColor.Blue = start.Red + ((end.Red - start.Red) * weight);
		}

		/// <summary>
		/// Function to add two colors together.
		/// </summary>
		/// <param name="left">Left color to add.</param>
		/// <param name="right">Right color to add.</param>
		/// <param name="outColor">Total of two colors.</param>
		public static void Add(ref GorgonColor left, ref GorgonColor right, out GorgonColor outColor)
		{
			outColor.Alpha = left.Alpha + right.Alpha;
			outColor.Red = left.Red + right.Red;
			outColor.Green = left.Green + right.Green;
			outColor.Blue = left.Blue + right.Blue;
		}

		/// <summary>
		/// Function to subtract two colors.
		/// </summary>
		/// <param name="left">Left color to subtract.</param>
		/// <param name="right">Right color to subtract.</param>
		/// <param name="outColor">Difference between the two colors.</param>
		public static void Subtract(ref GorgonColor left, ref GorgonColor right, out GorgonColor outColor)
		{
			outColor.Alpha = left.Alpha - right.Alpha;
			outColor.Red = left.Red - right.Red;
			outColor.Green = left.Green - right.Green;
			outColor.Blue = left.Blue - right.Blue;
		}

		/// <summary>
		/// Function to multiply two colors.
		/// </summary>
		/// <param name="left">Left color to multiply.</param>
		/// <param name="right">Right color to multiply.</param>
		/// <param name="outColor">Product of the two colors.</param>
		public static void Multiply(ref GorgonColor left, ref GorgonColor right, out GorgonColor outColor)
		{
			outColor.Alpha = left.Alpha * right.Alpha;
			outColor.Red = left.Red * right.Red;
			outColor.Green = left.Green * right.Green;
			outColor.Blue = left.Blue * right.Blue;
		}

		/// <summary>
		/// Function to multiply a color by a value.
		/// </summary>
		/// <param name="color">Color to multiply.</param>
		/// <param name="value">Value to multiply.</param>
		/// <param name="outColor">Product of the color and the value.</param>
		public static void Multiply(ref GorgonColor color, float value, out GorgonColor outColor)
		{
			outColor.Alpha = color.Alpha * value;
			outColor.Red = color.Red * value;
			outColor.Green = color.Green * value;
			outColor.Blue = color.Blue * value;
		}

		/// <summary>
		/// Function to return a packed color value.
		/// </summary>
		/// <returns>The packed color value.</returns>
		public int ToARGB()
		{
			uint result = (((uint)(Alpha * 255.0f)) & 0xff) << 24 | (((uint)(Red * 255.0f)) & 0xff) << 16 | (((uint)(Green * 255.0f)) & 0xff) << 8 | ((uint)(Blue * 255.0f)) & 0xff;
			return (int)result;
		}

		/// <summary>
		/// Function to convert this color into a .NET drawing color.
		/// </summary>
		/// <returns>A .NET drawing color.</returns>
		public Color ToColor()
		{
			return Color.FromArgb(ToARGB());
		}

		/// <summary>
		/// Function to convert this color into a 3D vector.
		/// </summary>
		/// <returns>The 3D vector.</returns>
		/// <remarks>This will map the R, G and B components to X, Y and Z respectively.</remarks>
		public Vector3D ToVector3D()
		{
			return new Vector3D(Red, Green, Blue);
		}

		/// <summary>
		/// Function to convert this color into a 4D vector.
		/// </summary>
		/// <returns>The 4D vector.</returns>
		/// <remarks>This will map the R, G, B and A components to X, Y, Z and W respectively.</remarks>
		public Vector4D ToVector4D()
		{
			return new Vector4D(Red, Green, Blue, Alpha);
		}

		/// <summary>
		/// Implements the operator +.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>The result of the operator.</returns>
		public static GorgonColor operator +(GorgonColor left, GorgonColor right)
		{
			GorgonColor result = new GorgonColor();

			GorgonColor.Add(ref left, ref right, out result);

			return result;
		}

		/// <summary>
		/// Implements the operator -.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>The result of the operator.</returns>
		public static GorgonColor operator -(GorgonColor left, GorgonColor right)
		{
			GorgonColor result = new GorgonColor();

			GorgonColor.Subtract(ref left, ref right, out result);

			return result;
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>The result of the operator.</returns>
		public static GorgonColor operator *(GorgonColor left, GorgonColor right)
		{
			GorgonColor result = new GorgonColor();

			GorgonColor.Multiply(ref left, ref right, out result);

			return result;
		}

		/// <summary>
		/// Implements the operator *.
		/// </summary>
		/// <param name="color">The color to multiply.</param>
		/// <param name="value">The value to multiply by.</param>
		/// <returns>The result of the operator.</returns>
		public static GorgonColor operator *(GorgonColor color, float value)
		{
			GorgonColor result = new GorgonColor();

			GorgonColor.Multiply(ref color, value, out result);

			return result;
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left value.</param>
		/// <param name="right">The right value.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(GorgonColor left, GorgonColor right)
		{
			return GorgonColor.Equals(ref left, ref right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(GorgonColor left, GorgonColor right)
		{
			return !GorgonColor.Equals(ref left, ref right);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="GorgonLibrary.Graphics.GorgonColor"/> to <see cref="System.Drawing.Color"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator Color(GorgonColor color)
		{
			return color.ToColor();
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="System.Drawing.Color"/> to <see cref="GorgonLibrary.Graphics.GorgonColor"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator GorgonColor(Color color)
		{
			return new GorgonColor(color);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="GorgonLibrary.Graphics.GorgonColor"/> to <see cref="System.Int32"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator int(GorgonColor color)
		{
			return color.ToARGB();
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="GorgonLibrary.Graphics.GorgonColor"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator GorgonColor(int color)
		{
			return new GorgonColor(color);
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="GorgonLibrary.Graphics.GorgonColor"/> to <see cref="GorgonLibrary.Math.Vector3D"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static explicit operator Vector3D(GorgonColor color)
		{
			return color.ToVector3D();
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="GorgonLibrary.Math.Vector3D"/> to <see cref="GorgonLibrary.Graphics.GorgonColor"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static explicit operator GorgonColor(Vector3D color)
		{
			return new GorgonColor(color);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="GorgonLibrary.Graphics.GorgonColor"/> to <see cref="GorgonLibrary.Math.Vector4D"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator Vector4D(GorgonColor color)
		{
			return color.ToVector4D();
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="GorgonLibrary.Math.Vector4D"/> to <see cref="GorgonLibrary.Graphics.GorgonColor"/>.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator GorgonColor(Vector4D color)
		{
			return new GorgonColor(color);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="color">The SharpDX color to convert.</param>
		internal GorgonColor(SharpDX.Color3 color)
			: this(1.0f, color.Red, color.Green, color.Blue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="color">The SharpDX color to convert.</param>
		internal GorgonColor(SharpDX.Color4 color)
			: this(color.Alpha, color.Red, color.Green, color.Blue)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="a">Alpha component.</param>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		public GorgonColor(float a, float r, float g, float b)
		{
			Alpha = a;
			Red = r;
			Green = g;
			Blue = b;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		public GorgonColor(float r, float g, float b)
			: this(1.0f, r, g, b)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="argb">Packed color components..</param>
		public GorgonColor(int argb)			
		{
			Alpha = ((float)((argb >> 24) & 0xff)) / 255.0f;
			Red = ((float)((argb >> 16) & 0xff)) / 255.0f;
			Green = ((float)((argb >> 8) & 0xff)) / 255.0f;
			Blue = ((float)(argb & 0xff)) / 255.0f;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="color">The .NET color value to convert.</param>
		public GorgonColor(Color color)
		{
			Alpha = ((float)color.A) / 255.0f;
			Red = ((float)color.R) / 255.0f;
			Green = ((float)color.G) / 255.0f;
			Blue = ((float)color.B) / 255.0f;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="color">The 3D vector to convert to a color.</param>
		public GorgonColor(Vector3D color)
			: this(1.0f, color.X, color.Y, color.Z)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonColor"/> struct.
		/// </summary>
		/// <param name="color">The 3D vector to convert to a color.</param>
		public GorgonColor(Vector4D color)
			: this(color.X, color.Y, color.Z, color.W)
		{
		}
		#endregion

		#region IEquatable<GorgonColor> Members
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(GorgonColor other)
		{
			return (GorgonMathUtility.EqualFloat(other.Red, this.Red)) && (GorgonMathUtility.EqualFloat(other.Green, this.Green)) && (GorgonMathUtility.EqualFloat(other.Blue, this.Blue)) && (GorgonMathUtility.EqualFloat(other.Alpha, this.Alpha));
		}
		#endregion
	}
}
