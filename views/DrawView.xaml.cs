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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for DrawView.xaml
    /// </summary>
    public partial class DrawView : UserControl
    {

        private bool mapOn { get; set; }
        public DrawView()
        {
            InitializeComponent();

            mapOn = false;
        }

        private void SwitchTool(object sender, RoutedEventArgs e)
        {
            var myValue = ((Button)sender).Tag;
            int tool = Int32.Parse(myValue.ToString());
            Services.tool = tool;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.Undo();
        }

        private void ToggleMap(object sender, RoutedEventArgs e)
        {
            if (mapOn)
            {
                Services.mapImage._Opacity = 0;
                mapOn = false;
                return;
            }
            Services.mapImage._Opacity = 1;
            mapOn = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SketchWindow.UiState.ClearLines();
        }
    }
}
