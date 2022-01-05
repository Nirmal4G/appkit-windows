// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Community.Parsers.Markdown.Blocks
{
    [Obsolete(Constants.ParserObsoleteMsg)]
    internal class ListItemPreamble
    {
        public ListStyle Style { get; set; }

        public int ContentStartPos { get; set; }
    }
}