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
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public DetailsModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }

        public GameDb GameDb { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GameDb = await _context.Games
                .Include(g => g.GameSettingsDb).FirstOrDefaultAsync(m => m.GameDbId == id);

            if (GameDb == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
