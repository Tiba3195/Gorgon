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
// Created: Saturday, January 5, 2013 3:33:05 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Gorgon.Core;
using Gorgon.Examples.Properties;
using Gorgon.Input;
using Gorgon.Plugins;
using Gorgon.Timing;
using Gorgon.UI;
using GorgonMouseButtons = Gorgon.Input.MouseButtons;

namespace Gorgon.Examples
{
	/// <summary>
	/// Our main form for the example.
	/// </summary>
	/// <remarks>
	/// In this example we will instance and control the mouse (pointing device) and keyboard devices.  So, naturally, you'll
	/// need a mouse and keyboard for this.  If you don't have one... well... how are you doing anything?
	/// 
	/// Unlike the InputFactory example, we're just going to load the Raw Input plug-in and use the keyboard/mouse from that
	/// interface.  The Raw Input is most useful for mice (although it has a keyboard component) and gives very precise control
	/// over the mouse.  It allows the developer to lock the mouse to the window while it has focus, or even when the window
	/// isn't in focus (this of course means no other applications will get access to the mouse).  This isn't the default 
	/// behaviour, and must be turned on by setting the Exclusive property to true (and AllowBackground to true for background
	/// exclusive access).  Keyboards also have these properties as well.  Be warned though that when a keyboard goes into
	/// exclusive mode the standard windows hot keys will no longer be recognized (e.g. Alt+F4, etc..) and must be processed by 
	/// your application.
	/// 
	/// If you're curious about the "BufferContext" stuff, that's all from GDI+ (System.Drawing) and allows us to set up a 
	/// double buffer scenario so our mouse cursor can be drawn without flicker.  But that is not the scope of this example.
	/// 
	/// To create the mouse and keyboard object we need to instance it from the InputFactory that comes from our Raw Input 
	/// plug-in.  To learn more about loading the InputFactories from their plug-ins see the InputFactory example.
	/// 
	/// Once we have these objects via the InputFactory.CreatePointingDevice and InputFactory.CreateKeyboard methods, we can then
	/// set them up for exclusive access and then assign events that will be triggered by input from the device.  This is a 
	/// similar setup to the Windows Forms input events (KeyDown, MouseDown, MouseMove, etc...) and is great in an environment 
	/// where the overhead of events is not an issue.  But in performance critical sections or just for absolute control, these
	/// objects can be polled at any time (e.g. in your Idle loop method).  This way the application can retrieve data from a 
	/// device like the mouse in real time, that is, as fast as your computer can call the update loop.  
	/// 
	/// To see the difference between polling and events do the following while running the example:
	/// 1. Hold down the left mouse button while in event mode (default), and move around.  The spray effect updates.
	/// 2. Stop moving (but keep the left button pressed). The spray effect stops updating.  This is because there are no events
	///    being fired and the events are the methods that update the spray effect.
	/// 3. Now change to polling by pressing the "P" key.
	/// 4. Hold down the left mouse button and notice that the spray keeps updating regardless of whether we're moving.  
	/// 
	/// And that is the difference between polling and event driven input data.
	/// 
	/// Joysticks are exposed in the Raw Input plug-in, but use a very basic interface and don't expose any special features 
	/// (e.g. force feedback).  And unlike other devices, joysticks don't raise events and must be polled via its Poll() method.  
	/// In this example, if a joystick is detected it will be noted on the display panel and pressing the primary button will 
	/// draw a spray effect to the display panel.
    /// 
    /// Joystick axis information usually returns values much larger than the available display area, usually between a negative
    /// and positive value and with the y-axis flipped.  So when we gather the information, we need to flip the y-axis and 
    /// transform the coordinates into screen space via the JoystickTransformed property.
	/// </remarks>
	public partial class formMain : Form
	{
		#region Variables.
		// The spray effect.
		private Spray _spray;                                   
		// Our mouse cursor.
        private MouseCursor _cursor;                            
		// Our input service.
		private IGorgonInputService _service;
		// Our mouse interface.
		private IGorgonMouse _mouse;
		// A joystick interface.
		private GorgonJoystick _joystick;
		// Our keyboard interface.
		private IGorgonKeyboard _keyboard;
		// Mouse position.
		private Point _mousePosition = Point.Empty;
		// Current image for the cursor.
		private Image _currentCursor;
		// Flag to indicate whether to use polling or events.
		private bool _usePolling;
		// A spin wait used to give up CPU while we're keeping our app under control in the idle event.
		private SpinWait _spinner;
		// The timer used to determine how long to wait until the next idle loop iteration.
		private IGorgonTimer _updateTimer = GorgonTimerQpc.SupportsQpc() ? (IGorgonTimer)new GorgonTimerQpc() : new GorgonTimerMultimedia();
		#endregion

