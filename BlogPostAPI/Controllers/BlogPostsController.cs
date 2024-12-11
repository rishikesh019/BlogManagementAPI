using BlogPostAPI.DTO_s;
using BlogPostAPI.Repositories;
using BlogPostService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostAPI.Controllers
{
    /// <summary>
    /// Controller for managing blog posts.
    /// Provides endpoints to create, retrieve, update, and delete blog posts.
    /// </summary>
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

        /// <summary>
        /// Asynchronously retrieves all blog posts from the repository.
        /// </summary>
        /// <returns>Returns list of blog posts</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _repository.GetAllAsync();
            return Ok(posts);
        }

        /// <summary>
        /// Asynchronously retrieves blog post against id from the repository.
        /// </summary>
        /// <param name="id">This parameter holds integer value of id of blog</param>
        /// <returns>Returns object of blog post</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _repository.GetByIdAsync(id);
            return post != null ? Ok(post) : NotFound(new { Message = "Blog post not found" });
        }

        /// <summary>
        /// Asynchronously creates a new blog post from the provided data transfer object (DTO).
        /// </summary>
        /// <param name="postDto">The data transfer object containing the blog post details.</param>
        /// <returns>An IActionResult containing a CreatedAtAction response with the newly created blog post.</returns>
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
        /// <summary>
        /// Asynchronously updates an existing blog post with the provided data transfer object (DTO).
        /// </summary>
        /// <param name="id">The ID of the blog post to update.</param>
        /// <param name="postDto">The data transfer object containing the updated blog post details.</param>
        /// <returns>An IActionResult indicating the result of the update operation.</returns>
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

        /// <summary>
        /// Asynchronously deletes an existing blog post by its ID.
        /// </summary>
        /// <param name="id">The ID of the blog post to delete.</param>
        /// <returns>An IActionResult indicating the result of the delete operation.</returns>
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
