// <copyright file="ProfileManager.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace Community.PowerToys.Run.Plugin.vAtisLauncher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.Win32;
    using Wox.Plugin.Logger;

    /// <summary>
    /// Maintains a list of vAtis profiles.
    /// </summary>
    public static class ProfileManager
    {
        /// <summary>
        /// Gets the list of profiles.
        /// </summary>
        public static IList<Profile> Profiles { get; } = [];

        /// <summary>
        /// Returns a list of profiles that match the specified query.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <returns>The list of matching profiles. If query is null or empty all profiles are returned. If no profiles match an empty list is returned.</returns>
        public static IEnumerable<Profile> GetMatchingProfiles(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || Profiles.Count == 0)
            {
                return Profiles;
            }

            return Profiles.Where(profile =>
                profile.Name != null && profile.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Loads the profiles from disk.
        /// </summary>
        public static void LoadProfiles()
        {
            var folderPath = ProfileManager.GetProfilePath();
            Profiles.Clear();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                Log.Error("Profile folder not found.", typeof(ProfileManager));
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
                        Log.Info($"Loaded profile {profile.Name} from {file}", typeof(ProfileManager));
                        profile.FilePath = file;
                        Profiles.Add(profile);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error loading profiles: {ex.Message}", typeof(ProfileManager));
                }
            }

            Log.Info($"Loaded {Profiles.Count} profiles.", typeof(ProfileManager));
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