		#region Properties.
        /// <summary>
        /// Property to return the joystick primary axis coordinates transformed into screen space.
        /// </summary>
        /// <remarks>The joystick axis coordinates can be larger or smaller than screen space, so we 
        /// need to transform them to the confines of the display area.</remarks>
        private Point JoystickTransformed
        {
            get
            {
                Point screenPosition = Point.Empty;

                if (_joystick == null)
                {
                    return Point.Empty;
                }

                // We -must- call poll here or else the joystick will appear to be
                // disconnected and will not have any current data.
                _joystick.Poll();

                // Ensure that the joystick is connected and the button is pressed.
	            if (!_joystick.Info.IsConnected)
	            {
		            return screenPosition;
	            }

	            // Get our joystick data and constrain it.
	            // First get the normalized joystick value.
	            // Do this by first shifting the coordinates to the minimum range value.
	            var stickNormalized = new PointF(_joystick.Axis[JoystickAxis.XAxis] - (float)_joystick.Info.AxisRanges[JoystickAxis.XAxis].Minimum,
		            _joystick.Axis[JoystickAxis.YAxis] - (float)_joystick.Info.AxisRanges[JoystickAxis.YAxis].Minimum);
	            // Then normalize.
	            stickNormalized = new PointF(stickNormalized.X / (_joystick.Info.AxisRanges[JoystickAxis.XAxis].Range + 1), 
		            stickNormalized.Y / (_joystick.Info.AxisRanges[JoystickAxis.YAxis].Range + 1));

	            // Now transform the normalized point into display space.
	            screenPosition = new Point((int)(stickNormalized.X * (panelDisplay.ClientSize.Width - 1)) 
		            , (panelDisplay.ClientSize.Height - 1) - (int)(stickNormalized.Y * panelDisplay.ClientSize.Height));

					
	            if (_joystick.Button[0] == JoystickButtonState.Down)
	            {
		            // Spray the screen.
		            _currentCursor = Resources.hand_pointer_icon;
		            _mouse.Position = _mousePosition = screenPosition;	
		            _spray.SprayPoint(_mousePosition);
	            }
	            else
	            {
		            // Turn off the cursor if the mouse button isn't held down.
		            if ((_mouse.Buttons & GorgonMouseButtons.Button1) == GorgonMouseButtons.Button1)
		            {
			            _currentCursor = Resources.hand_icon;
		            }
	            }

	            return screenPosition;
            }
        }
		#endregion

		#region Methods.
        /// <summary>
        /// Handles the Paint event of the panelMouse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
        private void DevicePanelsPaint(object sender, PaintEventArgs e)
        {
            var control = sender as Control;

	        if (control == null)
	        {
		        return;
	        }

	        using(var pen = new Pen(Color.Black, SystemInformation.BorderSize.Height))
	        {
		        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(control.Width, 0));
	        }
        }
        
        /// <summary>
		/// Function to process during application idle time.
		/// </summary>
		/// <returns><b>true</b> to continue processing, <b>false</b> to stop.</returns>
		private bool Idle()
		{
			// If we're using a polling method, then we can retrieve all the mouse
			// info in real time during our idle time.
			if (_usePolling)
			{
                UpdateMouseLabel(_mouse.Position, _mouse.Buttons);
			}

            // Update the joystick information.
            UpdateJoystickLabel(JoystickTransformed);

			// Display the mouse cursor.			
            _cursor.DrawMouseCursor(_mousePosition, _currentCursor, _spray.Surface);

			// Free up CPU time if we're not using it.
	        while (_updateTimer.Milliseconds <= GorgonTiming.FpsToMilliseconds(60))
	        {
		        _spinner.SpinOnce();
	        }

			_updateTimer.Reset();

			return true;
		}

