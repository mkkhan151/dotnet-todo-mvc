using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModels
{
    public class CreateTodoViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}