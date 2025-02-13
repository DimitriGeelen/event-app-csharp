using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventApp.Web.Data;
using EventApp.Web.Models;
using EventApp.Web.Services;

namespace EventApp.Web.Controllers;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly ILocationService _locationService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        ApplicationDbContext context,
        IFileService fileService,
        ILocationService locationService,
        ILogger<EventsController> logger)
    {
        _context = context;
        _fileService = fileService;
        _locationService = locationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var events = await _context.Events
            .OrderByDescending(e => e.StartDateTime)
            .ToListAsync();
        return View(events);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,StartDateTime,EndDateTime,Location,Address,Latitude,Longitude")] Event @event, IFormFile? image, IFormFile? document)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (image != null)
                {
                    @event.ImagePath = await _fileService.SaveFileAsync(image, "images");
                }

                if (document != null)
                {
                    @event.DocumentPath = await _fileService.SaveFileAsync(document, "documents");
                }

                if (!string.IsNullOrEmpty(@event.Address) && (!@event.Latitude.HasValue || !@event.Longitude.HasValue))
                {
                    var location = await _locationService.GeocodeAddressAsync(@event.Address);
                    @event.Latitude = location.Latitude;
                    @event.Longitude = location.Longitude;
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                ModelState.AddModelError("", "An error occurred while creating the event. Please try again.");
            }
        }
        return View(@event);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
        {
            return NotFound();
        }
        return View(@event);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDateTime,EndDateTime,Location,Address,Latitude,Longitude,ImagePath,DocumentPath")] Event @event, IFormFile? image, IFormFile? document)
    {
        if (id != @event.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (image != null)
                {
                    if (!string.IsNullOrEmpty(@event.ImagePath))
                    {
                        _fileService.DeleteFile(@event.ImagePath);
                    }
                    @event.ImagePath = await _fileService.SaveFileAsync(image, "images");
                }

                if (document != null)
                {
                    if (!string.IsNullOrEmpty(@event.DocumentPath))
                    {
                        _fileService.DeleteFile(@event.DocumentPath);
                    }
                    @event.DocumentPath = await _fileService.SaveFileAsync(document, "documents");
                }

                if (!string.IsNullOrEmpty(@event.Address) && (!@event.Latitude.HasValue || !@event.Longitude.HasValue))
                {
                    var location = await _locationService.GeocodeAddressAsync(@event.Address);
                    @event.Latitude = location.Latitude;
                    @event.Longitude = location.Longitude;
                }

                _context.Update(@event);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
            }
        }
        return View(@event);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var @event = await _context.Events
            .FirstOrDefaultAsync(m => m.Id == id);
        if (@event == null)
        {
            return NotFound();
        }

        return View(@event);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            try
            {
                if (!string.IsNullOrEmpty(@event.ImagePath))
                {
                    _fileService.DeleteFile(@event.ImagePath);
                }
                if (!string.IsNullOrEmpty(@event.DocumentPath))
                {
                    _fileService.DeleteFile(@event.DocumentPath);
                }

                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Event deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {Id}", id);
                TempData["Error"] = "An error occurred while deleting the event.";
            }
        }
        
        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}