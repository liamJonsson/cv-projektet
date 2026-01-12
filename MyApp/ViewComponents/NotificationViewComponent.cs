using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;

namespace MyApp.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationViewComponent(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Metoden tar fram antalet olästa meddeladen för den inloggade användaren
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Hämtar den inloggade användaren med "UserClaimsPrincipal" istället för "User" då man är i en ViewComponent
            var currentUser = await _userManager.GetUserAsync((ClaimsPrincipal)UserClaimsPrincipal);

            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id && m.Read == false)
                .CountAsync();

            //Antalet olästa meddelanden skickas till komponentens vy (Default.cshtml)
            return View(unreadMessages);
        }

    }
}
