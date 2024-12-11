using System.ComponentModel.DataAnnotations;

namespace BlogPostAPI.DTO_s
{
    public class BlogPostDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(1)]
        public string Text { get; set; }
    }
}
