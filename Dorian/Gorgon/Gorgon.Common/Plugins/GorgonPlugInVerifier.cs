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
// Created: Saturday, January 5, 2013 1:40:39 PM
// 
#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GorgonLibrary.PlugIns
{
	/// <summary>
	/// Plug-in verification object.
	/// </summary>
	class GorgonPlugInVerifier
		: MarshalByRefObject
	{
		#region Variables.
	    private Assembly _assembly;             // The assembly for this type.
		private Type _plugInType;				// Type info for the Gorgon Plug-In type.
		#endregion

		#region Methods.
		/// <summary>
		/// Function to retrieve the Gorgon plug-in type.
		/// </summary>
		/// <remarks>This loads the base Gorgon assembly as a reflection only library.  This is done to extract specific reflection only 
		/// information for type comparisons later on.</remarks>
		private void GetGorgonPlugInType()
		{
			if (_plugInType == null)
			{
				AssemblyName gorgonAssemblyName = typeof(GorgonPlugInVerifier).Assembly.GetName();
				Assembly gorgonReflection = Assembly.ReflectionOnlyLoad(gorgonAssemblyName.FullName);

				// Get the Gorgon reflection only plug-in type.
				_plugInType = gorgonReflection.GetTypes().Single(item => typeof(GorgonPlugIn).FullName == item.FullName);

			    _assembly = _plugInType.Assembly;
			}
		}

        /// <summary>
        /// Function to retrieve the assembly from the same directory as the requesting assembly.
        /// </summary>
        /// <param name="name">Name of the assembly.</param>
        /// <param name="requesting">Requesting assembly.</param>
        /// <returns>The assembly if found, NULL if not.</returns>
        private Assembly GetFromRequestedDir(AssemblyName name, Assembly requesting)
        {
            string location = string.Format("{0}{1}{2}", 
                                    Path.GetDirectoryName(requesting.Location), 
                                    Path.DirectorySeparatorChar,
                                    name.Name);

            // Search for DLLs.
            location = Path.ChangeExtension(location, "dll");

            if (File.Exists(location))
            {
                return Assembly.ReflectionOnlyLoadFrom(location);
            }

            // Search for executables.
            location = Path.ChangeExtension(location, "exe");

            if (File.Exists(location))
            {
                return Assembly.ReflectionOnlyLoadFrom(location);
            }

            return null;
        }

        /// <summary>
        /// Function to retrieve the assembly from the same directory as assembly that holds this type.
        /// </summary>
        /// <param name="name">Name of the assembly.</param>
        /// <returns>The assembly if found, NULL if not.</returns>
        private Assembly GetFromLocalDir(AssemblyName name)
        {
            string location = string.Format("{0}{1}{2}",
                                    Path.GetDirectoryName(_assembly.Location),
                                    Path.DirectorySeparatorChar,
                                    name.Name);

            // Search for DLLs.
            location = Path.ChangeExtension(location, "dll");

            if (File.Exists(location))
            {
                return Assembly.ReflectionOnlyLoadFrom(location);
            }

            // Search for executables.
            location = Path.ChangeExtension(location, "exe");

            if (File.Exists(location))
            {
                return Assembly.ReflectionOnlyLoadFrom(location);
            }

            return null;
        }
        
        /// <summary>
	    /// Handles the ReflectionOnlyAssemblyResolve event of the CurrentDomain control.
	    /// </summary>
	    /// <param name="sender">The source of the event.</param>
	    /// <param name="args">The <see cref="ResolveEventArgs" /> instance containing the event data.</param>
	    /// <returns></returns>
	    private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
	    {
            AssemblyName name = new AssemblyName(args.Name);
            Assembly result = null;

            // Try from the GAC first.
            try
            {
                result = Assembly.ReflectionOnlyLoad(args.Name);

                return result;
            }
            catch (FileNotFoundException)
            {
                // Eat this exception.
            }

            result = GetFromRequestedDir(name, args.RequestingAssembly) ?? GetFromLocalDir(name);

            // We couldn't find the assembly in the requesting assembly directory, move on the to current.
            return result;
		}

		/// <summary>
		/// Function to load plug-in type names from the specified assembly file.
		/// </summary>
		/// <param name="plugInPath">Path to the assembly file.</param>
		/// <returns>A list of plug-in type names.</returns>
		public string[] GetPlugInTypes(string plugInPath)
		{
			string[] types = { };

			try
			{
				GetGorgonPlugInType();

				var assembly = Assembly.ReflectionOnlyLoadFrom(plugInPath);

				types = assembly.GetTypes()
								.Where(item => item.IsSubclassOf(_plugInType) && !item.IsAbstract)
								.Select(item => item.FullName)
								.ToArray();
			}
			catch (ReflectionTypeLoadException)
			{
				// Do nothing here.
			}

			return types;
		}

		/// <summary>
		/// Function to determine if an assembly contains plug-ins.
		/// </summary>
		/// <param name="name">Name of the assembly to check.</param>
		/// <returns>TRUE if plug-ins are found, FALSE if not.</returns>
		public bool IsPlugInAssembly(AssemblyName name)
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;

			try
			{
				GetGorgonPlugInType();

				var assembly = Assembly.ReflectionOnlyLoadFrom(name.CodeBase);
				var types = assembly.GetTypes();

				return types.Any(item => item.IsSubclassOf(_plugInType) && !item.IsAbstract);
			}
			catch(ReflectionTypeLoadException)
			{
				return false;
			}
			finally
			{
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomain_ReflectionOnlyAssemblyResolve;
			}
		}
		#endregion
	}
}
