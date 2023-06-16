using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public class Player : Border
    {
        public Canvas? _canvas;
        private bool moving = false;
        public PlayerData data { get; set; }
        private Point offsetPoint = new();

        public Player()
        {
            PreviewMouseDown += PlayerMouseDown;
            PreviewMouseUp += PlayerMouseUp;
            Services.mapImage.OpacityChanged += UpdateOpacity;

            Panel.SetZIndex(this, 10000);

            Opacity = 0;

            Loaded += Player_Loaded;
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            Label roleText = new()
            {
                Content = data.Role,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Child = roleText;

            if (data.Team == "Blue") { Background = Brushes.Blue; }
            else { Background = Brushes.Red; }
        }

        private void UpdateOpacity(object? sender, EventArgs e)
        {
            Opacity = Services.mapImage.Opacity;
        }

        public Canvas canvas
        {
            set { _canvas = value; _canvas.PreviewMouseMove += PlayerMouseMove; }
        }

        public void ResetPosition()
        {
            double XOffset = Canvas.GetLeft(Services.mapImage);
            double YOffset = Canvas.GetTop(Services.mapImage);

            double XPosition = Services.mapImage.Width * data.Offsets.Item1 - Width / 2;
            double YPosition = Services.mapImage.Width * data.Offsets.Item2 - Width / 2;

            Canvas.SetLeft(this, XOffset + XPosition);
            Canvas.SetTop(this, YOffset + YPosition);
        }


        private void PlayerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Services.tool != 4) { return; }
            moving = true;
            Mouse.Capture(this);

            offsetPoint = e.GetPosition(this);
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
                Canvas.SetTop(this, e.GetPosition(_canvas).Y - offsetPoint.Y);
                Canvas.SetLeft(this, e.GetPosition(_canvas).X - offsetPoint.X);

                e.Handled = true;
            }
        }
    }

    public class Ward : Border
    {

        public bool moving = false;
        public Canvas? _canvas;
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


            Border child = new() { Width = 20, Height = 20, CornerRadius = new CornerRadius(50), Background = Brushes.Blue };

            child.PreviewMouseDown += WardMouseDown;
            child.PreviewMouseUp += WardMouseUp;
            child.MouseEnter += WardMouseEnter;

            Child = child;
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

    public class MapImage : Image
    {
        public event EventHandler? OpacityChanged;

        public MapImage()
        {
            Source = new BitmapImage(new Uri(@"/assets/lol_map.png", UriKind.Relative));
            _Opacity = 0;
        }

        public double _Opacity
        {
            set { Opacity = value; OnOpacityChange(); }
            get { return Opacity; }
        }

        protected virtual void OnOpacityChange()
        {
            if (OpacityChanged != null) OpacityChanged(this, EventArgs.Empty);
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

    public class Drawing
    {
        private List<DrawingPath> _Paths = new List<DrawingPath>();
        public List<DrawingPath> Paths
        {
            get { return _Paths; }
            set { _Paths = value; }
        }
        public bool MapState { get; set; }
    }

    public class DrawingPath
    {
        public string? Stroke { get; set; }
        public string? Data { get; set; }
        public double? BrushSize { get; set; }
    }

    public class PlaceholderTextBox : TextBox
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                "Placeholder",
                typeof(string),
                typeof(PlaceholderTextBox),
                new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public PlaceholderTextBox()
        {
            GotFocus += TextBox_GotFocus;
            LostFocus += TextBox_LostFocus;

            Loaded += PlaceholderTextBox_Loaded;
        }

        private void PlaceholderTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = Placeholder;
                Foreground = Brushes.Gray;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Text == Placeholder)
            {
                Text = string.Empty;
                Foreground = Brushes.WhiteSmoke;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = Placeholder;
                Foreground = Brushes.Gray;
            }
        }
    }

    public class PlayerData
    {
        public Tuple<double, double> Offsets { get; set; }
        public string Role { get; set; }
        public string Team { get; set; }
    }
}