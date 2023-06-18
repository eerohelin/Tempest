using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    /// Interaction logic for Timestamp.xaml
    /// </summary>
    public partial class Timestamp : UserControl
    {
        public List<string> Tags = new();
        public BindingList<string> AvailableTags = new();
        public TimestampTagPopup Popup = new();
        public TimestampData Data { get; set; }
        public Timestamp()
        {
            InitializeComponent();

            Loaded += Timestamp_Loaded;
        }

        private void Timestamp_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSpan parsedTime = TimeSpan.FromSeconds(Data.Time);
            Popup._owner = this;

            titleLabel.Content = Data.Title;
            timeLabel.Content = $"{(int)parsedTime.TotalMinutes}:{parsedTime.Seconds}";

            GetTags();
            CheckTags();
        }

        public void AddTag(string tag)
        {
            if (AvailableTags.Contains(tag)) { return; }
            AvailableTags.Add(tag);
            Popup.CreateTag(tag);
        }

        public void RemoveTag(string tag)
        {
            AvailableTags.Remove(tag);
            Tags.Remove(tag);
        }


        private void GetTags()
        {
            foreach (string tag in ReplayView.Tags)
            {
                AddTag(tag);
            }
        }

        private void CheckTags()
        {
            if (ReplayView.CurrentTags.Count > 0)
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private async void onPlayTimeButtonClick(object sender, RoutedEventArgs e)
        {
            await Replay.setPosition(Data.Time);
        }

        private void onDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            StackPanel _parent = (StackPanel)VisualTreeHelper.GetParent(this);
            _parent.Children.Remove(this);
        }

        private void TagMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Popup.PlacementTarget = button;
            Popup.IsOpen = true;
        }
    }
}
