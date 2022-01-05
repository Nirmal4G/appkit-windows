// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Community.Windows.ShowcaseApp.Data;
using Community.Windows.ShowcaseApp.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Community.Windows.ShowcaseApp.Samples.ConnectedAnimations.Pages
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