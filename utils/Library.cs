using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Tempest
{
    internal class Player : Border
    {
        public Canvas? _canvas;
        private bool moving = false;
        private EllipseGeometry _ellipseGeometry;

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

            CreateVision();

            Opacity = 0;
        }

        private void CreateVision()
        {
            EllipseGeometry ellipseGeometry = new EllipseGeometry();
            ellipseGeometry.RadiusX = 60;
            ellipseGeometry.RadiusY = 60;

            _ellipseGeometry = ellipseGeometry;

            SketchWindow.UiState.clipGroup.Children.Add(_ellipseGeometry);
        }

        public void LoadPosition()
        {
            double imageLeft = Canvas.GetLeft(Services.mapImage);
            double imageTop = Canvas.GetTop(Services.mapImage);

            Point position = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

            double relativeX = position.X - imageLeft + (Width / 2);
            double relativeY = position.Y - imageTop + (Height / 2);
            
            _ellipseGeometry.Center = new Point(relativeX, relativeY);
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

                LoadPosition();
                e.Handled = true;
            }
        }
    }

    internal class Ward : Border
    {

        public bool moving = false;
        public Canvas? _canvas;
        private EllipseGeometry _ellipseGeometry;
        public Ward()
        {
            Opacity = Services.mapImage.Opacity;
            BorderBrush = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(1, 1, 1, 1);
            Width = 120;
            Height = 120;
            CornerRadius = new CornerRadius(90);

            Services.mapImage.OpacityChanged += UpdateOpacity;

            Panel.SetZIndex(this, 10000);

            CreateVision();


            Border child = new() { Width = 20, Height = 20, CornerRadius = new CornerRadius(50), Background = Brushes.Blue };

            child.PreviewMouseDown += WardMouseDown;
            child.PreviewMouseUp += WardMouseUp;
            child.MouseEnter += WardMouseEnter;

            Child = child;
        }

        private void CreateVision()
        {
            EllipseGeometry ellipseGeometry = new EllipseGeometry();
            ellipseGeometry.RadiusX = 60;
            ellipseGeometry.RadiusY = 60;

            _ellipseGeometry = ellipseGeometry;

            SketchWindow.UiState.clipGroup.Children.Add(_ellipseGeometry);
        }

        public void LoadPosition()
        {
            double imageLeft = Canvas.GetLeft(Services.mapImage);
            double imageTop = Canvas.GetTop(Services.mapImage);

            Point position = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));

            double relativeX = position.X - imageLeft + (Width / 2);
            double relativeY = position.Y - imageTop + (Height / 2);

            _ellipseGeometry.Center = new Point(relativeX, relativeY);
        }

        public Canvas canvas
        {
            set { _canvas = value; _canvas.PreviewMouseMove += WardMouseMove; }
        }

        private void WardMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Services.tool == 2) { SketchWindow.UiState.Remove(this); return; }
            if (Services.tool != 4) { return; }
            moving = true;
            Mouse.Capture(Child);
        }

        private void WardMouseUp(object sender, MouseButtonEventArgs e)
        {
            moving = false;
            Mouse.Capture(null);
        }

        private void UpdateOpacity(object? sender, EventArgs e)
        {
            Opacity = Services.mapImage.Opacity;
        }

        private void WardMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                Canvas.SetTop(this, e.GetPosition(_canvas).Y - this.Width / 2);
                Canvas.SetLeft(this, e.GetPosition(_canvas).X - this.Width / 2);

                LoadPosition();
                e.Handled = true;
            }
        }

        private void WardMouseEnter(object sender, MouseEventArgs e)
        {
            if (Services.tool == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                SketchWindow.UiState.Remove(this);
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
