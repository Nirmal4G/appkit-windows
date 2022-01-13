// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Windows.ShowcaseApp.Data;
using CommunityToolkit.Windows.ShowcaseApp.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples.ConnectedAnimations.Pages
{
    public sealed partial class SecondPage : Page
    {
        private static ObservableCollection<PhotoDataItem> items;

        public SecondPage()
        {
            this.InitializeComponent();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ThirdPage), e.ClickedItem, new SuppressNavigationTransitionInfo());
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (items == null)
            {
                items = await new Data.PhotosDataSource().GetItemsAsync();
            }

            listView.ItemsSource = items;
        }
    }
}