using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Elixir.Models;
using X.PagedList;

namespace Elixir.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ADPostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ADPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ADPosts
        public IActionResult Index(int? page, string searchQuery)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var posts = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                posts = posts.Where(p => p.Title.Contains(searchQuery) || p.ShortContent.Contains(searchQuery) || p.Content.Contains(searchQuery));
                ViewData["searchQuery"] = searchQuery;
            }

            var pagedPosts = posts.ToPagedList(pageNumber, pageSize);
            return View(pagedPosts);
        }


        // GET: Admin/ADPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/ADPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ADPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile postImage)
        {
            if (ModelState.IsValid)
            {
                if (postImage != null && postImage.Length > 0)
                {
                    var fileName = Path.GetFileName(postImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/post", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postImage.CopyToAsync(stream);
                    }

                    post.PostImage = "/img/post/" + fileName;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/ADPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ShortContent,Content,Author,DateCreate,Status,PostImage")] Post post, IFormFile newImage)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (newImage != null && newImage.Length > 0)
                    {
                        var fileName = Path.GetFileName(newImage.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/post", fileName);

                        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        post.PostImage = "/img/post/" + fileName;
                    }
                    else
                    {
                        _context.Entry(post).Property(x => x.PostImage).IsModified = false;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }
            return View(post);
        }

        // GET: Admin/ADPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/ADPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
