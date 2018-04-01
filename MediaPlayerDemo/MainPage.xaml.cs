using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayerDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async System.Threading.Tasks.Task openMediaFile()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".mp3");
            fileOpenPicker.FileTypeFilter.Add(".mp4");

            StorageFile file = await fileOpenPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                MediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                MediaPlayer.Visibility = Visibility.Visible;
                OpenBtn.Visibility = Visibility.Collapsed;

                MediaPlayer.MediaPlayer.MediaEnded += new TypedEventHandler<Windows.Media.Playback.MediaPlayer, object>((player, resource) => {
                    OpenBtn.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        OpenBtn.Visibility = Visibility.Visible;
                        MediaPlayer.Visibility = Visibility.Collapsed;
                    });
                });

            }
            else
            {
                State.Text = "unable to open the file!";
            }
        }

        // action listener and handle the event
        private void ButtonClickHandler(object sender, RoutedEventArgs e) 
        {
            openMediaFile();
        }
    }
}