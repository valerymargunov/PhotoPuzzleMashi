using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhotoPuzzle
{
	public partial class Tile : UserControl
	{

		public Tile()
		{
			// Required to initialize variables
			InitializeComponent();
            ANI_Move.Completed += new EventHandler(ANI_Move_Completed);
		}
        public ImageSource ImageSource
        {
            get { return imageout.Source; }
            set { imageout.Source = value; }
        }
        void ANI_Move_Completed(object sender, EventArgs e)
        {
            if (OnMoveComplete != null)
                OnMoveComplete();
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