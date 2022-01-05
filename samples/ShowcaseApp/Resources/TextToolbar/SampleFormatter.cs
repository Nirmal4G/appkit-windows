// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Windows.UI.Controls;
using CommunityToolkit.Windows.UI.Controls.TextToolbarButtons;
using CommunityToolkit.Windows.UI.Controls.TextToolbarButtons.Common;
using CommunityToolkit.Windows.UI.Controls.TextToolbarFormats;

namespace CommunityToolkit.Windows.ShowcaseApp.Samples.TextToolbarSamples
{
    public class SampleFormatter : Formatter
    {
        public override void SetModel(TextToolbar model)
        {
            base.SetModel(model);

            CommonButtons = new CommonButtons(model);
        }

        public override ButtonMap DefaultButtons
        {
            get
            {
                var bold = CommonButtons.Bold;
                bold.Activation = item => Selected.Text = "BOLD!!!";

                return new ButtonMap
                {
                    bold
                };
            }
        }

        private CommonButtons CommonButtons { get; set; }
    }
}