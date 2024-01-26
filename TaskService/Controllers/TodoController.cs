using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TaskService.Entities;
using TaskService.Service;



namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {

        private TodoDb TodoDb { get; set; }

        public TodoController(TodoDb taskDb)
        {
            TodoDb = taskDb;
        }

        //Récuperation de la liste des tache
        [HttpGet("list/{UserId}")]
        public ActionResult<IEnumerable<Entities.Todo>> Get(int UserId)
        {
            List<Entities.Todo>? tasks;
            if (TodoDb.Todos.TryGetValue(UserId, out tasks) && tasks != null)
            {
                return tasks;
            }
            else //Si il n'y a pas de liste de tache on en crée une
            {
                TodoDb.Todos[UserId] = new List<Entities.Todo>();
                return Ok(TodoDb.Todos[UserId]);
            }
        }

        //Creation de tache
        [HttpPost("create/{UserId}")]
        public ActionResult<Entities.Todo> CreateTask(int UserId, TodoCreate task)
        {
            List<Entities.Todo>? tasks;
            if (!TodoDb.Todos.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<Entities.Todo>();
                TodoDb.Todos[UserId] = tasks;
            }
            var index = 0;
            //On augmente le compteur de tache
            if (tasks.Count > 0)
            {
                index = tasks.Max(t => t.Id) + 1;
            }
            //Creation de la tache
            var NewTask = new Entities.Todo
            {
                Id = index,
                IsDone = task.IsDone,
                Text = task.Text
            };

            TodoDb.Todos[UserId].Add(NewTask);
            return Ok(NewTask);
        }

        //Mise à jour de tache
        [HttpPut("update/{UserId}/{id}")]
        public ActionResult<Entities.Todo> Put(int UserId, int id, TodoCreate taskUpdate)
        {
            List<Entities.Todo>? tasks;

            //On récupère la liste de tache
            if (!TodoDb.Todos.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<Todo>();
                TodoDb.Todos[UserId] = tasks;
            }
            var task = tasks.Find(t => t.Id == id);
            //On vérifie que la tache existe
            if (task == null)
            {
                return NotFound();
            }
            task.Text = taskUpdate.Text;
            task.IsDone = taskUpdate.IsDone;

            return Ok(task);
        }

        //Suppression de tache
        [HttpDelete("delete/{UserId}/{id}")]
        public ActionResult<bool> Delete(int UserId, int id)
        {
            //On récupère la liste de tache
            List<Entities.Todo>? tasks;
            if (!TodoDb.Todos.TryGetValue(UserId, out tasks) || tasks == null)
            {
                tasks = new List<Todo>();
                TodoDb.Todos[UserId] = tasks;
            }
            //On vérifie que la tache existe
            var index = tasks.FindIndex(t => t.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            tasks.RemoveAt(index);
            return Ok(true);
        }
    }
}
