using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    public class EntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public EntriesController(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Entries
        public async Task<IActionResult> Index(string title, string journal)
        {

            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }

            var userId = _userManager.GetUserId(User);
            var entries = from e in _context.Entry
                          where e.UserId == userId
                          select e;

            if (!string.IsNullOrEmpty(title))
            {
                entries = entries.Where(x => x.Title.ToUpperInvariant().Contains(title.ToUpperInvariant()));
            }
            if (!string.IsNullOrEmpty(journal))
            {
                entries = entries.Where(x => x.Journal.Equals(journal));
            }

            var journals = from e in _context.Entry
                           where e.UserId == userId
                           select e.Journal;

            var model = new EntriesIndexViewModel
            {
                TitleFilter = title,
                JournalFilter = journal,
                Journals = await journals.Distinct().ToListAsync(),
                Entries = await entries.OrderByDescending(e => e.Journal).ThenBy(e => e.Id).ToListAsync()
            };
            return View(model);
        }

        // GET: Entries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entry
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _userManager.GetUserId(User));
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // GET: Entries/Create
        public IActionResult Create()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Journal,Title,Pages")] Entry entry)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            entry.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                _context.Add(entry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }

        // GET: Entries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entry.FindAsync(id);
            if (entry == null || entry.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Journal,Title,Pages")] Entry entry)
        {
            // TODO Check if the ID related to the item to edit is owned by the currently connected user.
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            if (id != entry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntryExists(entry.Id))
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
            return View(entry);
        }

        // GET: Entries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.Entry
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _userManager.GetUserId(User));
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/Identity/Account/Login");
            }
            var entry = await _context.Entry.FirstOrDefaultAsync(m => m.Id == id && m.UserId == _userManager.GetUserId(User));
            _context.Entry.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntryExists(int id)
        {
            return _context.Entry.Any(e => e.Id == id);
        }
    }
}
