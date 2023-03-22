using System;
using System.Collections.Generic;
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

namespace Tempest
{
    /// <summary>
    /// Interaction logic for OpenReplayView.xaml
    /// </summary>
    public partial class OpenReplayView : Window
    {

        public static string version = string.Empty;
        public OpenReplayView()
        {
            InitializeComponent();

            Loaded += OpenReplayView_Loaded;

        }

        private async void OpenReplayView_Loaded(object sender, RoutedEventArgs e)
        {
            version = await GetLatestVersion();

            CreateReplayComponents();
        
        }

        private void CreateReplayComponents()
        {
            string replayPath = "C:\\Users\\zedpl\\Documents\\League of Legends\\Replays";

            var replayPaths = Directory.GetFiles(replayPath, "*.rofl").OrderByDescending(d => new FileInfo(d).CreationTime);

            foreach (string replay in replayPaths)
            {
                ReplayObject replayObject = ROFLHandler.ParseROFL(replay);
                ReplayComponent replayComponent = new(replayObject, replay, this);
                replayContainer.Children.Add(replayComponent);
            }
        }

        private async Task<string> GetLatestVersion()
        {
            string url = "http://ddragon.leagueoflegends.com/api/versions.json";

            // best practice to create one HttpClient per Application and inject it
            HttpClient client = new();

            using HttpResponseMessage response = await client.GetAsync(url);
            using HttpContent content = response.Content;
            string json = await content.ReadAsStringAsync();

            JsonNode objects = JsonArray.Parse(json);
            return objects[0].ToString();
        }
    }
}
