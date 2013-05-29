using System;

namespace GorgonLibrary.Graphics
{
	/// <summary>
	/// Settings for the index buffer.
	/// </summary>
	public class GorgonIndexBufferSettings
		: IBufferSettings2
	{
		/// <summary>
		/// Property to set or return the usage for the buffer.
		/// </summary>
		/// <para>The default value is Default.</para>
		public BufferUsage Usage
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return whether to allow this buffer to be used for stream output.
		/// </summary>
		/// <remarks>
		/// The default value is FALSE.
		/// </remarks>
		public bool IsOutput
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return the size of the buffer, in bytes.
		/// </summary>
		/// <remarks>
		/// This value must be a multiple of 16.
		/// </remarks>
		public int SizeInBytes
		{
			get;
			set;
		}

		/// <summary>
		/// Property to set or return whether the indices are 32 bits in size or not.
		/// </summary>
		/// <remarks>Pass TRUE to use 32 bit indices, FALSE to use 16 bit indices.
		/// <para>The default value is TRUE.</para>
		/// </remarks>
		public bool Use32BitIndices
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GorgonIndexBufferSettings"/> class.
		/// </summary>
		public GorgonIndexBufferSettings()
		{
			SizeInBytes = 0;
			IsOutput = false;
			Use32BitIndices = true;
			Usage = BufferUsage.Default;
		    AllowUnorderedAccessViews = false;
		    AllowShaderViews = false;
		    AllowRenderTarget = false;
		    AllowRawViews = false;
		    AllowIndirectArguments = false;
            DefaultShaderViewFormat = BufferFormat.Unknown;
		}

        /// <summary>
        /// Property to set or return whether to allow unordered access to the buffer.
        /// </summary>
        /// <remarks>This value must be set to FALSE if <see cref="IsOutput" /> is set to TRUE.
        /// <para>The default value is FALSE.</para>
        /// </remarks>
        public bool AllowUnorderedAccessViews
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return whether to allow shader resource views for this buffer.
        /// </summary>
        /// <remarks>The default value is FALSE.
        /// </remarks>
        public bool AllowShaderViews
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return whether to allow this buffer to be used as a render target.
        /// </summary>
        /// <remarks>
        /// This will allow the buffer to hold render target data.  As of right now, Gorgon does not have a way to bind a render target to the buffer objects.
        /// <para>This value does not apply to constant or structured buffers.</para>
        /// <para>The default value is FALSE.</para>
        /// </remarks>
        public bool AllowRenderTarget
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return the format for the default shader view.
        /// </summary>
        /// <remarks>
        /// Setting this value to any other value than Unknown will create a default shader view for the buffer that will encompass the entire buffer with the specified format.
        /// <para>This value does not apply to constant or structured buffers.</para>
        /// <para>The default value is Unknown.</para>
        /// </remarks>
        public BufferFormat DefaultShaderViewFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return whether a buffer will allow raw views.
        /// </summary>
        /// <remarks>
        /// This value must be set to FALSE if <see cref="AllowShaderViews" /> or <see cref="AllowUnorderedAccessViews" /> is set to FALSE.
        /// <para>The default value is FALSE.</para>
        /// </remarks>
        public bool AllowRawViews
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return whether the buffer will be used as an indirect argument buffer.
        /// </summary>
        /// <remarks>
        /// This value does not apply to structured buffers or constant buffers.
        /// <para>The default value is FALSE.</para>
        /// </remarks>
        public bool AllowIndirectArguments
        {
            get;
            set;
        }

        /// <summary>
        /// Property to set or return the size, in bytes, of an individual item in a structured buffer.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when an attempt to set a value to this property was made.</exception>
        /// <remarks>This value is only applicable to a structured buffer and will always return 0.</remarks>
        int IBufferSettings2.StructureSize
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}