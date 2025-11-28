using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Controllers
{
    public class TodosController : Controller
    {
        private readonly TodoAppDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly ILogger<TodosController> logger;

        public TodosController(TodoAppDbContext dbContext,
                               UserManager<User> userManager,
                               ILogger<TodosController> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // return a view with the user's todos
            logger.LogInformation("Fetching todos for user {UserId}", userManager.GetUserId(User));
            var todos = await dbContext.Todos.Where(todo => todo.UserId == userManager.GetUserId(User)).ToListAsync();
            return View(todos);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var todo = new Todo
            {
                Title = model.Title,
                Description = model.Description!,
                IsCompleted = false,
                UserId = userManager.GetUserId(User)!
            };

            dbContext.Todos.Add(todo);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created new todo {TodoId} for user {UserId}", todo.Id, todo.UserId);

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();

            if (todo.UserId != userManager.GetUserId(User))
                return Forbid();
            logger.LogInformation("Editing todo {TodoId} for user {UserId}", todo.Id, todo.UserId);
            return View(new EditTodoViewModel
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTodoViewModel model)
        {
            logger.LogInformation("Received edit request for todo {TodoId} by user {UserId}.", id, userManager.GetUserId(User));
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null) return NotFound();

            var currentUserId = userManager.GetUserId(User);
            if (todo.UserId != currentUserId) return Forbid();

            // update the todo fields
            todo.Title = model.Title;
            todo.Description = model.Description;
            todo.IsCompleted = model.IsCompleted;

            await dbContext.SaveChangesAsync();
            logger.LogInformation("Updated todo {TodoId} for user {UserId}", todo.Id, todo.UserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();

            if (todo.UserId != userManager.GetUserId(User))
                return Forbid();

            todo.IsCompleted = true;
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await dbContext.Todos.FindAsync(id);
            if (todo == null)
                return NotFound();

            if (todo.UserId != userManager.GetUserId(User))
                return Forbid();

            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}