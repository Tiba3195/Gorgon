﻿#region MIT
// 
// Gorgon.
// Copyright (C) 2018 Michael Winsor
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
// Created: November 9, 2018 3:30:20 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gorgon.Editor.UI;

namespace Gorgon.Editor.ImageEditor.ViewModels
{
    /// <summary>
    /// The zoom levels for image magnification.
    /// </summary>
    internal enum ZoomLevels
    {
        /// <summary>
        /// Resize to the client area of the window.
        /// </summary>
        ToWindow = 0,
        /// <summary>
        /// Shrink to 12% of normal size.
        /// </summary>
        Percent12 = 1,
        /// <summary>
        /// Shrink to 25% of normal size.
        /// </summary>
        Percent25 = 2,
        /// <summary>
        /// Shrink to 50% of normal size.
        /// </summary>
        Percent50 = 3,
        /// <summary>
        /// Shrink to 100% of normal size.
        /// </summary>
        Percent100 = 4,
        /// <summary>
        /// Shrink to 200% of normal size.
        /// </summary>
        Percent200 = 5,
        /// <summary>
        /// Shrink to 400% of normal size.
        /// </summary>
        Percent400 = 6,
        /// <summary>
        /// Shrink to 800% of normal size.
        /// </summary>
        Percent800 = 7,
        /// <summary>
        /// Shrink to 1600% of normal size.
        /// </summary>
        Percent1600 = 8
    }

    /// <summary>
    /// The image content view model.
    /// </summary>
    internal interface IImageContentVm
        : IViewModel
    {
        #region Variables.

        #endregion

        #region Properties.
        /// <summary>
        /// Property to set or return the zoom level.
        /// </summary>
        ZoomLevels ZoomLevel
        {
            get;
            set;
        }
        #endregion

        #region Methods.

        #endregion

        #region Constructor/Finalizer.

        #endregion
    }
}
