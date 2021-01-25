﻿using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class SavePageModel : PageModel
    {
        
        private readonly DAL.AppDatabaseContext _context;
        public bool FromSave { get; set; } = true;
        public int GameId { get; set; }
        public string SaveName { get; set; }
        public bool GameOver { get; set; }

        public SavePageModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }
        
        [BindProperty] public SaveGame SaveGame { get; set; }
        
        public void OnGet(string? saveName, int? gameId, bool? gameOver)
        {
            if (gameId != null)
            {
                GameId = gameId.Value;
            } 
            if (gameOver != null)
            {
                GameOver = gameOver.Value;
            }
            if (saveName != null)
            {
                SaveName = saveName;
            }
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine(SaveGame.SaveGameName.ToUpper());
            if (_context.SaveGames.ToList().Find(x => x.SaveGameName == SaveGame.SaveGameName.ToUpper()) != null)
            {
                return RedirectToPage("ConfirmationPage", new {saveName = SaveGame.SaveGameName, gameOver = GameOver});
            }

            
            var game = _context.Games.ToList().Last();
            SaveGame.GameDb = game;
            SaveGame.SaveGameName = SaveGame.SaveGameName.ToUpper();
            _context.SaveGames.Add(SaveGame);
            await _context.SaveChangesAsync();
            var lastSave = _context.SaveGames.ToList().Last().SaveGameId;
            return RedirectToPage("PlayGame", new {gameId = lastSave, fromLoad = true, gameOver = GameOver});
        }
    }
}