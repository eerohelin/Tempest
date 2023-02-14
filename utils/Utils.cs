using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace Tempest
{
    public class Replay
    {
        private static string url = $"https://127.0.0.1:2999/replay/playback";
        public static async Task<JsonObject> getPosition()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            HttpResponseMessage result;

            try
            {
                result = await client.GetAsync(url);
            } catch (HttpRequestException)
            {
                // TODO Implement informing the user of the reason for the exception
                return null;
            }

            var response = await result.Content.ReadAsStringAsync();
            Trace.WriteLine($"Get result {response}");
            JsonObject? obj = JsonSerializer.Deserialize<JsonObject>(response);
            return obj;
        }

        /// <param name='seconds'>time position in seconds<param>
        public static async Task<string> setPosition(int seconds)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            HttpResponseMessage result;

            var values = new Dictionary<string, int> { { "time", seconds } };
            var json = JsonSerializer.Serialize(values);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                result = await client.PostAsync(url, stringContent);
            } catch (HttpRequestException)
            {
                return null;
            }
                var response = await result.Content.ReadAsStringAsync();
            Trace.WriteLine($"Post result {response}");
            return response;
        }
    }

    public class LeaguePaths
    {

        public static List<string> GetPaths()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> directories = new List<string>();

            for (int i = 0; i < drives.Length; i++)
            {
                directories.AddRange(GetDirectories(drives[i].ToString()));
            }

            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern = "*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<string> directories = new List<string>(GetDirectoriesSafe(path, searchPattern));

            for (int i = 0; i < directories.Count; i++)
            {
                if (directories[i].Contains("Riot Games"))
                {
                    return Directory.GetFiles(directories[i], "League of Legends.exe", SearchOption.AllDirectories).ToList();
                }
            }

            for (int i = 0; i < directories.Count; i++)
            {
                List<string> data = GetDirectoriesSafe(directories[i], searchPattern);
                for (int u = 0; u < data.Count; u++)
                {
                    if (data[u].Contains("Riot Games") && !data[u].Contains("Riot Games\\"))
                    {
                        return Directory.GetFiles(data[u], "League of Legends.exe", SearchOption.AllDirectories).ToList();
                    }
                }
            }
            return new List<string>();
        }

        private static List<string> GetDirectoriesSafe(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }
    }

    public class ROFLHandler
    {
        public static void OpenROFL(string roflPath)
        {

            if (!roflPath.Contains(".rofl")) { return; }

            ReplayObject replay = ParseROFL(roflPath);
            var parsedROFLVersion = replay.gameVersion.ToString().Split('.').Take(2);
            string roflVersion = string.Join(".", parsedROFLVersion);

            // TODO: Loop through all possible League.exe's in the settings.json file and compare their versions to the rofl's version
            var versionInfo = FileVersionInfo.GetVersionInfo(@"C:\Riot Games\League of Legends\Game\League of Legends.exe");
            string version = versionInfo.FileVersion ?? throw new ArgumentException();
            string parsedLeagueVersion = $"{version.Split(".")[0]}.{version.Split(".")[1]}";

            if (roflVersion != parsedLeagueVersion)
            {
                Console.WriteLine("Incorrect rofl version");
                return;
            }

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.WorkingDirectory = @"C:\Riot Games\League of Legends\Game";
                    process.StartInfo.FileName = @"C:\Riot Games\League of Legends\Game\League of Legends.exe";
                    process.StartInfo.Arguments = @$"""{roflPath}""";
                    process.Start();
                    // process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ReplayObject ParseROFL(string path)
        {
            if (!path.Contains(".rofl")) { return null; }
            string replayFileContents = string.Join("", File.ReadLines(path, Encoding.Default).Take(20).ToList<string>().ToArray<string>());
            return GetReplay(replayFileContents);
        }


        private static ReplayObject GetReplay(string replayFileContents)
        {
            int jsonStartIndex = replayFileContents.IndexOf("{\"gameLength\"");
            int jsonEndIndex = replayFileContents.IndexOf("\\\"}]\"}") + "\\\"}]\"}".Length;

            try
            {
                JsonNode parsed = JsonObject.Parse(replayFileContents.Substring(jsonStartIndex, (jsonEndIndex - jsonStartIndex)));

                string cleanedJSON = parsed.ToString().Replace("\"[", "[").Replace("]\"", "]").Replace(@"\u0022", "\"");

                ReplayObject replay = JsonSerializer.Deserialize<ReplayObject>(cleanedJSON);

                return replay;

            } catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

    public class SmoothLine
    {
        public static System.Windows.Shapes.Path Smooth(PointCollection points)
        {
            System.Windows.Shapes.Path path = new();
            PathGeometry geometry = new();
            PathFigure figure = new()
            {
                StartPoint= points[0]
            };
            PolyBezierSegment segment = new()
            {
                Points = new PointCollection(points)
            };

            figure.Segments.Add(segment);
            geometry.Figures.Add(figure);
            path.Data = geometry;

            return path;
        }

    }
}
