using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POE_PART_1.Models;



namespace EventEase.Controllers
{
    public class BookinggController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookinggController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var bookinggs = _context.Bookingg
                .Include(b => b.Evvent)
                .Include(b => b.Venuee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookinggs = bookinggs.Where(b =>
                    b.Venuee.Name.Contains(searchString) ||
                    b.Evvent.Name.Contains(searchString));
            }

            return View(await bookinggs.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["Events"] = _context.Evvent.ToList();
            ViewData["Venues"] = _context.Venuee.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bookingg bookingg)
        {
            var selectedEvent = await _context.Evvent.FirstOrDefaultAsync(e => e.Id == bookingg.EvventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Evvent.ToList();
                ViewData["Venues"] = _context.Venuee.ToList();
                return View(bookingg);
            }

            // Check manually for double booking
            var conflict = await _context.Bookingg
                .Include(b => b.Evvent)
                .AnyAsync(b => b.Id == bookingg.VenueeId &&
                         b.Evvent.Date == selectedEvent.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Evvent.ToList();
                ViewData["Venues"] = _context.Venuee.ToList();
                return View(bookingg);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(bookingg);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // If database constraint fails (e.g., unique key violation), show friendly message
                    ModelState.AddModelError("", "This venue is already booked for that date.");
                    ViewData["Events"] = _context.Evvent.ToList();
                    ViewData["Venues"] = _context.Venuee.ToList();
                    return View(bookingg);
                }
            }

            ViewData["Events"] = _context.Evvent.ToList();
            ViewData["Venues"] = _context.Venuee.ToList();
            return View(bookingg);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingg = await _context.Bookingg
                .Include(b => b.Evvent)
                .Include(b => b.Venuee)
                .FirstOrDefaultAsync(m => m.Id== id);

            if (bookingg == null)
            {
                return NotFound();
            }

            return View(bookingg);
        }
    }
}