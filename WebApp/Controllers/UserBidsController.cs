using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;

namespace WebApp.Controllers
{
    public class UserBidsController : Controller
    {
        private readonly AppDbContext _context;

        public UserBidsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserBids
        public IActionResult Index()
        {
            var userBids = _context.UserBids
                .Include(u => u.User)
                .Include(u => u.Product)
                .ToList()
                .Where(p => DateTime.Parse(p.Product.biddingEndDate) < DateTime.Now)
                .OrderByDescending(p => p.Amount)
                .GroupBy(x => x.ProductId)
                .Select(x => x.First())
                .ToList();

            return View(userBids);
        }

        // GET: UserBids/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBid = await _context.UserBids
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBid == null)
            {
                return NotFound();
            }

            return View(userBid);
        }

        // GET: UserBids/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserBids/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,UserId")] UserBid userBid)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userBid);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userBid.UserId);
            return View(userBid);
        }

        // GET: UserBids/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBid = await _context.UserBids.FindAsync(id);
            if (userBid == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userBid.UserId);
            return View(userBid);
        }

        // POST: UserBids/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,UserId")] UserBid userBid)
        {
            if (id != userBid.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBid);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBidExists(userBid.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userBid.UserId);
            return View(userBid);
        }

        // GET: UserBids/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBid = await _context.UserBids
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBid == null)
            {
                return NotFound();
            }

            return View(userBid);
        }

        // POST: UserBids/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userBid = await _context.UserBids.FindAsync(id);
            _context.UserBids.Remove(userBid);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBidExists(int id)
        {
            return _context.UserBids.Any(e => e.Id == id);
        }
    }
}
