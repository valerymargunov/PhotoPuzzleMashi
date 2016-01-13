using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PuzzleMash._8._1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameInterface : Page
    {
        bool isGameRunning = false;
        public GameInterface()
        {
            this.InitializeComponent();
            this.ManipulationMode = ManipulationModes.TranslateInertia;
            ANI_Starting.Completed += ANI_Starting_Completed;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.helpPanel.Visibility = Visibility.Visible;
        }

        void ANI_Starting_Completed(object sender, object e)
        {
            isGameRunning = true;
            pictrueshow.Visibility = Visibility.Collapsed;
        }

        void OnMoveStart()
        {
            isMoving = true;
        }

        void OnMoveComplete()
        {
            isMoving = false;
            if (isGameOver() && isGameRunning)
            {
                ANI_End.Begin();
                gameOver.Visibility = Visibility.Visible;
                isGameRunning = false;
            }
        }

        bool isGameOver()
        {
            bool over = false;
            int i = 0;
            foreach (Tile item in Tiles.Children)
            {
                if (item.IsCorrectIndex())
                    i += 1;
            }
            if (i >= Tiles.Children.Count)
                over = true;
            return over;
        }

        bool isMoving = false;
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            StartPoint = e.Position;
        }

        Point StartPoint;
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            if (saveTile == null)
                return;
            double offsetX = (e.Position.X - StartPoint.X);
            double offsetY = (e.Position.Y - StartPoint.Y);
            var radians = Math.Atan2(offsetY, offsetX);
            var angle = radians * (180 / Math.PI) + 180;
            var l = Math.Sqrt(Math.Pow(offsetX, 2) + Math.Pow(offsetY, 2));
            if (l >= 5)
            {
                if ((angle >= 0 && angle <= 20) || (angle <= 360 && angle >= 340))
                {
                    if (FindTile(saveTile.IndexX - 1, saveTile.IndexY, saveTile) != null)
                    {
                        saveTile.IndexX -= 1;
                        FindTile(saveTile.IndexX, saveTile.IndexY, saveTile).IndexX += 1;
                    }
                }
                else
                    if (angle >= 70 && angle <= 110)
                {
                    if (FindTile(saveTile.IndexX, saveTile.IndexY - 1, saveTile) != null)
                    {
                        saveTile.IndexY -= 1;
                        FindTile(saveTile.IndexX, saveTile.IndexY, saveTile).IndexY += 1;
                    }
                }
                else
                        if (angle >= 160 && angle <= 200)
                {
                    if (FindTile(saveTile.IndexX + 1, saveTile.IndexY, saveTile) != null)
                    {
                        saveTile.IndexX += 1;
                        FindTile(saveTile.IndexX, saveTile.IndexY, saveTile).IndexX -= 1;
                    }
                }
                else
                            if (angle >= 250 && angle <= 290)
                {
                    if (FindTile(saveTile.IndexX, saveTile.IndexY + 1, saveTile) != null)
                    {
                        saveTile.IndexY += 1;
                        FindTile(saveTile.IndexX, saveTile.IndexY, saveTile).IndexY -= 1;
                    }
                }
            }
            saveTile = null;
        }
        public Tile FindTile(int x, int y, Tile origin)
        {
            foreach (Tile item in Tiles.Children)
            {
                if (item.IndexX == x && item.IndexY == y)
                {
                    if (origin == item)
                        continue;
                    return item;
                }
            }
            return null;
        }
        Point savePoint;
        Tile saveTile;
        void tile_MouseLeftButtonDown(object sender, TappedRoutedEventArgs e)
        {
            if (isMoving)
                return;
            saveTile = sender as Tile;

            savePoint = e.GetPosition(Tiles);

        }
        void GameInterface_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                isGameRunning = false;
                helpImage.Source = GameGlobal.PictrueSource;
                ///////////////////////////////////////
                var bmi = GameGlobal.PictrueSource;
                int imgWidth = bmi.PixelWidth;
                int imgHeight = bmi.PixelHeight;
                double maxWidth = (int)sizeGrid.ActualWidth;
                double maxHeight = (int)sizeGrid.ActualHeight;
                double kW = (double)maxWidth / imgWidth;
                double kH = (double)maxHeight / imgHeight;

                if (imgWidth > imgHeight)
                {
                    double k = (double)imgWidth / imgHeight;
                    if (kW < kH)
                    {
                        Tiles.Width = maxWidth;
                        Tiles.Height = maxWidth / k;
                    }
                    else
                    {
                        Tiles.Height = maxHeight;
                        Tiles.Width = maxHeight * k;
                    }
                }
                else
                {
                    double k = (double)imgHeight / imgWidth;
                    if (kW < kH)
                    {
                        Tiles.Width = maxWidth;
                        Tiles.Height = maxWidth * k;
                    }
                    else
                    {
                        Tiles.Height = maxHeight;
                        Tiles.Width = maxHeight / k;
                    }
                }
                MaxWidth = pictrueshow.Width = pictrueshow.MinWidth = pictrueshow.MaxWidth = blackBorder.Width = blackBorder.MinWidth = blackBorder.MaxWidth = Tiles.MinWidth = Tiles.MaxWidth = Tiles.Width;
                MaxHeight = pictrueshow.Height = pictrueshow.MinHeight = pictrueshow.MaxHeight = blackBorder.Height = blackBorder.MinHeight = blackBorder.MaxHeight = Tiles.MinHeight = Tiles.MaxHeight = Tiles.Height;
                previewImage.Width = Tiles.Width * 0.8;
                previewImage.Height = Tiles.Height * 0.8;
                GameGlobal.TileW = Tiles.Width / GameGlobal.MaxTilesW;
                GameGlobal.TileH = Tiles.Height / GameGlobal.MaxTilesH;
                ///////////////////////////////////////
                for (int i = 0; i < GameGlobal.MaxTilesH * GameGlobal.MaxTilesW; i++)
                {
                    Tiles.Children.Add(new Tile() { Width = GameGlobal.TileW, Height = GameGlobal.TileH });
                }
                foreach (Tile tile in this.Tiles.Children)
                {
                    tile.PointerPressed += Tile_PointerPressed;
                    tile.OnMoveComplete = OnMoveComplete;
                    tile.OnMoveStart = OnMoveStart;
                }
                CreatePictrue(GameGlobal.PictrueSource);
                progressRing.IsActive = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void Tile_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (isMoving)
                return;
            saveTile = sender as Tile;

            savePoint = e.GetCurrentPoint(Tiles).Position;
        }

        public void CreatePictrue(WriteableBitmap source)
        {
            pictrueshow.Source = source;
            pictrueshow.UpdateLayout();
            int i = 0;
            for (int w = 0; w < GameGlobal.MaxTilesW; w++)
            {
                for (int h = 0; h < GameGlobal.MaxTilesH; h++)
                {
                    var tile = Tiles.Children[i] as Tile;
                    WriteableBitmap temp = source.Resize((int)Tiles.Width, (int)Tiles.Height, WriteableBitmapExtensions.Interpolation.Bilinear);
                    var rect = new Rect(w * GameGlobal.TileW, h * GameGlobal.TileH, (int)GameGlobal.TileW, (int)GameGlobal.TileH);
                    temp = temp.Crop(rect);
                    temp.Invalidate();
                    tile.IndexX = tile.CorrectIndexX = w;
                    tile.IndexY = tile.CorrectIndexY = h;
                    tile.ImageSource = temp;
                    i += 1;
                }
            }
            foreach (Tile item in Tiles.Children)
            {
                var r = GameGlobal.Random.Next(Tiles.Children.Count);
                var temp = Tiles.Children[r] as Tile;
                var x = temp.IndexX;
                var y = temp.IndexY;
                temp.IndexX = item.IndexX;
                temp.IndexY = item.IndexY;
                item.IndexX = x;
                item.IndexY = y;
            }
            ANI_Starting.Begin();
        }

        double MaxWidth { get; set; }
        double MaxHeight { get; set; }

        private void button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            this.helpPanel.Visibility = Visibility.Collapsed;
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
#if WIN10
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
#endif
        }

        private void RateReminder_TryReminderCompleted(object sender, AppPromo.RateReminderResult e)
        {
            if (e.Runs == RateReminder.RunsBeforeReminder && !e.RatingShown)
            {
                RateReminder.RunsBeforeReminder *= 2;
                RateReminder.ResetCounters();
            }
        }
    }
}
