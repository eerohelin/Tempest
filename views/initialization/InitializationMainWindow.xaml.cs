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

namespace Tempest
{
    /// <summary>
    /// Interaction logic for InitializationMainWindow.xaml
    /// </summary>
    public partial class InitializationMainWindow : Window
    {
        public InitializationMainWindow()
        {
            InitializeComponent();
        }

        private void Previous_Tab(object sender, RoutedEventArgs e)
        {
            if (tabContainer.SelectedIndex <= 0) { return; }
            tabContainer.SelectedIndex--;
        }

        private void Next_Tab(object sender, RoutedEventArgs e)
        {
            if (tabContainer.SelectedIndex == (tabContainer.Items.Count - 1)) { return; }
            TabItem item = (TabItem)tabContainer.Items[tabContainer.SelectedIndex];
            if (item.Content is LeaguePathsSelectionView)
            {
                LeaguePathsSelectionView content = (LeaguePathsSelectionView)item.Content;
                if (content.leaguePaths.Count == 0)
                {
                    if (MessageBox.Show("A path to a League of Legends.exe is required in order to open Replay files using this program.\nAre you sure you want to continue?",
                        "Tempest Setup",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }
            tabContainer.SelectedIndex++;
        }
    }
}
