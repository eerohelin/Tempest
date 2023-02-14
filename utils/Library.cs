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

            Label roleText = new()
            {
                Content = role,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Child = roleText;

            Panel.SetZIndex(this, 10000);

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
            if (Services.tool != 4) { return; }
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

    public class ReplayObject
    {
        public int? gameLength { get; set; }
        public string? gameVersion { get; set; }
        public List<Summoner>? statsJson { get; set; }
    }


    public class Summoner
    {
        public string? ASSISTS { get; set; }
        public string? CHAMPIONS_KILLED { get; set; }
        public string? ID { get; set; }
        public string? INDIVIDUAL_POSITION { get; set; }
        public string? LEVEL { get; set; }
        public string? NAME { get; set; }
        public string? NUM_DEATHS { get; set; }
        public string? PLAYER_POSITION { get; set; }
        public string? PLAYER_ROLE { get; set; }
        public string? PUUID { get; set; }
        public string? SKIN { get; set; }
        public string? TEAM { get; set; }
        public string? TEAM_POSITION { get; set; }
        public string? VICTORY_POINT_TOTAL { get; set; }
        public string? WIN { get; set; }
    }
}
