using Microsoft.Advertising.Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PhotoPuzzle.Advertising
{
    public class MicrosoftAds
    {
        public void AddAds(Grid grid, string unitId)
        {
#if FREE7
            AdControl adControl = new AdControl("57fa3b83-82f9-49fa-a395-2f13cdda6765", unitId, true);
            adControl.Width = 480;
            adControl.Height = 80;
            grid.Children.Add(adControl);
#endif
        }
    }
}
