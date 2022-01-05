// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Community.Windows.Connectivity;
using Windows.UI.Popups;

namespace Community.Windows.ShowcaseApp
{
    internal static class Tools
    {
        internal static async Task<bool> CheckInternetConnectionAsync()
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                var dialog = new MessageDialog("Internet connection not detected. Please try again later.");
                await dialog.ShowAsync();

                return false;
            }

            return true;
        }
    }
}