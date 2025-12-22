using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista alla meddelanden (Inbox)
        public IActionResult Index()
        {
            var messages = _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.MessageId) // Nyast först
                .ToList();
            return View(messages);
        }

        // Skriv nytt meddelande
        [HttpGet]
        public IActionResult Send()
        {
            // Vi behöver två listor med användare (Avsändare och Mottagare)
            ViewBag.Users = new SelectList(_context.Users, "UserId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Send(Message message)
        {
            message.Read = false; // Ej läst när det skickas
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}