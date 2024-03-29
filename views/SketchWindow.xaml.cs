﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for SketchWindow.xaml
    /// </summary>
    public partial class SketchWindow : Window
    {

        Point currentPoint;
        PointCollection points = new();
        List<UIElement> arrowLineGroup = new();

        public SketchWindow()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            Loaded += SketchWindow_Loaded;

            Services.ToolChanged += Services_ToolChanged;
        }

        private async void SketchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                Owner = this
            };
            await MainWindow.ChampionImageCacheHandler.LoadAsync();
            mainWindow.Show();
            Create_Map();

            UiState.sketchCanvas = sketchCanvas;
            Load_Players();
        }

        public class UiState : List<UIElement>
        {
            public static List<UIElement> CurrentUiState = new();
            public static List<List<UIElement>> UiStates = new();

            public static Canvas sketchCanvas = new();

            public static new void Add(UIElement Item)
            {
                UiStates.Add(new List<UIElement>(CurrentUiState));
                CurrentUiState.Add(Item);

                if (UiStates.Count > 20) { UiStates.RemoveAt(0); }

                ReloadUI();
            }

            public static new void Remove(UIElement Item)
            {
                UiStates.Add(new List<UIElement>(CurrentUiState));
                CurrentUiState.Remove(Item);
                ReloadUI();
            }

            public static void ReloadUI()
            {
                UIElement[] tempArray = new UIElement[sketchCanvas.Children.Count];
                sketchCanvas.Children.CopyTo(tempArray, 0);
                foreach (UIElement Element in tempArray)
                {
                    if (Element is Line || Element is Path || Element is Polyline || Element is Canvas || Element is Ward)
                    {
                        if (CurrentUiState.Contains(Element)) { continue; }
                        sketchCanvas.Children.Remove(Element);
                    }
                }
                foreach (UIElement Element in CurrentUiState)
                {
                    if (!sketchCanvas.Children.Contains(Element)) { sketchCanvas.Children.Add(Element); }
                    
                }
            }

            public static void Undo()
            {
                if (!UiStates.Any()) { return; }
                CurrentUiState = UiStates[UiStates.Count - 1];
                UiStates.RemoveAt(UiStates.Count - 1);
                ReloadUI();
            }

            public static void ClearLines()
            {
                UiStates.Add(new List<UIElement>(CurrentUiState));
                foreach (UIElement Element in new List<UIElement>(CurrentUiState))
                {
                    if (Element is Line || Element is Path || Element is Polyline || Element is Canvas)
                    {
                        CurrentUiState.Remove(Element);
                    }
                }
                foreach (UIElement Element in new List<UIElement>(CurrentUiState))
                {
                    if (Element is Ward)
                    {
                        Ward ward = (Ward)Element;
                        CurrentUiState.Remove(Element);
                    }
                }
                ReloadUI();
            }

            public static void ClearPlayers()
            {
                UIElement[] tempArray = new UIElement[sketchCanvas.Children.Count];
                sketchCanvas.Children.CopyTo(tempArray, 0);
                foreach(UIElement child in tempArray)
                {
                    if (child is Player)
                    {
                        sketchCanvas.Children.Remove(child);
                    }
                }
            }
        }

        private void Services_ToolChanged(object? sender, EventArgs e)
        {
            switch(Services.tool)
            {
                case 1:
                    Cursor = Cursors.Pen;
                    break;
                case 2:
                    Cursor = Cursors.Arrow;
                    break;
                case 3:
                    Cursor = Cursors.Cross;
                    break;
                case 4:
                    Cursor = Cursors.Arrow;
                    break;
                case 5:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void Load_Players()
        {
            foreach(PlayerData playerData in defaultPlayerData())
            {
                Create_Player(playerData);
            }
        }

        private List<PlayerData> defaultPlayerData()
        {
            List<PlayerData> playerDatas = new()
            {
                new PlayerData() { Offsets = new Tuple<double, double>(0.1351, 0.2136), Role = "T", Team = "Blue" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.2165, 0.1379), Role = "T", Team = "Red" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.2107, 0.5077), Role = "J", Team = "Blue" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.7998, 0.5019), Role = "J", Team = "Red" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.4741, 0.5316), Role = "M", Team = "Blue" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.5383, 0.4808), Role = "M", Team = "Red" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.7874, 0.8448), Role = "B", Team = "Blue" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.8573, 0.7778), Role = "B", Team = "Red" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.8170, 0.8803), Role = "S", Team = "Blue" },
                new PlayerData() { Offsets = new Tuple<double, double>(0.8879, 0.8065), Role = "S", Team = "Red" }
            };
            return playerDatas;
        }

        private void Create_Map()
        {
            Services.mapImage.Width = sketchCanvas.ActualHeight - 50;
            Services.mapImage.Height = sketchCanvas.ActualHeight - 50;


            double left = sketchCanvas.ActualWidth / 4.4;
            Canvas.SetLeft(Services.mapImage, left);
            Canvas.SetTop(Services.mapImage, 25);
            Panel.SetZIndex(Services.mapImage, -3);
            sketchCanvas.Children.Add(Services.mapImage);
        }

        private Ward Create_Ward(Point point)
        {
            Ward ward = new()
            {
                canvas = sketchCanvas
            };

            Canvas.SetLeft(ward, point.X - (ward.Width / 2));
            Canvas.SetTop(ward, point.Y - (ward.Height / 2));
            UiState.Add(ward);
            return ward;
        }

        public static Player Create_Player(PlayerData data)
        {
            Player player = new()
            {
                Width = 30,
                Height = 30,
                canvas = UiState.sketchCanvas,
                CornerRadius = new CornerRadius(50),
                data = data
            };
            UiState.sketchCanvas.Children.Add(player);
            player.InitializePosition();
            return player;
        }

        private void Draw(MouseEventArgs e)
        {

            SolidColorBrush brush = new(Services.brushColor);
            Line line = new()
            {
                Stroke = brush,
                StrokeThickness = Services.brushSize,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                X1 = currentPoint.X,
                Y1 = currentPoint.Y,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };

            points.Add(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            currentPoint = e.GetPosition(this);

            sketchCanvas.Children.Add(line);
        } 

        private void CreateLine()
        {
            if (points.Count <= 0 ) { return; }
            Point previousPoint = points[0];
            for (int i = 1; i < points.Count; i++) // Remove points which are too close to eachother
            {
                if (points.Count <= 3) { break; }
                Point currentPoint = points[i];
                double distance = Math.Sqrt(Math.Pow(currentPoint.X - previousPoint.X, 2) + Math.Pow(currentPoint.Y - previousPoint.Y, 2));
                if (distance < 3)
                {
                    points.RemoveAt(i);
                    i--;
                }
                else
                {
                    previousPoint = currentPoint;
                }
            }

            SolidColorBrush brush = new(Services.brushColor);
            switch(Services.lineSmoothing)
            {
                case true:
                    Path path = SmoothLine.Smooth(points);
                    path.Stroke = brush;
                    path.StrokeThickness = Services.brushSize;
                    path.StrokeEndLineCap = PenLineCap.Round;
                    path.StrokeStartLineCap = PenLineCap.Round;
                    path.SnapsToDevicePixels = true;

                    path.MouseEnter += (x, y) => { LineHover(x, y, path); };

                    UiState.Add(path);
                    break;
                case false:
                    Polyline polyline = new()
                    {
                        Points = new PointCollection(points),
                        Stroke = brush,
                        StrokeThickness = Services.brushSize,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round,
                        SnapsToDevicePixels = true
                    };

                    polyline.MouseEnter += (x, y) => { LineHover(x, y, polyline); };

                    UiState.Add(polyline);
                    break;
            }
            points.Clear();
        }

        private void DrawArrow(MouseEventArgs e)
        {
            foreach(UIElement element in arrowLineGroup)
            {
                sketchCanvas.Children.Remove(element);
            }

            arrowLineGroup.Clear();


            int angle = 25; // Angle of the lines on the Arrow


            double midPointX = (currentPoint.X + e.GetPosition(this).X) / 2;
            double midPointY = (currentPoint.Y + e.GetPosition(this).Y) / 2;

            SolidColorBrush brush = new(Services.brushColor);

            Line mainLine = new()
            {
                Stroke = brush,
                StrokeThickness = Services.brushSize,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                X1 = currentPoint.X,
                Y1 = currentPoint.Y,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };

            Line arrowLine1 = new()
            {
                Stroke = brush,
                StrokeThickness = Services.brushSize,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                X1 = midPointX,
                Y1 = midPointY,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };

            Line arrowLine2 = new()
            {
                Stroke = brush,
                StrokeThickness = Services.brushSize,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                X1 = midPointX,
                Y1 = midPointY,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };

            RotateTransform rotate1 = new()
            {
                CenterX = e.GetPosition(this).X,
                CenterY = e.GetPosition(this).Y,
                Angle = angle
            };
            RotateTransform rotate2 = new()
            {
                CenterX = e.GetPosition(this).X,
                CenterY = e.GetPosition(this).Y,
                Angle = -angle
            };

            arrowLine1.RenderTransform = rotate1;
            arrowLine2.RenderTransform = rotate2;

            arrowLineGroup.Add(mainLine);
            arrowLineGroup.Add(arrowLine1);
            arrowLineGroup.Add(arrowLine2);
            sketchCanvas.Children.Add(mainLine);
            sketchCanvas.Children.Add(arrowLine1);
            sketchCanvas.Children.Add(arrowLine2);
        }

        private void CreateArrow()
        {
            Canvas arrowContainer = new();
            foreach(UIElement element in new List<UIElement>(arrowLineGroup))
            {
                sketchCanvas.Children.Remove(element);
                arrowContainer.Children.Add(element);
            }

            arrowLineGroup.Clear();

            arrowContainer.MouseEnter += (x, y) => { LineHover(x, y, arrowContainer); };
            UiState.Add(arrowContainer);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch(Services.tool)
            {
                case 1:
                    currentPoint = e.GetPosition(this);
                    Mouse.Capture(this);
                    break;
                case 3:
                    currentPoint = e.GetPosition(this);
                    Mouse.Capture(this);
                    break;
                case 5:
                    Ward ward = Create_Ward(e.GetPosition(this));
                    Mouse.Capture(ward.Child);
                    ward.moving = true;
                    break;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            switch (Services.tool)
            {
                case 1:
                    CreateLine();
                    break;
                case 3:
                    CreateArrow();
                    break;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                switch(Services.tool)
                {
                    case 1:
                        Draw(e);
                        break;
                    case 3:
                        DrawArrow(e);
                        break;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                UiState.Undo();
            }
        }
        public static void LineHover(object sender, MouseEventArgs e, UIElement element)
        {
            if (Services.tool == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                UiState.Remove(element);
            }
        }

    }
}
