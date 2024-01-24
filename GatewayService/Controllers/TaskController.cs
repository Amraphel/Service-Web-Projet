using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserService.Entities;
using GatewayService.entities;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        HttpClient client;

        public TaskController()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:5002/api/Tasks/");
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskAsync()
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();
            HttpResponseMessage response = await client.GetAsync($"{UserId}");

            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<entities.Task[]>();
                return Ok(tasks);
            }
            else
            {
                return BadRequest("GetMyTaskAsync failed");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateTask(TaskCreate task)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PostAsJsonAsync($"create/{UserId}", task);
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<entities.Task>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("CreateTask failed");
            }
            
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateTask(int id, TaskCreate task)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.PutAsJsonAsync($"update/{id}", task);

            // Check if the response status code is 2XX
            if (response.IsSuccessStatusCode)
            {
                var newTask = await response.Content.ReadFromJsonAsync<entities.Task>();
                return Ok(newTask);
            }
            else
            {
                return BadRequest("UpdateTask failed");
            }

        }
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (UserId == null) return Unauthorized();

            HttpResponseMessage response = await client.DeleteAsync($"delete/{id}");

            Console.WriteLine(response.Content);
            Console.WriteLine(response.StatusCode);
            // Check if the response status code is 2XX
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
