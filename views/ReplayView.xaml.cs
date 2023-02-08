using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ReplayView.xaml
    /// </summary>
    public partial class ReplayView : UserControl
    {

        public static BindingList<string> CurrentTags = new();
        public static BindingList<string> Tags = new();
        public static StackPanel tsContainer = new();
        public static TagFilterPopup tagFilterPopup = new();

        public ReplayView()
        {
            InitializeComponent();

            //timestampContainer.Children.Add(new Timestamp("Test1", 500)); Example Timestamp

            Loaded += ReplayWindow_loaded;
        }

        public class TagManager
        {

            public static void CreateTag(string tag)
            {
                Tags.Add(tag);
                tagFilterPopup.CreateTag(tag);

                foreach(Timestamp timestamp in tsContainer.Children)
                {
                    timestamp.AddTag(tag);
                }
            }

            public static void DeleteTag(string tag)
            {
                Tags.Remove(tag);
                tagFilterPopup.RemoveTag(tag);

                foreach (Timestamp timestamp in tsContainer.Children)
                {
                    timestamp.RemoveTag(tag);
                }
                CurrentTags.Remove(tag);
            }

            public static void AddTag(string tag, Timestamp timestamp)
            {
                timestamp.Tags.Add(tag);
            }

            public static void RemoveTag(string tag, Timestamp timestamp)
            {
                timestamp.Tags.Remove(tag);
            }
        }

        private void ReplayWindow_loaded(object sender, RoutedEventArgs e)
        {
            ContextMenuService.SetIsEnabled(this, false);

            CurrentTags.ListChanged += CurrentTags_ListChanged;
            tsContainer = timestampContainer;
        }

        private void CurrentTags_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (CurrentTags.Count <= 0 ) { ShowAllTimestamps(); return; }
            foreach (Timestamp timestamp in timestampContainer.Children)
            {
                if (timestamp.Tags.Any(item => CurrentTags.Contains(item)))
                {
                    timestamp.Visibility = Visibility.Visible;
                    continue;
                }
                timestamp.Visibility = Visibility.Hidden;
            }
        }

        private void ShowAllTimestamps()
        {
            foreach (Timestamp timestamp in timestampContainer.Children)
            {
                timestamp.Visibility = Visibility.Visible;
            }
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

        private void onAddTimestampButtonClick(object sender, RoutedEventArgs e)
        {
            AddTimestamp();
        }

        private void AddTimestamp()
        {
            string[] splitTime = Regex.Split(timeTextBox.Text, @"[;:.,]+");

            int minutes = Int32.Parse(splitTime[0]);
            int seconds = Int32.Parse(splitTime[1]);

            int totalSeconds = minutes * 60 + seconds;

            string title = titleTextBox.Text.Length != 0 ? titleTextBox.Text : $"Timestamp #{timestampContainer.Children.Count + 1}";

            timestampContainer.Children.Add(new Timestamp(title, totalSeconds));

            titleTextBox.Text = "";
            timeTextBox.Text = "";
        }

        private void onTitleTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                timeTextBox.Focus();
            }
        }

        private void onTimeTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (!addTimestampButton.IsEnabled) { return; }

            if (e.Key == Key.Return)
            {
                AddTimestamp();
            }
        }

        private void onTagFilterButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            tagFilterPopup.PlacementTarget = button;
            tagFilterPopup.IsOpen = true;
        }

        private void onClearButtonClick(object sender, RoutedEventArgs e)
        {
            timestampContainer.Children.Clear();
        }
    }
}
