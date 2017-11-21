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
// Created: Saturday, February 23, 2013 4:33:38 PM
// 
#endregion

using System;
using System.Threading;
using Gorgon.Core;
using Gorgon.Diagnostics;
using Gorgon.Graphics.Core.Properties;
using DXGI = SharpDX.DXGI;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;

namespace Gorgon.Graphics.Core
{
	/// <summary>
	/// A wrapper for a Direct 3D 11.4 device object and DXGI adapter.
	/// </summary>
	internal class VideoAdapter
		: IGorgonVideoAdapter
	{
		#region Variables.
		// The logging interface for debug logging.
		private readonly IGorgonLog _log;
        // The Direct 3D 11.4 device object.
	    private D3D11.Device5 _d3DDevice;
        // The DXGI factory used to create DXGI objects.
	    private DXGI.Factory5 _factory;
        // The DXGI adapter.
	    private DXGI.Adapter4 _adapter;
	    #endregion

		#region Properties.
	    /// <summary>
	    /// Property to return the factory used to create DXGI objects.
	    /// </summary>
	    public DXGI.Factory5 Factory => _factory;

	    /// <summary>
	    /// Property to return the Direct 3D 11.4 device object
	    /// </summary>
	    public D3D11.Device5 D3DDevice => _d3DDevice;

	    /// <summary>
	    /// Property to return the adapter for the video adapter.
	    /// </summary>
	    public DXGI.Adapter4 Adapter => _adapter;

	    /// <summary>
	    /// Property to return the maximum number of render targets allow to be assigned at the same time.
	    /// </summary>
	    public int MaxRenderTargetCount => D3D11.OutputMergerStage.SimultaneousRenderTargetCount;
        
        /// <summary>
        /// Property to return the maximum number of array indices for 1D and 2D textures.
        /// </summary>
        public int MaxTextureArrayCount => 2048;

	    /// <summary>
	    /// Property to return the maximum width of a 1D or 2D texture.
	    /// </summary>
	    public int MaxTextureWidth => 16384;

	    /// <summary>
	    /// Property to return the maximum height of a 2D texture.
	    /// </summary>
	    public int MaxTextureHeight => 16384;

		/// <summary>
		/// Property to return the maximum width of a 3D texture.
		/// </summary>
		public int MaxTexture3DWidth => 2048;

		/// <summary>
		/// Property to return the maximum height of a 3D texture.
		/// </summary>
		public int MaxTexture3DHeight => 2048;

		/// <summary>
		/// Property to return the maximum depth of a 3D texture.
		/// </summary>
		public int MaxTexture3DDepth => 2048;
        
		/// <summary>
		/// Property to return the maximum size, in bytes, for a constant buffer.
		/// </summary>
		/// <remarks>
		/// On devices with a a <see cref="FeatureSet"/> of <see cref="FeatureSet.Level_12_0"/>, this value will return <see cref="int.MaxValue"/>, indicating that there is no limit to the size of a 
		/// constant buffer. On devices with a lower feature set this value is limited to 65536 (4096 * 16) bytes.
		/// </remarks>
		public int MaxConstantBufferSize => RequestedFeatureLevel < FeatureSet.Level_12_0 ? int.MaxValue : 65536;

	    /// <summary>
	    /// Property to return the maximum number of allowed scissor rectangles.
	    /// </summary>
	    public int MaxScissorCount => 16;
        
	    /// <summary>
	    /// Property to return the maximum number of allowed viewports.
	    /// </summary>
	    public int MaxViewportCount => 16;

        /// <summary>
        /// Property to return the <see cref="VideoAdapterInfo"/> used to create this device.
        /// </summary>
        public IGorgonVideoAdapterInfo Info
		{
			get;
		}

		/// <summary>
		/// Property to return the actual supported <see cref="FeatureSet"/> from the device.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A user may request a lower <see cref="FeatureSet"/> than what is supported by the device to allow the application to run on older video adapters that lack support for newer functionality. 
		/// This requested feature set will be returned by this property if supported by the device. 
		/// </para>
		/// <para>
		/// If the user does not request a feature set, or has specified one higher than what the video adapter supports, then the highest feature set supported by the video adapter 
		/// (indicated by the <see cref="VideoAdapterInfo.SupportedFeatureLevel"/> property in the <see cref="IGorgonVideoAdapter.Info"/> property) will be returned.
		/// </para>
		/// </remarks>
		/// <seealso cref="FeatureSet"/>
		public FeatureSet RequestedFeatureLevel
		{
			get;
			private set;
		}
	    #endregion

        #region Methods.
        /// <summary>
        /// Function to convert a Gorgon feature set into a D3D feature set.
        /// </summary>
        /// <param name="requestedFeatureLevel">The requested feature set.</param>
        /// <returns>The D3D feature sets to use.</returns>
        private static D3D.FeatureLevel[] GetFeatureLevel(D3D.FeatureLevel requestedFeatureLevel)
		{
			switch (requestedFeatureLevel)
			{
				case D3D.FeatureLevel.Level_12_1:
					return new[]
					       {
						       D3D.FeatureLevel.Level_12_1,
						       D3D.FeatureLevel.Level_12_0
					       };
				case D3D.FeatureLevel.Level_12_0:
					return new[] {
							D3D.FeatureLevel.Level_12_0
					};
				default:
					throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GORGFX_ERR_FEATURE_LEVEL_INVALID, requestedFeatureLevel));
			}
		}

		/// <summary>
		/// Function to create the Direct 3D device and Adapter for use with Gorgon.
		/// </summary>
		/// <param name="requestedFeatureLevel">The requested feature set for the device.</param>
		private void CreateDevice(D3D.FeatureLevel requestedFeatureLevel)
		{
			D3D11.DeviceCreationFlags flags = GorgonGraphics.IsDebugEnabled ? D3D11.DeviceCreationFlags.Debug : D3D11.DeviceCreationFlags.None;

		    using (DXGI.Factory2 factory2 = new DXGI.Factory2(GorgonGraphics.IsDebugEnabled))
		    {
		        _factory = factory2.QueryInterface<DXGI.Factory5>();

                using (DXGI.Adapter adapter = (Info.VideoDeviceType == VideoDeviceType.Hardware ? _factory.GetAdapter1(Info.Index) : _factory.GetWarpAdapter()))
		        {
		            _adapter = adapter.QueryInterface<DXGI.Adapter4>();

		            using (D3D11.Device device = new D3D11.Device(_adapter, flags, GetFeatureLevel(requestedFeatureLevel))
		                                         {
		                                             DebugName =
		                                                 $"'{Info.Name}' D3D11.4 {(Info.VideoDeviceType == VideoDeviceType.Software ? "Software Adapter" : "Adapter")}"
		                                         })
		            {
		                _d3DDevice = device.QueryInterface<D3D11.Device5>();
		                // Get the maximum supported feature set for this device.
		                RequestedFeatureLevel = (FeatureSet)_d3DDevice.FeatureLevel;

		                _log.Print($"Direct 3D 11.4 device created for video adapter '{Info.Name}' at feature set [{RequestedFeatureLevel}]", LoggingLevel.Simple);
		            }
		        }
		    }
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format(Resources.GORGFX_TOSTR_DEVICE, Info.Name);
		}

	    /// <summary>
	    /// Function to retrieve the supported functionality for a given format.
	    /// </summary>
	    /// <param name="format">The format to evaluate.</param>
	    /// <returns>A <see cref="BufferFormatSupport"/> containing OR'd values representing the functionality supported by the format.</returns>
	    /// <remarks>
	    /// <para>
	    /// Use this method to determine if a format can be used with a specific resource type (e.g. a 2D texture, vertex buffer, etc...). The value returned will be of the <see cref="BufferFormatSupport"/>
	    /// enumeration and will contain the supported functionality represented as OR'd values.
	    /// </para>
	    /// </remarks>
	    /// <seealso cref="BufferFormatSupport"/>
	    public BufferFormatSupport GetBufferFormatSupport(BufferFormat format)
		{
			return (BufferFormatSupport)D3DDevice.CheckFormatSupport((DXGI.Format)format);
		}

	    /// <summary>
	    /// Function to retrieve the supported unordered access compute resource functionality for a given format.
	    /// </summary>
	    /// <param name="format">The format to evaluate.</param>
	    /// <returns>A <see cref="ComputeShaderFormatSupport"/> containing OR'd values representing the functionality supported by the format.</returns>
	    /// <remarks>
	    /// <para>
	    /// Use this method to determine if a format can be used with specific unordered access view operations in a compute shader. The value returned will be of the <see cref="ComputeShaderFormatSupport"/> 
	    /// enumeration type and will contain the supported functionality represented as OR'd values.
	    /// </para>
	    /// </remarks>
	    /// <seealso cref="ComputeShaderFormatSupport"/>
		public ComputeShaderFormatSupport GetBufferFormatComputeSupport(BufferFormat format)
		{
			return (ComputeShaderFormatSupport)D3DDevice.CheckComputeShaderFormatSupport((DXGI.Format)format);
		}

		/// <summary>
		/// Function to return a <see cref="GorgonMultisampleInfo"/> with the best quality level for the given count and format.
		/// </summary>
		/// <param name="format">A <see cref="BufferFormat"/> to evaluate.</param>
		/// <param name="count">The number of samples.</param>
		/// <returns>A <see cref="GorgonMultisampleInfo"/> containing the quality count and sample count for multisampling.</returns>
		/// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="count"/> is not supported by this video adapter.</exception>
		/// <remarks>
		/// <para>
		/// Use this to return a <see cref="GorgonMultisampleInfo"/> containing the best quality level for a given <paramref name="count"/> and <paramref name="format"/>.
		/// </para>
		/// <para>
		/// If <see cref="BufferFormat.Unknown"/> is passed to the <paramref name="format"/> parameter, then this method will return <see cref="GorgonMultisampleInfo.NoMultiSampling"/>.
		/// </para>
		/// <para>
		/// Before calling this method, call the <see cref="O:Gorgon.Graphics.IGorgonVideoDevice.SupportsMultisampleCount"/> method to determine if multisampling is supported for the given <paramref name="count"/> and <paramref name="format"/>.
		/// </para>
		/// </remarks>
		public GorgonMultisampleInfo GetMultisampleInfo(BufferFormat format, int count)
		{
			if (format == BufferFormat.Unknown)
			{
				return GorgonMultisampleInfo.NoMultiSampling;
			}

			int quality = D3DDevice.CheckMultisampleQualityLevels((DXGI.Format)format, count);

			if (quality == 0)
			{
				throw new ArgumentException(string.Format(Resources.GORGFX_ERR_MULTISAMPLE_COUNT_NOT_SUPPORTED, count, format, Info.Name));
			}

			return new GorgonMultisampleInfo(count, quality - 1);
		}

		/// <summary>
		/// Function to return whether or not the device supports multisampling for the given format and sample count.
		/// </summary>
		/// <param name="format">A <see cref="BufferFormat"/> to evaluate.</param>
		/// <param name="count">The number of samples.</param>
		/// <returns><b>true</b> if the device supports the format, or <b>false</b> if not.</returns>
		/// <remarks>
		/// <para>
		/// Use this to determine if the video adapter will support multisampling with a specific sample <paramref name="count"/> and <paramref name="format"/>. 
		/// </para>
		/// <para>
		/// If <see cref="BufferFormat.Unknown"/> is passed to the <paramref name="format"/> parameter, then this method will return <b>true</b> because this will equate to no multisampling.
		/// </para>
		/// </remarks>
		public bool SupportsMultisampleCount(BufferFormat format, int count)
		{
			if (count < 1)
			{
				return false;
			}

			if (format == BufferFormat.Unknown)
			{
				return true;
			}

			return D3DDevice.CheckMultisampleQualityLevels((DXGI.Format)format, count) > 0;
		}

		/// <summary>
		/// Function to return whether or not the device supports multisampling for the given format and the supplied <see cref="GorgonMultisampleInfo"/>.
		/// </summary>
		/// <param name="format">A <see cref="BufferFormat"/> to evaluate.</param>
		/// <param name="multiSampleInfo">The multisample info to use when evaluating.</param>
		/// <returns><b>true</b> if the device supports the format, or <b>false</b> if not.</returns>
		/// <remarks>
		/// <para>
		/// Use this to determine if the video adapter will support multisampling with a specific <paramref name="multiSampleInfo"/> and <paramref name="format"/>. 
		/// </para>
		/// <para>
		/// If <see cref="BufferFormat.Unknown"/> is passed to the <paramref name="format"/> parameter, then this method will return <b>true</b> because this will equate to no multisampling.
		/// </para>
		/// </remarks>
		public bool SupportsMultisampleInfo(BufferFormat format, GorgonMultisampleInfo multiSampleInfo)
		{
			if (format == BufferFormat.Unknown)
			{
				return true;
			}

			if (multiSampleInfo.Count < 1)
			{
				return false;
			}

			int quality = D3DDevice.CheckMultisampleQualityLevels((DXGI.Format)format, multiSampleInfo.Count);

			return ((quality != 0) && (multiSampleInfo.Quality < quality));
		}

	    /// <summary>
	    /// Function to find a display mode supported by the Gorgon.
	    /// </summary>
	    /// <param name="output">The output to use when looking for a video mode.</param>
	    /// <param name="videoMode">The <see cref="GorgonVideoMode"/> used to find the closest match.</param>
	    /// <param name="suggestedMode">A <see cref="GorgonVideoMode"/> that is the nearest match for the provided video mode.</param>
	    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="output"/> parameter is <b>null</b>.</exception>
	    /// <remarks>
	    /// <para>
	    /// Users may leave the <see cref="GorgonVideoMode"/> values at unspecified (either 0, or default enumeration values) to indicate that these values should not be used in the search.
	    /// </para>
	    /// <para>
	    /// The following members in <see cref="GorgonVideoMode"/> may be skipped (if not listed, then this member must be specified):
	    /// <list type="bullet">
	    ///		<item>
	    ///			<description><see cref="GorgonVideoMode.Width"/> and <see cref="GorgonVideoMode.Height"/>.  Both values must be set to 0 if not filtering by width or height.</description>
	    ///		</item>
	    ///		<item>
	    ///			<description><see cref="GorgonVideoMode.RefreshRate"/> should be set to empty in order to skip filtering by refresh rate.</description>
	    ///		</item>
	    ///		<item>
	    ///			<description><see cref="GorgonVideoMode.Scaling"/> should be set to <see cref="ModeScaling.Unspecified"/> in order to skip filtering by the scaling mode.</description>
	    ///		</item>
	    ///		<item>
	    ///			<description><see cref="GorgonVideoMode.ScanlineOrder"/> should be set to <see cref="ModeScanlineOrder.Unspecified"/> in order to skip filtering by the scanline order.</description>
	    ///		</item>
	    /// </list>
	    /// </para>
	    /// <para>
	    /// <note type="important">
	    /// <para>
	    /// The <see cref="GorgonVideoMode.Format"/> member must be one of the UNorm format types and cannot be set to <see cref="BufferFormat.Unknown"/>.
	    /// </para>
	    /// </note>
	    /// </para>
	    /// </remarks>
	    public void FindNearestVideoMode(IGorgonVideoOutputInfo output, ref GorgonVideoMode videoMode, out GorgonVideoMode suggestedMode)
		{
			using (DXGI.Output giOutput = Adapter.GetOutput(output.Index))
			{
				using (DXGI.Output1 giOutput1 = giOutput.QueryInterface<DXGI.Output1>())
				{
					DXGI.ModeDescription1 matchMode = videoMode.ToModeDesc1();

					giOutput1.FindClosestMatchingMode1(ref matchMode, out DXGI.ModeDescription1 mode, D3DDevice);

					suggestedMode =  mode.ToGorgonVideoMode();
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		    D3D11.Device5 device = Interlocked.Exchange(ref _d3DDevice, null);
		    DXGI.Adapter4 adapter = Interlocked.Exchange(ref _adapter, null);
		    DXGI.Factory5 factory = Interlocked.Exchange(ref _factory, null);

            device?.Dispose();
            adapter?.Dispose();
            factory?.Dispose();
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="VideoAdapter"/> class.
		/// </summary>
		/// <param name="deviceInfo">A <see cref="VideoAdapterInfo"/> containing information about which video adapter to use.</param>
		/// <param name="requestedFeatureLevel">The desired feature set for the device.</param>
		/// <param name="log">A <see cref="IGorgonLog"/> used for logging debug output.</param>
		public VideoAdapter(IGorgonVideoAdapterInfo deviceInfo, FeatureSet requestedFeatureLevel, IGorgonLog log)
		{
			_log = log ?? GorgonLogDummy.DefaultInstance;
			Info = deviceInfo;
			CreateDevice((D3D.FeatureLevel)requestedFeatureLevel);
		}
		#endregion
	}
}