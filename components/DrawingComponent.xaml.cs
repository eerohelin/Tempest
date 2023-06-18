using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for DrawingComponent.xaml
    /// </summary>
    public partial class DrawingComponent : UserControl
    {
        public string Title { get; set; }
        public Drawing _Drawing;
        public DrawingComponent()
        {
            InitializeComponent();

            Loaded += DrawingComponent_Loaded;
        }

        private void DrawingComponent_Loaded(object sender, RoutedEventArgs e)
        {
            titleLabel.Content = Title;
            if (_Drawing == null)
            {
                _Drawing = new Drawing();
                SaveDrawing();
            }
        }

        private void SaveDrawing()
        {
            foreach (var child in SketchWindow.UiState.sketchCanvas.Children)
            {
                if (child is System.Windows.Shapes.Path)
                {
                    System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)child;
                    _Drawing.Paths.Add(new DrawingPath() { Data = path.Data.GetFlattenedPathGeometry().ToString(), Stroke = path.Stroke.ToString() });
                }

                if (child is Player)
                {
                    Player player = (Player)child;
                    Tuple<double, double> newTuple = new Tuple<double, double>(Canvas.GetLeft(player), Canvas.GetTop(player));
                    _Drawing.PlayerDatas.Add(new PlayerData() { Offsets = player.data.Offsets, Role = player.data.Role, Team = player.data.Team, Position = newTuple});
                }
            }

            switch (Services.mapImage.Opacity)
            {
                case 0:
                    {
                        _Drawing.MapState = false;
                        break;
                    }
                case 1:
                    {
                        _Drawing.MapState = true;
                        break;
                    }
            }
            _Drawing.Title = Title;
            
        }

        private void LoadDrawing()
        {
            SketchWindow.UiState.ClearLines();
            foreach (DrawingPath path in _Drawing.Paths)
            {
                DrawPath(path);
            }

            if (_Drawing.MapState) { DrawView._mapToggleButton.IsChecked = true; }
            else { DrawView._mapToggleButton.IsChecked = false; }

            SketchWindow.UiState.ClearPlayers();
            foreach(PlayerData playerData in _Drawing.PlayerDatas)
            {
                Player player = SketchWindow.Create_Player(playerData);
            }
        }

        private void DrawPath(DrawingPath path)
        {
            Path newPath = new()
            {
                StrokeThickness = Services.brushSize,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                SnapsToDevicePixels = true

            };
            newPath.MouseEnter += (x, y) => { SketchWindow.LineHover(x, y, newPath); };
            newPath.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(path.Stroke));
            newPath.Data = ParseGeometry(path.Data);
            SketchWindow.UiState.Add(newPath);
        }

        private Geometry ParseGeometry(string geometry)
        {
            string parsedGeometry = geometry.Replace(",", ".").Replace(";", ",");
            return Geometry.Parse(parsedGeometry);
        }

        private void LoadButton_Clicked(object sender, RoutedEventArgs e)
        {
            LoadDrawing();
        }
    }
}
