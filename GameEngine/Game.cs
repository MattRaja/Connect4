using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace GameEngine
{
    public class Game
    {
        private CellState[,] Board { get; set; }

        public static int MinBoardHeight { get; private set; } = 6;
        public static int MinBoardWidth { get; private set; } = 7;
        public static int MaxBoardHeight { get; private set; } = 100;
        public static int MaxBoardWidth { get; private set; } = 100;

        public bool GameOver { get; private set; }
        public static bool Draw { get; private set; }

        public static bool PlayerXMove { get; private set; }

        public int BoardWidth { get; private set; }

        public int BoardHeight { get; private set; }

        public bool AgainstComputer { get; private set; }
        public bool ComputerStarts { get; private set; }
        public string Player1Name { get; private set; }
        public string Player2Name { get; private set; }
        
        
        


        public Game(GameSettings settings, GameState? state = null)
        {
            ComputerStarts = settings.ComputerStarts;
            AgainstComputer = settings.AgainstComputer;
            Player1Name = settings.Player1Name;
            Player2Name = settings.Player2Name;

            BoardHeight = settings.BoardHeight;
            BoardWidth = settings.BoardWidth;
            PlayerXMove = state?.Turn ?? false;
            
            // initialize the board
            Board = state?.Board ?? new CellState[BoardHeight,BoardWidth];
        }

        public static Game LoadGame(int gameId)
        {
            GameSettings settings;
            GameState gameState;
            using (var ctx = new AppDatabaseContext())
            {
                var saveGame = ctx.SaveGames
                    .Include(p => p.GameDb)
                    .ThenInclude(p => p!.GameSettingsDb)
                    .First(a => a.SaveGameId == gameId);
                gameState = GameSaveHandler.BoardDeserializer(saveGame.GameDb!.Board);
                settings = new GameSettings()
                {
                    BoardHeight = saveGame!.GameDb.GameSettingsDb.BoardHeight!.Value,
                    BoardWidth = saveGame!.GameDb.GameSettingsDb.BoardWidth!.Value,
                    AgainstComputer = saveGame.GameDb.GameSettingsDb.AgainstComputer,
                    ComputerStarts = saveGame.GameDb.GameSettingsDb.ComputerStarts,
                    Player1Name = saveGame.GameDb.GameSettingsDb.Player1Name!,
                    Player2Name = saveGame.GameDb.GameSettingsDb.Player2Name!
                };
            }
            Console.WriteLine("inload");
            Console.WriteLine(settings.BoardHeight);
            Console.WriteLine(settings.BoardWidth);
            return new Game(settings, gameState);
        }

        public GameSettings GetGameSettings()
        {
            return new GameSettings()
            {
                BoardHeight = BoardHeight,
                BoardWidth = BoardWidth,
                AgainstComputer = AgainstComputer,
                ComputerStarts = ComputerStarts,
                Player1Name = Player1Name,
                Player2Name = Player2Name
            };
        }

        public GameState GetGameState()
        {
            return new GameState()
            {
                Board = Board,
                Turn = PlayerXMove,
                GameName = "Connect 4"
            };
        }
        
        public static Game RestoreBoardState(int gameId)
        {
            GameSettings settings;
            GameState gameState;
            using (var ctx = new AppDatabaseContext())
            {
                var game = ctx.Games.First(p => p.GameDbId == gameId);
                var gameSettings = ctx.GameSettingses
                    .First(p => p.GameSettingsDbId == game.GameSettingsDbId);
                gameState = GameSaveHandler.BoardDeserializer(game.Board);
                
                settings = new GameSettings()
                {
                    AgainstComputer = gameSettings.AgainstComputer,
                    ComputerStarts = gameSettings.ComputerStarts,
                    BoardHeight = gameSettings.BoardHeight!.Value,
                    BoardWidth = gameSettings.BoardWidth!.Value,
                    Player1Name = gameSettings.Player1Name!,
                    Player2Name = gameSettings.Player2Name!,
                };
            }
            return new Game(settings, gameState);
        }
        
        public static void SaveGame(string filename, GameSettings gameSettings, GameState gameState)
        {
            using (var ctx = new AppDatabaseContext())
            {
                var serializedBoard = GameSaveHandler.BoardSerializer(gameState);
                var entity = ctx.SaveGames.FirstOrDefault(item => item.SaveGameName == filename);
                if (entity != null)
                {
                    var gameSettingsDb = new GameSettingsDb
                    {
                        GameSettingsDbId = entity.GameDbId,
                        BoardHeight = gameSettings.BoardHeight,
                        BoardWidth = gameSettings.BoardWidth,
                        AgainstComputer = gameSettings.AgainstComputer,
                        ComputerStarts = gameSettings.ComputerStarts,
                        Player1Name = gameSettings.Player1Name,
                        Player2Name = gameSettings.Player2Name
                    };
                    entity.GameDb = new GameDb()
                    {
                        GameDbId = entity.GameDbId,
                        Board = serializedBoard,
                        GameSettingsDb = gameSettingsDb
                    };
                    ctx.SaveGames.Update(entity);
                } else {
                    ctx.SaveGames.Add(new SaveGame()
                    {
                        SaveGameName = filename,
                        GameDb = new GameDb()
                        {
                            Board = serializedBoard,
                            GameSettingsDb = new GameSettingsDb() 
                            {
                                BoardHeight = gameSettings.BoardHeight,
                                BoardWidth = gameSettings.BoardWidth,
                                AgainstComputer = gameSettings.AgainstComputer,
                                ComputerStarts = gameSettings.ComputerStarts,
                                Player1Name = gameSettings.Player1Name,
                                Player2Name = gameSettings.Player2Name
                            }
                        }
                    });
                }
                ctx.SaveChanges();
            }
        }

        public CellState[,] GetBoard()
        {
            //Console.WriteLine((BoardHeight, BoardWidth));
            var result = new CellState[BoardHeight, BoardWidth];
            Array.Copy(Board, result, Board.Length);
            return result;
        }

        public bool PlayerXMoves()
        {
            return PlayerXMove;
        }
        
        public void Move(int posX)
        {
            if (posX >= BoardWidth)
            {
                return;
            }
            
            if (Board[0, posX] != CellState.Empty) // if the topmost slot of the board is already occupied
            {
                return;
            }

            var posY = BoardHeight - Filter(Board, u => u[posX] != CellState.Empty).Count() - 1;
            Board[posY, posX] = PlayerXMove ? CellState.X : CellState.O;
            PlayerXMove = !PlayerXMove;
        
            HasWon(Board);
            if (GameOver)
            {
                return;
            }

            if (AgainstComputer)
            {
                ComputerMoves();
            }


//            if(Board.All(CellState x => x))
//            {
//                Draw = true;
//                GameOver = true;
//            }

        }

        public void ComputerMoves()
        {
            (int y, int x) preferredMove = (-2, -2);
            for (var i = 0; i < BoardWidth; i++)
            {
                if (Board[0, i] == CellState.Empty)
                {
                    var pozY = BoardHeight - Filter(Board, u => u[i] != CellState.Empty).Count() - 1;
                    Board[pozY, i] = CellState.X;
                    HasWon(Board);
                    if (GameOver) 
                    {
                        preferredMove = (pozY, i);
                        var prevValue = Board[pozY, i];
                        Board[pozY, i] = PlayerXMove ? CellState.X : CellState.O;
                        if (Board[pozY, i] != prevValue) // Player had a chance to win
                        {
                            GameOver = false;
                        }
                        else
                        {
                            break; // Winning move for computer
                        }
                    }

                    Board[pozY, i] = CellState.O;
                    HasWon(Board);
                    if (GameOver)
                    {
                        preferredMove = (pozY, i);
                        var prevValue = Board[pozY, i];
                        Board[pozY, i] = PlayerXMove ? CellState.X : CellState.O;
                        if (Board[pozY, i] != prevValue) // Player had a chance to win
                        {
                            GameOver = false;
                        }
                        else
                        {
                            break; // Winning move for computer
                        }
                    }

                    Board[pozY, i] = CellState.Empty;
                }
            }

            if (preferredMove != (-2, -2))
            {
                Board[preferredMove.y, preferredMove.x] = PlayerXMove ? CellState.X : CellState.O;
            }
            else
            {
                var random = new Random();
                int randomX;
                do
                {
                    randomX = random.Next(0, BoardWidth - 1);
                } while (Board[0, randomX] != CellState.Empty);

                var pozY = BoardHeight - Filter(Board, u => u[randomX] != CellState.Empty).Count() - 1;
                Board[pozY, randomX] = PlayerXMove ? CellState.X : CellState.O;
            }

            PlayerXMove = !PlayerXMove;
        }

        private static IEnumerable<T[]> Filter<T>(T[,] source, Func<T[], bool> predicate)
        {
            for (var i = 0; i < source.GetLength(0); ++i)
            {
                var values = new T[source.GetLength(1)];
                for (var j = 0; j < values.Length; ++j)
                {
                    values[j] = source[i, j];
                }

                if (predicate(values))
                {
                    yield return values;
                }
            }
        }
        
        public bool isBoardEmpty() {
            for (var i = 0; i < Board.GetLength(1); i++) {
                if(Board[Board.GetLength(0)-1, i] != CellState.Empty) { 
                    return false;
                }
            }
            return true;  
        }

        private void HasWon(CellState[,] board)
        {
            
            int[][] directions = {new[] {1, 0}, new[] {1, -1}, new[] {1, 1}, new[] {0, 1}};
            foreach (var d in directions)
            {
                var dx = d[0];
                var dy = d[1];
                for (var x = 0; x < BoardHeight; x++)
                {
                    for (var y = 0; y < BoardWidth; y++)
                    {
                        var lastx = x + 3 * dx;
                        var lasty = y + 3 * dy;
                        if (0 <= lastx && lastx < BoardHeight && 0 <= lasty && lasty < BoardWidth)
                        {
                            var w = board[x, y];
                            if (w != CellState.Empty && w == board[x + dx, y + dy]
                                                     && w == board[x + 2 * dx, y + 2 * dy]
                                                     && w == board[lastx, lasty])
                            {
                                GameOver = true;
                                return;
                            }
                        }
                    }
                }
            }
            //check for a draw
            for (var i = 0; i < BoardWidth; i++)
            {
                if (Board[0, i] == CellState.Empty)
                {
                    Draw = false;
                    GameOver = false;
                    return;
                }
                Draw = true;
                GameOver = true;
            }
        }
    }
}
