using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userMessages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == currentUser.Id)
                .OrderByDescending(m => m.MessageId)
                .ToListAsync();

            foreach (var message in userMessages)
            {
                //Om avsändaren är avaktiverad så sätts avsändarnamnet till "[Avaktiverad användare]"
                if (message.Sender?.Deactivated == true)
                {
                    message.SenderName = "[Avaktiverad användare]";
                }
            }
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
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                message.SenderId = currentUser.Id;
                message.SenderName = currentUser.Name;
            }

            message.SentAt = DateTime.Now;

            //SentAt måste ignoreras från ModelState då SentAt sätts efter ModelState sätts och SentAt inte får vara null
            ModelState.Remove("SentAt");

            if (ModelState.IsValid)
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Meddelandet har skickats.";
            }
            //Ett anonymt objekt med id skickas med så att rätt profilsida visas(den man var på) när ett meddelande har skickats
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