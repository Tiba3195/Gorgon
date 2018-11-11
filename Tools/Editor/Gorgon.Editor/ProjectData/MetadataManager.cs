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
// Created: September 15, 2018 9:19:09 PM
// 
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gorgon.Core;
using Gorgon.Editor.Metadata;
using Gorgon.Editor.Plugins;
using Gorgon.Editor.Properties;
using Gorgon.Editor.Services;

namespace Gorgon.Editor.ProjectData
{
    /// <summary>
    /// Manages the metadata for a project.
    /// </summary>
    internal class OBSOLETE_MetadataManager
        : IMetadataManager
    {
        #region Variables.
        // The project that contains the metadata to manage.
        private readonly IProject _project;
        // The provider to use to persist metadata information.
        private readonly IMetadataProvider _metadataProvider;
        // The service for the content plugins.
        private readonly IContentPluginService _contentPlugins;
        #endregion

        #region Properties.

        #endregion

        #region Methods.
        /// <summary>
        /// Function to load the metadata for a project from metadata store.
        /// </summary>
        public void Load()
        {
            _project.Metadata.IncludedPaths.Clear();

            if (!_metadataProvider.MetadataFile.Exists)
            {
                return;
            }

            IList<ProjectItemMetadata> paths = _metadataProvider.GetIncludedPaths();

            foreach (ProjectItemMetadata path in paths)
            {
                _project.Metadata.IncludedPaths[path.Path] = path;
            }
        }

        /// <summary>
        /// Function to save the metadata to the metadata store in the project.
        /// </summary>
        /// <param name="title">The title of the project.</param>
        /// <param name="writerType">The type of writer used to write the project data.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="title"/>, or the <paramref name="writerType"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="title"/>, or the <paramref name="writerType"/> parameter is empty.</exception>
        public void Save(string title, string writerType)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (writerType == null)
            {
                throw new ArgumentNullException(nameof(writerType));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentEmptyException(nameof(title));
            }

            if (string.IsNullOrWhiteSpace(writerType))
            {
                throw new ArgumentEmptyException(nameof(writerType));
            }

            _metadataProvider.UpdateProjectData(title, writerType, _project);
        }

        /// <summary>
        /// Function to rename items in meta data.
        /// </summary>
        /// <param name="oldPath">The old path for the items.</param>
        /// <param name="newPath">The new path for the items.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="newPath"/>, or the <paramref name="oldPath"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="oldPath"/>, or the <paramref name="newPath"/> parameter is empty.</exception>
        public void RenameIncludedPaths(string oldPath, string newPath)
        {
            if (oldPath == null)
            {
                throw new ArgumentNullException(nameof(oldPath));
            }

            if (newPath == null)
            {
                throw new ArgumentNullException(nameof(newPath));
            }

            if (string.IsNullOrWhiteSpace(oldPath))
            {
                throw new ArgumentEmptyException(nameof(oldPath));
            }

            if (string.IsNullOrWhiteSpace(newPath))
            {
                throw new ArgumentEmptyException(nameof(newPath));
            }

            ProjectItemMetadata[] oldIncluded = _project.Metadata.IncludedPaths.Where(item => item.Path.StartsWith(oldPath, StringComparison.OrdinalIgnoreCase))
                                                        .ToArray();

            if (oldIncluded.Length == 0)
            {
                return;
            }

            for (int i = 0; i < oldIncluded.Length; ++i)
            {
                ProjectItemMetadata metadata = oldIncluded[i];

                string oldPathNoRoot = metadata.Path.Substring(oldPath.Length);
                string newPathRoot = newPath + oldPathNoRoot;

                _project.Metadata.IncludedPaths.Remove(metadata);
                _project.Metadata.IncludedPaths.Add(new ProjectItemMetadata(metadata, newPathRoot));
            }
        }

        /// <summary>
        /// Function to exclude the specified items from the project metadata.
        /// </summary>
        /// <param name="items">The list of project items to exclude.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="items"/> parameter is <b>null</b>.</exception>
        public void ExcludeItems(IEnumerable<ProjectItemMetadata> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (ProjectItemMetadata item in items)
            {
                if (!_project.Metadata.IncludedPaths.TryGetValue(item.Path, out ProjectItemMetadata metadata))
                {
                    continue;
                }

                _project.Metadata.IncludedPaths.Remove(metadata.Path);
            }
        }

        /// <summary>
        /// Function to include a single project item.
        /// </summary>
        /// <param name="item">The item to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="item"/> parameter is <b>null</b>.</exception>
        public void IncludeItem(ProjectItemMetadata item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _project.Metadata.IncludedPaths[item.Path] = item;
        }

        /// <summary>
        /// Function to include a list of project items.
        /// </summary>
        /// <param name="items">The items to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="item"/> parameter is <b>null</b>.</exception>
        public void IncludeItems(IEnumerable<ProjectItemMetadata> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (ProjectItemMetadata item in items)
            {
                _project.Metadata.IncludedPaths[item.Path] = item;
            }
        }

        /// <summary>
        /// Function to include the specified paths in the project metadata.
        /// </summary>
        /// <param name="paths">The list of paths to evaluate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="paths"/> parameter is <b>null</b>.</exception>
        public void IncludePaths(IReadOnlyList<string> paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            for (int i = 0; i < paths.Count; ++i)
            {
                string path = paths[i];

                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                _project.Metadata.IncludedPaths[path] = new ProjectItemMetadata(path);
            }
        }

