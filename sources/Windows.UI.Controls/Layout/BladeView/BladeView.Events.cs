// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Windows.UI.Controls
{
    /// <summary>
    /// A container that hosts <see cref="BladeItem"/> controls in a horizontal scrolling list
    /// Based on the Azure portal UI
    /// </summary>
    public partial class BladeView
    {
        /// <summary>
        /// Fires whenever a <see cref="BladeItem"/> is opened
        /// </summary>
        public event EventHandler<BladeItem> BladeOpened;

        /// <summary>
        /// Fires whenever a <see cref="BladeItem"/> is closed
        /// </summary>
        public event EventHandler<BladeItem> BladeClosed;
    }
}