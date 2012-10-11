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
// Created: Wednesday, October 3, 2012 8:58:49 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimMath;

namespace GorgonLibrary.Animation
{
	/// <summary>
	/// An animation track for unsigned 16 bit integer values.
	/// </summary>
	/// <typeparam name="T">Type of object being animated.</typeparam>
	internal class GorgonTrackUInt16<T>
		: GorgonAnimationTrack<T>
		where T : class
	{
		#region Variables.
		private Func<T, UInt16> _getProperty = null;			// Get property method.
		private Action<T, UInt16> _setProperty = null;		// Set property method.
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
		/// Function to set up the spline for the animation.
		/// </summary>
		protected internal override void SetupSpline()
		{
			base.SetupSpline();

			for (int i = 0; i < KeyFrames.Count; i++)
			{
				GorgonKeyUInt16 key = (GorgonKeyUInt16)KeyFrames[i];
				Spline.Points.Add(new Vector4(key.Value, 0.0f, 0.0f, 1.0f));
			}

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
		protected override IKeyFrame GetTweenKey(ref GorgonAnimationTrack<T>.NearestKeys keyValues, float keyTime, float unitTime)
		{
			GorgonKeyUInt16 next = (GorgonKeyUInt16)keyValues.NextKey;
			GorgonKeyUInt16 prev = (GorgonKeyUInt16)keyValues.PreviousKey;

			switch (InterpolationMode)
			{
				case TrackInterpolationMode.Linear:
					return new GorgonKeyUInt16(keyTime, (UInt16)((float)prev.Value + (float)(next.Value - prev.Value) * unitTime));
				case TrackInterpolationMode.Spline:
					return new GorgonKeyUInt16(keyTime, (UInt16)Spline.GetInterpolatedValue(keyValues.PreviousKeyIndex, unitTime).X);
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
			GorgonKeyUInt16 value = (GorgonKeyUInt16)key;
			_setProperty(Animation.AnimationController.AnimatedObject, value.Value);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonTrackUInt16{T}" /> class.
		/// </summary>
		/// <param name="property">The property information for the track.</param>
		internal GorgonTrackUInt16(GorgonAnimatedProperty property)
			: base(property)
		{
			_getProperty = BuildGetAccessor<UInt16>();
			_setProperty = BuildSetAccessor<UInt16>();

			InterpolationMode = TrackInterpolationMode.Linear;
		}
		#endregion
	}
}