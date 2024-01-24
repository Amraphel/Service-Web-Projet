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

        public LoginService(HttpClient httpClient, ProtectedLocalStorage sessionStorage)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<UserDTO> AuthenticateUserAsync(string username, string password)
        {
            var login = new UserLogin() { Name = username, Pass = password };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/login", login);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JWTAndUser>();
                if(result!=null)
                {
                    await _sessionStorage.SetAsync("jwt", result.Token);
                    return result.User;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        public async Task<UserDTO> RegisterUser(string username, string password, string email)
        {
            var registerInfo = new UserCreate() { Name = username, Password = password, Email = email };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/User/register", registerInfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}

