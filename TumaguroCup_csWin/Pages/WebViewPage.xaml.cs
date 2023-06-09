// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TumaguroCup_csWin.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebViewPage : Page
    {
        public WebViewPage()
        {
            this.InitializeComponent();
            
            this.UriBox.Text = this.webViewer.Source.ToString();

            webViewer.NavigationStarting += EnsureHttps;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is string)
            {
                try
                {
                    this.webViewer.Source = new Uri(e.Parameter.ToString()); 
                }
                catch (Exception ex)
                {
                    this.UriBox.Text = e.Parameter.ToString();
                }
            }
            base.OnNavigatedTo(e);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.UriBox.Text.StartsWith("http://") 
                || this.UriBox.Text.StartsWith("https://"))
            {
                this.webViewer.Source = new Uri(this.UriBox.Text);
            }
        }

        private void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            string uri = args.Uri;
            if(!uri.StartsWith("http://") 
                && !uri.StartsWith("https://"))
            {
                args.Cancel = true;
            }
            else
            {
                UriBox.Text = uri;
            }
        }
    }
}
