using GameOfLife.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOfLife.ConApp
{
    public class Game
    {
        private RestClient _client = new RestClient("https://localhost:5001");

        internal GameModel GetNewGame(NewGameModel model)
        {
            var request = new RestRequest("api/games/", Method.POST);

            request.AddJsonBody(model);

            var response = _client.Execute(request);
            if(response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else 
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }

        internal GameModel GetNextGameState(Guid id)
        {
            var request = new RestRequest($"api/games/{id}/next", Method.GET);
            
            var response = _client.Execute(request);
            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }

        internal GameModel GetGameState(Guid id)
        {
            var request = new RestRequest($"api/games/{id}", Method.GET);

            var response = _client.Execute(request);
            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<GameModel>(response.Content);
            else
                throw new Exception(response.ErrorMessage, response.ErrorException);
        }
    }
}
