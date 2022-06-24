using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UCC_Datasets.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Text;
using Microsoft.Extensions.Hosting.Internal;

namespace UCC_Datasets.Controllers
{
    public class DatasetsController : Controller
    {
        private readonly UCCDatasetsDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatasetsController(UCCDatasetsDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //[Route("Datasets/Index")]
        //// GET: Datasets
        //public async Task<IActionResult> Index()
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var ALs = await _context.Dataset
        //        .Include(d => d.IdUserNavigation)
        //        .ToListAsync();

        //    if (ALs == null)
        //    {
        //        ALs = new List<Dataset>();
        //    }
        //    return View(ALs);
        //}

        //[Route("Datasets/Index")]
        //[Route("Datasets/Index/{id?}")]
        //public async Task<IActionResult> Index(string id)
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var datasets = from m in _context.Dataset
        //                 select m;

        //    if (!String.IsNullOrEmpty(id))
        //    {
        //        datasets = datasets.Where(s => s.Name.ToUpper().Contains(id.ToUpper()));
        //    }

        //    return View(await datasets.ToListAsync());
        //}

        public async Task<IActionResult> Index(string datasetcategory, string id)
        {
            // Use LINQ to get list of genres.
            IQueryable<String> categoryQuery = from m in _context.Category
                                               orderby m.Name
                                               select m.Name;
            var datasets = from m in _context.Dataset
                           select m;

            //var datasets = _context.Dataset
            //    .Include(d => d.IdUserNavigation);

            var datasetscategories = from m in _context.Datasetcategory
                                     select m;

            if (!string.IsNullOrEmpty(id))
            {
                datasets = datasets.Where(s => s.Name.ToUpper().Contains(id.ToUpper()));
                
            }


            if (!string.IsNullOrEmpty(datasetcategory))
            {
                datasets = datasets.Join(_context.Datasetcategory, a => a.IdDataset, b => b.IdDataset, (a, b) => new { A = a, B = b })
                                   .Join(_context.Category, ab => ab.B.IdCategory, c => c.IdCategory, (ab, c) => new { AB = ab, C = c }).Where(c => c.C.Name == datasetcategory).Select(a => a.AB.A);
            }


            var datasetCategoryVM = new DatasetCategoryViewModel
            {
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
                Datasets = await datasets.Include(d => d.IdUserNavigation).ToListAsync()
            };

            return View(datasetCategoryVM);
        }

        // GET: Datasets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataset = await _context.Dataset
                .Include(d => d.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdDataset == id);

            var categories = _context.Datasetcategory
                .Where(d => d.IdDataset == id).Join(_context.Category, c => c.IdCategory, cm => cm.IdCategory, (c, cm) => new {c, cm})
                .ToList();

            List<SelectListItem> ObjItem = new List<SelectListItem>();

            if (categories != null)
            {
                foreach (var item in categories)
                {
                    ObjItem.Add(new SelectListItem { Text = item.cm.Name, Value = item.cm.IdCategory.ToString()});
                }

                ViewBag.ItemList = ObjItem;
            };

            if (dataset == null)
            {
                return NotFound();
            }

            return View(dataset);
        }

        public async Task<IActionResult> MyDatasets()
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ALs = await _context.Dataset.Include(d => d.IdUserNavigation)
                .Where(d => d.IdUserNavigation.Id == userId)
                .ToListAsync();


            if (ALs == null)
            {
                ALs = new List<Dataset>();
            }
            return View(ALs);
        }


        // GET: Datasets/Create
        public IActionResult Create()
        {
            ViewData["IdUser"] = new SelectList(_context.Aspnetusers, "Id", "Id");

            var Categories = _context.Category.ToList();


            List<SelectListItem> ObjItem = new List<SelectListItem>();

            if (Categories != null)
            {
                foreach (Category item in Categories)
                {
                    ObjItem.Add(new SelectListItem { Text = item.Name, Value = item.IdCategory.ToString()});
                }

                ViewBag.ItemList = ObjItem;
            };

            return View();
        }

