using GameOfLife.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace GameOfLife.ConApp
{
    public class RestGame : IGame
    {
        private readonly RestClient _client = new RestClient("https://localhost:5003");

        public GameModel GetNewGame(NewGameModel model)
        {
            var request = new RestRequest("api/games/", Method.POST);

            request.AddJsonBody(model);

            var response = _client.Execute(request);
            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }

        public GameModel GetNextGameState(Guid id)
        {
            var request = new RestRequest($"api/games/{id}/next", Method.GET);

            var response = _client.Execute(request);
            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }

        public GameModel GetGameState(Guid id)
        {
            var request = new RestRequest($"api/games/{id}", Method.GET);

            var response = _client.Execute(request);
            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }

        public async Task<GameModel> GetGameStateAsync(Guid id)
        {
            return await Task.FromResult(GetGameState(id));
        }

        public async Task<GameModel> GetNewGameAsync(NewGameModel model)
        {
            return await Task.FromResult(GetNewGame(model));
        }

        public async Task<GameModel> GetNextGameStateAsync(Guid id)
        {
            return await Task.FromResult(GetNextGameState(id));
        }
    }
}
