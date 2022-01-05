// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Community.Windows.UI.Controls;
using Community.Windows.UI.Controls.TextToolbarButtons;
using Community.Windows.UI.Controls.TextToolbarButtons.Common;
using Community.Windows.UI.Controls.TextToolbarFormats;

namespace Community.Windows.ShowcaseApp.Samples.TextToolbarSamples
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