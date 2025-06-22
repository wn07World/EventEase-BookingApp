using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POE_PART_1.Models;
using Azure.Storage.Blobs; // Add this namespace for BlobServiceClient
using System.Globalization;
using System.Net;

namespace POE_PART_1.Controllers
{
    public class VenueeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenueeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venuee.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venuee venuee)
        {
            if (ModelState.IsValid)
            {
                //handles image upload to Azure Blob Storage if an image file is provided
                //This is the step 4c. Modify Controller to receive ImageFile from View (user upload)
                if (venuee.Image != null)
                {
                    //This is step 5: Upload selected image to Azure Blob Storage
                    var BlobUrl = await UploadImageToBlobAsync(venuee.Image);

                    //Step 6: Save the Blob URL into ImageUrl property (the database)
                    venuee.ImageUrl = BlobUrl;
                }

                _context.Add(venuee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(venuee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venuee = await _context.Venuee.FindAsync(id);
            if (venuee == null) return NotFound();

            return View(venuee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venuee venuee)
        {
            if (id != venuee.VenueeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
               {
                      if(venuee.Image != null)
                      {
                        //Step 4c Upload new image if provided
                        var BlobUrl = await UploadImageToBlobAsync(venuee.Image);


                        //Step 6
                        //update Venue.ImageUrl with the new Blob URL
                        venuee.ImageUrl = BlobUrl;
                      }
                      else
                      {
                        //keep the existing ImageURL (Optional depending on your UI design)
                      }

                      _context.Update(venuee);
                      await _context.SaveChangesAsync();
                      TempData["SuccessMessage"] = "Venue updated successfully.";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueeExists(venuee.VenueeId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(venuee);
        }

        // STEP 1: Confirm Deletion (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venuee = await _context.Venuee.FirstOrDefaultAsync(v => v.VenueeId == id);
            if (venuee == null) return NotFound();

            return View(venuee);
        }

        // STEP 2: Perform Deletion (POST) - Logic to restrict the deletion of venues associated with active bookings.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venuee = await _context.Venuee.FindAsync(id);
            if (venuee == null) return NotFound();

            var hasBookinggs = await _context.Bookingg.AnyAsync(b => b.Id == id);
            if (hasBookinggs)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venuee.Remove(venuee);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venuee = await _context.Venuee
                .FirstOrDefaultAsync(m => m.VenueeId == id);

            if (venuee == null)
            {
                return NotFound();
            }

            return View(venuee);
        }


        private async Task<string> UploadImageToBlobAsync(IFormFile image)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=eventeast;AccountKey=GM7fKtCDk4plf1eS2tHNELd7GZTfSfPvds8scolqcCUt9ta+cg3R3LoYUCVnq+5CsOHfBj00b0cV+AStAxT4SQ==;EndpointSuffix=core.windows.net";
            var containerName = "eventease";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(image.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = image.ContentType
            };

            using (var stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }
        private bool VenueeExists(int id)
        {
            return _context.Venuee.Any(e => e.VenueeId == id);
        }
    }
} 