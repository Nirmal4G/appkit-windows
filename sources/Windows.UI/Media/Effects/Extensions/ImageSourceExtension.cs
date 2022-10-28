// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Community.Windows.UI.Media.Pipelines;

namespace Community.Windows.UI.Media
{
    /// <summary>
    /// An image effect, which displays an image loaded as a Win2D surface
    /// </summary>
    public sealed class ImageSourceExtension : ImageSourceBaseExtension
    {
        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            return PipelineBuilder.FromImage(Uri, DpiMode, CacheMode);
        }
    }
}