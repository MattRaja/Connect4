using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class PlayGameModel : PageModel
    {
        private readonly DAL.AppDatabaseContext _context;

        public PlayGameModel(DAL.AppDatabaseContext context)
        {
            _context = context;
        }
        
        public GameEngine.Game Game;
        public int GameId { get; set; }
        public bool PlayerXTurn { get; set; }
        public bool GameOver { get; set; }
        public string GameOverMessage { get; set; } = "";
        public bool OnlyComputers { get; set; }


        public ActionResult OnGet(int? gameId, int? col, bool? fromLoad, bool? gameOver, bool? onlyComputers)
        {
            if (gameId == null)
            {
                return RedirectToPage("../Index");
            }

            if (gameOver != null) GameOver = gameOver.Value;
            if (onlyComputers != null) OnlyComputers = onlyComputers.Value;
            GameId = gameId.Value;
            Game = fromLoad == true ? GameEngine.Game.LoadGame(GameId) : GameEngine.Game.RestoreBoardState(GameId);
            if (fromLoad == true)
            {
                GameId = _context.SaveGames.First(p => p.SaveGameId == GameId).GameDbId;
            }

            if (OnlyComputers)
            {
                if (!GameOver)
                {
                    Game.ComputerMoves();
                    GameOver = Game.GameOver;
                }
            }
            else
            {

                if (Game.AgainstComputer && Game.ComputerStarts && Game.isBoardEmpty())
                {
                    Game.ComputerMoves();
                }

                if (col != null && !GameOver)
                {
                    Game.Move(col.Value);
                    GameOver = Game.GameOver;
                }
            }

            if (GameOver)
            {
                GameOverMessage = "GAME OVER! \n" +
                                  $"{(PlayerXTurn ? Game.Player2Name : Game.Player1Name)} has won!";
            }
            PlayerXTurn = Game.PlayerXMoves();
            var gameState = new GameState()
            {
                GameName = "Connect 4",
                Turn = Game.PlayerXMoves(),
                Board = Game.GetBoard()
            };
            var entity = _context.Games.FirstOrDefault(item => item.GameDbId == GameId);
            entity.Board = GameSaveHandler.BoardSerializer(gameState);
            _context.Games.Update(entity);
            _context.SaveChanges();
            if (OnlyComputers)
            {
                RedirectToPage("./Game/PlayGame", new {GameId, onlycomputers = OnlyComputers});
            }
            return Page();
        }
    }
}