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
// Created: Tuesday, March 12, 2013 9:22:44 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace GorgonLibrary.Editor
{
	/// <summary>
	/// A tree node for the editor.
	/// </summary>
	class EditorTreeNode
		: TreeNode
	{
		#region Variables.

		#endregion

		#region Properties.
        /// <summary>
        /// Gets or sets the background color of the tree node.
        /// </summary>
        /// <returns>The background <see cref="T:System.Drawing.Color" /> of the tree node. The default is <see cref="F:System.Drawing.Color.Empty" />.</returns>
        ///   <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   </PermissionSet>
        public virtual new Color BackColor
        {
            get
            {
                if (IsSelected)
                {
                    return DarkFormsRenderer.MenuHilightBackground;
                }
                
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

		/// <summary>
		/// Gets or sets the foreground color of the tree node.
		/// </summary>
		/// <returns>The foreground <see cref="T:System.Drawing.Color" /> of the tree node.</returns>
		///   <PermissionSet>
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   </PermissionSet>
		public virtual new Color ForeColor
		{
			get
			{
                if (IsSelected)
                {
                    return DarkFormsRenderer.MenuHilightForeground;
                }

				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the text displayed in the label of the tree node.
		/// </summary>
		/// <returns>The text displayed in the label of the tree node.</returns>
		///   <PermissionSet>
		///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
		///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   </PermissionSet>
		public virtual new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Property to return the collapsed image for the node.
		/// </summary>
		public Image CollapsedImage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to return the expanded image for the node.
		/// </summary>
		public Image ExpandedImage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the bounds of the tree node.
		/// </summary>
		/// <returns>The <see cref="T:System.Drawing.Rectangle" /> that represents the bounds of the tree node.</returns>
		///   <PermissionSet>
		///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
		///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   </PermissionSet>
		public new Rectangle Bounds
		{
			get
			{
				if (this.TreeView != null)
				{
					return new Rectangle(0, base.Bounds.Top, this.TreeView.ClientSize.Width, base.Bounds.Height);
				}

				return base.Bounds;
			}
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to refresh this node.
		/// </summary>
		public void Redraw()
		{
			if (this.TreeView != null)
			{
				this.TreeView.Invalidate(new Rectangle(0, base.Bounds.Top, this.TreeView.ClientSize.Width, base.Bounds.Height), true);
			}			
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="EditorTreeNode"/> class.
		/// </summary>
		public EditorTreeNode()
		{
				
		}
		#endregion
	}
}