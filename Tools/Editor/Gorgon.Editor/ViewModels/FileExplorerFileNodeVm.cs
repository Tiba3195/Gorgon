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
// Created: September 4, 2018 10:48:10 PM
// 
#endregion

using System;
using System.IO;
using System.Threading.Tasks;
using Gorgon.Core;
using Gorgon.Editor.Services;
using System.Threading;
using Gorgon.Editor.Properties;
using Gorgon.Editor.Plugins;
using Gorgon.Editor.Content;
using Gorgon.Editor.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Gorgon.Editor.ViewModels
{
    /// <summary>
    /// A node for a file system file.
    /// </summary>
    internal class FileExplorerFileNodeVm
        : FileExplorerNodeCommon, IContentFile
    {
        #region Variables.
        // Flag to indicate whether the node is open for editing.
        private bool _isOpen;
        // The file system information object.
        private FileInfo _fileInfo;
        #endregion

        #region Events.
        /// <summary>Event triggered if this content file was deleted.</summary>
        public event EventHandler Deleted;

        /// <summary>
        /// Event triggered if this content file was moved in the file system.
        /// </summary>
        public event EventHandler<ContentFileMovedEventArgs> Moved;

        /// <summary>
        /// Event triggered if this content file is excluded from the project.
        /// </summary>
        public event EventHandler Excluded;
        #endregion

        #region Properties.        
        /// <summary>
        /// Property to return whether to allow child node creation for this node.
        /// </summary>
        public override bool AllowChildCreation => false;

        /// <summary>
        /// Property to return the type of node.
        /// </summary>
        public override NodeType NodeType => NodeType.File;

        /// <summary>
        /// Property to return the full path to the node.
        /// </summary>
        public override string FullPath => (Parent == null ? "/" : Parent.FullPath) + Name;

        /// <summary>
        /// Property to return the image name to use for the node type.
        /// </summary>
        public override string ImageName => Metadata?.ContentMetadata == null ? "generic_file_20x20.png" : Metadata.ContentMetadata.SmallIconID.ToString("N");

        /// <summary>
        /// Property to return whether or not the allow this node to be deleted.
        /// </summary>
        public override bool AllowDelete => true;

        /// <summary>Property to return whether this node represents content or not.</summary>
        public override bool IsContent => true;

        /// <summary>Property to return the path to the file.</summary>
        string IContentFile.Path => FullPath;

        /// <summary>Property to return the extension for the file.</summary>
        string IContentFile.Extension => Path.GetExtension(Name);

        /// <summary>Property to return the plugin associated with the file.</summary>
        ContentPlugin IContentFile.ContentPlugin => Metadata?.ContentMetadata as ContentPlugin;

        /// <summary>Property to set or return the metadata for the node.</summary>
        public override ProjectItemMetadata Metadata
        {
            get => base.Metadata;
            set
            {
                if (base.Metadata == value)
                {
                    return;
                }

                base.Metadata = value;

                if (value == null)
                {
                    EventHandler handler = Excluded;
                    handler?.Invoke(this, EventArgs.Empty);
                }

                NotifyPropertyChanged(nameof(ImageName));
            }
        }

        /// <summary>Property to set or return whether the node is open for editing.</summary>
        public override bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen == value)
                {
                    return;
                }

                OnPropertyChanging();
                _isOpen = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Property to return the physical path to the node.</summary>
        public override string PhysicalPath => Parent == null ? null : Path.Combine(Parent.PhysicalPath, Name);
        #endregion

        #region Methods.
        /// <summary>Function called when the parent of this node is moved.</summary>
        /// <param name="newNode">The new node representing this node under the new parent.</param>
        protected override void OnNotifyParentMoved(IFileExplorerNodeVm newNode)
        {
            if ((!(newNode is IContentFile contentFile)) || (!contentFile.IsOpen))
            {
                return;
            }

            var args = new ContentFileMovedEventArgs(contentFile);
            EventHandler<ContentFileMovedEventArgs> handler = Moved;
            Moved?.Invoke(this, args);
        }

        /// <summary>Function to assign the appropriate content plug in to a node.</summary>
        /// <param name="plugins">The plugins.</param>
        /// <param name="deepScan"><b>true</b> to perform a more in depth scan for the associated plug in type, <b>false</b> to use the node metadata exclusively.</param>
        /// <returns><b>true</b> if a plug in was assigned, <b>false</b> if not.</returns>
        /// <remarks>
        /// <para>
        /// If the <paramref name="deepScan" /> parameter is set to <b>true</b>, then the lookup for the plug ins will involve opening the file using each plug in to find a matching plug in for the node
        /// file type. This, obviously, is much slower, so should only be used when the node metadata is not sufficient for association information.
        /// </para>
        /// </remarks>
        protected override bool OnAssignContentPlugin(IContentPluginManagerService plugins, bool deepScan) => plugins.AssignContentPlugin(this, !deepScan);

        /// <summary>Function to retrieve the physical file system object for this node.</summary>
        /// <param name="path">The path to the physical file system object.</param>
        /// <returns>Information about the physical file system object.</returns>
        protected override FileSystemInfo GetFileSystemObject(string path)
        {
            _fileInfo = new FileInfo(path);
            return _fileInfo;
        }

        /// <summary>Function called to refresh the underlying data for the node.</summary>
        public override void Refresh()
        {
            NotifyPropertyChanging(nameof(PhysicalPath));

            try
            {
                _fileInfo.Refresh();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
            finally
            {
                NotifyPropertyChanged(nameof(PhysicalPath));
            }
        }

        /// <summary>
        /// Function to rename the node.
        /// </summary>
        /// <param name="newName">The new name for the node.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="newName"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="newName"/> parameter is empty.</exception>
        public override void RenameNode(string newName)
        {
            if (newName == null)
            {
                throw new ArgumentNullException(nameof(newName));
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentEmptyException(nameof(newName));
            }

            NotifyPropertyChanging(nameof(Name));

            try
            {
                FileSystemService.RenameFile(_fileInfo, newName);
                Refresh();
            }
            finally
            {
                NotifyPropertyChanged(nameof(Name));
            }            
        }

        /// <summary>
        /// Function to delete the node.
        /// </summary>
        /// <param name="onDeleted">[Optional] A function to call when a node or a child node is deleted.</param>
        /// <param name="cancelToken">[Optional] A cancellation token used to cancel the operation.</param>
        /// <returns>A task for asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// The <paramref name="onDeleted"/> parameter is not used for this type.
        /// </para>
        /// <para>
        /// This implmentation does not delete the underlying file outright, it instead moves it into the recycle bin so the user can undo the delete if needed.
        /// </para>
        /// </remarks>
        public override Task DeleteNodeAsync(Action<FileSystemInfo> onDeleted = null, CancellationToken? cancelToken = null)
        {
            try
            {
                // Delete the physical object first. If we fail here, our node will survive.
                FileSystemService.DeleteFile(_fileInfo);
                
                NotifyPropertyChanging(nameof(FullPath));

                // Drop us from the parent list.
                // This will begin a chain reaction that will remove us from the UI.
                Parent.Children.Remove(this);
                Parent = null;
                
                // If this node is open in an editor, then we need to notify the editor that we just deleted the node.
                if (IsOpen)
                {
                    EventHandler deleteEvent = Deleted;
                    Deleted?.Invoke(this, EventArgs.Empty);
                    IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Function to copy the file node into another node.
        /// </summary>
        /// <param name="copyNodeData">The data containing information about what to copy.</param>
        /// <returns>The newly copied node.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="copyNodeData"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentMissingException">Thrown when the the <see cref="CopyNodeData.Destination"/> member of the <paramref name="copyNodeData"/> parameter are <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown if the destination node in <paramref name="copyNodeData"/> is unable to create child nodes.</exception>
        public override async Task<IFileExplorerNodeVm> CopyNodeAsync(CopyNodeData copyNodeData)
        {
            if (copyNodeData == null)
            {
                throw new ArgumentNullException(nameof(copyNodeData));
            }

            if (copyNodeData.Destination == null)
            {
                throw new ArgumentMissingException(nameof(CopyNodeData.Destination), nameof(copyNodeData));
            }

            if (!copyNodeData.Destination.AllowChildCreation)
            {
                throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GOREDIT_ERR_NODE_CANNOT_CREATE_CHILDREN, copyNodeData.Destination.Name));
            }

            FileSystemConflictResolution? conflictResolution = copyNodeData.DefaultResolution;
            string newPath = Path.Combine(copyNodeData.Destination.PhysicalPath, Name);
            bool isConflictDueToOpenFile = false;

            // Renames a node that is in conflict when the file is the same in the source and dest, or if the user chooses to not overwrite.
            string RenameNodeInConflict(string path)
            {
                string newName = FileSystemService.GenerateFileName(path);
                return Path.Combine(copyNodeData.Destination.PhysicalPath, newName);                
            }

            // If we attempt to copy over ourselves, just default to rename.
            if (string.Equals(newPath, _fileInfo.FullName, StringComparison.OrdinalIgnoreCase))
            {
                newPath = RenameNodeInConflict(newPath);
            }

            var destFile = new FileInfo(newPath);
            // A duplicate node that we are conflicting with.
            IFileExplorerNodeVm dupeNode = null;

            // Check for a duplicate file and determine how to proceed.
            if (FileSystemService.FileExists(destFile))
            {
                // If we choose to overwrite.
                dupeNode = copyNodeData.Destination.Children.FirstOrDefault(item => string.Equals(item.Name, Name, StringComparison.OrdinalIgnoreCase));

                // If by some weird circumstance we can't find the node with the same name, then just default to overwrite it as it's not managed by the application, and if it's under 
                // the file system root, it really should be ours.
                if (dupeNode != null)
                {
                    // If we previously gave a conflict resolution, and it indicated all operations should continue then skip the call back since we have our answer.
                    // Otherwise, determine how to resolve the conflict.
                    // If the file is open in the editor then we have no choice but to force the resolver to run because we cannot mess with an open file.
                    isConflictDueToOpenFile = (dupeNode.IsOpen) && (conflictResolution == FileSystemConflictResolution.OverwriteAll);
                    if ((copyNodeData.ConflictHandler != null) && (isConflictDueToOpenFile) 
                        || ((conflictResolution != FileSystemConflictResolution.OverwriteAll) && (conflictResolution != FileSystemConflictResolution.RenameAll)))
                    {                       
                        conflictResolution = copyNodeData.ConflictHandler(this, dupeNode, true, copyNodeData.UseToAllInConflictHandler);
                    }
                    else if (copyNodeData.ConflictHandler == null)
                    {
                        // Default to exception if we have a conflict.
                        conflictResolution = FileSystemConflictResolution.Exception;
                    }
                }
                else
                {
                    conflictResolution = FileSystemConflictResolution.Overwrite;
                }

                switch (conflictResolution)
                {
                    case FileSystemConflictResolution.Rename:
                    case FileSystemConflictResolution.RenameAll:
                        newPath = RenameNodeInConflict(newPath);
                        destFile = new FileInfo(newPath);
                        dupeNode = null;
                        break;
                    case FileSystemConflictResolution.Skip:
                        if (isConflictDueToOpenFile)
                        {
                            // Reset back to overwrite if we hit an open file (just so we don't get the resolver again).
                            copyNodeData.DefaultResolution = FileSystemConflictResolution.OverwriteAll;
                        }
                        else
                        {
                            copyNodeData.DefaultResolution = conflictResolution;
                        }
                        return null;
                    case FileSystemConflictResolution.Cancel:                        
                        copyNodeData.DefaultResolution = conflictResolution;
                        return null;
                    case FileSystemConflictResolution.Exception:
                    case null:
                        throw new IOException(string.Format(Resources.GOREDIT_ERR_NODE_EXISTS, Name));
                }
            }
            
            if ((conflictResolution == FileSystemConflictResolution.RenameAll)
                || (conflictResolution == FileSystemConflictResolution.OverwriteAll))
            {
                copyNodeData.DefaultResolution = conflictResolution;
            }

            // Now that the duplicate check is done, we can actually copy the file.
            // Unlike copying directories, we don't have to worry about digging down through the hierarchy and reporting back.
            // However, if the file is large, it will take time to copy, so we'll send notifcation back to indicate how much data we've copied.
            void ReportProgress(long bytesCopied, long bytesTotal) => copyNodeData.CopyProgress?.Invoke(this, bytesCopied, copyNodeData.TotalSize ?? bytesTotal);

            try
            {
                await Task.Run(() => FileSystemService.CopyFile(_fileInfo, destFile, ReportProgress, copyNodeData.CancelToken));
                destFile.Refresh();
            }
            catch
            {
                destFile.Refresh();
                // Clean up the partially copied file.
                if (destFile.Exists)
                {
                    destFile.Delete();
                }
                throw;
            }

            if (dupeNode == null)
            {
                // Once the file is actually on the file system, make a node and attach it to the parent.            
                return ViewModelFactory.CreateFileExplorerFileNodeVm(Project, FileSystemService, copyNodeData.Destination, destFile, Metadata);
            }

            // If we've overwritten a node, then just refresh its state and return it.  There's no need to re-add it to the list at this point.
            dupeNode.Refresh();
            return dupeNode;
        }

        /// <summary>Function to move this node to another node.</summary>
        /// <param name="destNode">The node that will receive this node as a child.</param>
        /// <returns><b>true</b> if the node was moved, <b>false</b> if it was cancelled or had an error moving.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="destNode" /> parameter is <b>null</b>.</exception>
        /// <exception cref="GorgonException">Thrown if the <paramref name="destNode" /> is unable to create child nodes.</exception>
        public override bool MoveNode(IFileExplorerNodeVm destNode)
        {
            if (destNode == null)
            {
                throw new ArgumentNullException(nameof(destNode));
            }

            if (destNode == null)
            {
                throw new GorgonException(GorgonResult.CannotCreate, string.Format(Resources.GOREDIT_ERR_NODE_CANNOT_CREATE_CHILDREN, destNode.Name));
            }

            if (string.Equals(Parent.FullPath, destNode.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            FileSystemConflictResolution resolution = FileSystemConflictResolution.Exception;
            string newPath = Path.Combine(destNode.PhysicalPath, Name);

#warning Broken.
            if (FileSystemService.FileExists(null))
            {
                //resolution = FileSystemConflictHandler(FullPath, newPath, false);

                switch (resolution)
                {
                    case FileSystemConflictResolution.Rename:
                    case FileSystemConflictResolution.RenameAll:
                    case FileSystemConflictResolution.Cancel:
                        return false;
                    case FileSystemConflictResolution.Exception:
                        throw new IOException(string.Format(Resources.GOREDIT_ERR_NODE_EXISTS, Name));
                }
            }

            FileSystemService.MoveFile(_fileInfo, newPath);

            if (IsOpen)
            {
                var args = new ContentFileMovedEventArgs(this);
                EventHandler<ContentFileMovedEventArgs> handler = Moved;
                Moved?.Invoke(this, args);
            }

            NotifyPropertyChanged(FullPath);
            NotifyPropertyChanged(PhysicalPath);

            return true;
        }

        /// <summary>
        /// Function to export the contents of this node to the physical file system.
        /// </summary>
        /// <param name="destPath">The path to the directory on the physical file system that will receive the contents.</param>
        /// <param name="onCopy">[Optional] The method to call when a file is about to be copied.</param>
        /// <param name="cancelToken">[Optional] A token used to cancel the operation.</param>
        /// <returns>A task for asynchronous operation.</returns>
        /// <remarks>The <paramref name="onCopy" /> callback method sends the file system node being copied, the destination file system node, the current item #, and the total number of items to copy.</remarks>
        public override Task ExportAsync(string destPath, Action<FileSystemInfo, FileSystemInfo, int, int> onCopy = null, CancellationToken? cancelToken = null)
        {
            if (destPath == null)
            {
                throw new ArgumentNullException(nameof(destPath));
            }

            if (string.IsNullOrWhiteSpace(destPath))
            {
                throw new ArgumentEmptyException(nameof(destPath));
            }

            // Progress update
            void ProgressUpdate(FileSystemInfo file) => onCopy?.Invoke(file, null, 1, 1);

            return FileSystemService.ExportFileAsync(PhysicalPath, destPath, ProgressUpdate);
        }

        /// <summary>Function to open the file for reading.</summary>
        /// <returns>A stream containing the file data.</returns>
        Stream IContentFile.OpenRead() => File.OpenRead(PhysicalPath);

        /// <summary>Function to notify that the metadata should be refreshed.</summary>
        void IContentFile.RefreshMetadata() => NotifyPropertyChanged(nameof(Metadata));

        /// <summary>
        /// Function to retrieve the size of the data on the physical file system.
        /// </summary>        
        /// <returns>The size of the data on the physical file system, in bytes.</returns>
        /// <remarks>
        /// <para>
        /// For nodes with children, this will sum up the size of each item in the <see cref="Children"/> list.  For items that do not have children, then only the size of the immediate item is returned.
        /// </para>
        /// </remarks>
        public override long GetSizeInBytes() => _fileInfo.Length;
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerFileNodeVm"/> class.
        /// </summary>
        /// <param name="copy">The node to copy.</param>
        internal FileExplorerFileNodeVm(FileExplorerFileNodeVm copy)
            : base(copy)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExplorerFileNodeVm" /> class.
        /// </summary>
        public FileExplorerFileNodeVm()
        {            
        }
        #endregion
    }
}
