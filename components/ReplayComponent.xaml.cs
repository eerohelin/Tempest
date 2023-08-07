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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for ReplayComponent.xaml
    /// </summary>
    public partial class ReplayComponent : StackPanel
    {

        private ReplayObject _replay;
        private string _filePath;
        private OpenReplayView _owner;
        private string _roflVersion;
        public ReplayComponent(ReplayObject replay, string filePath, OpenReplayView owner)
        {
            InitializeComponent();
            _replay = replay;
            _filePath = filePath;
            _owner = owner;


            Loaded += ReplayComponent_Loaded;
        }

        public ReplayObject GetReplayObject() { return _replay; }

        private void ReplayComponent_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Summoner summoner in _replay.statsJson)
            {
                LoadImage(summoner);
            }

            var parsedROFLVersion = _replay.gameVersion.ToString().Split('.').Take(2);
            string roflVersion = string.Join(".", parsedROFLVersion);
            _roflVersion = roflVersion;

            ROFLName.Content = System.IO.Path.GetFileName(_filePath);
            ROFLVersion.Content = _roflVersion;

            CheckLeagueVersion();
        }

        private void LoadImage(Summoner summoner)
        {
            Image championImage = CreateImage();
            championImage.ToolTip = summoner.SKIN;
            championImage.Source = MainWindow.ChampionImageCacheHandler.GetChampionBitmap(summoner.SKIN.ToLower());
            playerContainer.Children.Add(championImage);
        }

        public void CheckLeagueVersion()
        {
            if (!Services.leagueVersions.ContainsKey(_roflVersion))
            {
                ROFLVersion.Foreground = new SolidColorBrush(Colors.Red);
                ROFLVersion.ToolTip = "No League version found capable of opening this version";
                playButton.IsEnabled = false;
                ReplayAPIButton.Visibility = Visibility.Collapsed;
            }
            else if (!LeaguePaths.CheckReplayAPI(Services.leagueVersions[_roflVersion]))
            {
                ROFLVersion.Foreground = new SolidColorBrush(Colors.Yellow);
                ROFLVersion.ToolTip = "Replay API not enabled for this League version. Features such as timestamps will not work.";
                playButton.IsEnabled = true;
                ReplayAPIButton.Visibility = Visibility.Visible;
            }
            else
            {
                ROFLVersion.Foreground = new SolidColorBrush(Colors.LightGreen);
                ROFLVersion.ToolTip = Services.leagueVersions[_roflVersion];
                playButton.IsEnabled = true;
                ReplayAPIButton.Visibility = Visibility.Collapsed;
            }
        }

        private Image CreateImage(BitmapImage bitmap = null)
        {
            var image = new Image();
            if (bitmap == null)
            {
                BitmapImage placeholderImage = new BitmapImage(new Uri("/assets/noicon.png", UriKind.Relative));
                image.Source = placeholderImage;
            } else
            {
                image.Source = bitmap;
            }
            
            image.Width = 40;
            image.Height = 40;

            return image;
        }

        private void PlayReplayButton_Clicked(object sender, RoutedEventArgs e)
        {
            ROFLHandler.OpenROFL(_filePath);
            _owner.Close();
        }

        private void EnableReplayAPI_Click(object sender, RoutedEventArgs e)
        {
            LeaguePaths.EnableReplayAPI(Services.leagueVersions[_roflVersion]);
            _owner.CheckComponents();
        }
    }
}
