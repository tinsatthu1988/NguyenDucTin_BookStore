using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NguyenDucTin_BookStore;
using NguyenDucTin_BookStore.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NguyenDucTin_BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(MainDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Admin/Admin/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookModel = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookModel == null)
            {
                return NotFound();
            }

            return View(bookModel);
        }
        [Route("Create")]
        // GET: Admin/Admin/Create
        public IActionResult Create()
        {
            return View();
        }
        [Route("CreateBook")]
        // POST: Admin/Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBook([Bind("BookId,Title,Author,ImageFile")] BookModel bookModel)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (bookModel.ImageFile != null)
                {
                    //Save Image to wwwroot/image
                    
                    string fileName = Path.GetFileNameWithoutExtension(bookModel.ImageFile.FileName);
                    string extension = Path.GetExtension(bookModel.ImageFile.FileName);
                    bookModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await bookModel.ImageFile.CopyToAsync(fileStream);

                    }
                }
                //else
                //{
                //    bookModel.ImageName = "noimage" + DateTime.Now.ToString("yymmssfff") + ".png";
                //    Path.Combine(wwwRootPath + "/Image/", bookModel.ImageName);
                //}
                _context.Add(bookModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookModel);
        }
        [Route("Edit")]
        // GET: Admin/Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookModel = await _context.Books.FindAsync(id);
            if (bookModel == null)
            {
                return NotFound();
            }
            return View(bookModel);
        }
        [Route("EditBook")]
        // POST: Admin/Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(int id, [Bind("BookId,Title,Author,ImageFile")] BookModel bookModel)
        {
   

            if (ModelState.IsValid)
            {
                try
                {
                    var BookNameOld = await _context.Books.Where(book => book.BookId == bookModel.BookId).Select(book=>book.ImageName).FirstOrDefaultAsync();
                    if (bookModel.ImageFile !=null)
                    {
                        //Save Image to wwwroot/image
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(bookModel.ImageFile.FileName);
                        string extension = Path.GetExtension(bookModel.ImageFile.FileName);
                        bookModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await bookModel.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        bookModel.ImageName = BookNameOld;
                    }
                    _context.Update(bookModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookModelExists(bookModel.BookId))
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
            return View(bookModel);
        }
        
        [Route("Delete")]
        // GET: Admin/Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var bookModel = await _context.Books.FindAsync(id);
            _context.Books.Remove(bookModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookModelExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