        /// <summary>
        /// Function to update the joystick label.
        /// </summary>
        /// <param name="joystickTransformed">The transformed screen point for the joystick.</param>
        private void UpdateJoystickLabel(Point joystickTransformed)
        {
            if (_joystick == null)
            {
                return;
            }

            // Display the proper joystick text.
            if (_joystick.Info.IsConnected)
            {
                labelJoystick.Text = string.Format("{0} connected.  Position: {1}x{2} (Raw {4}x{5}).  Primary button {3}"
                                            , _joystick.Name
                                            , joystickTransformed.X
                                            , joystickTransformed.Y
                                            , (_joystick.Button[0] == JoystickButtonState.Down ? "pressed" : "not pressed (press the button to spray).")
                                            , _joystick.Axis[JoystickAxis.XAxis]
											, _joystick.Axis[JoystickAxis.YAxis]);
            }
            else
            {
                labelJoystick.Text = $"{_joystick.Name} not connected.";
            }
        }

        /// <summary>
        /// Function to update the mouse information.
        /// </summary>
        /// <param name="position">The position of the mouse cursor.</param>
        /// <param name="button">The current button being held down.</param>
        private void UpdateMouseLabel(Point position, GorgonMouseButtons button)
        {
            if (button == GorgonMouseButtons.Button1)
            {
                _spray.SprayPoint(Point.Round(position));
                _currentCursor = Resources.hand_pointer_icon;
            }
            else
            {
                _currentCursor = Resources.hand_icon;
            }

            _mousePosition = new Point(position.X, position.Y);

	        labelMouse.Text = $"{_mouse.Info.Description}: {position.X.ToString("0.#")}x{position.Y.ToString("0.#")}.  " +
	                          $"Button: {button}.  Using {(_usePolling ? "Polling" : "Events")} for data retrieval.";
        }

	    /// <summary>
	    /// Function to update the keyboard label.
	    /// </summary>
	    /// <param name="key">Key that's currently pressed.</param>
	    /// <param name="shift">Shifted keys.</param>
	    private void UpdateKeyboardLabel(Keys key, Keys shift)
		{
			var shiftKey = Keys.None;

			if ((Keys.Alt & shift) == Keys.Alt)
			{
				shiftKey = (shift & Keys.LMenu) == Keys.LMenu ? Keys.LMenu : Keys.RMenu;
			}

			if ((shift & Keys.Control) == Keys.Control)
			{
				shiftKey = (shift & Keys.LControlKey) == Keys.LControlKey ? Keys.LControlKey : Keys.RControlKey;
			}

			if ((shift & Keys.Shift) == Keys.Shift)
			{
				shiftKey = (shift & Keys.LShiftKey) == Keys.LShiftKey ? Keys.LShiftKey : Keys.RShiftKey;
			}


			labelKeyboard.Text = string.Format("{2}. Currently pressed key: {0}{1}  (Press 'P' to switch between polling and events for the mouse. Press 'ESC' to close.)"
												, key
												, ((shiftKey != Keys.None) && (shiftKey != key) ? " + " + shiftKey : string.Empty)
                                                , _keyboard.Info.Description);				
		}

		/// <summary>
		/// Handles the PointingDeviceUp event of the _mouse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PointingDeviceEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		private void _mouse_ButtonUp(object sender, GorgonMouseEventArgs e)
		{
			// Update the buttons so that only the buttons we have held down are showing.
            UpdateMouseLabel(e.Position, e.ShiftButtons & ~e.Buttons);
		}

		/// <summary>
		/// Handles the PointingDeviceDown event of the _mouse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PointingDeviceEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotSupportedException"></exception>
        private void _mouse_ButtonDown(object sender, GorgonMouseEventArgs e)
		{
            UpdateMouseLabel(e.Position, e.Buttons | e.ShiftButtons);
        }

		/// <summary>
		/// Handles the PointingDeviceMove event of the _mouse control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="PointingDeviceEventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		private void _mouse_Move(object sender, GorgonMouseEventArgs e)
		{
            UpdateMouseLabel(e.Position, e.Buttons | e.ShiftButtons);
		}

		/// <summary>
		/// Handles the KeyUp event of the _keyboard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GorgonKeyboardEventArgs" /> instance containing the event data.</param>
		private void _keyboard_KeyUp(object sender, GorgonKeyboardEventArgs e)
		{
			UpdateKeyboardLabel(Keys.None, e.ModifierKeys);
		}

