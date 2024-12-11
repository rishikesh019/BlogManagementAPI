using System.Reflection;
using BlogPostService.Models;
using System.Text.Json;

namespace BlogPostAPI.Repositories
{
    /// <summary>
    /// Implementation of <see cref="IBlogPostRepository"/> that uses a JSON file as data storage.
    /// </summary>
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly string _filePath;
        /// <summary>
        /// Initializes a new instance of the <see cref="BlogPostRepository"/> class.
        /// </summary>
        public BlogPostRepository()
        {
            // Ensure the file path points to the `Data` folder
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyPath == null)
                throw new InvalidOperationException("Unable to determine assembly path");

            _filePath = Path.Combine(assemblyPath, "Data", "blogposts.json");
            EnsureFileExists();
        }
        /// <summary>
        /// Asynchronously retrieves all blog posts by reading from a file.
        /// </summary>
        /// <returns>Returns task result contains an IEnumerable of BlogPost objects.</returns>
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await ReadFromFileAsync();
        }
        /// <summary>
        /// Asynchronously retrieves blog post by reading from a file based on intiger Id.
        /// </summary>
        /// <param name="id">The ID of the blog post to retrieve.</param>
        /// <returns>Returns task result contains the BlogPost object if found; otherwise, null.</returns>
        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            var posts = await ReadFromFileAsync();
            return posts.Find(p => p.Id == id);
        }
        /// <summary>
        /// Asynchronously adds a new blog post by reading the existing posts from a file, assigning a new ID, setting the creation date, and writing the updated list back to the file.
        /// </summary>
        /// <param name="post">The blog post to add.</param>
        /// <returns>A task that represents the asynchronous operation. </returns>
        public async Task AddAsync(BlogPost post)
        {
            var posts = await ReadFromFileAsync();
            post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
            post.DateCreated = DateTime.UtcNow;
            posts.Add(post);
            await WriteToFileAsync(posts);
        }
        /// <summary>
        /// Asynchronously updates an existing blog post by reading the current posts from a file, finding the post by its ID, and writing the updated list back to the file.
        /// </summary>
        /// <param name="post">The blog post with updated details.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(BlogPost post)
        {
            var posts = await ReadFromFileAsync();
            var index = posts.FindIndex(p => p.Id == post.Id);
            if (index >= 0)
            {
                posts[index] = post;
                await WriteToFileAsync(posts);
            }
        }
        /// <summary>
        /// Asynchronously deletes an existing blog post by reading the current posts from a file, removing the post with the specified ID, and writing the updated list back to the file.
        /// </summary>
        /// <param name="id">The ID of the blog post to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            var posts = await ReadFromFileAsync();
            posts.RemoveAll(p => p.Id == id);
            await WriteToFileAsync(posts);
        }
        /// <summary>
        /// Asynchronously reads blog posts from a file. If the file does not exist, returns an empty list.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of BlogPost objects.</returns>
        private async Task<List<BlogPost>> ReadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new List<BlogPost>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<BlogPost>>(json) ?? new List<BlogPost>();
        }
        /// <summary>
        /// Asynchronously writes a collection of blog posts to a file in JSON format.
        /// </summary>
        /// <param name="posts">The collection of blog posts to write to the file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task WriteToFileAsync(IEnumerable<BlogPost> posts)
        {
            var json = JsonSerializer.Serialize(posts);
            await File.WriteAllTextAsync(_filePath, json);
        }
        /// <summary>
        /// Ensures that the file for storing blog posts exists. If the file does not exist, it creates the file with initial empty data.
        /// </summary>
        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                var initialData = JsonSerializer.Serialize(new List<BlogPost>());
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? string.Empty);
                File.WriteAllText(_filePath, initialData);
            }
        }
    }
}
