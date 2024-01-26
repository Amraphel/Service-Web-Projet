using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        HttpClient client;
        public TodoController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/");
        }

        //Récuperation des taches
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyTaskAsync()
        {
            //Vérification des autorisations utilisateur
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.GetAsync($"api/Todo/list/{UserId}");

            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<Entities.Todo[]>();
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetMyTaskAsync failed");
            }

        }

        //Creation de tache
        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateTask(TodoCreate task)
        {
            //Vérification des autorisations utilisateur
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"api/Todo/create/{UserId}", task);

            
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<Entities.Todo>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("CreateTask failed");
            }

        }

        //Mise a jour de tache
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateTask(int id, TodoCreate task)
        {
            //Vérification des autorisations utilisateur
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"api/Todo/update/{UserId}/{id}", task);


            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<Entities.Todo>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }

        }

        //Suppression de tache
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            //Vérification des autorisations utilisateur
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"api/Todo/delete/{UserId}/{id}");

            if (response.IsSuccessStatusCode)
            {
                string str = await response.Content.ReadAsStringAsync();
                if (str == "true")
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }
        }
    }
}