        // POST: Datasets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDataset,Name,Description,Available,Url,Filesource,Filepath")] Dataset dataset, List<IFormFile> fileuploaded, IEnumerable<int> ItemList, string NewCategory)
        {
            if (ModelState.IsValid)
            {
                if (NewCategory != null)
                {
                    Category newcat = new Category();
                    newcat.Name = NewCategory.ToUpper();
                    _context.Add(newcat);
                    await _context.SaveChangesAsync();

                    Datasetcategory a = new Datasetcategory();
                    a.IdCategory = newcat.IdCategory;
                    a.IdDataset = dataset.IdDataset;
                    dataset.Datasetcategory.Add(a);
                }

                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var ALs = await _context.Aspnetusers
                    .Where(d => d.Id == userId)
                    .ToListAsync();


                if (ALs == null)
                {
                    ALs = new List<Aspnetusers>();
                }

                foreach (var cat in ItemList)
                {
                    if (cat != 4)
                    {
                        Datasetcategory a = new Datasetcategory();
                        a.IdCategory = cat;
                        a.IdDataset = dataset.IdDataset;
                        dataset.Datasetcategory.Add(a);
                    }
                }

                foreach (var file in fileuploaded)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);

                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        dataset.Filesource = dataStream.ToArray();
                        dataset.Filepath = fileName + extension;
                    }
                }



                dataset.Date = DateTime.Now;
                dataset.LastModified = DateTime.Now;
                dataset.IdUserNavigation = ALs[0];
                dataset.IdUser = ALs[0].Id;
                _context.Add(dataset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dataset);
        }


        // GET: Datasets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dataset = await _context.Dataset.FindAsync(id);

            if (dataset != null && userId == dataset.IdUser)
            {

                if (id == null)
                {
                    return NotFound();
                }



                var Categories = _context.Category.ToList();

                List<SelectListItem> ObjItem = new List<SelectListItem>();

                if (Categories != null)
                {
                    foreach (Category item in Categories)
                    {
                        ObjItem.Add(new SelectListItem { Text = item.Name, Value = item.IdCategory.ToString() });
                    }

                    ViewBag.ItemList = ObjItem;
                };

                dataset = await _context.Dataset.FindAsync(id);
                if (dataset == null)
                {
                    return NotFound();
                }
                ViewData["IdUser"] = new SelectList(_context.Aspnetusers, "Id", "Id", dataset.IdUser);
                return View(dataset);
            }
            else
            {
                return RedirectToAction("Index", "Datasets");
            }
        }

        // POST: Datasets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDataset,IdUser,Name,Description,Available,Url,Filesource,Filepath")] Dataset dataset, List<IFormFile> fileuploaded, IEnumerable<int> ItemList, string NewCategory)
        {
            if (id != dataset.IdDataset)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                //var olddataset = await _context.Dataset.FindAsync(id).Result.IdUser;

                //if (userId == (String)(_context.Dataset.Find(id).IdUser))
                //{
                    var ALs = await _context.Aspnetusers
                        .Where(d => d.Id == userId)
                        .ToListAsync();

                    try
                    {
                        // Clean up
                        var oldcategories = _context.Datasetcategory
                            .Where(d => d.IdDataset == dataset.IdDataset);
                        foreach (var entity in oldcategories)
                            _context.Datasetcategory.Remove(entity);
                        foreach (Datasetcategory dc in dataset.Datasetcategory)
                            dataset.Datasetcategory.Remove(dc);
                        dataset.Filepath = null;
                        dataset.Filesource = null;
                        _context.Update(dataset);
                        await _context.SaveChangesAsync();

                        if (NewCategory != null)
                        {
                            Category newcat = new Category();
                            newcat.Name = NewCategory.ToUpper();
                            _context.Add(newcat);
                            await _context.SaveChangesAsync();

                            Datasetcategory a = new Datasetcategory();
                            a.IdCategory = newcat.IdCategory;
                            a.IdDataset = dataset.IdDataset;
                            dataset.Datasetcategory.Add(a);
                        }

                        foreach (var cat in ItemList)
                        {
                            if (cat != 4)
                            {
                                Datasetcategory a = new Datasetcategory();
                                a.IdCategory = cat;
                                a.IdDataset = dataset.IdDataset;
                                dataset.Datasetcategory.Add(a);
                            }
                        }

                        foreach (var file in fileuploaded)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var extension = Path.GetExtension(file.FileName);

                            using (var dataStream = new MemoryStream())
                            {
                                await file.CopyToAsync(dataStream);
                                dataset.Filesource = dataStream.ToArray();
                                dataset.Filepath = fileName + extension;
                            }
                        }

                        dataset.IdUserNavigation = ALs[0];
                        dataset.IdUser = ALs[0].Id;
                        dataset.LastModified = DateTime.Now;
                        //_context.Remove(dataset);
                        _context.Dataset.Update(dataset);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DatasetExists(dataset.IdDataset))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(MyDatasets));
            }
            return RedirectToAction("Index", "Datasets");
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Datasets");
            //}
        }

        // GET: Datasets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dataset = await _context.Dataset.FindAsync(id);

            if (dataset != null && userId == dataset.IdUser)
            {

                var categories = _context.Datasetcategory
                .Where(d => d.IdDataset == id).Join(_context.Category, c => c.IdCategory, cm => cm.IdCategory, (c, cm) => new { c, cm })
                .ToList();

                List<SelectListItem> ObjItem = new List<SelectListItem>();

                if (categories != null)
                {
                    foreach (var item in categories)
                    {
                        ObjItem.Add(new SelectListItem { Text = item.cm.Name, Value = item.cm.IdCategory.ToString() });
                    }

                    ViewBag.ItemList = ObjItem;
                };


                if (id == null)
                {
                    return NotFound();
                }

                dataset = await _context.Dataset
                    .Include(d => d.IdUserNavigation)
                    .FirstOrDefaultAsync(m => m.IdDataset == id);
                if (dataset == null)
                {
                    return NotFound();
                }

                return View(dataset);
            }
            else
            {
                return RedirectToAction("Index", "Datasets");
            }
        }

        // POST: Datasets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dataset = await _context.Dataset.FindAsync(id);
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (dataset != null && userId == dataset.IdUser)
            {
                _context.Dataset.Remove(dataset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyDatasets));
            }
            else
            {
                return RedirectToAction("Index", "Datasets");
            }

        }

        private bool DatasetExists(int id)
        {
            return _context.Dataset.Any(e => e.IdDataset == id);
        }

        public FileResult DownloadFile(int id)
        {
            var dataset = _context.Dataset.Where(x => x.IdDataset == id).ToList()[0];
            if (dataset.Filepath != null)
            {
                byte[] bytes = dataset.Filesource;
                string filepath = dataset.Filepath;
                string extension = filepath.Split('.')[1];
                return File(bytes, "application/" + extension, filepath);
            }
            else
                return null;
        }
    }
}
