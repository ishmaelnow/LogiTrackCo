using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        // ✅ Constructor Injection - receives the context from DI container
        public InventoryController(LogiTrackContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> GetInventory()
        {
            var items = _context.InventoryItems.Include(i => i.Order).ToList();
            return Ok(items);
        }

        // Other actions remain unchanged, using _context
    }
}
