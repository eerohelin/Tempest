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

        private void ReplayComponent_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Summoner summoner in _replay.statsJson)
            {
                if (_owner._parent.championImageCache.ContainsKey(summoner.SKIN))
                {
                    playerContainer.Children.Add(CreateImage(_owner._parent.championImageCache[summoner.SKIN]));
                }
                else
                {
                    BitmapImage championBitmap = LoadChampionImage(summoner.SKIN);
                    _owner._parent.championImageCache.Add(summoner.SKIN, championBitmap);

                    Image championImage = CreateImage(championBitmap);
                    playerContainer.Children.Add(championImage);
                }
            }

            var parsedROFLVersion = _replay.gameVersion.ToString().Split('.').Take(2);
            string roflVersion = string.Join(".", parsedROFLVersion);
            _roflVersion = roflVersion;

            ROFLName.Content = System.IO.Path.GetFileName(_filePath);
            ROFLVersion.Content = _roflVersion;

            CheckLeagueVersion();
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

        private Image CreateImage(BitmapImage bitmapImage)
        {
            var image = new Image();

            image.Source = bitmapImage;
            image.Width = 40;
            image.Height = 40;

            return image;
        }

        private BitmapImage LoadChampionImage(string ChampionName)
        {
            var fullFilePath = $"https://cdn.communitydragon.org/latest/champion/{ChampionName}/square";


            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            return bitmap;
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
