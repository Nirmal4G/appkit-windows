// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Windows.UI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThemeListenerPage : Page
    {
        public ThemeListenerPage()
        {
            this.InitializeComponent();
            Listener = new ThemeListener();
            this.Loaded += ThemeListenerPage_Loaded;
            Listener.ThemeChanged += Listener_ThemeChanged;
            ShowcaseController.Current.ThemeChanged += Current_ThemeChanged;
        }

        private void Current_ThemeChanged(object sender, Models.ThemeChangedArgs e)
        {
            UpdateThemeState();
        }

        private void ThemeListenerPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateThemeState();
        }

        private void Listener_ThemeChanged(ThemeListener sender)
        {
            UpdateThemeState();
        }

        private void UpdateThemeState()
        {
            SystemTheme.Text = Listener.CurrentThemeName;
            CurrentTheme.Text = ShowcaseController.Current.GetCurrentTheme().ToString();
        }

        public ThemeListener Listener { get; }
    }
}