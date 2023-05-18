using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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

            properties.Settings.Default.Reset();
        }

        private void Previous_Tab(object sender, RoutedEventArgs e)
        {
            if (tabContainer.SelectedIndex <= 0) { return; }
            tabContainer.SelectedIndex--;
            SaveButton.IsEnabled = false;
            NextButton.IsEnabled = true;
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
            if (tabContainer.SelectedIndex == tabContainer.Items.Count - 1) { SaveButton.IsEnabled = true; NextButton.IsEnabled = false; return; }
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            Dictionary<string, StringCollection> settingsDict = new();
            foreach (TabItem item in tabContainer.Items)
            {
                if (item.Content is LeaguePathsSelectionView)
                {
                    LeaguePathsSelectionView pathView = (LeaguePathsSelectionView)item.Content;
                    StringCollection leaguePaths = new();

                    if (pathView.leaguePaths.Count <= 0) 
                    {
                        settingsDict.Add("lol_location", leaguePaths);
                    } else
                    {
                        foreach (string path in pathView.leaguePaths)
                        {
                            leaguePaths.Add(path);
                        }
                        settingsDict.Add("lol_location", leaguePaths);
                    }
                    
                }
                if (item.Content is FolderSelectionView)
                {
                    FolderSelectionView folderView = (FolderSelectionView)item.Content;
                    StringCollection folders = new();

                    if (folderView.replayPaths.Count <= 0) 
                    { 
                        settingsDict.Add("rofl_paths", folders);
                    } else
                    {
                        foreach (string path in folderView.replayPaths)
                        {
                            folders.Add(path);
                        }
                        settingsDict.Add("rofl_paths", folders);
                    }

                    
                }
            }

            SaveSettings(settingsDict);

            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void SaveSettings(Dictionary<string, StringCollection> settingsDict)
        {
            if (settingsDict["lol_location"].Count != 0)
            {
                properties.Settings.Default.lol_location = settingsDict["lol_location"];
            }

            if (settingsDict["rofl_paths"].Count != 0)
            {
                properties.Settings.Default.rofl_paths = settingsDict["rofl_paths"];
            }
            properties.Settings.Default.initialized = true;
            properties.Settings.Default.Save();
        }
    }
}
