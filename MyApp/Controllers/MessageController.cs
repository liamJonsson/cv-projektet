using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Visa användarens meddelanden
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id)
                .OrderByDescending(m => m.MessageId)
                .ToListAsync();

            return View(userMessages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateRead(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);

            if(message != null)
            {
                message.Read = !message.Read;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Send(Message message)
        {
            message.SentAt = DateTime.Now;

            ModelState.Remove("SentAt");
            ModelState.Remove("Sender");
            ModelState.Remove("Receiver");

            if (ModelState.IsValid)
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                //"Meddelandet har skickats" läggs till i lådan "Sent"
                TempData["SuccessMessage"] = "Meddelandet har skickats.";
            }
            return RedirectToAction("Profile", "User", new { id = message.ReceiverId});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            var currentUser = await _userManager.GetUserAsync(User);

            if(message != null && message.ReceiverId == currentUser.Id)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Meddelandet har tagits bort!";
            }
            return RedirectToAction("Index");
        }
    }
}