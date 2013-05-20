﻿namespace GorgonLibrary.Graphics
{
    /// <summary>
    /// A shader view for raw buffers.
    /// </summary>
    /// <remarks>Use a resource view to allow a shader access to the contents of a resource (or sub resource).  When the resource is created with a typeless format, this will allow 
    /// the resource to be cast to any format within the same group.</remarks>
    public class GorgonRawBufferShaderView
        : GorgonBufferShaderView
    {
        #region Methods.
        /// <summary>
        /// Function to perform initialization of the shader view resource.
        /// </summary>
        protected override void InitializeImpl()
        {
            var desc = new SharpDX.Direct3D11.ShaderResourceViewDescription
                {
                    BufferEx = new SharpDX.Direct3D11.ShaderResourceViewDescription.ExtendedBufferResource
                        {
                            FirstElement = ElementStart,
                            ElementCount = ElementCount,
                            Flags = SharpDX.Direct3D11.ShaderResourceViewExtendedBufferFlags.Raw
                        },
                    Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.ExtendedBuffer,
                    Format = (SharpDX.DXGI.Format)Format
                };

            D3DView = new SharpDX.Direct3D11.ShaderResourceView(Resource.Graphics.D3DDevice, Resource.D3DResource, desc)
                {
                    DebugName = "Gorgon Shader View for " + Resource.GetType().FullName
                };
        }
        #endregion

        #region Constructor/Destructor.
        /// <summary>
        /// Initializes a new instance of the <see cref="GorgonBufferShaderView"/> class.
        /// </summary>
        /// <param name="buffer">The buffer to bind to the view.</param>
        /// <param name="format">Format of the view.</param>
        /// <param name="elementStart">The starting element for the view.</param>
        /// <param name="elementCount">The number of elements in the view.</param>
        internal GorgonRawBufferShaderView(GorgonResource buffer, BufferFormat format, int elementStart, int elementCount)
            : base(buffer, format, elementStart, elementCount)
        {
        }
        #endregion
    }
}