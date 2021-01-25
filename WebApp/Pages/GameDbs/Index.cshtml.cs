using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_GameDbs
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public IndexModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }

        public IList<GameDb> GameDb { get;set; }

        public async Task OnGetAsync()
        {
            GameDb = await _context.Games
                .Include(g => g.GameSettingsDb).ToListAsync();
        }
    }
}
