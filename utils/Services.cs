﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tempest.utils
{
    internal class Services
    {
        public static Color brushColor = Color.FromRgb(255,255,255);
        public static Image mapImage = new()
        {
            Width = 1000,
            Height = 1000,
            //Source = new BitmapImage(new Uri(@"")),
            Opacity = 0
        };
    }
}
