// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.Windows.ShowcaseApp.Data;
using CommunityToolkit.Windows.ShowcaseApp.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples.ConnectedAnimations.Pages
{
    public sealed partial class ThirdPage : Page
    {
        private PhotoDataItem item;

        public ThirdPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            item = e.Parameter as PhotoDataItem;

            base.OnNavigatedTo(e);
        }
    }
}