using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoPuzzle.Helpers
{
    public class ImageRepository
    {
        public string[] DirectoryGetFiles(string directory)
        {
            return Directory.GetFiles("Res\\Image\\" + directory + "\\");
        }
    }
}
