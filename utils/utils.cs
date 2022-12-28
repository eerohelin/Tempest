using System.Text.Json;
namespace tempest;

public class Replay {
  private string url = $"https://127.0.0.1:2999/replay/playback";
  public async Task<string> getPosition() {
    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
    HttpClient client = new HttpClient(clientHandler);

    var result = await client.GetAsync(url);
    var response = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"Get result {response}");
    return response;
  }

  /// <param name='seconds'>time position in seconds<param>
  public async Task<string> setPosition(int seconds) {
    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
    HttpClient client = new HttpClient(clientHandler);

    var values = new Dictionary<string, int>{{ "time", seconds }};
    var json = JsonSerializer.Serialize(values);
    var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    var result = await client.PostAsync(url, stringContent);
    var response = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"Post result {response}");
    return response;
  }
}

public class LeaguePaths { 

    public static List<string> GetPaths() {
        DriveInfo[] drives = DriveInfo.GetDrives();
        List<string> directories = new List<string>();

        for (int i = 0; i < drives.Length; i++) {
            directories.AddRange(GetDirectories(drives[i].ToString()));
        }

        return directories;
    }

    private static List<string> GetDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.AllDirectories) {
        List<string> directories = new List<string>(GetDirectoriesSafe(path, searchPattern));

        for (int i = 0; i < directories.Count; i++) {
            if (directories[i].Contains("Riot Games")) {
                return Directory.GetFiles(directories[i], "League of Legends.exe", SearchOption.AllDirectories).ToList();
            }
        }

        for (int i = 0; i < directories.Count; i++) {
            List<string> data = GetDirectoriesSafe(directories[i], searchPattern);
            for (int u = 0; u < data.Count; u++) {
                if (data[u].Contains("Riot Games") && !data[u].Contains("Riot Games\\")) {
                    return Directory.GetFiles(data[u], "League of Legends.exe", SearchOption.AllDirectories).ToList();
                }
            }
        }
        return new List<string>();
    }

    private static List<string> GetDirectoriesSafe(string path, string searchPattern) {
        try {
            return Directory.GetDirectories(path, searchPattern).ToList();
        }
        catch (UnauthorizedAccessException) {
            return new List<string>();
        }
    }
}