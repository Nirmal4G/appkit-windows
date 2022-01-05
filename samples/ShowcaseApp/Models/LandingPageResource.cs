// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Community.Windows.ShowcaseApp
{
    public class LandingPageResource
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("links")]
        public LandingPageLink[] Links { get; set; }
    }
}