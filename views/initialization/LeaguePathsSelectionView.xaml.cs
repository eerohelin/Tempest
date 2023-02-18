using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for LeaguePathsSelectionView.xaml
    /// </summary>
    public partial class LeaguePathsSelectionView : UserControl
    {

        public BindingList<string> leaguePaths = new();

        public LeaguePathsSelectionView()
        {
            InitializeComponent();

            leaguePathsContainer.ItemsSource = leaguePaths;
        }

        private void ScanButton_Clicked(object sender, RoutedEventArgs e)
        {
            List<string> paths =  LeaguePaths.GetPaths();

            foreach (string path in paths)
            {
                if (leaguePaths.Contains(path)) { continue; }
                leaguePaths.Add(path);
            }
        }

        private void AddButton_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "League of Legends (*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                if (leaguePaths.Contains(openFileDialog.FileName)) { return; }
                leaguePaths.Add(openFileDialog.FileName);
            }
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            leaguePaths.RemoveAt(leaguePathsContainer.SelectedIndex);
        }
    }
}
