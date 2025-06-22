using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POE_PART_1.Models;

namespace POE_PART_1.Controllers
{
    public class EvventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchType, int? venueeId, DateTime? startDate, DateTime? endDate)
        {
            var evvents = _context.Evvent
                .Include(e => e.Venuee) // Include related venue
                .Include(e => e.EvventType) // Include related event type
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
                evvents = evvents.Where(e => e.EvventType.Name == searchType);

            if (venueeId.HasValue)
                evvents = evvents.Where(e => e.VenueeId == venueeId);

            if (startDate.HasValue && endDate.HasValue)
            {
                // Fix: Convert DateOnly to DateTime for comparison
                var startDateTime = startDate.Value.Date; // Extract DateTime.Date
                var endDateTime = endDate.Value.Date; // Extract DateTime.Date
                evvents = evvents.Where(e => e.Date.Date >= startDateTime && e.Date.Date <= endDateTime);
            }

            // Provide data from dropdown filters in the view
            ViewData["EventTypes"] = _context.EvventType.ToList();
            ViewData["Venues"] = _context.Venuee.ToList();

            return View(await evvents.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["Venues"] = _context.Venuee.ToList();

            //Part 3 Question (Step 5) Added: load event types for dropdown
            ViewData["EventTypes"] = _context.EvventType.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evvent @evvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@evvent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venues"] = _context.Venuee.ToList();

            //Part 3 Question (Step 5) Added: Reload event types on failed form submit
            ViewData["EventTypes"] = _context.EvventType.ToList();

            return View(@evvent);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @evvent = await _context.Evvent.FindAsync(id);
            if (@evvent == null) return NotFound();

            ViewData["Venues"] = _context.Venuee.ToList();

            // Part 3 Question (Step 5) Added: Load event types for dropdown in edit view
            ViewData["EventTypes"] = _context.EvventType.ToList();

            return View(@evvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evvent @evvent)
        {
            if (id != @evvent.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(@evvent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venues"] = _context.Venuee.ToList();

            ViewData["EventTypes"] = _context.EvventType.ToList();

            return View(@evvent);
        }

        // STEP 1: Confirm Deletion (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @evvent = await _context.Evvent
                .Include(e => e.Venuee)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@evvent == null) return NotFound();

            return View(@evvent);
        }

        // STEP 2: Perform Deletion (POST) - Logic to restrict the deletion of events associated with active bookings.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @evvent = await _context.Evvent.FindAsync(id);
            if (@evvent == null) return NotFound();

            var isBooked = await _context.Bookingg.AnyAsync(b => b.Id == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Evvent.Remove(@evvent);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @evvent = await _context.Evvent
                .Include(e => e.Venuee) // Include related venue if applicable
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@evvent == null)
            {
                return NotFound();
            }

            return View(@evvent);
        }
    }
}