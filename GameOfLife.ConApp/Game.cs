using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GameOfLife.Common.Models;

namespace GameOfLife.ConApp;

public class Game : IGame
{
    private const string _uri = "https://localhost:7168";
    private readonly HttpClient _client = new HttpClient();

    public GameModel GetGameState(Guid id) => GetGameStateAsync(id).Result;

    public async Task<GameModel> GetGameStateAsync(Guid id)
    {
        var result = await _client.GetFromJsonAsync<GameModel>(_uri + $"/api/game/{id}");
        return result ?? throw new Exception($"No game found with id {id}");
    }

    public GameModel GetNewGame(NewGameModel model) => GetNewGameAsync(model).Result;

    public async Task<GameModel> GetNewGameAsync(NewGameModel model)
    {
        var response = await _client.PostAsJsonAsync(_uri + "/api/game", model);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GameModel>();
        return result ?? throw new Exception("Failed to create new game");
    }

    public GameModel GetNextGameState(Guid id) => GetNextGameStateAsync(id).Result;

    public async Task<GameModel> GetNextGameStateAsync(Guid id)
    {
        var result = await _client.GetFromJsonAsync<GameModel>(_uri + $"/api/game/{id}/next");
        return result ?? throw new Exception($"No game found with id {id}");
    }
}
