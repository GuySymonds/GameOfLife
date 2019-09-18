using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GameOfLife.Common.Models;
using Newtonsoft.Json;

namespace GameOfLife.ConApp
{
    public class Game : IGame
    {
        private const string _uri = "https://localhost:5003";
        private readonly HttpClient _client = new HttpClient();

        public GameModel GetGameState(Guid id)
        {
            return GetGameStateAsync(id).Result;
        }

        public async Task<GameModel> GetGameStateAsync(Guid id)
        {
            var response = await _client.GetAsync(_uri + $"/api/games/{id}");

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<GameModel>(await response.Content.ReadAsStringAsync());
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public GameModel GetNewGame(NewGameModel model)
        {
            return GetNewGameAsync(model).Result;
        }

        public async Task<GameModel> GetNewGameAsync(NewGameModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_uri + "/api/games/", content);

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<GameModel>(await response.Content.ReadAsStringAsync());
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public GameModel GetNextGameState(Guid id)
        {
            return GetNextGameStateAsync(id).Result;
        }

        public async Task<GameModel> GetNextGameStateAsync(Guid id)
        {
            var response = await _client.GetAsync(_uri + $"/api/games/{id}/next");
            
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<GameModel>(await response.Content.ReadAsStringAsync());
            else
                throw new Exception(response.StatusCode.ToString());
        }
    }
}
