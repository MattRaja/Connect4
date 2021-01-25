using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Game
{
    public class LoadGamesPageModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public LoadGamesPageModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }
        
        [BindProperty] public SaveGame SaveGame { get; set; } = default!;
        public SelectList SaveGameSelectList { get; set; } = default!;
        
        public IActionResult OnGet()
        {
            SaveGameSelectList = new SelectList(_context.SaveGames, nameof(SaveGame.SaveGameId), nameof(SaveGame.SaveGameName));
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            /*if (!ModelState.IsValid)
            {
            Console.WriteLine("hereog");
                return Page();
            }*/

            var saveGame = _context.SaveGames.First(p => p.SaveGameId == SaveGame.SaveGameId);
            var gameId = saveGame.SaveGameId;
            await _context.SaveChangesAsync();

            return RedirectToPage("PlayGame", new {gameId, fromLoad = true});
        }
    }
}