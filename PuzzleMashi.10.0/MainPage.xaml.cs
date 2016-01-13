using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
//using VungleSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SuperPuzzle
{
    public class ImageEntity
    {
        public ImageSource ImageSource { get; set; }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //VungleAd sdkInstance;
        private IList<ImageEntity> mImages = new List<ImageEntity>();

        private Border TempBorder { get; set; }

        private int CountParts = 4;

        public string IsGallery = string.Empty;
        private double K = 1.5;

        private bool IsPictureSelected { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            GameGlobal.Random = new Random((int)DateTime.Now.Ticks);
            ListImages.ItemsSource = mImages;
        }

        public static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        //private async void SdkInstance_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        //{
        //    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
        //        new DispatchedHandler(() => ChangeProgressState()));
        //}

        //private async void ChangeProgressState()
        //{
        //    if (GameGlobal.IsShowVideoAds)
        //    {
        //        progressRing.IsActive = false;
        //        GameGlobal.IsShowVideoAds = false;
        //        await sdkInstance.PlayAdAsync(new AdConfig { Incentivized = true });
        //    }
        //}

        public string[] DirectoryGetFiles(string directory)
        {
            return Directory.GetFiles("Res\\Image\\" + directory + "\\");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (sdkInstance == null && GameGlobal.IsShowVideoAds && IsConnectedToInternet())
            //{
            //    //sdkInstance = AdFactory.GetInstance("vungleTest");
            //    sdkInstance = AdFactory.GetInstance("565f4a8bd0ed15880f00004c");
            //    sdkInstance.OnAdPlayableChanged += SdkInstance_OnAdPlayableChanged;
            //}
            //else
            //{
            //    progressRing.IsActive = false;
            //}
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
            //var timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(5);
            //timer.Tick += delegate
            //{
            //    if (progressRing.IsActive && GameGlobal.IsShowVideoAds)
            //    {
            //        progressRing.IsActive = false;
            //        timer.Stop();
            //    }
            //};
            //timer.Start();

            Frame rootFrame = Window.Current.Content as Frame;
            string myPages = "";
            foreach (PageStackEntry page in rootFrame.BackStack)
            {
                myPages += page.SourcePageType.ToString() + "\n";
            }
            if (rootFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            //IsGallery = string.Empty;
            //string[] parameters = e.Parameter.ToString().Split(',');
            //string sCategory = parameters[0];
            //IsGallery = sCategory == "-1" ? parameters[1] : string.Empty;
            //mImages.Clear();
            //if (string.IsNullOrEmpty(IsGallery))
            //{
            string[] dirs = DirectoryGetFiles("Masha");
            foreach (string dir in dirs)
            {
                mImages.Add(new ImageEntity { ImageSource = new BitmapImage(new Uri("ms-appx:///" + dir, UriKind.RelativeOrAbsolute)) });
            }
            //}
            //else
            //{
            //    mImages.Add(new ImageEntity { ImageSource = GameGlobal.PictrueSource });
            //}
        }

        bool IsBackNavigation { get; set; }

        public async void StartGame()
        {
            if (progressRing.IsActive == false)
            {
                progressRing.IsActive = true;
                if (GameGlobal.PictrueSource == null && string.IsNullOrEmpty(IsGallery) || !IsPictureSelected && string.IsNullOrEmpty(IsGallery))
                {
                    var bmp = (TempBorder.Child as Image).Source as BitmapImage;
                    StorageFile file = GetFileFromUriAsync(bmp.UriSource.OriginalString);
                    if (file != null)
                    {
                        using (FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read))
                        {
                            var property = await file.Properties.GetImagePropertiesAsync();
                            WriteableBitmap wbm = new WriteableBitmap((int)property.Width, (int)property.Height);
                            wbm.SetSource(stream);
                            GameGlobal.PictrueSource = wbm;
                            GameGlobal.CurrentUriSource = bmp.UriSource;
                            stream.Dispose();
                        }
                    }
                }
                GameGlobal.SetLevel(new Size(CountParts * K, CountParts));
                progressRing.IsActive = false;
                Frame.Navigate(typeof(GameInterface));
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (SliderAmount != null)
            {
                double newValue = e.NewValue;
                double drob = Math.Abs(Math.Floor(newValue) - newValue);
                int ceilValue = (int)Math.Ceiling(newValue);

                if (ceilValue % 2 != 0 && K != 1)
                {
                    newValue = ceilValue;
                    SliderAmount.Text = (K * Math.Pow(newValue, 2) - K).ToString();
                }
                else
                {
                    newValue = ceilValue;
                    SliderAmount.Text = (K * Math.Pow(newValue, 2)).ToString();
                }
                CountParts = (int)newValue;
            }
        }

        private StorageFile GetFileFromUriAsync(string fileName)
        {
            var task = StorageFile.GetFileFromApplicationUriAsync(new Uri(fileName, UriKind.RelativeOrAbsolute)).AsTask();
            task.Wait();
            return task.Result;
        }

        private async void ListImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && progressRing.IsActive == false)
            {
                progressRing.IsActive = true;
                IsPictureSelected = true;
                var file = GetFileFromUriAsync((((ImageEntity)e.AddedItems[0]).ImageSource as BitmapImage).UriSource.OriginalString);
                if (file != null)
                {
                    var property = await file.Properties.GetImagePropertiesAsync();
                    using (FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read))
                    {
                        if (property.Height > property.Width)
                        {
                            WriteableBitmap wbm = new WriteableBitmap((int)property.Width, (int)property.Height);
                            wbm.SetSource(stream);
                            wbm = wbm.Resize((int)property.Width, (int)property.Height, WriteableBitmapExtensions.Interpolation.Bilinear).Rotate(270);
                            GameGlobal.PictrueSource = wbm;
                        }
                        else
                        {
                            WriteableBitmap wbm = new WriteableBitmap((int)property.Width, (int)property.Height);
                            wbm.SetSource(stream);
                            GameGlobal.PictrueSource = wbm;
                        }
                        stream.Dispose();
                    }
                    GameGlobal.CurrentUriSource = (((ImageEntity)e.AddedItems[0]).ImageSource as BitmapImage).UriSource;

                    K = 1.5;
                    if (property.Height < property.Width)
                    {
                        double div = (double)property.Width / property.Height;
                        if (div < 1.5)
                        {
                            K = 1;
                        }
                    }
                    SliderAmount.Text = (K * Math.Pow(4, 2)).ToString();
                    countParts.Value = 4;
                }
                progressRing.IsActive = false;
            }
        }

        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void Border_Tap(object sender, TappedRoutedEventArgs e)
        {
            TempBorder.BorderThickness = new Thickness(0);
            TempBorder = sender as Border;
            TempBorder.BorderThickness = new Thickness(5);
        }

        #region Tree Helpers
        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        private DependencyObject FindChildControl<T>(DependencyObject control)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsBackNavigation)
            {
                var firstListBoxItem = this.ListImages.ContainerFromIndex(0) as ListBoxItem;
                Image firstImage = FindFirstElementInVisualTree<Image>(firstListBoxItem);
                TempBorder = firstImage.Parent as Border;
                TempBorder.BorderThickness = new Thickness(5);
            }
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
