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

namespace Tempest.Utils
{
    public class Replay
    {
        private static string url = $"https://127.0.0.1:2999/replay/playback";
        public static async Task<string> getPosition()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            var result = await client.GetAsync(url);
            var response = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Get result {response}");
            return response;
        }

        /// <param name='seconds'>time position in seconds<param>
        public static async Task<string> setPosition(int seconds)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            var values = new Dictionary<string, int> { { "time", seconds } };
            var json = JsonSerializer.Serialize(values);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync(url, stringContent);
            var response = await result.Content.ReadAsStringAsync();
            Console.WriteLine($"Post result {response}");
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

            string roflVersion = GetROFLVersion(roflPath);

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

        private static string GetROFLVersion(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                fs.Read(b, 0, b.Length);
                string[] parsedData = temp.GetString(b).Split(",");
                string gameVersion = "";

                for (int i = 0; i < parsedData.Length; i++)
                {
                    if (parsedData[i].Contains("gameVersion"))
                    {
                        gameVersion = parsedData[i].Split(":")[1].Split(@"""")[1];
                    }
                }

                return $"{gameVersion.Split(".")[0]}.{gameVersion.Split(".")[1]}";
            }
        }
    }
}
