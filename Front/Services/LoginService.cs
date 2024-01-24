﻿using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Front.Services
{
    public class LoginService
    {

        private ProtectedLocalStorage _sessionStorage;
        private readonly HttpClient _httpClient;

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDTO> AuthenticateUserAsync(string username, string password)
        {
            var login = new UserLogin() { Name = username, Pass = password };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/login", login);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JWTAndUser>();

                await _sessionStorage.SetAsync("jwt", result.Token);
                return result.User;
            }
            else
            {
                return null;
            }
        }
    }
}

