using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Tempest.utils;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for SketchWindow.xaml
    /// </summary>
    public partial class SketchWindow : Window
    {

        Point currentPoint;
        PointCollection points = new();

        public SketchWindow()
        {
            InitializeComponent();

            ShowInTaskbar = false;

            Loaded += SketchWindow_Loaded;
        }

        private void SketchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                Owner = this
            };
            mainWindow.Show();
            Create_Map();

            Create_Player("T");

            UiState.sketchCanvas = sketchCanvas;

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
                    if (Element is Line || Element is Polyline)
                    {
                        sketchCanvas.Children.Remove(Element);
                    }
                }
                foreach (UIElement Element in CurrentUiState)
                {
                    sketchCanvas.Children.Add(Element);
                }
            }

            public static void Undo()
            {
                if (!UiStates.Any()) { return; }
                CurrentUiState = UiStates[UiStates.Count - 1];
                UiStates.RemoveAt(UiStates.Count - 1);
                ReloadUI();
            }
        }

        private void Create_Map()
        {
            Services.mapImage.Width = sketchCanvas.ActualHeight - 50;
            Services.mapImage.Height = sketchCanvas.ActualHeight - 50;

            double left = sketchCanvas.ActualWidth / 4.4;
            Canvas.SetLeft(Services.mapImage, left);
            Canvas.SetTop(Services.mapImage, 25);
            Panel.SetZIndex(Services.mapImage, 0);
            sketchCanvas.Children.Add(Services.mapImage);
        }

        private Player Create_Player(string role)
        {
            Player player = new(role)
            {
                Width = 30,
                Height = 30,
                canvas = sketchCanvas,
                Background = Brushes.Red,
                CornerRadius = new CornerRadius(50),
            };
            Canvas.SetLeft(player, 500);
            Canvas.SetTop(player, 500);
            sketchCanvas.Children.Add(player);
            return player;
        }

        private void Draw(MouseEventArgs e)
        {
            Line line = new();

            SolidColorBrush brush = new(Services.brushColor);

            line.Stroke = brush;
            line.X1 = currentPoint.X;
            line.Y1 = currentPoint.Y;
            line.X2 = e.GetPosition(this).X;
            line.Y2 = e.GetPosition(this).Y;

            points.Add(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            currentPoint = e.GetPosition(this);

            sketchCanvas.Children.Add(line);
        } 

        private void CreateLine()
        {
            Polyline myPolyLine = new();

            SolidColorBrush brush = new(Color.FromArgb(255, 255, 139, 0));

            myPolyLine.Stroke = brush;
            myPolyLine.StrokeThickness = 2;

            myPolyLine.Points = new PointCollection(points);

            myPolyLine.MouseEnter += (x, y) => { PolyLineHover(x, y, myPolyLine); };

            UiState.Add(myPolyLine);

            points.Clear();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            currentPoint = e.GetPosition(this);
            Mouse.Capture(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            switch (Services.tool)
            {
                case 1:
                    CreateLine();
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
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                UiState.Undo();
            }
            if (e.Key == Key.X) // Map image testing shortcuts
            {
                Services.mapImage._Opacity = 0;
            }
            if (e.Key == Key.C)
            {
                Services.mapImage._Opacity = 1;
            }
        }

        private void PolyLineHover(object sender, MouseEventArgs e, Polyline polyline)
        {
            if (Services.tool == 2 && e.LeftButton == MouseButtonState.Pressed)
            {
                UiState.Remove(polyline);
            }
        }

    }
}
