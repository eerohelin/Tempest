using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public static MenuItem openRecentMenu = new();
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LeaguePaths.LoadLeagueVersions();

            Dispatcher.BeginInvoke(new Action(CheckArgs), System.Windows.Threading.DispatcherPriority.Background);

            openRecentMenu = OpenRecentMenu;
            LoadRecentProjects();

        }

        public class ProjectHandler
        {
            public static Project? CurrentProject;

            public static void SaveProject(string path)
            {
                Project tempProject = new Project()
                {
                    Timestamps = GatherTimestamps(),
                    Tags = GatherTags(),
                    Drawings = GatherDrawings(),
                    Path = path
                };

                CurrentProject = tempProject;

                SaveToRecentProjects(path);
            }

            public static void NewProject()
            {
                CurrentProject = null;
                ReplayView.CurrentTags.Clear();
                ReplayView.tsContainer.Children.Clear();
            }

            public static void LoadProject(string path)
            {
                string stream = File.ReadAllText(path);
                Project loadedProject = JsonSerializer.Deserialize<Project>(stream);
                CurrentProject = loadedProject;
                LoadTimestamps();

                LoadDrawings();

                LoadTags();

                SaveToRecentProjects(path);
            }


            private static void LoadTimestamps()
            {
                ReplayView.tsContainer.Children.Clear();

                foreach(TimestampData timestampData in CurrentProject.Timestamps)
                {
                    ReplayView.tsContainer.Children.Add(new Timestamp { Data = timestampData });
                }
            }

            private static void LoadDrawings()
            {
                DrawView._drawingContainer.Children.Clear();

                foreach(Drawing drawing in CurrentProject.Drawings)
                {
                    DrawView._drawingContainer.Children.Add(new DrawingComponent() { _Drawing = drawing, Title = drawing.Title});
                }
            }

            private static void LoadTags()
            {
                foreach(string Tag in CurrentProject.Tags)
                {
                    ReplayView.TagManager.CreateTag(Tag);
                }
                ReplayView.CurrentTags.Clear();
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

            private static void SaveToRecentProjects(string path)
            {
                if (properties.Settings.Default.past_projects.Count > 10)
                {
                    properties.Settings.Default.past_projects.RemoveAt(0);
                }
                if (properties.Settings.Default.past_projects.Contains(path))
                {
                    properties.Settings.Default.past_projects.Remove(path);
                    properties.Settings.Default.past_projects.Add(path);
                }
                else
                {
                    properties.Settings.Default.past_projects.Add(path);
                }
                LoadRecentProjects();
                properties.Settings.Default.Save();
            }
        }

        public static void CheckArgs()
        {

            if (Environment.GetCommandLineArgs().Count() > 0)
            {
                foreach(string arg in Environment.GetCommandLineArgs())
                {
                    if (File.Exists(arg) && arg.Contains("tempest"))
                    {
                        ProjectHandler.LoadProject(arg);
                    }
                }
            }
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

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectHandler.CurrentProject != null)
            {
                StreamWriter writer = new StreamWriter(ProjectHandler.CurrentProject.Path);

                ProjectHandler.SaveProject(ProjectHandler.CurrentProject.Path);
                string jsonString = JsonSerializer.Serialize(ProjectHandler.CurrentProject);

                writer.WriteLine(jsonString);

                writer.Dispose();
                writer.Close();
                return;
            }

            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "Tempest Project File|*.tempest";
            saveFileDialog.Title = "Save New Project";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                StreamWriter writer = new StreamWriter(saveFileDialog.OpenFile());

                ProjectHandler.SaveProject(saveFileDialog.FileName);
                string jsonString = JsonSerializer.Serialize(ProjectHandler.CurrentProject);

                writer.WriteLine(jsonString);

                writer.Dispose();
                writer.Close();
            }

        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "Tempest Project Files (*.tempest)|*.tempest";
                openFileDialog.RestoreDirectory = true;

                openFileDialog.ShowDialog();

                if (openFileDialog.FileName.Contains("tempest"))
                {
                    ProjectHandler.LoadProject(openFileDialog.FileName);
                }
            }
        }

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectHandler.NewProject();
        }

        private void ClearRecentProjects_Click(object sender, RoutedEventArgs e)
        {
            properties.Settings.Default.past_projects.Clear();
            properties.Settings.Default.Save();

            LoadRecentProjects();
        }

        public static void LoadRecentProjects()
        {
            if (properties.Settings.Default.past_projects.Count == 0)
            {
                ClearRecentProjectsMenu();
                return;
            }
            StringCollection pastProjects = properties.Settings.Default.past_projects;
            int limiter = 0;
            ClearRecentProjectsMenu();
            for (int i = pastProjects.Count - 1; i >= 0; i--)
            {
                if (limiter >= 10) { break; }
                AddRecentProjectToMenu(pastProjects[i]);
                limiter++;
            }
        }

        private static void AddRecentProjectToMenu(string path)
        {
            MenuItem item = new MenuItem() { Header = path, Foreground = Brushes.Black };
            item.Click += RecentProject_Click;
            openRecentMenu.Items.Add(item);
        }

        private static void ClearRecentProjectsMenu()
        {
            List<MenuItem> tempList = new();
            foreach (UIElement item in openRecentMenu.Items)
            {
                if (item is not MenuItem) { continue; }
                tempList.Add((MenuItem)item);
            }
            foreach(MenuItem item in tempList)
            {
                if (item.Header.ToString() == "Clear") { continue; }
                openRecentMenu.Items.Remove(item);
            }
        }

        private static void RecentProject_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ProjectHandler.LoadProject(item.Header.ToString());
        }
    }
}
