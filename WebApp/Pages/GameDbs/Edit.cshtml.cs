using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_GameDbs
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public EditModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
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
           ViewData["GameSettingsDbId"] = new SelectList(_context.GameSettingses, "GameSettingsDbId", "GameSettingsDbId");
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(GameDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameDbExists(GameDb.GameDbId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool GameDbExists(int id)
        {
            return _context.Games.Any(e => e.GameDbId == id);
        }
    }
}
