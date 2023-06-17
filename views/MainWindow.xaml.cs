using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Test add to lol_location
            //StringCollection strings = properties.Settings.Default.lol_location;
            //strings.Add(@"C:\Riot Games\League of Legends (Tournament Realm 1)\Game\League of Legends.exe");
            //properties.Settings.Default.lol_location = strings;
            //properties.Settings.Default.Save();

            Loaded += MainWindow_Loaded;
        }

        public class ProjectHandler
        {
            public static Project? CurrentProject;

            public static void SaveProject()
            {
                Project tempProject = new Project()
                {
                    Timestamps = GatherTimestamps(),
                    Tags = GatherTags(),
                    Drawings = GatherDrawings()
                };

                CurrentProject = tempProject;
            }


            private static List<TimestampData> GatherTimestamps()
            {
                List<TimestampData> tempList = new();

                foreach(Timestamp timestamp in ReplayView.tsContainer.Children)
                {
                    tempList.Add(timestamp.Data);
                }

                return tempList;
            }

            private static List<string> GatherTags()
            {
                return ReplayView.Tags.ToList();
            }

            private static List<Drawing> GatherDrawings()
            {
                List<Drawing> tempList = new();

                foreach(DrawingComponent drawingComponent in DrawView._drawingContainer.Children)
                {
                    tempList.Add(drawingComponent._Drawing);
                }
                
                return tempList;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LeaguePaths.LoadLeagueVersions();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            if(Height == 30)
            {
                Height = 600;
            }
            else
            {
               Height = 30;
            }
        }

        private void buttonClose_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void appBar_drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void drawStateChecked(object sender, RoutedEventArgs e)
        {
            Owner.Opacity = 1;
        }

        private void drawStateUnchecked(object sender, RoutedEventArgs e)
        {
            Owner.Opacity = 0;
        }
    }
}
