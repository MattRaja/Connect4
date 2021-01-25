using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class ConfirmationPageModel : PageModel
    {
        
        private readonly DAL.AppDatabaseContext _context;

        public ConfirmationPageModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }
        
        [BindProperty]

        public string SaveName { get; set; }
        public bool GameOver { get; set; }
        
        public void OnGet(string saveName)
        {
            SaveName = saveName;
        }
        
        public async Task<IActionResult> OnPostAsync(bool overWriteBool, bool gameOver)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            GameOver = gameOver;
            var game = _context.Games.ToList().Last();
            var lastGame = _context.Games.ToList().Count;
            var entity = _context.SaveGames.FirstOrDefault(item => item.SaveGameName == SaveName.ToUpper());
            if (overWriteBool)
            {
                entity.GameDb = game;
                entity.SaveGameName = SaveName.ToUpper();
                _context.SaveGames.Update(entity);
                await _context.SaveChangesAsync();
                return RedirectToPage("PlayGame", new {gameId = lastGame, gameOver = GameOver});
            }
            return RedirectToPage("SavePage", new {saveName = SaveName, gameId = lastGame, gameOver = GameOver});
        }
    }
}