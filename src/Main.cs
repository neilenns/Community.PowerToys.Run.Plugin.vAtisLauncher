// <copyright file="Main.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Community.PowerToys.Run.Plugin.vAtisLauncher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using ManagedCommon;
    using Wox.Plugin;
    using Wox.Plugin.Logger;

    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public partial class Main : IPlugin, IContextMenu, IDisposable
    {
        /// <summary>
        /// Gets the ID of the plugin.
        /// </summary>
        public static string PluginID => "B3D2951C87324B17810F3510CF49AF82";

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name => "vATIS launcher";

        /// <summary>
        /// Gets the description of the plugin.
        /// </summary>
        public string Description => "Launches vATIS with the selected profile.";

        private PluginInitContext Context { get; set; }

        private string IconPath { get; set; }

        private bool Disposed { get; set; }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            var results = ProfileViewModel.GetMatchingProfiles(query.Search);

            return results;
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.Context.API.ThemeChanged += this.OnThemeChanged;
            this.UpdateIconPath(this.Context.API.GetCurrentTheme());
            ProfileViewModel.Initialize(this.IconPath);
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            return [];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.Disposed || !disposing)
            {
                return;
            }

            if (this.Context?.API != null)
            {
                this.Context.API.ThemeChanged -= this.OnThemeChanged;
            }

            this.Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => this.IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/community.powertoys.run.plugin.vAtisLauncher.light.png" : "Images/community.powertoys.run.plugin.vAtisLauncher.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => this.UpdateIconPath(newTheme);
    }
}
