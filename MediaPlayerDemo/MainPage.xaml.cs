using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Storage.Streams;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayerDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private String url = "http://www.neu.edu.cn/indexsource/neusong.mp3";
        private String filename = "neusong20153159.mp3";

        Frame root = Window.Current.Content as Frame;

        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            if (root.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
        }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        public async Task Gets(Uri uri)
        {
            try
            {
                StorageFile destinationFile = await KnownFolders.MusicLibrary.CreateFileAsync(this.filename);
                try
                {
                    HttpClient httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(uri);
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var sourceStream = await response.Content.ReadAsInputStreamAsync();

                    using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (var destinationOutputStream = destinationStream.GetOutputStreamAt(0))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(sourceStream, destinationStream);
                        }
                    }
                    MessageDialog msg = new MessageDialog("get√");
                    await msg.ShowAsync();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("xxxxx{0}", e);
                    MessageDialog msg = new MessageDialog("oooops");
                    await msg.ShowAsync();
                }
            }
            catch
            {
                MessageDialog msg = new MessageDialog("you've downloaded it");
                await msg.ShowAsync();
            }
            finally
            {
                DownloadBtn.Visibility = Visibility.Visible;
                MainPageRing.IsActive = false;
            }
        }

        private void DownloadButtonClickHandler(Object sender, RoutedEventArgs e)
        {
            MainPageRing.IsActive = true;
            Gets(new Uri(this.url));
            DownloadBtn.Visibility = Visibility.Collapsed;
        }
       
        private void PlayerButtonClickHandler(Object sender, RoutedEventArgs e)
        {
            root.Navigate(typeof(Player), this.filename);
        }

    } // end class MainPage
} // end namespace