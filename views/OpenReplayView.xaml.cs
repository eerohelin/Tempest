using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for OpenReplayView.xaml
    /// </summary>
    public partial class OpenReplayView : Window
    {

        public ReplayView _parent;
        public OpenReplayView(ReplayView parent)
        {
            InitializeComponent();
            _parent = parent;

            Loaded += OpenReplayView_Loaded;

        }

        private async void OpenReplayView_Loaded(object sender, RoutedEventArgs e)
        {
            CreateReplayComponents();
        
        }

        private void CreateReplayComponents()
        {
            List<string> replayPaths = new();
            foreach (string roflFolder in properties.Settings.Default.rofl_paths)
            {
                var tempFiles = Directory.GetFiles(roflFolder, "*.rofl");
                foreach (string tempFile in tempFiles)
                {
                    replayPaths.Add(tempFile);
                }
            }

            var orderedPaths = replayPaths.OrderByDescending(d => new FileInfo(d).CreationTime);

            foreach (string replay in orderedPaths)
            {
                ReplayObject replayObject = ROFLHandler.ParseROFL(replay);
                if (replayObject is null) { continue; }
                ReplayComponent replayComponent = new(replayObject, replay, this);
                replayContainer.Children.Add(replayComponent);
            }
        }

        public void CheckComponents()
        {
            foreach (ReplayComponent replayComponent in replayContainer.Children)
            {
                replayComponent.CheckLeagueVersion();
            }
        }
    }
}
