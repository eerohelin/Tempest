using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ReplayView.xaml
    /// </summary>
    public partial class ReplayView : UserControl
    {
        public ReplayView()
        {
            InitializeComponent();

            //timestampContainer.Children.Add(new Timestamp("Test1", 500)); Example Timestamp
        }

        private void onTimeTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Regex regex = new(@"^[0-9]+[,.;:][0-9]{1,2}$");
            Button button = addTimestampButton;

            if (regex.IsMatch(textBox.Text))
            {
                button.IsEnabled = true;
                return;
            }
            button.IsEnabled = false;
        }

        private async void onGetCurrentTimeButtonClick(object sender, RoutedEventArgs e)
        {
            var response = await Replay.getPosition();
            if (response == null) { return; }

            int currentTime = (int)(double)response["time"];

            TimeSpan parsedTime = TimeSpan.FromSeconds(currentTime);

            timeTextBox.Text = $"{parsedTime.Minutes}:{parsedTime.Seconds}";
        }
    }
}
