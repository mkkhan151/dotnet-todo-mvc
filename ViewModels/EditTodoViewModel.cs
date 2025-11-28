using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModels
{
    public class EditTodoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = null!;
        
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}