using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync((ClaimsPrincipal)UserClaimsPrincipal);
            
            if (currentUser == null) 
            {
                return View(0);
            }
            var CurrentUserId = currentUser.Id;

            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == CurrentUserId && !m.Read)
                .CountAsync();

            return View(unreadMessages);
        }

    }
}
