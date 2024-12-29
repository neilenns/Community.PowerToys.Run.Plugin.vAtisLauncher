// <copyright file="Station.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace Community.PowerToys.Run.Plugin.vAtisLauncher;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a vAtis station.
/// </summary>
public class Station
{
    /// <summary>
    /// Gets or sets the station id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the station identifier.
    /// </summary>
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    /// <summary>
    /// Gets or sets the station name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
