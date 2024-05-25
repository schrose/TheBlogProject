
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;
using TheBlogProject.Services;
using X.PagedList;

namespace TheBlogProject.Controllers
{
    public class PostsController(
        ApplicationDbContext context, 
        BlogSearchService blogSearchService, 
        ISlugService slugService, 
        IImageService imageService,
        UserManager<BlogUser> userManager) : Controller
    {
        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var posts = blogSearchService.Search(searchTerm);
            
            return View(await posts.ToPagedListAsync(pageNumber, pageSize));
        }
        
        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }
        // BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id, int? page)
        {
            if (id is null)
            {
                return NotFound();
            }
            var pageNumber = page ?? 1;
            var pageSize = 5;
            
            var posts = await context.Posts
                .Where(p => p.BlogId == id && p.ReadyStatus == ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .ToPagedListAsync(pageNumber, pageSize);
            
            return View(posts);
        }
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(context.Blogs, "Id", "Name");
            ViewData["BlogUserId"] = new SelectList(context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                var authorId = userManager.GetUserId(User);
                post.BlogUserId = authorId;
                
                post.ImageData = await imageService.EncodeImageAsync(post.Image);
                post.ImageType = imageService.ImageType(post.Image);
                
                var slug = slugService.UrlFriendly(post.Title);
                // Create a variable to store whether an error has occurred
                var validationError = false;
                
                if (string.IsNullOrEmpty(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("","The Title you provided can not be used as it results in an empty slug.");
                }
                // Detect incoming duplicate Slugs
                else if (!slugService.IsUnique(slug))
                {
                    validationError = true;
                    ModelState.AddModelError("Title","The Title your provided cannot be used as it result in a duplicate slug.");
                }
                else if (slug.Contains("test"))
                {
                    validationError = true;
                    ModelState.AddModelError("","Uh-oh are you testing again??");
                    ModelState.AddModelError("Title","The Title can not contain the word test.");
                }
                
                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;
                
                context.Add(post);
                await context.SaveChangesAsync();
                
                //How do I loop over the incoming list of string?
                foreach (var tag in tagValues)
                {
                    context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tag
                    });
                }
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post post, IFormFile newImage, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var originalPost = await context.Posts
                        .Include(p => p.Tags)
                        .FirstOrDefaultAsync(p => p.Id == post.Id);
                    
                    originalPost.Updated = DateTime.Now;
                    originalPost.Title = post.Title;
                    originalPost.Abstract = post.Abstract;
                    originalPost.Content = post.Content;
                    originalPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = slugService.UrlFriendly(post.Title);
                    if (newSlug != originalPost.Slug)
                    {
                        if (slugService.IsUnique(newSlug))
                        {
                            originalPost.Title = post.Title;
                            originalPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title","This Title your provided cannot be used as it result in a duplicate slug.");
                            ViewData["TagValues"] = string.Join(",", tagValues);
                            return View(post);
                        }
                    }
                    
                    if (newImage is not null)
                    {
                        originalPost.ImageData = await imageService.EncodeImageAsync(newImage);
                        originalPost.ImageType = imageService.ImageType(newImage);
                    }
                    
                    // Remove all Tags previously associated with this Post
                    context.Tags.RemoveRange(originalPost.Tags);
                    
                    // Add in the new Tags from the Edit form
                    foreach (var tagText in tagValues)
                    {
                        context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = originalPost.BlogUserId,
                            Text = tagText
                        });
                    }
                    
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(context.Blogs, "Id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(context.Users, "Id", "Id", post.BlogUserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post != null)
            {
                context.Posts.Remove(post);
                
                List<Tag> tagsToDelete = context.Tags.Where(t => t.PostId == id).ToList();
                
                foreach (var tag in tagsToDelete)
                {
                    context.Tags.Remove(tag);
                }
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return context.Posts.Any(e => e.Id == id);
        }
    }
}
