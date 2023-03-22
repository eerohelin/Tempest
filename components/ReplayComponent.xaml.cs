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
            ROFLName.Content = System.IO.Path.GetFileName(_filePath);
            foreach(Summoner summoner in _replay.statsJson)
            {
                playerContainer.Children.Add(ChampionImage(summoner.SKIN));
            }
            
        }

        private Image ChampionImage(string ChampionName)
        {
            var image = new Image();
            var fullFilePath = $"https://ddragon.leagueoflegends.com/cdn/{OpenReplayView.version}/img/champion/{ChampionName}.png";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;
            image.Width = 40;
            image.Height = 40;
            return image;
        }

        private void PlayReplayButton_Clicked(object sender, RoutedEventArgs e)
        {
            ROFLHandler.OpenROFL(_filePath);
            _owner.Close();
        }
    }
}
