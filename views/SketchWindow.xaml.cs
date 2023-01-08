using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using static System.Net.Mime.MediaTypeNames;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for SketchWindow.xaml
    /// </summary>
    public partial class SketchWindow : Window
    {

        Point currentPoint;
        PointCollection points = new();
        List<Polyline> lines = new();
        List<Line> tempLines = new();

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

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            currentPoint = e.GetPosition(this);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Polyline myPolyLine = new();

            SolidColorBrush brush = new(Color.FromArgb(255, 255, 139, 0));

            myPolyLine.Stroke = brush;
            myPolyLine.StrokeThickness = 2;

            myPolyLine.Points = new PointCollection(points);

            myPolyLine.MouseEnter += (x, y) => { PolyLineHover(x, y, myPolyLine); };

            sketchCanvas.Children.Add(myPolyLine);
            lines.Add(myPolyLine);

            points.Clear();

            foreach(Line line in tempLines)
            {
                sketchCanvas.Children.Remove(line);
            }
            tempLines.Clear();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            //return;
            if (e.LeftButton == MouseButtonState.Pressed && Services.tool == 1)
            {
                Line line = new();

                SolidColorBrush brush = new(Services.brushColor);

                line.Stroke = brush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                points.Add(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));
                tempLines.Add(line);

                currentPoint = e.GetPosition(this);

                sketchCanvas.Children.Add(line);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Services.tool = 2;
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
                sketchCanvas.Children.Remove(polyline);
                lines.Remove(polyline);
            }
        }

    }
}
