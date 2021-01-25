using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_GameDbs
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public CreateModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["GameSettingsDbId"] = new SelectList(_context.GameSettingses, "GameSettingsDbId", "GameSettingsDbId");
            return Page();
        }

        [BindProperty]
        public GameDb GameDb { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Games.Add(GameDb);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
