// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples
{
    /// <summary>
    /// Textbox Mask sample page
    /// </summary>
    public sealed partial class TextBoxMaskPage : Page, IXamlRenderListener
    {
        private TextBox alphaTextBox;

        public TextBoxMaskPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            alphaTextBox = control.FindChild("AlphaTextBox") as TextBox;
        }

        private void Load()
        {
            ShowcaseController.Current.RegisterNewCommand("Apply Full Mask", (s, e2) =>
            {
                if (alphaTextBox != null)
                {
                    alphaTextBox.Text = "7b1y--x4a5";
                }
            });

            ShowcaseController.Current.RegisterNewCommand("Apply Partial Mask", (s, e2) =>
            {
                if (alphaTextBox != null)
                {
                    alphaTextBox.Text = "7b1yZW";
                }
            });
        }
    }
}