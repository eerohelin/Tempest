using System;
using System.Collections.Generic;
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
    /// Interaction logic for Timestamp.xaml
    /// </summary>
    public partial class Timestamp : UserControl
    {

        private int _time;
        private string _title;
        public List<string> Tags = new();
        public Timestamp(string title, int time)
        {
            InitializeComponent();

            _title = title;
            _time = time;

            titleLabel.Content = _title;
            timeLabel.Content = _time;

            Tags.Add("test");
        }

        private async void onPlayTimeButtonClick(object sender, RoutedEventArgs e)
        {
            await Replay.setPosition(_time);
        }

        private void onDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            StackPanel _parent = (StackPanel)VisualTreeHelper.GetParent(this);
            _parent.Children.Remove(this);
        }
    }
}
