namespace tempest;
using System.Text.Json;

public class Replay {
  private int port = 2999;
  private string url = $"https://127.0.0.1:{port}/replay/playback";
  private HttpClient client = new HttpClient();

  private async Task<HttpResponseMessage> getPosition(){
    var result = await client.GetAsync(url);
    Console.WriteLine(result.StatusCode);
    return result;
  }

  /// <param name='seconds'>time position in seconds<param>
  private async Task<HttpResponseMessage> setPosition(int seconds){
    var values = new Dictionary<string, string>{{ "time", seconds.ToString() }};
    var content = new FormUrlEncodedContent(values);

    using var result = await client.PostAsync(url, content);
    Console.WriteLine(result.StatusCode);
    return result;
  }
}

public class JsonManager {
  public static void readJson(){

  }
  public static void writeJson(){
    using (StreamReader r = new StreamReader("data.json"))
    {
        string json = r.ReadToEnd();
        source = JsonSerializer.Deserialize<List<Person>>(json);
    }
  }
}