// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;

using TumaguroCup_csWin.Pages;
using TumaguroCup_csWin.Library;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TumaguroCup_csWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        #region paramater

#nullable enable
        private string? TrancelatingText { get; set; } = "";
        private string? TrancelatedText { get; set; } = "";
        private bool IsDetected { get; set; } = false;
        private SoftwareBitmap? Image { get; set; }
        private SoftwareBitmapSource ImageSource { get; set; } = new();

        private SettingWindow SettingWindow { get; set; } 
#nullable disable

        #endregion

        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(Topbar);

            #region AddEventHandler

            Clipboard.ContentChanged += new EventHandler<object>(this.Clipboad_Chenged);
            this.Closed += MainWindow_Closed;
            
            #endregion

            ToolPalette.SourcePageType = typeof(Note);
        }

        private void Update()
        {
            TrancelatingTextBox.Text = TrancelatingText;
            TrancelatedTextBox.Text = TrancelatedText;
            InputPictureView.Source = ImageSource;
        }

        //GetContentFromClipBoad
        private async Task<SoftwareBitmap> GetClipboardImage()
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                RandomAccessStreamReference imageStreamRef = await dataPackageView.GetBitmapAsync();

                using IRandomAccessStreamWithContentType streamWithContent = await imageStreamRef.OpenReadAsync();
                // SoftwareBitmapÇ…ïœä∑Ç∑ÇÈ
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(streamWithContent);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                return softwareBitmap;
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

        private async Task ExcuteOcrAsync()
        {
            await ImageSource.SetBitmapAsync(Image);

            if(ModeChange.SelectedIndex == 0 || ModeChange.SelectedIndex == 2)
            {
                TrancelatingText = await CharacterRecognizer.RunOcr(Image, Library.Language.EN);
                Task.WaitAll();
            }
            else if(ModeChange.SelectedIndex == 1 || ModeChange.SelectedIndex == 3)
            {
                TrancelatingText = await CharacterRecognizer.RunOcr(Image, Library.Language.JP);
                Task.WaitAll();
            }

            if(string.IsNullOrEmpty(TrancelatingText))
            {
                TrancelatingText = "ï∂éöóÒÇ™åüèoÇ≥ÇÍÇ‹ÇπÇÒÇ≈ÇµÇΩ.";
                TrancelatedText = string.Empty;
                IsDetected = false;
            }
            else
            {
                IsDetected = true;
            }
        }

        private async Task TrancelateAsync()
        {
            if (IsDetected == true)
            {
                string language;

                if(ModeChange.SelectedIndex == 0)
                {
                    language = Language.Language.JA;
                }
                else if(ModeChange.SelectedIndex == 1)
                {
                    language = Language.Language.EN;
                }
                else
                {
                    return;
                }

                TrancelatedText = await Translator.Translator.Translate(language, TrancelatingText);
                Task.WaitAll();
                IsDetected = false;
            }
        }

        private async Task ReadQRAsync()
        {
            if (ExtendOptionMode.SelectedIndex == 1)
            {
                var qrContent = await QRCodeReader.QRCodeRead(Image);
                if (qrContent != null)
                {
                    ToolPalette.Navigate(typeof(WebViewPage), qrContent);
                }
            }
        }

        private void ConvertSoftwareBitmap()
        {
            if(Image != null)
            {
                if (Image.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                    Image.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    Image = SoftwareBitmap.Convert(Image, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
            }
        }

        //EventHandler
        #region EventHandler

        private async void Clipboad_Chenged(object sender, object e)
        {
            Image = await GetClipboardImage();
            if(Image != null)
            {
                ConvertSoftwareBitmap();
                await ReadQRAsync();
                await ExcuteOcrAsync();
                await TrancelateAsync();
                Update();
            }
            else
            {
                TrancelatingText = await GetClipboadText();

                if(TrancelatingText != null)
                {
                    IsDetected = true;
                    await TrancelateAsync();
                    Update();
                }
            }
        }

        private async void ReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var picker = new FileOpenPicker
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

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(FileSource.Text)) { return; }

            StorageFile inputFile = await StorageFile.GetFileFromPathAsync(FileSource.Text);

            using (IRandomAccessStream stream = await inputFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                Image = await decoder.GetSoftwareBitmapAsync();
            }

            ConvertSoftwareBitmap();
            await ReadQRAsync();
            await ExcuteOcrAsync();
            await TrancelateAsync();
            Update();
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            ToolPalette.Navigate(typeof(Note));
        }

        private void WebViewButton_Click(object sender, RoutedEventArgs e)
        {
            ToolPalette.Navigate(typeof(WebViewPage));
        }

        private async void TracelationButton_Click(object sender, RoutedEventArgs e)
        {
            TrancelatingText = TrancelatingTextBox.Text;
            if(!string.IsNullOrEmpty(TrancelatingText))
            {
                IsDetected = true;
                await TrancelateAsync();
                Update();
            }
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow = new();
            SettingWindow.Activate();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (SettingWindow != null)
            {
                SettingWindow.Close();
            }
        }

        #endregion
    }
}
