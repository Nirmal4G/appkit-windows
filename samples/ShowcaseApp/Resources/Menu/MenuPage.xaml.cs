// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Windows.UI;
using CommunityToolkit.Windows.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples
{
    public sealed partial class MenuPage : IXamlRenderListener
    {
#pragma warning disable CS0618 // Type or member is obsolete
        private MenuItem fileMenu;

        public MenuPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            fileMenu = control.FindChild("FileMenu") as MenuItem;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        private void Load()
        {
            ShowcaseController.Current.RegisterNewCommand("Append Item to file menu", (sender, args) =>
            {
                if (fileMenu != null)
                {
                    var flyoutItem = new MenuFlyoutItem
                    {
                        Text = "Click to remove"
                    };

                    flyoutItem.Click += (a, b) =>
                    {
                        fileMenu.Items.Remove(flyoutItem);
                    };

                    fileMenu.Items.Add(flyoutItem);
                }
            });

            ShowcaseController.Current.RegisterNewCommand("Prepend Item to file menu", (sender, args) =>
            {
                if (fileMenu != null)
                {
                    var flyoutItem = new MenuFlyoutItem
                    {
                        Text = "Click to remove"
                    };

                    flyoutItem.Click += (a, b) =>
                    {
                        fileMenu.Items.Remove(flyoutItem);
                    };

                    fileMenu.Items.Insert(0, flyoutItem);
                }
            });
        }
    }
}