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
            var fullFilePath = $"https://cdn.communitydragon.org/{OpenReplayView.version}/champion/{ChampionName}/square";


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
    }
}
