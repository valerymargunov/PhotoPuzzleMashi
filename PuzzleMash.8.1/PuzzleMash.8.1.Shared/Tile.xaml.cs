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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PuzzleMash._8._1
{
    public sealed partial class Tile : UserControl
    {
        public Tile()
        {
            this.InitializeComponent();
            ANI_Move.Completed += ANI_Move_Completed1;
        }

        private void ANI_Move_Completed1(object sender, object e)
        {
            if (OnMoveComplete != null)
                OnMoveComplete();
        }

        public ImageSource ImageSource
        {
            get { return imageout.Source; }
            set { imageout.Source = value; }
        }

        public Action OnMoveStart { get; set; }
        public Action OnMoveComplete { get; set; }
        private int indexX;

        public int IndexX
        {
            get { return indexX; }
            set
            {
                if (value >= GameGlobal.MaxTilesW || value < 0)
                    return;
                XSP.Value = GameGlobal.TileW * indexX;
                indexX = value;
                XEP.Value = GameGlobal.TileW * value;
                YSP.Value = GameGlobal.TileH * indexY;
                YEP.Value = GameGlobal.TileH * indexY;
                ANI_Move.Begin();
            }
        }
        private int indexY;

        public int IndexY
        {
            get { return indexY; }
            set
            {
                if (value >= GameGlobal.MaxTilesH || value < 0)
                    return;
                XSP.Value = GameGlobal.TileW * indexX;
                XEP.Value = GameGlobal.TileW * indexX;
                YSP.Value = GameGlobal.TileH * indexY;
                indexY = value;
                YEP.Value = GameGlobal.TileH * value;
                ANI_Move.Begin();
            }
        }
        public int CorrectIndexX { get; set; }
        public int CorrectIndexY { get; set; }
        public bool IsCorrectIndex()
        {
            if (CorrectIndexX == indexX && CorrectIndexY == indexY)
                return true;
            else
                return false;
        }
    }
}
