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
// Created: Saturday, March 9, 2013 3:36:32 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GorgonLibrary.Editor
{
	/// <summary>
	/// A base interface for content display in the main interface.
	/// </summary>
	public partial class ContentPanel : UserControl
	{
		#region Variables.
        private ContentObject _content = null;
		private bool _captionVisible = true;
        private bool _contentChanged = false;
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the display panel.
		/// </summary>
        [Browsable(false)]
		public Panel PanelDisplay
		{
			get
			{
				return _panelContentDisplay;
			}
		}

		/// <summary>
		/// Property to set or return the content object to be manipulated in this interface.
		/// </summary>
		[Browsable(false)]
		public ContentObject Content
		{
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnContentChanged();
            }
		}        

		/// <summary>
		/// Property to set or return the text caption for this control.
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Sets the text for the caption on the content panel control."), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
                UpdateCaption();
			}
		}

		/// <summary>
		/// Property to set or return whether the caption for the content panel is visible or not.
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Shows or hides the caption for the content panel."), DefaultValue(true)]
		public bool CaptionVisible
		{
			get
			{
				return _captionVisible;
			}
			set
			{
				_captionVisible = value;
				panelCaption.Visible = _captionVisible;
			}
		}
		#endregion

		#region Methods.
        /// <summary>
        /// Function to update the caption label.
        /// </summary>
        private void UpdateCaption()
        {
            labelCaption.Text = base.Text;

            if (_contentChanged)
            {
                labelCaption.Text += "*";
            }
        }

		/// <summary>
		/// Handles the MouseEnter event of the labelClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void labelClose_MouseEnter(object sender, EventArgs e)
		{
			labelClose.ForeColor = Color.White;
		}

		/// <summary>
		/// Handles the MouseLeave event of the labelClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void labelClose_MouseLeave(object sender, EventArgs e)
		{
			labelClose.ForeColor = Color.Silver;
		}

		/// <summary>
		/// Handles the MouseMove event of the labelClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
		private void labelClose_MouseMove(object sender, MouseEventArgs e)
		{
			labelClose.ForeColor = Color.White;
		}

		/// <summary>
		/// Handles the Click event of the labelClose control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void labelClose_Click(object sender, EventArgs e)
		{
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                OnCloseClicked();
            }
            catch (Exception ex)
            {
                GorgonLibrary.UI.GorgonDialogs.ErrorBox(ParentForm, ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
		}

		/// <summary>
		/// Function called when the close button is clicked.
		/// </summary>
		protected virtual void OnCloseClicked()
		{
			if (Content != null)
			{
				var form = FindForm() as formMain;

				if (form != null)
				{
					form.LoadContentPane<DefaultContent>();
				}
			}
		}

        /// <summary>
        /// Function called when the content has changed.
        /// </summary>
        public virtual void OnContentChanged()
        {
            if (Content != null)
            {
                _contentChanged = Content.HasChanges;
            }
            else
            {
                _contentChanged = false;
            }

            UpdateCaption();
        }
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentPanel"/> class.
		/// </summary>
		public ContentPanel()
		{
			InitializeComponent();			
		}
		#endregion
	}
}