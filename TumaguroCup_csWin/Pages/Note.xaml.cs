// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TumaguroCup_csWin.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Note : Page
    {
        public Note()
        {
            this.InitializeComponent();
        }

        private void NotePad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(markdownTogle.SelectedIndex == 1)
            {
                MarkdownText.Text = NotePad.Text;
            }
        }

        private void markdownTogle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if(markdownTogle.SelectedIndex == 0)
            {
                MarkdownText.Visibility = Visibility.Collapsed;
            }
            else
            {
                MarkdownText.Visibility=Visibility.Visible;
            }
        }
    }
}
