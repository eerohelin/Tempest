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
        public DrawView()
        {
            InitializeComponent();
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
            SketchWindow.UiState.ClearWards();
        }

        private void MapToggleButton_Check(object sender, RoutedEventArgs e)
        {
            Services.mapImage._Opacity = 1;
            SketchWindow.UiState.fogOfWar.Opacity = .5;

            foreach (UIElement element in mapButtons.Children)
            {
                if ((object)element == sender) { continue; }

                if (element is Button)
                {
                    element.IsEnabled = true;
                    Button button = (Button)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                } else if (element is RadioButton)
                {
                    element.IsEnabled = true;
                    RadioButton button = (RadioButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                } else if (element is ToggleButton)
                {
                    element.IsEnabled = true;
                    ToggleButton button = (ToggleButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = 1;
                }
            }
        }

        private void MapToggleButton_Uncheck(object sender, RoutedEventArgs e)
        {
            Services.mapImage._Opacity = 0;
            SketchWindow.UiState.fogOfWar.Opacity = 0;

            foreach (UIElement element in mapButtons.Children)
            {
                if ((object)element == sender) { continue; }

                if (element is Button)
                {
                    element.IsEnabled = false;
                    Button button = (Button)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                } else if (element is RadioButton)
                {
                    element.IsEnabled = false;
                    RadioButton button = (RadioButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                } else if (element is ToggleButton)
                {
                    element.IsEnabled = false;
                    ToggleButton button = (ToggleButton)element;
                    Image image = (Image)button.Content;
                    image.Opacity = .3;
                }
            }
        }

        private void FogOfWarButton_Check(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.AdjustFoW("On");
        }

        private void FogOfWarButton_Uncheck(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.AdjustFoW("Off");
        }
    }
}
