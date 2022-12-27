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