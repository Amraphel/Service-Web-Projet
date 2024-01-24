using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace Front.Services
{
    public class TaskService
    {

        private ProtectedLocalStorage _sessionStorage;
        private readonly HttpClient _httpClient;

        public TaskService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;

        }


        public async Task<Task[]> GetAllTasks()
        {
            var jwt = await _sessionStorage.GetAsync<string>("jwt");

            HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/api/Todo");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Task[]>();

                return result;
            }
            else
            {
                return [];
            }
        }
    }
}
