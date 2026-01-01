using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id)
                .OrderByDescending(m => m.MessageId)
                .ToListAsync();

            return View("ViewMessages", userMessages);
        }

        //[HttpPost]
        //public async Task<IActionResult> MarkAsRead(int[] messageIds)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);

        //    var userMessages = await _context.Messages
        //        .Where(m => m.ReceiverId == currentUser.Id)
        //        .ToListAsync();

        //    foreach (var message in userMessages)
        //    {
        //        if (messageIds != null && messageIds.Contains(message.MessageId))
        //        {
        //             message.Read = true;
        //        }
        //        else
        //        {
        //            message.Read = false;
        //        }
        //    }
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

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

        // Skriv nytt meddelande
        [HttpGet]
        public IActionResult Send()
        {
            // Vi behöver två listor med användare (Avsändare och Mottagare)
            ViewBag.Users = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}