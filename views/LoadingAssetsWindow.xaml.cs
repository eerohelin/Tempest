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
using System.Windows.Shapes;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for LoadingAssetsWindow.xaml
    /// </summary>
    public partial class LoadingAssetsWindow : Window
    {

        public ProgressBar _progressBar;
        public bool _isLoading = true;
        public LoadingAssetsWindow()
        {
            InitializeComponent();

            _progressBar = AssetProgressBar;

            Closing += LoadingAssetsWindow_Closing;
        }

        private void LoadingAssetsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
           if (_isLoading) { e.Cancel = true; return; }
           e.Cancel = false;
        }
    }
}
