using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for FolderSelectionView.xaml
    /// </summary>
    public partial class FolderSelectionView : UserControl
    {

        public BindingList<string> replayPaths = new();
        public FolderSelectionView()
        {
            InitializeComponent();

            foldersListBox.ItemsSource = replayPaths;

            CheckDocuments();

            Loaded += FolderSelectionView_Loaded;
        }

        private void CheckDocuments()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString(), @"League of Legends\Replays");

            if (System.IO.Directory.Exists(path))
            {
                replayPaths.Add(path);
            }
        }

        private void FolderSelectionView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddFolderButton_Clicked(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFolder = dialog.SelectedPath;
                replayPaths.Add(selectedFolder);
            }

        }
    }
}
