using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace PuzzleMash._8._1
{
    public class GameGlobal
    {
        public static void SetLevel(Size size)
        {
            SetLevel((int)size.Width, (int)size.Height);
        }
        public static void SetLevel(int w, int h)
        {
            MaxTilesW = w;
            MaxTilesH = h;
        }
        public static Size GameLevel { get { return new Size(MaxTilesW, MaxTilesH); } }
        public static int MaxTilesW = 6;
        public static int MaxTilesH = 4;
        public static double TileW = 100;
        public static double TileH = 100;
        public static WriteableBitmap PictrueSource { get; set; }
        public static Uri CurrentUriSource { get; set; }
        public static Random random = null;
        public static bool IsShowVideoAds = true;
        public static Random Random
        {
            get
            {
                if (random == null)
                    random = new Random((int)DateTime.Now.Ticks);
                return random;
            }
            set
            {
                random = value;
            }
        }
        public static Stream PhotoStream
        {
            get;
            set;
        }
    }
}
