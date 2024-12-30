// <copyright file="ProfileViewModel.cs" company="Neil Enns">
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
    using System.Text.Json;
    using Microsoft.Win32;
    using Wox.Plugin;
    using Wox.Plugin.Logger;

    /// <summary>
    /// Maintains a list of vAtis profiles.
    /// </summary>
    public static class ProfileViewModel
    {
        private static readonly string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "current", "vATIS.exe");

        /// <summary>
        /// Gets the list of profiles.
        /// </summary>
        public static IList<Profile> Profiles { get; } = [];

        /// <summary>
        /// Gets the icon path.
        /// </summary>
        public static string IconPath { get; private set; }

        /// <summary>
        /// Initializes the ProfileViewModel with the specified icon path.
        /// </summary>
        /// <param name="iconPath">The path to the icon.</param>
        public static void Initialize(string iconPath)
        {
            IconPath = iconPath;
        }

        /// <summary>
        /// Returns a list of profiles that match the specified query.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <returns>The list of matching profiles. If query is null or empty all profiles are returned. If no profiles match an empty list is returned.</returns>
        public static List<Result> GetMatchingProfiles(string query)
        {
            IEnumerable<Profile> results;

            // Issue 9: If vATIS isn't installed then the only result should ever be
            // a warning that launches the website to download it.
            if (!File.Exists(AppPath))
            {
                return [new Result
                    {
                        QueryTextDisplay = string.Empty,
                        IcoPath = IconPath,
                        Title = "Download vATIS to use this plugin",
                        SubTitle = "vATIS is not installed",
                        Action = _ =>
                        {
                            Log.Info("Launching https://vatis.app", typeof(ProfileViewModel));
                            LaunchUrl("https://vatis.app");
                            return true;
                        },
                    }
                ];
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                // Always reload profiles on first query
                ProfileViewModel.LoadProfiles();
                results = Profiles;
            }
            else
            {
                results = Profiles.Where(profile =>
                    profile.Name != null && profile.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return [.. results.Select(profile => new Result
            {
                QueryTextDisplay = profile.Name,
                IcoPath = IconPath,
                Title = profile.Name,
                SubTitle = profile.StationIdentifiers,
                Action = _ =>
                {
                    Launch(profile.Id);
                    Log.Info($"Selected {profile.Name} {profile.Id}", typeof(ProfileViewModel));
                    return true;
                },
            })
            .OrderBy(profile => profile.Title)];
        }

        private static void LaunchUrl(string url)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = url,
                    UseShellExecute = true,
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open URL: {ex.Message}", typeof(ProfileViewModel));
            }
        }

        private static void Launch(string id)
        {
            var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "current", "vATIS.exe");
            var argument = $"--profile {id}";

            Log.Info($"Launching {AppPath} {argument}", typeof(Main));
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = argument,
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to launch vATIS: {ex.Message}", typeof(Main));
            }
        }

        /// <summary>
        /// Loads the profiles from disk.
        /// </summary>
        private static void LoadProfiles()
        {
            var folderPath = ProfileViewModel.GetProfilePath();
            Profiles.Clear();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Log.Error("Profile folder not found.", typeof(ProfileViewModel));
                return;
            }

            var jsonFiles = Directory.GetFiles(folderPath, "*.json");

            foreach (var file in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    Profile profile = JsonSerializer.Deserialize<Profile>(jsonContent, options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    if (profile != null)
                    {
                        Log.Info($"Loaded profile {profile.Name} from {file}", typeof(ProfileViewModel));
                        profile.FilePath = file;
                        Profiles.Add(profile);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error loading profiles: {ex.Message}", typeof(ProfileViewModel));
                }
            }

            Log.Info($"Loaded {Profiles.Count} profiles.", typeof(ProfileViewModel));
        }

        /// <summary>
        /// Looks up the path to the vAtis profiles from the registry.
        /// </summary>
        /// <returns>The path to the profiles.</returns>
        private static string GetProfilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "Profiles");
        }
    }
}
