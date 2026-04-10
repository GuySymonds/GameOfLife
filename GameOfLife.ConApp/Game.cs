using GameOfLife.Common.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameOfLife.ConApp;

public class Game : IGame
{
    private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private const string _uri = "https://localhost:7168";
    private readonly HttpClient _client = new();

    public GameModel GetGameState(Guid id) => GetGameStateAsync(id).Result;

    public async Task<GameModel> GetGameStateAsync(Guid id)
    {
        var result = await _client.GetFromJsonAsync<GameModel>(_uri + $"/api/game/{id}", options);
        return result ?? throw new Exception($"No game found with id {id}");
    }

    public GameModel GetNewGame(NewGameModel model) => GetNewGameAsync(model).Result;

    public async Task<GameModel> GetNewGameAsync(NewGameModel model)
    {
        var response = await _client.PostAsJsonAsync(_uri + "/api/game", model, options);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameModel>(options);
        return result ?? throw new Exception("Failed to create new game");
    }

    public GameModel GetNextGameState(GameModel model) => GetNextGameStateAsync(model).Result;

    public async Task<GameModel> GetNextGameStateAsync(GameModel model)
    {
        var response = await _client.PostAsJsonAsync(_uri + $"/api/game/{model.GameId}/next", model, options);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameModel>(options);
        return result ?? throw new Exception($"No game found with id {model.GameId}");
    }
}