        /// <summary>
        /// Function to remove the specified path from the included file/directory metadata.
        /// </summary>
        /// <param name="path">The path to evaluate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="path"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="path"/> parameter is empty.</exception>
        public void DeleteFromIncludeMetadata(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentEmptyException(nameof(path));
            }

            // Update metadata.
            ProjectItemMetadata[] included = _project.Metadata.IncludedPaths
                                                                        .Where(item => item.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                                                                        .ToArray();

            for (int i = 0; i < included.Length; ++i)
            {
                _project.Metadata.IncludedPaths.Remove(included[i]);
            }
        }

        /// <summary>
        /// Function to determine if the path is in the project or not.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><b>true</b> if the path is included in the project, <b>false</b> if not.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="path"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="path"/> parameter is empty.</exception>
        public bool PathInProject(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentEmptyException(nameof(path));
            }

            return _project.Metadata.IncludedPaths.Contains(path);
        }

        /// <summary>
        /// Function to retrieve all directories included in the project under the root path.
        /// </summary>
        /// <param name="rootPath">The root path to evaluate.</param>
        /// <returns>A list of included directories.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="rootPath"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="rootPath"/> parameter is empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory specified by <paramref name="rootPath"/> does not exist.</exception>
        public IEnumerable<DirectoryInfo> GetIncludedDirectories(string rootPath)
        {
            if (rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentEmptyException(nameof(rootPath));
            }

            var root = new DirectoryInfo(rootPath);

            if (!root.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            return root.GetDirectories("*", SearchOption.TopDirectoryOnly)
                       .Where(item => ((item.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                                    && ((_project.ShowExternalItems) || (PathInProject(item.ToFileSystemPath(_project.ProjectWorkSpace)))));
        }

        /// <summary>
        /// Function to retrieve all file included in the project under the root path.
        /// </summary>
        /// <param name="rootPath">The root path to evaluate.</param>
        /// <returns>A list of included files.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="rootPath"/> parameter is <b>null</b>.</exception>
        /// <exception cref="ArgumentEmptyException">Thrown when the <paramref name="rootPath"/> parameter is empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory specified by <paramref name="rootPath"/> does not exist.</exception>
        public IEnumerable<FileInfo> GetIncludedFiles(string rootPath)
        {
            if (rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentEmptyException(nameof(rootPath));
            }

            var root = new DirectoryInfo(rootPath);

            if (!root.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            return root.GetFiles("*", SearchOption.TopDirectoryOnly)
                       .Where(item => ((item.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                                    && ((_project.ShowExternalItems) || (PathInProject(item.ToFileSystemPath(_project.ProjectWorkSpace))))
                                    && (!string.Equals(item.Name, CommonEditorConstants.EditorMetadataFileName, StringComparison.OrdinalIgnoreCase)));

        }

        /// <summary>
        /// Function to retrieve the plug in for a specified path.
        /// </summary>
        /// <param name="path">The path to evaluate.</param>
        /// <returns>A tuple containing the plugin (if available), and a <see cref="MetadataPluginState"/>.</returns>
        public (ContentPlugin plugin, MetadataPluginState state) GetPlugin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(string.Format(Resources.GOREDIT_ERR_PATH_NOT_INCLUDED, "NULL"), nameof(path));
            }

            if (!_project.Metadata.IncludedPaths.TryGetValue(path, out ProjectItemMetadata metaData))
            {
                throw new ArgumentException(string.Format(Resources.GOREDIT_ERR_PATH_NOT_INCLUDED, path), nameof(path));
            }

            // No plugin was found, and was never assigned.
            if (metaData.PluginName == null)
            {
                return (null, MetadataPluginState.Unassigned);
            }

            if (string.IsNullOrWhiteSpace(metaData.PluginName))
            {
                return (null, MetadataPluginState.Assigned);
            }

            if (!_contentPlugins.Plugins.TryGetValue(metaData.PluginName, out ContentPlugin plugin))
            {
                return (null, MetadataPluginState.NotFound);
            }

            return (plugin, MetadataPluginState.Assigned);
        }

        /// <summary>
        /// Function to assign a plugin to a path.
        /// </summary>
        /// <param name="path">The path to evaluate.</param>
        /// <param name="plugin">The plugin to assign.</param>
        public void SetPlugin(string path, ContentPlugin plugin)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!_project.Metadata.IncludedPaths.TryGetValue(path, out ProjectItemMetadata metaData))
            {
                throw new ArgumentException(string.Format(Resources.GOREDIT_ERR_PATH_NOT_INCLUDED, path), nameof(path));
            }

            metaData.PluginName = plugin?.Name ?? string.Empty;
        }

        /// <summary>
        /// Function to retrieve the metadata for the item specified by the path.
        /// </summary>
        /// <param name="path">The path to the project item.</param>
        /// <returns>The metadata for the project item, or <b>null</b> if no metadata exists for the project item.</returns>
        public ProjectItemMetadata GetMetadata(string path) => !_project.Metadata.IncludedPaths.TryGetValue(path, out ProjectItemMetadata metaData) ? null : metaData;
        #endregion

        #region Constructor/Finalizer.
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataManager"/> class.
        /// </summary>
        /// <param name="project">The project to manage metadata for.</param>
        /// <param name="contentPlugins">The content plugins.</param>
        /// <param name="provider">The provider used to store or retrieve metadata.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="project"/>, or the <paramref name="provider"/> parameter is <b>null</b>.</exception>        
        public MetadataManager(IProject project, IContentPluginService contentPlugins, IMetadataProvider provider)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _contentPlugins = contentPlugins ?? throw new ArgumentNullException(nameof(contentPlugins));
            _metadataProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        #endregion
    }
}