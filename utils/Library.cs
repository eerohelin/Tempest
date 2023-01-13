using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tempest
{
    internal class Player : Border
    {
        public Canvas? _canvas;
        private bool moving = false;

        public Player(string role)
        {
            PreviewMouseDown += PlayerMouseDown;
            PreviewMouseUp += PlayerMouseUp;
            Services.mapImage.OpacityChanged += UpdateOpacity;

            Label testText = new();
            testText.Content = role;
            testText.VerticalContentAlignment = VerticalAlignment.Center;
            testText.HorizontalAlignment = HorizontalAlignment.Center;

            this.Child = testText;

            Opacity = 0;
        }

        private void UpdateOpacity(object? sender, EventArgs e)
        {
            Opacity = Services.mapImage.Opacity;
        }

        public Canvas canvas
        {
            set { _canvas = value; _canvas.PreviewMouseMove += PlayerMouseMove; }
        }


        private void PlayerMouseDown(object sender, MouseButtonEventArgs e)
        {
            moving = true;
            Mouse.Capture(this);
        }

        private void PlayerMouseUp(object sender, MouseButtonEventArgs e)
        {
            moving = false;
            Mouse.Capture(null);
        }

        private void PlayerMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                Canvas.SetTop(this, e.GetPosition(_canvas).Y - this.Width / 2);
                Canvas.SetLeft(this, e.GetPosition(_canvas).X - this.Width / 2);
                e.Handled = true;
            }
        }
    }

    internal class MapImage : Image
    {
        public event EventHandler? OpacityChanged;

        public MapImage()
        {
            Source = new BitmapImage(new Uri(@"/assets/lol_map.png", UriKind.Relative));
            Opacity = 0;
        }

        public double _Opacity
        {
            set { Opacity = value; OpacityChanged(this, EventArgs.Empty); }
            get { return Opacity; }
        }
    }
}
