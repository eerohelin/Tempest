using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : UserControl
    {
        private static WrapPanel _mapButtons;
        public static ToggleButton _mapToggleButton;
        public DrawView()
        {
            InitializeComponent();

            _mapButtons = mapButtons;
            _mapToggleButton = mapToggleButton;
        }

        private void SwitchTool(object sender, RoutedEventArgs e)
        {
            var myValue = ((RadioButton)sender).Tag;
            int tool = Int32.Parse(myValue.ToString());
            Services.tool = tool;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.Undo();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.ClearLines();
        }

        private void MapToggleButton_Check(object sender, RoutedEventArgs e)
        {
            TurnMapOn(sender);
        }

        private void MapToggleButton_Uncheck(object sender, RoutedEventArgs e)
        {
            TurnMapOff(sender);
        }

        public static void TurnMapOn(object sender)
        {
            Services.mapImage._Opacity = 1;

            foreach (UIElement element in _mapButtons.Children)
            {
                if ((object)element == sender) { continue; }

                if (element is Button)
                {
                    element.IsEnabled = true;
                    Button button = (Button)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                }
                else if (element is RadioButton)
                {
                    element.IsEnabled = true;
                    RadioButton button = (RadioButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                }
                else if (element is ToggleButton)
                {
                    element.IsEnabled = true;
                    ToggleButton button = (ToggleButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                }
            }
        }

        public static void TurnMapOff(object sender)
        {
            Services.mapImage._Opacity = 0;

            foreach (UIElement element in _mapButtons.Children)
            {
                if ((object)element == sender) { continue; }

                if (element is Button)
                {
                    element.IsEnabled = false;
                    Button button = (Button)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                }
                else if (element is RadioButton)
                {
                    element.IsEnabled = false;
                    RadioButton button = (RadioButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                }
                else if (element is ToggleButton)
                {
                    element.IsEnabled = false;
                    ToggleButton button = (ToggleButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                }
            }
        }

        private void ColorChangeButton_Clicked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            Services.brushColor = (Color)ColorConverter.ConvertFromString(button.Background.ToString());
        }

        private void ClearDrawingsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveDrawingsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDrawing();
        }

        private void SaveDrawing()
        {
            string title = DrawingTitleTextBox.Text.Length != 0 ? DrawingTitleTextBox.Text : $"Timestamp #{drawingContainer.Children.Count + 1}";
            drawingContainer.Children.Add(new DrawingComponent() { Title = title });

            DrawingTitleTextBox.Text = "";
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var element in SketchWindow.UiState.sketchCanvas.Children)
            {
                if (element is Player)
                {
                    Player player = (Player)element;
                    player.ResetPosition();
                }
            }
        }

        private void OnDrawingTitleTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SaveDrawing();
            }
        }
    }
}
