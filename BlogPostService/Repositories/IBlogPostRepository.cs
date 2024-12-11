using BlogPostService.Models;

namespace BlogPostAPI.Repositories
{
    /// <summary>
    /// Interface for managing blog posts.
    /// </summary>
    public interface IBlogPostRepository
    {
        /// <summary>
        /// Retrieves all blog posts.
        /// </summary>
        /// <returns>A collection of <see cref="BlogPost"/>.</returns>
        Task<IEnumerable<BlogPost>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific blog post by ID.
        /// </summary>
        /// <param name="id">The ID of the blog post.</param>
        /// <returns>The <see cref="BlogPost"/> with the specified ID, or null if not found.</returns>
        Task<BlogPost?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new blog post.
        /// </summary>
        /// <param name="post">The <see cref="BlogPost"/> to add.</param>
        Task AddAsync(BlogPost post);

        /// <summary>
        /// Updates an existing blog post.
        /// </summary>
        /// <param name="post">The <see cref="BlogPost"/> with updated details.</param>
        Task UpdateAsync(BlogPost post);

        /// <summary>
        /// Deletes a blog post by ID.
        /// </summary>
        /// <param name="id">The ID of the blog post to delete.</param>
        Task DeleteAsync(int id);
    }
}
