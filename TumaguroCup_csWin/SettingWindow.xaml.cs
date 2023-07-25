// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

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
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TumaguroCup_csWin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            this.InitializeComponent();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
        }

        private void applicationButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(inputApiKey.Text))
            {
                this.ErrorMassage.Text = "APIキーを入力してください。";
            }
            else
            {
                try
                {
                    string jsonString = JsonSerializer.Serialize(inputApiKey.Text);
                    using (var sw = new StreamWriter("./temp.json", false, System.Text.Encoding.UTF8))
                    {
                        // JSON データをファイルに書き込み
                        sw.Write(jsonString);
                    }
                    var flg = Translator.Translator.SetUp();
                    if(flg == false)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMassage.Text = "ファイル入出力に失敗しました。";
                }
            }
        }

        private void canseleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