		/// <summary>
		/// Handles the KeyDown event of the _keyboard control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GorgonKeyboardEventArgs" /> instance containing the event data.</param>		
		private void _keyboard_KeyDown(object sender, GorgonKeyboardEventArgs e)
		{
			// If we press "P", then switch between polling and events.
			if (e.Key == Keys.P)
			{
				_usePolling = !_usePolling;
				if (_usePolling)
				{
					// Turn off mouse events when polling.
					_mouse.MouseMove -= _mouse_Move;
					_mouse.MouseButtonDown -= _mouse_ButtonDown;
					_mouse.MouseButtonUp -= _mouse_ButtonUp;
				}
				else
				{
					// Turn on mouse events when not polling.
					_mouse.MouseMove += _mouse_Move;
					_mouse.MouseButtonDown += _mouse_ButtonDown;
					_mouse.MouseButtonUp += _mouse_ButtonUp;
				}
			}

			// Exit the application.
			if (e.Key == Keys.Escape)
			{
				Close();
				return;
			}

			UpdateKeyboardLabel(e.Key, e.ModifierKeys);
		}

		/// <summary>
		/// Handles the Resize event of the panelDisplay control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		private void panelDisplay_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				return;
			}

			// If we resize our window, update our position range.
			if (_mouse != null)
			{
				_mouse.PositionConstraint = panelDisplay.ClientRectangle;
			}

			_spray?.Resize(panelDisplay.ClientSize);
		}

        /// <summary>
        /// Function to create the mouse device.
        /// </summary>
        /// <param name="mouseInfo">The information about the current mouse to use.</param>
        private void CreateMouse(IGorgonMouseInfo2 mouseInfo)
        {
            // Create the device from the factory.
            _mouse = new GorgonMouse(_service, mouseInfo);


            // Assign an event to notify us when the mouse is moving.
            _mouse.MouseMove += _mouse_Move;

            // Assign another event to notify us when a mouse button was clicked.
            _mouse.MouseButtonDown += _mouse_ButtonDown;
            _mouse.MouseButtonUp += _mouse_ButtonUp;

            // Limit the mouse position to the client area of the window.				
            _mouse.PositionConstraint = panelDisplay.ClientRectangle;

			// This will bind the device to this window, allowing us to intercept its data.
			// We will set the mouse as exclusively owned by this window. This keeps the windows messages (WM_MOUSEMOVE, etc...) and 
			// the corresponding WinForms events from being processed.
			_mouse.BindWindow(this, true);

			// This must be set to true, or the device will not send any data.
			_mouse.IsAcquired = true;

			// Center the mouse on the window.
			_mouse.Position = new Point(panelDisplay.ClientSize.Width / 2, panelDisplay.ClientSize.Height / 2);

			UpdateMouseLabel(_mouse.Position, GorgonMouseButtons.None);			
        }

        /// <summary>
        /// Function to create the keyboard device.
        /// </summary>
        /// <param name="keyboardInfo">The information about the current keyboard to use.</param>
        private void CreateKeyboard(IGorgonKeyboardInfo2 keyboardInfo)
        {
            // Create our device.
            _keyboard = new GorgonKeyboard2(_service, keyboardInfo);
            
            // Set up an event handler for our keyboard.
            _keyboard.KeyDown += _keyboard_KeyDown;
            _keyboard.KeyUp += _keyboard_KeyUp;

			// This will bind the device to this window, allowing us to intercept its data.
			_keyboard.BindWindow(this);

			// This must be set to true, or the device will not send any data.
	        _keyboard.IsAcquired = true;

			UpdateKeyboardLabel(Keys.None, Keys.None);
        }

        /// <summary>
        /// Function to create the joystick device.
        /// </summary>
        private void CreateJoystick(IReadOnlyList<IGorgonJoystickInfo2> joystickDevices)
        {
            // If we have a joystick controller, then let's activate it.
	        if (joystickDevices.Count <= 0)
	        {
		        return;
	        }

	        // Find the first one that's active.
			var activeDevice = (from joystick in joystickDevices
		        where joystick.IsConnected
		        select joystick).FirstOrDefault();

	        if (activeDevice == null)
	        {
		        return;
	        }

	        // Note that joysticks from Raw Input are always exclusive access,
	        // so setting _joystick.Exclusive = true; does nothing.
			// TODO: Needs to be refactored.
#warning Fix this later.
//	        _joystick = _service.CreateJoystick(panelDisplay, null);

	        // Show our joystick information.
	        labelJoystick.Text = string.Empty;
	        panelJoystick.Visible = true;

	        UpdateJoystickLabel(JoystickTransformed);
        }


		/// <summary>
		/// Handles the <see cref="E:Activated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			// If we've lost focus, our devices will become unacquired and will stop registering input.
			// This tell the devices that it is now OK to regain acquisition.
			if (_mouse != null)
			{
				_mouse.IsAcquired = true;
			}

			if (_keyboard != null)
			{
				_keyboard.IsAcquired = true;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Load" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			IGorgonPluginAssemblyCache assemblies = null;
			string pluginPath = Program.PlugInPath;

			base.OnLoad(e);

			try
			{
				// We can't use the raw input under RDP/Terminal services.
				string pluginName;

				if (!SystemInformation.TerminalServerSession)
				{
					pluginName = "Gorgon.Input.GorgonRawPlugIn";
					pluginPath += "Gorgon.Input.Raw.DLL";
				}
				else
				{
					pluginName = "Gorgon.Input.GorgonWinFormsPlugIn";
					pluginPath += "Gorgon.Input.WinForms.DLL";
				}

				// Set our default cursor.
				_currentCursor = Resources.hand_icon;

				// Load our raw input plug-in assembly.
				assemblies = new GorgonPluginAssemblyCache(GorgonApplication.Log);
				if (!assemblies.IsPluginAssembly(pluginPath))
				{
					GorgonDialogs.ErrorBox(this, "The assembly '" + pluginPath + "' is not a valid plugin assembly.");
					GorgonApplication.Quit();
				}
				assemblies.Load(pluginPath);

				// Create our input factory.
				IGorgonPluginService pluginService = new GorgonPluginService(assemblies, GorgonApplication.Log);
				IGorgonInputServiceFactory factory = new GorgonInputServiceFactory2(pluginService, GorgonApplication.Log);
				_service = factory.CreateService(pluginName);

				// Ensure that we have the necessary input devices.
				IReadOnlyList<IGorgonMouseInfo2> mice = _service.EnumerateMice();

				if (mice.Count == 0)
				{
					GorgonDialogs.ErrorBox(this, "There were no mice detected on this computer.  The application requires a mouse.");
					GorgonApplication.Quit();
				}

				IReadOnlyList<IGorgonKeyboardInfo2> keyboards = _service.EnumerateKeyboards();

				if (keyboards.Count == 0)
				{
					GorgonDialogs.ErrorBox(this, "There were no keyboards detected on this computer.  The application requires a keyboard.");
					GorgonApplication.Quit();
				}

				// Get our input devices.				
				CreateMouse(mice[0]);
				CreateKeyboard(keyboards[0]);
				CreateJoystick(new IGorgonJoystickInfo2[0]);

				// When the display area changes size, update the spray effect
				// and limit the mouse.
				panelDisplay.Resize += panelDisplay_Resize;

				// Set the initial range of the mouse cursor.
				_mouse.PositionConstraint = panelDisplay.ClientRectangle;

				// Set up our spray object.
				_spray = new Spray(panelDisplay.ClientSize);
				_cursor = new MouseCursor(panelDisplay)
				          {
					          Hotspot = new Point(-16, -3)
				          };

				// Set up our idle method.
				_updateTimer.Reset();
				GorgonApplication.IdleMethod = Idle;
			}
			catch (Exception ex)
			{
				// We do this here instead of just calling the dialog because this
				// function will send the exception to the Gorgon log file.
				ex.Catch(_ => GorgonDialogs.ErrorBox(this, _), GorgonApplication.Log);
				GorgonApplication.Quit();
			}
			finally
			{
				assemblies?.Dispose();
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.FormClosing" /> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.FormClosingEventArgs" /> that contains the event data.</param>
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			_mouse?.UnbindWindow();
			_keyboard?.UnbindWindow();

			_cursor?.Dispose();

			_spray?.Dispose();
		}
		#endregion

		#region Constructor/Destructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="formMain" /> class.
		/// </summary>
		public formMain()
		{
			InitializeComponent();
		}
		#endregion
	}
}
