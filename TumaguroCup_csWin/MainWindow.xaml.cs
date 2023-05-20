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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TumaguroCup_csWin.Pages;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

using Translator;
using Language;
using TumaguroCup_csWin.Library;
using Windows.Globalization;

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
            Clipboard.Clear();

            //Add EventHandler
            Clipboard.ContentChanged += new EventHandler<object>(this.Clipboad_Chenged);

            ToolPalette.SourcePageType = typeof(LogPage);
        }

        //GetContentFromClipBoad
        private async Task<SoftwareBitmap> GetClipboardImage()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                RandomAccessStreamReference imageStreamRef = await dataPackageView.GetBitmapAsync();
                using (IRandomAccessStreamWithContentType streamWithContent = await imageStreamRef.OpenReadAsync())
                {
                    // SoftwareBitmapに変換する
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(streamWithContent);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    return softwareBitmap;
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
            SoftwareBitmap image = await GetClipboardImage();
            if(image != null)
            {
                if (image.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                    image.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    image = SoftwareBitmap.Convert(image, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                var source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(image);

                InputPictureView.Source = source;

                if(ModeChange.SelectedIndex == 0)
                {
                    //OCR
                    var text = await CharacterRecognizer.RunOcr(image,TumaguroCup_csWin.Library.Language.EN);
                    if(text ==  null)
                    {
                        RichText.Text = "文字が検出されませんでした。";
                        return;
                    }
                    RichText.Text = text;
                    //DeepL
                    var result = await Translator.Translator.Translate(Language.Language.JA, RichText.Text);
                    TranceratedText.Text = result;
                }
                else if(ModeChange.SelectedIndex == 1)
                {
                    //OCR
                    var text = await CharacterRecognizer.RunOcr(image, TumaguroCup_csWin.Library.Language.JP);
                    if (text == null)
                    {
                        RichText.Text = "文字が検出されませんでした。";
                        return;
                    }
                    RichText.Text = text;
                    //DeepL
                    var result = await Translator.Translator.Translate(Language.Language.EN, RichText.Text);
                    TranceratedText.Text = result;
                }
                else if (ModeChange.SelectedIndex == 2)
                {
                    //OCR
                    var text = await CharacterRecognizer.RunOcr(image, TumaguroCup_csWin.Library.Language.JP);
                    if (text == null)
                    {
                        RichText.Text = "文字が検出されませんでした。";
                        return;
                    }
                    RichText.Text = text;
                }
                else if (ModeChange.SelectedIndex == 3) 
                {
                    //OCR
                    var text = await CharacterRecognizer.RunOcr(image, TumaguroCup_csWin.Library.Language.EN);
                    if (text == null)
                    {
                        RichText.Text = "文字が検出されませんでした。";
                        return;
                    }
                    RichText.Text = text;
                }
            }
            else
            {
                var text = await GetClipboadText();

                if(text != null)
                {
                    RichText.Text = text;

                    if(ModeChange.SelectedIndex == 2 || ModeChange.SelectedIndex == 3)
                    {
                        return;
                    }

                    //翻訳処理にぶん投げる
                    var result = await Translator.Translator.Translate(Language.Language.JA, text);
                    TranceratedText.Text = result;
                }
                else
                {
                    RichText.Text = "クリップボードがクリアされました";
                }
            }
        }

        private BitmapImage inputImage = null;

        private async void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var bitmap = new BitmapImage();
                bitmap.UriSource = new Uri(file.Path, UriKind.Absolute);

                InputPictureView.Source = bitmap;
                FileSource.Text = file.Path.ToString();
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if(inputImage != null)
            {
                //一旦放置
            }
        }

        private void TrancelationModeButton_Click(object sender, RoutedEventArgs e)
        {
            ModeChange.IsDropDownOpen = true;
        }
    }
}
