using BlogPostAPI.DTO_s;
using BlogPostAPI.Models;
using BlogPostAPI.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAngularApp")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository _repository;
        public BlogPostsController(IBlogPostRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _repository.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _repository.GetByIdAsync(id);
            return post != null ? Ok(post) : NotFound(new { Message = "Blog post not found" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogPostDto postDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var post = new BlogPost
            {
                Username = postDto.Username,
                Text = postDto.Text,
                DateCreated = DateTime.UtcNow
            };

            await _repository.AddAsync(post);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlogPostDto postDto)
        {
            var existingPost = await _repository.GetByIdAsync(id);
            if (existingPost == null) return NotFound(new { Message = "Blog post not found" });

            existingPost.Text = postDto.Text;
            existingPost.Username = postDto.Username;
            await _repository.UpdateAsync(existingPost);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingPost = await _repository.GetByIdAsync(id);
            if (existingPost == null) return NotFound(new { Message = "Blog post not found" });

            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
