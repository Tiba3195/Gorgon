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
// Created: Wednesday, October 3, 2012 9:16:33 PM
// 
#endregion

using System;
using GorgonLibrary.Graphics;

namespace GorgonLibrary.Animation
{
	/// <summary>
	/// A track that will animate properties with a GorgonColor data type.
	/// </summary>
	/// <typeparam name="T">Type of object to be animated.</typeparam>
	class GorgonTrackGorgonColor<T>
		: GorgonAnimationTrack<T>
		where T : class
	{
		#region Variables.
	    private readonly Action<T, GorgonColor> _setProperty;			// Set property method.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the supported interpolation modes for this track.
		/// </summary>
		public override TrackInterpolationMode SupportedInterpolation
		{
			get
			{
				return TrackInterpolationMode.Spline | TrackInterpolationMode.Linear;
			}
		}		
		#endregion

		#region Methods.
		/// <summary>
		/// Function to create the a key with the proper type for this track.
		/// </summary>
		/// <returns>The key with the proper type for this track.</returns>
		protected override IKeyFrame MakeKey()
		{
			return new GorgonKeyGorgonColor(0, GorgonColor.Transparent);
		}

		/// <summary>
		/// Function to set up the spline for the animation.
		/// </summary>
		protected internal override void SetupSpline()
		{
			base.SetupSpline();

			foreach (var key in KeyFrames.Times)
				Spline.Points.Add(((GorgonKeyGorgonColor)key.Value).Value);

			Spline.UpdateTangents();
		}

		/// <summary>
		/// Function to interpolate a new key frame from the nearest previous and next key frames.
		/// </summary>
		/// <param name="keyValues">Nearest previous and next key frames.</param>
		/// <param name="keyTime">The time to assign to the key.</param>
		/// <param name="unitTime">The time, expressed in unit time.</param>
		/// <returns>
		/// The interpolated key frame containing the interpolated values.
		/// </returns>
		protected override IKeyFrame GetTweenKey(ref NearestKeys keyValues, float keyTime, float unitTime)
		{
			var next = (GorgonKeyGorgonColor)keyValues.NextKey;
			var prev = (GorgonKeyGorgonColor)keyValues.PreviousKey;

			switch (InterpolationMode)
			{
				case TrackInterpolationMode.Linear:
					return new GorgonKeyGorgonColor(keyTime, GorgonColor.Lerp(prev.Value, next.Value, unitTime));
				case TrackInterpolationMode.Spline:
					return new GorgonKeyGorgonColor(keyTime, Spline.GetInterpolatedValue(keyValues.PreviousKeyIndex, unitTime));
				default:
					return prev;
			}
		}

		/// <summary>
		/// Function to apply the key value to the object properties.
		/// </summary>
		/// <param name="key">Key to apply to the properties.</param>
		protected internal override void ApplyKey(ref IKeyFrame key)
		{
			var value = (GorgonKeyGorgonColor)key;
			_setProperty(Animation.AnimationController.AnimatedObject, value.Value);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonTrackGorgonColor{T}" /> class.
		/// </summary>
		/// <param name="property">Property information.</param>
		internal GorgonTrackGorgonColor(GorgonAnimatedProperty property)
			: base(property)
		{
			_setProperty = BuildSetAccessor<GorgonColor>();

			InterpolationMode = TrackInterpolationMode.Linear;
		}
		#endregion
	}
}
