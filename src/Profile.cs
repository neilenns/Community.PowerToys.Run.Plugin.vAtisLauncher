// <copyright file="Profile.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Community.PowerToys.Run.Plugin.vAtisLauncher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a vAtis profile.
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [JsonPropertyName("version")]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the profile.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the stations.
        /// </summary>
        [JsonPropertyName("stations")]
        public List<Station> Stations { get; set; }

        /// <summary>
        /// Gets a comma separated list of the identifiers for all stations
        /// in the profile.
        /// </summary>
        public string StationIdentifiers
        {
            get
            {
                var stationIdentifiers = this.Stations
                    .OrderBy(station => station.Identifier)
                    .Select(station => station.Identifier);

                string result = string.Join(", ", stationIdentifiers);

                return result;
            }
        }
    }
}
