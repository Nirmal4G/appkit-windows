// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Windows.UI.Media;

namespace CommunityToolkit.Windows.UI.Animations
{
    /// <summary>
    /// An effect animation that targets <see cref="CrossFadeEffect.Factor"/>.
    /// </summary>
    public sealed class CrossFadeEffectAnimation : EffectAnimation<CrossFadeEffect, double?, double>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => Target.Id;

        /// <inheritdoc/>
        protected override (double?, double?) GetParsedValues()
        {
            return (To, From);
        }
    }
}