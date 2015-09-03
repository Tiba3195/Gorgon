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
// Created: Sunday, June 26, 2011 2:08:47 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gorgon.Diagnostics;
using Gorgon.Input.Raw;
using Gorgon.Input.Raw.Properties;

namespace Gorgon.Input
{
	/// <summary>
	/// The entry point for the raw input plug-in.
	/// </summary>
	public class GorgonRawPlugIn
		: GorgonInputServicePlugin
	{
		#region Properties.
		/// <summary>
		/// Property to return whether the plugin supports game devices like game pads, or joysticks.
		/// </summary>
		public override bool SupportsJoysticks => true;

		/// <summary>
		/// Property to return whether the plugin supports pointing devices like mice, trackballs, etc...
		/// </summary>
		public override bool SupportsMice => true;

		/// <summary>
		/// Property to return whether the plugin supports keyboard devices.
		/// </summary>
		public override bool SupportsKeyboards => true;
		#endregion

		#region Methods.
		/// <inheritdoc/>
		protected override GorgonInputService2 OnCreateInputService2(IGorgonLog log)
		{
			// The raw input processor for device data.
			var rawInputProcessors = new Dictionary<Control, RawInputProcessor>();

			var coordinator = new RawInputDeviceCoordinator();
			var registrar = new RawInputDeviceRegistrar(log, coordinator, rawInputProcessors);

			return new GorgonRawInputService(log, registrar, coordinator);
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonRawPlugIn"/> class.
		/// </summary>
		public GorgonRawPlugIn()
			: base(Resources.GORINP_RAW_SERVICEDESC)
		{
		}
		#endregion
	}
}
