using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Media;
using PhotoPuzzle.Advertising;
using System.IO.IsolatedStorage;

namespace PhotoPuzzle
{
    public partial class MainPage : PhoneApplicationPage
    {
#if FREE7
        PhotoPuzzle.Advertising.MicrosoftAds microsoftAds { get; set; }
#endif
#if FREE8
        //AdsMob adsMob { get; set; }
#endif
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        private readonly ObservableCollection<string> mImages = new ObservableCollection<string>();

        private Border TempBorder { get; set; }

        private int CountParts = 4;

        public string IsGallery = string.Empty;
        private double K = 1.5;

        private bool IsPictureSelected { get; set; }

        public MainPage()
        {
            InitializeComponent();
            GameGlobal.Random = new Random((int)DateTime.Now.Ticks);
            ListImages.ItemsSource = mImages;
        }

        public string[] DirectoryGetFiles(string directory)
        {
            return Directory.GetFiles("Res\\Image\\" + directory + "\\");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                mImages.Clear();
                string category = "Masha";
                string[] dirs = DirectoryGetFiles(category);
                foreach (string dir in dirs)
                {
                    mImages.Add(dir);
                }
            }
            else
            {
                IsBackNavigation = true;
            }

        }

        bool IsBackNavigation { get; set; }

        public void StartGame()
        {
            if (GameGlobal.PictrueSource == null && string.IsNullOrEmpty(IsGallery) && TempBorder != null || !IsPictureSelected && string.IsNullOrEmpty(IsGallery) && TempBorder != null)
            {
                GameGlobal.PictrueSource = (TempBorder.Child as Image).Source as BitmapImage;
            }
            GameGlobal.SetLevel(new Size(CountParts * K, CountParts));
            this.NavigationService.Navigate(new Uri("/GameInterface.xaml", UriKind.Relative));
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderAmount != null)
            {
                double newValue = e.NewValue;
                double drob = Math.Abs(Math.Floor(newValue) - newValue);
                int ceilValue = (int)Math.Ceiling(newValue);

                if (ceilValue % 2 != 0 && K != 1)
                {
                    //newValue = drob >= 0.5 ? ceilValue + 1 : ceilValue - 1;
                    newValue = ceilValue;
                    SliderAmount.Text = (K * Math.Pow(newValue, 2) - K).ToString();
                }
                else
                {
                    newValue = ceilValue;
                    SliderAmount.Text = (K * Math.Pow(newValue, 2)).ToString();
                }
                CountParts = (int)newValue;
                //SliderAmount.Text = (K * Math.Pow(newValue, 2) - 1.5).ToString();
            }
        }

        private void ListImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                IsPictureSelected = true;
                using (Stream myStream = Application.GetResourceStream(new Uri(e.AddedItems[0].ToString(), UriKind.RelativeOrAbsolute)).Stream)
                {
                    BitmapImage bt = new BitmapImage();
                    bt.SetSource(myStream);
                    if (bt.PixelHeight > bt.PixelWidth)
                    {
                        WriteableBitmap wbm = new WriteableBitmap(bt);
                        wbm = wbm.Resize(bt.PixelWidth, bt.PixelHeight, WriteableBitmapExtensions.Interpolation.Bilinear).Rotate(270);
                        using (var ms = new MemoryStream())
                        {
                            wbm.SaveJpeg(ms, wbm.PixelWidth, wbm.PixelHeight, 0, 100);
                            BitmapImage bmp = new BitmapImage();
                            bmp.SetSource(ms);
                            GameGlobal.PictrueSource = bmp;
                        }
                    }
                    else
                    {
                        GameGlobal.PictrueSource = bt;
                    }

                    K = 1.5;
                    if (bt.PixelHeight < bt.PixelWidth)
                    {
                        double div = (double)bt.PixelWidth / bt.PixelHeight;
                        if (div < 1.5)
                        {
                            K = 1;
                        }
                    }
                    SliderAmount.Text = (K * Math.Pow(4, 2)).ToString();
                    countParts.Value = 4;
                }
                //GameGlobal.PictrueSource = new BitmapImage(new Uri(e.AddedItems[0].ToString(), UriKind.RelativeOrAbsolute));
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsBackNavigation)
            {
                var firstListBoxItem = this.ListImages.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                Image firstImage = FindFirstElementInVisualTree<Image>(firstListBoxItem);
                TempBorder = firstImage.Parent as Border;
                TempBorder.BorderThickness = new Thickness(5);
                //GameGlobal.PictrueSource = (BitmapImage)firstImage.Source;
            }
            int countLoaded = (int)settings["countLoaded"];
            if (countLoaded > 1)
            {
                var adsMyGames = new AdsMyGames();
                adsMyGames.ShowAd(() => { });
            }
#if FREE7
            if (microsoftAds == null)
            {
                microsoftAds = new PhotoPuzzle.Advertising.MicrosoftAds();
                microsoftAds.AddAds(adsGrid, "11569650");
            }
#endif
#if FREE8
            //if (adsMob == null)
            //{
            //    adsMob = new AdsMob();
            //    adsMob.AddInterstitialAd("ca-app-pub-4981977246797551/6416121828");
            //}
#endif
        }

        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void Border_Tap(object sender, GestureEventArgs e)
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

    }
}
