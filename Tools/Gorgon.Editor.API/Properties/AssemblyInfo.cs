﻿#region MIT.
// 
// Gorgon.
// Copyright (C) 2014 Michael Winsor
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
// Created: Friday, May 02, 2014 8:40:13 PM
// 
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !DEBUG && _DEPLOY
[assembly: InternalsVisibleTo("Gorgon.Editor, PublicKey=002400000480000094000000060200000024000052534131000400000100010099ab3e9d6160dd" +
											"629f3d53173ad052f5e0127ab32336d385860fed858d2ef4ae3485813ea60a69cd6b8f67f1c5cc" +
											"82f869cdc37a565216def48b45447fec94533d497ef3f6fd2eddcec7052efbb8ea089b772536eb" +
											"ad3fc884202542e41e3bc9c2a3d05babf0685a54fb3b60f41e3723eae704f794739679e1989bed" +
											"1ebbc2c1")]
#else
[assembly: InternalsVisibleTo("Gorgon.Editor")]
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if DEBUG
[assembly: AssemblyTitle("Gorgon Editor API [DEBUG]")]
[assembly: AssemblyDescription("Common functionality for the editor and plug-ins. [DEBUG]")]
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyTitle("Gorgon Editor API")]
[assembly: AssemblyDescription("Common functionality for the editor and plug-ins.")]
[assembly: AssemblyConfiguration("RELEASE")]
#endif
[assembly: AssemblyCompany("Michael Winsor")]
[assembly: AssemblyProduct("Gorgon.Editor")]
[assembly: AssemblyCopyright("Copyright © Michael Winsor 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("e9e50b7f-823a-4df6-960d-a8d3f8c31f89")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.*")]
[assembly: AssemblyFileVersion("2.0.0.0")]
