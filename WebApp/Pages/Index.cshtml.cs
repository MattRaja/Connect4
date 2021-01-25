using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDatabaseContext _context;

        public IndexModel(AppDatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string PlayerX { get; set; }
        [BindProperty]
        public string PlayerO { get; set; }
        
        [BindProperty]
        public GameSettingsDb GameSettingsDb { get; set; }

        public int GameId { get; set; }
        public bool OnlyComputers { get; set; }

     
        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (PlayerO == "human" && PlayerX == "human")
                {
                    GameSettingsDb.AgainstComputer = false;
                    GameSettingsDb.ComputerStarts = false;
                }
                if (PlayerO == "computer" && PlayerX == "human")
                {
                    GameSettingsDb.AgainstComputer = true;
                    GameSettingsDb.ComputerStarts = true;
                }
                if (PlayerO == "human" && PlayerX == "computer")
                {
                    GameSettingsDb.AgainstComputer = true;
                    GameSettingsDb.ComputerStarts = false;
                }
                if (PlayerO == "computer" && PlayerX == "computer")
                {
                    GameSettingsDb.AgainstComputer = true;
                    GameSettingsDb.ComputerStarts = true;
                    OnlyComputers = true;
                }
                if (GameSettingsDb.ToDoButton == "Load Game")
                {
                    return RedirectToPage("./Game/LoadGamesPage");
                }

                var gameSettings = GameSettingsDbToGameSettings(GameSettingsDb);
                using (var ctx = new AppDatabaseContext())
                {
                    var game = new GameEngine.Game(gameSettings);
                    var gameState = new GameState()
                    {
                        GameName = gameSettings.GameName,
                        Turn = game.PlayerXMoves(),
                        Board = game.GetBoard()
                    };
                
                    ctx.Games.Add(new GameDb() {
                        Board = GameSaveHandler.BoardSerializer(gameState),
                        GameSettingsDb = GameSettingsDb
                        
                    });
                    ctx.SaveChanges();
                    var gameList = ctx.Games.ToList();
                    GameId = gameList.Count != 0 ? gameList.Last().GameDbId: 1;
                    
                }
                await _context.SaveChangesAsync();
            
                return OnlyComputers ? RedirectToPage("./Game/PlayGame", new {GameId, onlycomputers = OnlyComputers}) :
                    RedirectToPage("./Game/PlayGame", new {GameId});
            }

            return Page();
        }

        private GameSettings GameSettingsDbToGameSettings(GameSettingsDb gameSettingsDb)
        {
            return new GameSettings()
            {
                BoardHeight = gameSettingsDb.BoardHeight.Value,
                BoardWidth = gameSettingsDb.BoardWidth.Value,
                ComputerStarts = gameSettingsDb.ComputerStarts,
                AgainstComputer = gameSettingsDb.AgainstComputer,
                Player1Name = gameSettingsDb.Player1Name,
                Player2Name = gameSettingsDb.Player2Name
            };
        }
    }
}