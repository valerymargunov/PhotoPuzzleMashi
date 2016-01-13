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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhotoPuzzle
{
    public partial class GameInterface : PhoneApplicationPage
    {
#if FREE7
        PhotoPuzzle.Advertising.MicrosoftAds microsoftAds { get; set; }
#endif
#if FREE8
        PhotoPuzzle.Advertising.AdsMob adsMob { get; set; }
#endif
        bool isGameRunning = false;
        public GameInterface()
        {
            InitializeComponent();
            ANI_Starting.Completed += new EventHandler(ANI_Starting_Completed);
        }

        void ANI_Starting_Completed(object sender, EventArgs e)
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
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
        }
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            if (saveTile == null)
                return;

            var radians = Math.Atan2(e.TotalManipulation.Translation.Y, e.TotalManipulation.Translation.X);
            var angle = radians * (180 / Math.PI) + 180;
            var l = Math.Sqrt(Math.Pow(e.TotalManipulation.Translation.X, 2) + Math.Pow(e.TotalManipulation.Translation.Y, 2));
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
        void tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
#if FREE7
                if (microsoftAds == null)
                {
                    microsoftAds = new PhotoPuzzle.Advertising.MicrosoftAds();
                    microsoftAds.AddAds(adsGrid, "11569641");
                }
#endif
#if FREE8
                if (adsMob == null || adsMob != null && IsTryReminderCompleted)
                {
                    adsMob = new PhotoPuzzle.Advertising.AdsMob();
                    adsMob.AddInterstitialAd("ca-app-pub-4981977246797551/6369725024");
                    IsTryReminderCompleted = false;
#endif
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
                    previewImage.Width = Tiles.Width * 0.9;
                    previewImage.Height = Tiles.Height * 0.9;
                    GameGlobal.TileW = Tiles.Width / GameGlobal.MaxTilesW;
                    GameGlobal.TileH = Tiles.Height / GameGlobal.MaxTilesH;
                    ///////////////////////////////////////
                    if (GameGlobal.PictrueSource == null)
                    {
                        var uri = PhoneApplicationService.Current.State["ImageData"] as Uri;
                        if (uri.OriginalString == "")
                        {
                            GameGlobal.PictrueSource = null;
                            this.NavigationService.GoBack();
                        }
                        GameGlobal.PictrueSource = new BitmapImage(uri);
                        if (PhoneApplicationService.Current.State.ContainsKey("GameLevel"))
                        {
                            var size = (Size)PhoneApplicationService.Current.State["GameLevel"];
                            GameGlobal.SetLevel((int)size.Width, (int)size.Height);
                        }
                    }
                    for (int i = 0; i < GameGlobal.MaxTilesH * GameGlobal.MaxTilesW; i++)
                    {
                        Tiles.Children.Add(new Tile() { Width = GameGlobal.TileW, Height = GameGlobal.TileH });
                    }
                    foreach (Tile tile in this.Tiles.Children)
                    {
                        tile.MouseLeftButtonDown += new MouseButtonEventHandler(tile_MouseLeftButtonDown);
                        tile.OnMoveComplete = OnMoveComplete;
                        tile.OnMoveStart = OnMoveStart;
                    }
                    CreatePictrue(GameGlobal.PictrueSource);
#if FREE8
                }
#endif
            }
            catch (Exception ex)
            {

            }
        }

        public void CreatePictrue(BitmapImage source)
        {
            if (source == null)
                MessageBox.Show("资源错误");
            pictrueshow.Source = source;
            pictrueshow.UpdateLayout();
            int i = 0;
            for (int w = 0; w < GameGlobal.MaxTilesW; w++)
            {
                for (int h = 0; h < GameGlobal.MaxTilesH; h++)
                {
                    var tile = Tiles.Children[i] as Tile;
                    WriteableBitmap temp = new WriteableBitmap((int)GameGlobal.TileW, (int)GameGlobal.TileH);
                    tile.ImageSource = temp;
                    temp.Render(pictrueshow, new TranslateTransform() { X = -w * GameGlobal.TileW, Y = -h * GameGlobal.TileH });
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
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (this.helpPanel.Visibility == System.Windows.Visibility.Visible)
            {
                this.helpPanel.Visibility = System.Windows.Visibility.Collapsed;
                e.Cancel = true;
            }
            //GameGlobal.PictrueSource = null;
            base.OnBackKeyPress(e);
        }
        private void help_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.helpPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.helpPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void GoBackMainMenu(object sender, System.Windows.RoutedEventArgs e)
        {
            //GameGlobal.PictrueSource = null;
            this.NavigationService.GoBack();
        }

        double MaxWidth { get; set; }
        double MaxHeight { get; set; }
        bool IsTryReminderCompleted { get; set; }

#if WINDOWS_PHONE8
        private void RateReminder_TryReminderCompleted(object sender, AppPromo.RateReminderResult e)
        {
            if (e.Runs == RateReminder.RunsBeforeReminder)
            {
                IsTryReminderCompleted = true;
                adsMob = new PhotoPuzzle.Advertising.AdsMob();
                if (!e.RatingShown)
                {
                    RateReminder.RunsBeforeReminder *= 2;
                    RateReminder.ResetCounters();
                }
            }
        }
#endif
    }
}
