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
// Created: Thursday, December 15, 2011 1:24:31 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using D3D = SharpDX.Direct3D11;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.Math;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// Shader state interface.
	/// </summary>
	public abstract class GorgonShaderState<T>
		where T : GorgonShader
	{
		#region Classes.
		/// <summary>
		/// Sampler states.
		/// </summary>
		/// <remarks>This is used to control how textures are used by the shader.</remarks>
		public sealed class TextureSamplerState
			: GorgonState<GorgonTextureSamplerStates>, IList<GorgonTextureSamplerStates>
		{
			#region Variables.
			private readonly GorgonTextureSamplerStates[] _states;								// List of sampler states.
			private readonly D3D.SamplerState[] _D3DStates;										// List of sampler state objects.
			private readonly GorgonShaderState<T> _shader;										// Shader that owns the state.
			#endregion

			#region Properties.
			/// <summary>
			/// Property to set or return the current state.
			/// </summary>
			/// <remarks>This property is not used for texture samplers and will throw an exception if used.</remarks>
			/// <exception cref="System.NotSupportedException">Thrown when this property is accessed because it is not implemented for texture samplers.</exception>
			[EditorBrowsable(EditorBrowsableState.Never)]
			public override GorgonTextureSamplerStates States
			{
				get
				{
					return _states[0];
				}
				set
				{
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Function to retrieve the Direct 3D filter type.
			/// </summary>
			/// <param name="filter">Filter to evaluate.</param>
			/// <returns>The Direct 3D filter type.</returns>
			private static D3D.Filter GetFilter(TextureFilter filter)
			{
				var result = D3D.Filter.MinMagMipPoint;

				if (filter == TextureFilter.Anisotropic)
				{
					result = D3D.Filter.Anisotropic;
				}

				if (filter == TextureFilter.CompareAnisotropic)
				{
					result = D3D.Filter.ComparisonAnisotropic;
				}

				// Sort out filter stateType.
				// Check comparison stateType.
				if ((filter & TextureFilter.Comparison) == TextureFilter.Comparison)
				{
					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.ComparisonMinMagMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.ComparisonMinMagMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.ComparisonMinMagLinearMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.ComparisonMinLinearMagMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.ComparisonMinLinearMagPointMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.ComparisonMinPointMagMipLinear;
					}
					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.ComparisonMinMagPointMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.ComparisonMinPointMagLinearMipPoint;
					}
				}
				else
				{
					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.MinMagMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.MinMagMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.MinMagLinearMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.MinLinearMagMipPoint;
					}

					if (((filter & TextureFilter.MinLinear) == TextureFilter.MinLinear) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.MinLinearMagPointMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.MinPointMagMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagPoint) == TextureFilter.MagPoint) &&
						((filter & TextureFilter.MipLinear) == TextureFilter.MipLinear))
					{
						result = D3D.Filter.MinMagPointMipLinear;
					}

					if (((filter & TextureFilter.MinPoint) == TextureFilter.MinPoint) &&
						((filter & TextureFilter.MagLinear) == TextureFilter.MagLinear) &&
						((filter & TextureFilter.MipPoint) == TextureFilter.MipPoint))
					{
						result = D3D.Filter.MinPointMagLinearMipPoint;
					}
				}

				return result;
			}

			/// <summary>
			/// Applies the state.
			/// </summary>
			/// <param name="stateObject">The state object.</param>
			internal override void ApplyState(D3D.DeviceChild stateObject)
			{				
				// Not used.
			}

			/// <summary>
			/// Function to retrieve the D3D state object.
			/// </summary>
			/// <param name="stateType">The state type information.</param>
			/// <returns>The D3D state object.</returns>
			internal override D3D.DeviceChild GetStateObject(ref GorgonTextureSamplerStates stateType)
			{
#if DEBUG
				// Perform validation of the state type.
				if (stateType.ComparisonFunction == ComparisonOperators.Unknown)
				{
					throw new GorgonException(GorgonResult.CannotBind,
					                          string.Format(Properties.Resources.GORGFX_INVALID_ENUM_VALUE,
					                                        stateType.ComparisonFunction, "ComparisonFunction"));
				}

				if (stateType.DepthAddressing == TextureAddressing.Unknown)
				{
					throw new GorgonException(GorgonResult.CannotBind,
											  string.Format(Properties.Resources.GORGFX_INVALID_ENUM_VALUE,
															stateType.DepthAddressing, "DepthAddressing"));
				}

				if (stateType.HorizontalAddressing == TextureAddressing.Unknown)
				{
					throw new GorgonException(GorgonResult.CannotBind,
											  string.Format(Properties.Resources.GORGFX_INVALID_ENUM_VALUE,
															stateType.HorizontalAddressing, "HorizontalAddressing"));
				}

				if (stateType.VerticalAddressing == TextureAddressing.Unknown)
				{
					throw new GorgonException(GorgonResult.CannotBind,
											  string.Format(Properties.Resources.GORGFX_INVALID_ENUM_VALUE,
															stateType.VerticalAddressing, "VerticalAddressing"));
				}
#endif

				if (stateType.TextureFilter == TextureFilter.None)
				{
					stateType.TextureFilter = TextureFilter.Point;
				}

				var desc = new D3D.SamplerStateDescription
					{
						AddressU = (D3D.TextureAddressMode)stateType.HorizontalAddressing,
						AddressV = (D3D.TextureAddressMode)stateType.VerticalAddressing,
						AddressW = (D3D.TextureAddressMode)stateType.DepthAddressing,
						BorderColor =
							new SharpDX.Color4(stateType.BorderColor.Red, stateType.BorderColor.Green, stateType.BorderColor.Blue,
							                   stateType.BorderColor.Alpha),
						ComparisonFunction = (D3D.Comparison)stateType.ComparisonFunction,
						MaximumAnisotropy = stateType.MaxAnisotropy,
						MaximumLod = stateType.MaxLOD,
						MinimumLod = stateType.MinLOD,
						MipLodBias = stateType.MipLODBias,
						Filter = GetFilter(stateType.TextureFilter)
					};

				var state = new D3D.SamplerState(Graphics.D3DDevice, desc)
					{
						DebugName = "Gorgon Sampler State #" + StateCacheCount
					};

				return state;
			}

			/// <summary>
			/// Function to set a range of states at once.
			/// </summary>
			/// <param name="slot">Starting slot for the states.</param>
			/// <param name="states">States to set.</param>
			/// <remarks>This will bind several texture samplers at the same time.
			/// <para>Passing NULL (Nothing in VB.Net) to <paramref name="states"/> will set all the slots from <paramref name="slot"/> onward to an undefined texture sampler state.</para>
			/// </remarks>
			/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="slot"/> is less than 0, or greater than the available number of resource view slots.</exception>
			public void SetRange(int slot, GorgonTextureSamplerStates[] states)
			{
			    int count = _states.Length - slot;

				GorgonDebug.AssertParamRange(slot, 0, _states.Length, "slot");

                if (states != null)
                {
                    count = states.Length.Min(_states.Length);
                }

				for (int i = 0; i < count; i++)
				{
					int stateIndex = i + slot;
					GorgonTextureSamplerStates state = default(GorgonTextureSamplerStates);

                    if (states != null)
                    {
                        state = states[i];
                    }
					
					if (_states[stateIndex].Equals(ref state))
					{
						continue;
					}

                    // ReSharper disable InconsistentNaming
					D3D.DeviceChild D3DState = GetFromCache(ref state);
                    // ReSharper restore InconsistentNaming

					if (D3DState == null)
					{
						D3DState = GetStateObject(ref state);
						StoreInCache(ref state, D3DState);
					}

					_states[stateIndex] = state;
					_D3DStates[i] = (D3D.SamplerState)D3DState;
				}

				_shader.SetSamplers(slot, count, _D3DStates);
			}
			#endregion

			#region Constructor/Destructor.
			/// <summary>
			/// Initializes a new instance of the <see cref="GorgonBlendRenderState"/> class.
			/// </summary>
			/// <param name="shaderState">Shader that owns the state.</param>
			internal TextureSamplerState(GorgonShaderState<T> shaderState)
				: base(shaderState.Graphics)
			{
				int count = D3D.CommonShaderStage.SamplerSlotCount;

				_shader = shaderState;
				if (Graphics.VideoDevice.SupportedFeatureLevel == DeviceFeatureLevel.SM2_a_b)
				{
					if (shaderState is GorgonVertexShaderState)
						count = 0;
					else
						count = 16;
				}

				_states = new GorgonTextureSamplerStates[count];
				_D3DStates = new D3D.SamplerState[_states.Length];
			}
			#endregion

			#region IList<GorgonTextureSamplerStates> Members
			#region Properties.
			/// <summary>
			/// Gets or sets the element at the specified index.
			/// </summary>
			/// <returns>The element at the specified index.</returns>
			/// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
			public GorgonTextureSamplerStates this[int index]
			{
				get
				{
					return _states[index];
				}
				set
				{
					if (_states[index].Equals(ref value))
					{
						return;
					}

					D3D.DeviceChild state = GetFromCache(ref value);

					if (state == null)
					{
						state = GetStateObject(ref value);
						StoreInCache(ref value, state);
					}

					_states[index] = value;
					_D3DStates[0] = (D3D.SamplerState)state;
					_shader.SetSamplers(index, 1, _D3DStates);
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
			/// </summary>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
			/// <returns>
			/// The index of item if found in the list; otherwise, -1.
			/// </returns>
			public int IndexOf(GorgonTextureSamplerStates item)
			{
				for (int i = 0; i < _states.Length; i++)
				{
					if (_states[i].Equals(ref item))
					{
						return i;
					}
				}

				return -1;
			}

			/// <summary>
			/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
			/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonTextureSamplerStates>.Insert(int index, GorgonTextureSamplerStates item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonTextureSamplerStates>.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}
			#endregion
			#endregion

			#region ICollection<GorgonTextureSamplerStates> Members
			#region Properties.
			/// <summary>
			/// Property to return the number of sampler slots.
			/// </summary>
			public int Count
			{
				get
				{
					return _states.Length;
				}
			}

			/// <summary>
			/// Property to return whether this list is read-only or not.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonTextureSamplerStates>.Add(GorgonTextureSamplerStates item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonTextureSamplerStates>.Clear()
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Function to return whether the specified sampler state is bound.
			/// </summary>
			/// <param name="item">Sampler state to look up.</param>
			/// <returns>TRUE if found, FALSE if not.</returns>
			public bool Contains(GorgonTextureSamplerStates item)
			{
				return IndexOf(item) > -1;
			}

			/// <summary>
			/// Function to copy the list of bound sampler states to an array.
			/// </summary>
			/// <param name="array">The array to copy into.</param>
			/// <param name="arrayIndex">Index of the array to start writing at.</param>
			public void CopyTo(GorgonTextureSamplerStates[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if ((arrayIndex < 0) || (arrayIndex >= array.Length))
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}

				int count = array.Length.Min(_states.Length);

				for (int i = 0; i < count; i++)
				{
					array[i] = _states[i];
				}
			}

			/// <summary>
			/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <returns>
			/// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </returns>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			bool ICollection<GorgonTextureSamplerStates>.Remove(GorgonTextureSamplerStates item)
			{
				throw new NotSupportedException();
			}
			#endregion
			#endregion

			#region IEnumerable<GorgonTextureSamplerStates> Members
			/// <summary>
			/// Function to return an enumerator for the sampler states.
			/// </summary>
			/// <returns>The enumerator for the sampler states.</returns>
			public IEnumerator<GorgonTextureSamplerStates> GetEnumerator()
			{
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var item in _states)
				{
					yield return item;
				}
				// ReSharper restore LoopCanBeConvertedToQuery
			}
			#endregion

			#region IEnumerable Members
			/// <summary>
			/// Gets the enumerator.
			/// </summary>
			/// <returns></returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _states.GetEnumerator();
			}
			#endregion
		}

		/// <summary>
		/// A list of constant buffers.
		/// </summary>
		public sealed class ShaderConstantBuffers
			: IList<GorgonConstantBuffer>
		{
			#region Variables.
			private readonly GorgonConstantBuffer[] _buffers;
			private readonly GorgonShaderState<T> _shader;
			private readonly D3D.Buffer[] _D3DBufferArray;
			#endregion

			#region Properties.
			/// <summary>
			/// Property to return the number of buffers.
			/// </summary>
			public int Count
			{
				get
				{
					return _buffers.Length;
				}
			}

			/// <summary>
			/// Property to set or return a constant buffer at the specified index.
			/// </summary>
			public GorgonConstantBuffer this[int index]
			{
				get
				{
					return _buffers[index];
				}
				set
				{
					if (_buffers[index] == value)
					{
						return;
					}

					_buffers[index] = value;

					_D3DBufferArray[0] = value != null ? value.D3DBuffer : null;

					_shader.SetConstantBuffers(index, 1, _D3DBufferArray);
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Function to unbind a constant buffer.
			/// </summary>
			/// <param name="buffer">Buffer to unbind.</param>
			internal void Unbind(GorgonConstantBuffer buffer)
			{
				for (int i = 0; i < _buffers.Length; i++)
				{
					if (_buffers[i] == buffer)
					{
						_buffers[i] = null;
					}
				}
			}

			/// <summary>
			/// Function to set a range of constant buffers at once.
			/// </summary>
			/// <param name="slot">Starting slot for the buffer.</param>
			/// <param name="buffers">Buffers to set.</param>
			/// <remarks>This will bind several constant buffers at the same time.  A constant buffer must not already be bound to the shader at another index, or an exception will be thrown.
            /// <para>Passing NULL (Nothing in VB.Net) to the <paramref name="buffers"/> parameter will set the bindings to empty (starting at <paramref name="slot"/>).</para>
			/// </remarks>
			/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="slot"/> is less than 0, or greater than the available number of resource view slots.</exception>
			public void SetRange(int slot, GorgonConstantBuffer[] buffers)
			{
			    int count = _buffers.Length - slot;

				GorgonDebug.AssertParamRange(slot, 0, _buffers.Length, "slot");

                if (buffers != null)
                {
                    count = buffers.Length.Min(_buffers.Length);
                }

				for (int i = 0; i < count; i++)
				{
                    GorgonConstantBuffer buffer = null;
					var bufferIndex = i + slot;
					

                    if (buffers != null)
                    {
                        buffer = buffers[i];
                    }

                    if ((buffer != null) && (IndexOf(buffer) != -1))
                    {
                        continue;
                    }

					_buffers[bufferIndex] = buffer;
					_D3DBufferArray[i] = buffer != null ? buffer.D3DBuffer : null;
				}

				_shader.SetConstantBuffers(slot, count, _D3DBufferArray);
			}
			#endregion

			#region Constructor/Destructor.
			/// <summary>
			/// Initializes a new instance of the <see cref="ShaderConstantBuffers"/> class.
			/// </summary>
			/// <param name="shader">Shader stage state.</param>
			internal ShaderConstantBuffers(GorgonShaderState<T> shader)
			{
				_buffers = new GorgonConstantBuffer[D3D.CommonShaderStage.ConstantBufferApiSlotCount];
				_shader = shader;
				_D3DBufferArray = new D3D.Buffer[_buffers.Length];
			}
			#endregion

			#region IList<GorgonConstantBuffer> Members
			/// <summary>
			/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
			/// </summary>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
			/// <returns>
			/// The index of <paramref name="item" /> if found in the list; otherwise, -1.
			/// </returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="item"/> parameter is NULL (Nothing in VB.Net).</exception>
			public int IndexOf(GorgonConstantBuffer item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				
				return Array.IndexOf(_buffers, item);
			}

			/// <summary>
			/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
			/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonConstantBuffer>.Insert(int index, GorgonConstantBuffer item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonConstantBuffer>.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}
			#endregion

			#region ICollection<GorgonConstantBuffer> Members
			#region Properties.
			/// <summary>
			/// Property to return whether this list is read only or not.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonConstantBuffer>.Add(GorgonConstantBuffer item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonConstantBuffer>.Clear()
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
			/// </summary>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <returns>
			/// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
			/// </returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="item" /> parameter is NULL (Nothing in VB.Net).</exception>
			public bool Contains(GorgonConstantBuffer item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}

				return _buffers.Contains(item);
			}

			/// <summary>
			/// Function to copy the contents of this list to an array.
			/// </summary>
			/// <param name="array">Array to copy into.</param>
			/// <param name="arrayIndex">Index in the array to start writing at.</param>
			public void CopyTo(GorgonConstantBuffer[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if ((arrayIndex < 0) || (arrayIndex >= array.Length))
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}

				int count = array.Length.Min(_buffers.Length);

				for (int i = 0; i < count; i++)
				{
					array[i] = _buffers[i];
				}
			}

			/// <summary>
			/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <returns>
			/// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </returns>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			bool ICollection<GorgonConstantBuffer>.Remove(GorgonConstantBuffer item)
			{
				throw new NotSupportedException();
			}
			#endregion
			#endregion

			#region IEnumerable<GorgonConstantBuffer> Members
			/// <summary>
			/// Function to return an enumerator for the list.
			/// </summary>
			/// <returns>The enumerator for the list.</returns>
			public IEnumerator<GorgonConstantBuffer> GetEnumerator()
			{
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var item in _buffers)
				{
					yield return item;
				}
				// ReSharper restore LoopCanBeConvertedToQuery
			}

			#endregion

			#region IEnumerable Members
			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
			/// </returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _buffers.GetEnumerator();
			}
			#endregion
		}			

		/// <summary>
		/// A list of shader resource views.
		/// </summary>
		/// <remarks>A view is a way for a shader to read (or potentially write) a resource.  Views can also be used to cast the data 
        /// in a resource to another type.</remarks>
		public sealed class ShaderResourceViews
			: IList<GorgonShaderView>
		{
			#region Variables.
			private readonly D3D.ShaderResourceView[] _views;			// Shader resource views.
			private readonly GorgonShaderView[] _resources;				// Shader resources.
			private readonly GorgonShaderState<T> _shader;				// Shader that owns this interface.
			#endregion

			#region Methods.
			/// <summary>
			/// Function to unbind a shader resource view.
			/// </summary>
			/// <param name="resourceView">Resource view to unbind.</param>
			internal void Unbind(GorgonShaderView resourceView)
			{
				int index = IndexOf(resourceView);

				if (index == -1)
				{
					return;
				}

				SetView(index, null);
			}

            /// <summary>
            /// Function to unbind a shader resource view.
            /// </summary>
            /// <param name="resource">Resource containing the view to unbind.</param>
            internal void Unbind(GorgonBaseBuffer resource)
            {
				if (resource == null)
				{
					return;
				}

                var views = this.Where(item => item != null && item.Resource == resource);

                foreach (var view in views)
                {
                    Unbind(view);
                }
            }

            /// <summary>
            /// Function to unbind a shader resource view.
            /// </summary>
            /// <param name="resource">Resource containing the view to unbind.</param>
            internal void Unbind(GorgonTexture resource)
            {
                if (resource == null)
                {
                    return;
                }

                var views = this.Where(item => item != null && item.Resource == resource);

                foreach (var view in views)
                {
                    Unbind(view);
                }
            }

			/// <summary>
			/// Function to re-seat a resource view after it's been altered.
			/// </summary>
			/// <param name="resourceView">Resource view to re-seat.</param>
			internal void ReSeat(GorgonShaderView resourceView)
			{
				int index = IndexOf(resourceView);

				if (index == -1)
				{
					return;
				}

				SetView(index, null);
				SetView(index, resourceView);
			}

			/// <summary>
			/// Function to re-seat a resource view after it's been altered.
			/// </summary>
			/// <param name="resource">Resource containing the view to re-seat.</param>
            internal void ReSeat(GorgonBaseBuffer resource)
			{
			    var views = this.Where(item => item != null && item.Resource == resource);

			    foreach (var view in views)
			    {
			        ReSeat(view);
			    }
			}

            /// <summary>
            /// Function to re-seat a resource view after it's been altered.
            /// </summary>
            /// <param name="resource">Resource containing the view to re-seat.</param>
            internal void ReSeat(GorgonTexture resource)
            {
                var views = this.Where(item => item != null && item.Resource == resource);

                foreach (var view in views)
                {
                    ReSeat(view);
                }
            }

			/// <summary>
			/// Function to determine the index of a specific texture resource that's bound to the shader.
			/// </summary>
			/// <param name="texture">Texture to look up.</param>
			/// <returns>The index of the texture if found, or -1 if not.</returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="texture"/> parameter is NULL (Nothing in VB.Net).</exception>
			public int IndexOf(GorgonTexture texture)
			{
				if (texture == null)
				{
					throw new ArgumentNullException("texture");
				}

				for (int i = 0; i < _resources.Length; i++)
				{
					if ((_resources[i] != null) && (_resources[i].Resource == texture))
					{
						return i;
					}
				}

				return -1;
			}

			/// <summary>
			/// Function to determine the index of a specific buffer resource that's bound to the shader.
			/// </summary>
			/// <param name="buffer">Texture to look up.</param>
			/// <returns>The index of the texture if found, or -1 if not.</returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="buffer"/> parameter is NULL (Nothing in VB.Net).</exception>
			public int IndexOf(GorgonBaseBuffer buffer)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}

				for (int i = 0; i < _resources.Length; i++)
				{
					if ((_resources[i] != null) && (_resources[i].Resource == buffer))
					{
						return i;
					}
				}

				return -1;
			}

			/// <summary>
			/// Function to determine if a texture resource has a view bound to this shader stage.
			/// </summary>
			/// <param name="texture">Texture to look up.</param>
			/// <returns>TRUE if found, FALSE if not.</returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="texture"/> parameter is NULL (Nothing in VB.Net).</exception>
			public bool Contains(GorgonTexture texture)
			{
				return IndexOf(texture) > -1;
			}

			/// <summary>
			/// Function to determine if a buffer resource has a view bound to this shader stage.
			/// </summary>
			/// <param name="buffer">Buffer to look up.</param>
			/// <returns>TRUE if found, FALSE if not.</returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="buffer"/> parameter is NULL (Nothing in VB.Net).</exception>
			public bool Contains(GorgonBaseBuffer buffer)
			{
				return IndexOf(buffer) > -1;
			}

			/// <summary>
			/// Function to set a range of resource views at one time.
			/// </summary>
			/// <param name="slot">Resource view slot to start at.</param>
			/// <param name="resourceViews">A list of resource views to set.</param>
			/// <remarks>This will bind several resource views at the same time.  A view must not already be bound to the shader at another index, or an exception will be thrown.
            /// <para>Passing NULL (Nothing in VB.Net) to the <paramref name="resourceViews"/> parameter will set the bindings to empty (starting at <paramref name="slot"/>).</para>
			/// </remarks>
			/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="slot"/> is less than 0, or greater than the available number of resource view slots.</exception>
			public void SetRange(int slot, GorgonShaderView[] resourceViews)
			{
			    int count = _resources.Length - slot;

				GorgonDebug.AssertParamRange(slot, 0, _resources.Length, "slot");

                if (resourceViews != null)
                {
                    count = resourceViews.Length.Min(_resources.Length);
                }

				for (int i = 0; i < count; i++)
				{
					int resourceIndex = i + slot;
					GorgonShaderView view = null;

                    if (resourceViews != null)
                    {
                        view = resourceViews[i];
                    }

                    // We've already bound this view, skip it.
                    if ((view != null) && (IndexOf(view) != -1))
                    {
                        continue;
                    }

					_views[i] = view != null ? view.D3DView : null;
					_resources[resourceIndex] = view;
				}

				_shader.SetResources(slot, count, _views);
			}

            /// <summary>
            /// Function to retrieve a shader view at the specified index.
            /// </summary>
            /// <param name="index">Index of the shader view to retrieve.</param>
            /// <returns>The shader view at the specified index.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is less than 0 or greater than or equal to the number of resource slots.</exception>
            public GorgonShaderView GetView(int index)
            {
                GorgonDebug.AssertParamRange(index, 0, _resources.Length, "index");
                return _resources[index];
            }

            /// <summary>
            /// Function to set a shader view to the specified index.
            /// </summary>
            /// <param name="index">Index of the shader view to apply.</param>
            /// <param name="view">View to apply to the shader.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is less than 0 or greater than or equal to the number of resource slots.</exception>
            /// <exception cref="System.ArgumentException">Thrown when the <paramref name="view"/> parameter is already bound.</exception>
            public void SetView(int index, GorgonShaderView view)
            {
                GorgonDebug.AssertParamRange(index, 0, _resources.Length, "index");

                if (_resources[index] == view)
                {
                    return;
                }

#if DEBUG
                if (view != null)
                {
                    int currentIndex = IndexOf(view);

                    if (currentIndex != -1)
                    {
	                    throw new GorgonException(GorgonResult.CannotBind,
	                                              string.Format(Properties.Resources.GORGFX_VIEW_ALREADY_BOUND,
	                                                            currentIndex));
                    }
                }
#endif

                _resources[index] = view;
	            _views[0] = view == null ? null : view.D3DView;

	            _shader.SetResources(index, 1, _views);
            }

            /// <summary>
            /// Function to return the texture resource assigned to the view at the specified index.
            /// </summary>
            /// <typeparam name="TX">Type of texture.</typeparam>
            /// <param name="index">Index of the texture to look up.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> parameter is outside of the available resource view slots.</exception>
            /// <exception cref="System.InvalidCastException">Thrown when the type of resource at the specified index is not a texture.</exception>
            /// <returns>The texture assigned to the view at the specified index, or NULL if nothing is assigned to the specified index.</returns>
            public TX GetTexture<TX>(int index)
                where TX : GorgonTexture
            {
                GorgonDebug.AssertParamRange(index, 0, _resources.Length, "index");

                var resourceView = _resources[index];
                
#if DEBUG
                if ((resourceView != null) && (resourceView.Resource != null) && (!(resourceView.Resource is TX)))
                {
	                throw new InvalidCastException(string.Format(Properties.Resources.GORGFX_VIEW_RESOURCE_NOT_TYPE, index,
	                                                             typeof(TX).FullName));
                }
#endif

                if (resourceView == null)
                {
                    return null;    
                }

                return (TX)resourceView.Resource;                
            }

            /// <summary>
            /// Function to set a texture resource's default view at the specified index.
            /// </summary>
            /// <typeparam name="TX">Type of texture.</typeparam>
            /// <param name="index">Index of the resource view to use.</param>
            /// <param name="texture">Texture to assign.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> parameter is outside of the available resource view slots.</exception>
			/// <exception cref="GorgonLibrary.GorgonException">Thrown when attempting to bind a texture that has no default view or is a staging resource.</exception>
			public void SetTexture<TX>(int index, TX texture)
                where TX : GorgonTexture
            {
#if DEBUG
				if ((texture != null) && (texture.DefaultShaderView == null))
				{
					throw new GorgonException(GorgonResult.CannotBind, Properties.Resources.GORGFX_VIEW_CANT_BIND_STAGING_NO_VIEW);
				}
#endif

	            SetView(index, texture != null ? texture.DefaultShaderView : null);
            }

			/// <summary>
            /// Function to return the shader buffer resource assigned to the view at the specified index.
            /// </summary>
            /// <typeparam name="TB">Type of shader buffer.</typeparam>
            /// <param name="index">Index of the texture to look up.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> parameter is outside of the available resource view slots.</exception>
            /// <exception cref="System.InvalidCastException">Thrown when the type of resource at the specified index is not a shader buffer.</exception>
            /// <returns>The shader buffer assigned to the view at the specified index, or NULL if nothing is assigned to the specified index.</returns>
            public TB GetShaderBuffer<TB>(int index)
                where TB : GorgonBaseBuffer
            {
                GorgonDebug.AssertParamRange(index, 0, _resources.Length, "index");

                var resourceView = _resources[index];
#if DEBUG
                if ((resourceView != null) && (resourceView.Resource != null) && (!(resourceView.Resource is TB)))
                {
					throw new InvalidCastException(string.Format(Properties.Resources.GORGFX_VIEW_RESOURCE_NOT_TYPE, index,
																 typeof(TB).FullName));
                }
#endif

                if (resourceView == null)
                {
                    return null;    
                }

                return (TB)resourceView.Resource;                
            }

            /// <summary>
            /// Function to set a shader buffer resource's default view at the specified index.
            /// </summary>
            /// <typeparam name="TB">Type of shader buffer.</typeparam>
            /// <param name="index">Index of the resource view to use.</param>
            /// <param name="buffer">Shader buffer to assign.</param>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> parameter is outside of the available resource view slots.</exception>
            /// <exception cref="GorgonLibrary.GorgonException">Thrown when attempting to bind a buffer that has no default view or is a staging resource.</exception>
            public void SetShaderBuffer<TB>(int index, TB buffer)
                where TB : GorgonBaseBuffer
            {
#if DEBUG
				if ((buffer != null) && (buffer.DefaultShaderView == null))
				{
					throw new GorgonException(GorgonResult.CannotBind, Properties.Resources.GORGFX_VIEW_CANT_BIND_STAGING_NO_VIEW);
				}
#endif
	            SetView(index, buffer != null ? buffer.DefaultShaderView : null);
            }
			#endregion

			#region Constructor/Destructor.
			/// <summary>
			/// Initializes a new instance of the <see cref="GorgonShaderState&lt;T&gt;.ShaderResourceViews"/> class.
			/// </summary>
			/// <param name="shader">Shader state that owns this interface.</param>
			internal ShaderResourceViews(GorgonShaderState<T> shader)
			{
				_shader = shader;

				// SM2_a_b devices can't have resources bound to the vertex shader stage.
				if ((_shader is GorgonVertexShaderState) && (_shader.Graphics.VideoDevice.SupportedFeatureLevel == DeviceFeatureLevel.SM2_a_b))
				{
					_views = new D3D.ShaderResourceView[] { };
					_resources = new GorgonShaderView[] { };
				}
				else
				{
					_views = new D3D.ShaderResourceView[D3D.CommonShaderStage.InputResourceSlotCount];
					_resources = new GorgonShaderView[_views.Length];							
				}
			}
			#endregion

			#region IList<GorgonShaderView> Members
			#region Properties.
			/// <summary>
			/// Property to set or return the bound shader resource view.
			/// </summary>
			GorgonShaderView IList<GorgonShaderView>.this[int index]
			{
				get
				{
					return _resources[index];
				}
				set
				{
                    SetView(index, value);
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
			/// </summary>
			/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
			/// <returns>
			/// The index of <paramref name="item" /> if found in the list; otherwise, -1.
			/// </returns>
			/// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="item" /> parameter is NULL (Nothing in VB.Net).</exception>
			public int IndexOf(GorgonShaderView item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}

				return Array.IndexOf(_resources, item);
			}

			/// <summary>
			/// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
			/// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonShaderView>.Insert(int index, GorgonShaderView item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void IList<GorgonShaderView>.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}
			#endregion
			#endregion

			#region ICollection<GorgonShaderView> Members
			#region Properties.
			/// <summary>
			/// Property to return the number of resource view slots.
			/// </summary>
			public int Count
			{
				get
				{
					return _resources.Length;
				}
			}

			/// <summary>
			/// Property to return whether the list is read-only or not.
			/// </summary>
			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}
			#endregion

			#region Methods.
			/// <summary>
			/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonShaderView>.Add(GorgonShaderView item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			void ICollection<GorgonShaderView>.Clear()
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Function to return whether the list contains the specified texture.
			/// </summary>
			/// <param name="item">Texture to find.</param>
			/// <returns>TRUE if found, FALSE if not.</returns>
			public bool Contains(GorgonShaderView item)
			{
				return _resources.Contains(item);
			}

			/// <summary>
			/// Function to copy the resource views to an array.
			/// </summary>
			/// <param name="array">Array to copy into.</param>
			/// <param name="arrayIndex">Index in the array to start writing at.</param>
			public void CopyTo(GorgonShaderView[] array, int arrayIndex)
			{
                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if ((arrayIndex < 0) || (arrayIndex >= array.Length))
                {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

			    int count = array.Length.Min(_resources.Length);

			    for (int i = 0; i < count; i++)
			    {
			        array[i] = _resources[i];
			    }
			}

			/// <summary>
			/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </summary>
			/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
			/// <returns>
			/// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
			/// </returns>
			/// <exception cref="System.NotSupportedException">This method is not used.</exception>
			bool ICollection<GorgonShaderView>.Remove(GorgonShaderView item)
			{
				throw new NotSupportedException();
			}
			#endregion
			#endregion

			#region IEnumerable<GorgonShaderView> Members
			/// <summary>
			/// Function to return an enumerator for the list.
			/// </summary>
			/// <returns>The enumerator for the list.</returns>
			public IEnumerator<GorgonShaderView> GetEnumerator()
			{
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var resource in _resources)
				{
					yield return resource;
				}
				// ReSharper restore LoopCanBeConvertedToQuery
			}
			#endregion

			#region IEnumerable Members
			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
			/// </returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _resources.GetEnumerator();
			}
			#endregion
		}
		#endregion

		#region Variables.
		private T _current;									// Current shader.
		private TextureSamplerState _samplers;				// Sampler states.
		#endregion

		#region Properties.
		/// <summary>
		/// Property to return the graphics interface that owns this object.
		/// </summary>
		protected GorgonGraphics Graphics
		{
			get;
			private set;
		}

		/// <summary>
		/// Property to set or return the current shader.
		/// </summary>
		public virtual T Current
		{
			get
			{
				return _current;
			}
			set
			{
				if (_current == value)
				{
					return;
				}

				_current = value;
				SetCurrent();
			}
		}

		/// <summary>
		/// Property to return the list of constant buffers for the shaders.
		/// </summary>
		public virtual ShaderConstantBuffers ConstantBuffers
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Property to return the sampler states.
		/// </summary>
		/// <remarks>On a SM2_a_b device, and while using a Vertex Shader, setting a sampler will raise an exception.</remarks>
		/// <exception cref="System.InvalidOperationException">Thrown when the current video device is a SM2_a_b device.</exception>
		public virtual TextureSamplerState TextureSamplers
		{
			get
			{
				return _samplers;
			}
		}

		/// <summary>
		/// Property to return the list of resources for the shaders.
		/// </summary>
		/// <remarks>
		/// A resource may be a raw buffer (with shader binding enabled), <see cref="GorgonLibrary.Graphics.GorgonStructuredBuffer">structured buffer</see>, append/consume buffer, or a <see cref="GorgonLibrary.Graphics.GorgonTexture">texture</see>.
		/// <para>On a SM2_a_b device, and while using a Vertex Shader, setting a texture will raise an exception.</para></remarks>
		/// <exception cref="System.InvalidOperationException">Thrown when the current video device is a SM2_a_b device.</exception>
		public virtual ShaderResourceViews Resources
		{
			get;
			private set;
		}
		#endregion

		#region Methods.
		/// <summary>
		/// Function to set the current shader.
		/// </summary>
		protected abstract void SetCurrent();

		/// <summary>
		/// Function to set resources for the shader.
		/// </summary>
		/// <param name="slot">Slot to start at.</param>
		/// <param name="count">Number of resources to update.</param>
		/// <param name="resources">Resources to update.</param>
		protected abstract void SetResources(int slot, int count, D3D.ShaderResourceView[] resources);

		/// <summary>
		/// Function to set the texture samplers for a shader.
		/// </summary>
		/// <param name="slot">Slot to start at.</param>
		/// <param name="count">Number of samplers to update.</param>
		/// <param name="samplers">Samplers to update.</param>
		protected abstract void SetSamplers(int slot, int count, D3D.SamplerState[] samplers);		

		/// <summary>
		/// Function to set constant buffers for the shader.
		/// </summary>
		/// <param name="slot">Slot to start at.</param>
		/// <param name="count">Number of constant buffers to update.</param>
		/// <param name="buffers">Constant buffers to update.</param>
		protected abstract void SetConstantBuffers(int slot, int count, D3D.Buffer[] buffers);

		/// <summary>
		/// Function to clean up.
		/// </summary>
		internal virtual void CleanUp()
		{
			if (_samplers != null)
			{
				_samplers.CleanUp();
			}

			_samplers = null;
		}
		#endregion

		#region Constructor.
		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonShaderState&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="graphics">The graphics interface that owns this object.</param>
		protected GorgonShaderState(GorgonGraphics graphics)
		{
			Graphics = graphics;			
			ConstantBuffers = new ShaderConstantBuffers(this);
			_samplers = new TextureSamplerState(this);
			Resources = new ShaderResourceViews(this);
		}
		#endregion
	}
}
