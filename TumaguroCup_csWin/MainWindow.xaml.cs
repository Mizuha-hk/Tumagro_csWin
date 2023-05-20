// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TumaguroCup_csWin.Pages;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TumaguroCup_csWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(Topbar);

            //Add EventHandler
            Clipboard.ContentChanged += new EventHandler<object>(this.Clipboad_Chenged);

            ToolPalette.SourcePageType = typeof(LogPage);
            TranceratedText.Text = "hogehoge!!!!!!!!!!!!!!";
        }

        //GetContentFromClipBoad
        private async Task<BitmapImage> GetClipboardImage()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                RandomAccessStreamReference imageStreamRef = await dataPackageView.GetBitmapAsync();
                using (IRandomAccessStreamWithContentType streamWithContent = await imageStreamRef.OpenReadAsync())
                {
                    //ToBitmapImage
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(streamWithContent);
                    return bitmapImage;
                }
            }
            return null;
        }

        private async Task<string> GetClipboadText()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if(dataPackageView.Contains(StandardDataFormats.Text))
            {
                var text = await dataPackageView.GetTextAsync();
                return text;
            }

            return null;
        }

        //EventHandler
        private async void Clipboad_Chenged(object sender, object e)
        {
            BitmapImage image = await GetClipboardImage();
            if(image != null)
            {
                InputPictureView.Source = image;
                //ORCにぶん投げる
                //翻訳処理にぶん投げる
            }
            else
            {
                var text = await GetClipboadText();

                if(text != null)
                {
                    RichText.Text = text;
                    //翻訳処理にぶん投げる
                }
                else
                {
                    RichText.Text = "クリップボードがクリアされました";
                }
            }
        }
    }
}
