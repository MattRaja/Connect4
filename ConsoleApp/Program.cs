﻿using System;
using System.Collections.Generic;
 using System.Linq;
 using ConsoleUI;
 using DAL;
 using Domain;
 using GameEngine;
using MenuSystem;
 using Microsoft.EntityFrameworkCore;

 namespace ConsoleApp
{
    class Program
    {
        private static GameSettings _settings;
        private static GameState _gameState;
        private static Dictionary<string, MenuItem> _dictOfSaves;
        private static bool _fileNameCanceled;
        private static Menu _loadMenu;

        private static void Main()
        {
            Console.Clear();
            _gameState = new GameState();
            _settings = new GameSettings();
            InitialiseSettings();
            UpdateSavedGameList();
            Console.WriteLine($"Welcome to {_settings?.GameName}!");
            
            var againstComputerMenu = new Menu(2)
            {
                Title = "Please select who moves first",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "You",
                            CommandToExecute = () => StartComputerGame(false)
                        }
                        
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Computer",
                            CommandToExecute = () => StartComputerGame(true)
                        }
                    }
                }
            }; 
            
            _loadMenu = new Menu(3)
            {
                Title = "Please select a saved game",
                MenuItemsDictionary = _dictOfSaves!
            };
            
            var boardSizeMenu = new Menu(2)
            {
                Title = "Please select the board size",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "Default",
                            CommandToExecute = StartDefaultGame
                        }
                        
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Custom",
                            CommandToExecute = StartCustomGame
                        }
                    }
                }
            };
            
            var gameTypeMenu = new Menu(1)
            {
                Title = $"Start a new game of {_settings?.GameName}",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem()
                        {
                            Title = "Start a new 2-player game",
                            CommandToExecute = () => boardSizeMenu.Run()
                        }
                    },
                    {
                        "2", new MenuItem()
                        {
                            Title = "Play against computer",
                            CommandToExecute = () => againstComputerMenu.Run()
                        }
                    }, 
                    {
                        "3", new MenuItem()
                        {
                            Title = "Load a saved game",
                            CommandToExecute = () => _loadMenu.Run()
                        }
                    },
                }
            };
            
            var mainMenu = new Menu()
            {
                Title = "Main Menu",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "S", new MenuItem()
                        {
                            Title = "Start game",
                            CommandToExecute = () => gameTypeMenu.Run()
                        }
                    },
                    {
                        "D", new MenuItem()
                        {
                            Title = "Change board settings",
                            CommandToExecute = SaveSettings
                        }
                    }
                }
            };
            mainMenu.Run();
        }

        
        private static void InitialiseSettings()
        {
            using (var ctx = new AppDatabaseContext())
            {
                var defaultSettings = ctx.DefaultSettingses.ToList().LastOrDefault();
                _settings = new GameSettings {GameName = "Connect 4", AgainstComputer = false, ComputerStarts = false};
                _settings.BoardHeight = defaultSettings?.DefaultBoardHeight ?? 6;
                _settings.BoardWidth = defaultSettings?.DefaultBoardWidth ?? 7;
            }
            _gameState!.Board = null;
            _gameState!.Turn = false;
        }
        
        private static void UpdateSavedGameList()
        {
            using (var ctx = new AppDatabaseContext())
            {
                _dictOfSaves = new Dictionary<string, MenuItem>();
                var savedGames = ctx.SaveGames.ToList();
                foreach (var game in savedGames)
                {
                    _dictOfSaves!.Add((savedGames.IndexOf(game) + 1).ToString(),
                        new MenuItem()
                        {
                            Title = $"{game}",
                            CommandToExecute = () => LoadGame(game.ToString())
                        });
                }
            }
        }
        
        private static string StartDefaultGame()
        {
            InitialiseSettings();
            return StartGame();
        }
        
        private static string StartComputerGame(bool computerStarts)
        {
            InitialiseSettings();
            _settings!.AgainstComputer = true;
            _settings!.ComputerStarts = computerStarts;
            return StartGame();
        }
        
        private static string StartCustomGame()
        {
            var (boardHeight, boardWidth, isCanceled) = GetCustomSettings();
            if (isCanceled)
            {
                return "";
            }
            InitialiseSettings();
            _settings!.BoardHeight = boardHeight;
            _settings!.BoardWidth = boardWidth;
            return StartGame();
        } 
        private static void SaveGame(string filename)
        {
            
            Console.WriteLine("in console");
            Console.WriteLine(_settings.BoardHeight);
            Console.WriteLine(_settings.BoardWidth);
           Game.SaveGame(filename, _settings, _gameState);
        }

        private static string LoadGame(string file)
        {
            int saveGameId;
            using (var ctx = new AppDatabaseContext())
            {
                saveGameId = ctx.SaveGames.First(a => a.SaveGameName == file).SaveGameId;
            }
            var game = Game.LoadGame(saveGameId);
            return StartGame(game);
        }

        private static string SaveSettings()
        {
            var (boardHeight, boardWidth, userCanceled) = GetCustomSettings();
            if (!userCanceled)
            {
                _settings!.BoardHeight = boardHeight;
                _settings!.BoardWidth = boardWidth;
                using (var ctx = new AppDatabaseContext())
                {
                    var defaultGameSettings = new DefaultSettings
                    {
                        DefaultBoardHeight = _settings!.BoardHeight,
                        DefaultBoardWidth = _settings!.BoardWidth,
                        GameName = _settings!.GameName
                    };
                    ctx.DefaultSettingses.Add(defaultGameSettings);
                    ctx.SaveChanges();
                }
            }
            return "";
        }


        private static (int boardHeight, int boardWidth, bool isCanceled) GetCustomSettings()
        {
            bool userCanceled;
            int boardHeight;
            int boardWidth;
            (boardHeight, userCanceled) = GetUserInput($"Enter the number of rows: {Game.MinBoardHeight}-{Game.MaxBoardHeight}"
                , Game.MinBoardHeight, Game.MaxBoardHeight);
            if (userCanceled) return (0, 0, true);
            
            (boardWidth, userCanceled) = GetUserInput($"Enter the number of columns: {Game.MinBoardWidth}-{Game.MaxBoardWidth}"
                    , Game.MinBoardWidth, Game.MaxBoardWidth);
            return userCanceled ? (0, 0, true) : (boardHeight, boardWidth, false);
        }

        private static string StartGame(Game loadGame = null)
        {
            var firstRound = true;
            var game = loadGame ?? new Game(_settings!, _gameState!);
            _settings = game.GetGameSettings();
            _gameState = game.GetGameState();
            var done = false;
            do
            {
                if (_settings!.AgainstComputer && _settings!.ComputerStarts && game.isBoardEmpty())
                {
                    game.ComputerMoves();
                }
                //Console.Clear();
                GameUi.PrintBoard(game);
                
                int userXint;
                bool userCanceled;
                
                (userXint, userCanceled) = GetUserInput($"Choose a column: 1-{_settings!.BoardWidth}", 1,
                    _settings.BoardWidth);
                
                        
                if (userCanceled)
                {
                    if (!firstRound)  // Don't save a game with an empty board
                    {
                        string filename;
                        bool overWriteConfirmed;
                        var saved = false;
                        //Console.Clear();
                        do
                        {
                            filename = GetFileNameInput("Please enter the file name in which you wish to save");
                            if (_fileNameCanceled)
                            {
                                _fileNameCanceled = false;
                                break;
                            }
                            using (var ctx = new AppDatabaseContext())
                            {
                                if (ctx.SaveGames.ToList().Find(x => x.SaveGameName == filename) != null)
                                {
                                    overWriteConfirmed = OverWriteConfirmation();
                                    if (!overWriteConfirmed) continue;
                                }
                                _gameState!.Board = game.GetBoard();
                                _gameState!.Turn = game.PlayerXMoves();
                                SaveGame(filename);
                                UpdateSavedGameList();
                                _loadMenu.UpdateDict(_dictOfSaves!);
                                saved = true;
                            }
                        } while (!saved);
                    }
                    done = true;
                }
                else
                {
                    game.Move(userXint-1);
                    firstRound = false;
                    if (!game.GameOver) continue;
                    done = true;
                    Console.Clear();
                    GameUi.PrintBoard(game);
                    GameUi.PrintVictoryScreen();
                }

            } while (!done);
            
            return "Game over!";
        }

        private static bool OverWriteConfirmation()
        {
            do
            {
                Console.WriteLine("Another save file with the same name exists \n" +
                                  " Would you like to overwrite it?:  Y/N");

                Console.Write(">");
                var consoleLine = Console.ReadLine()?.ToUpper();
                if (consoleLine == "Y") return true;
                if (consoleLine == "N") return false;
            } while (true);
        }
        private static string GetFileNameInput(string prompt, string cancelStrValue = "Q")
        {
            do
            {
                Console.WriteLine(prompt);
                Console.WriteLine($"To cancel input enter: {cancelStrValue}");

                Console.Write(">");
                var consoleLine = Console.ReadLine()?.ToUpper();
                if (consoleLine == cancelStrValue)
                {
                    _fileNameCanceled = true;
                    return consoleLine!;
                }

                if (consoleLine == "")
                {
                    Console.Clear();
                    Console.WriteLine("File name must contain at least 1 character");
                }
                else
                {
                    return consoleLine!;
                }
            } while (true);
        }
        private static (int result, bool wasCanceled) GetUserInput(string prompt, int min, int max, int? cancelIntValue = 0, 
            string cancelStrValue = "Q")
        {
            do
            {
                Console.WriteLine(prompt);
                if (cancelIntValue.HasValue || !string.IsNullOrWhiteSpace(cancelStrValue))
                {
                    Console.WriteLine($"To cancel input enter: {cancelIntValue}" +
                                      $"{ (cancelIntValue.HasValue && !string.IsNullOrWhiteSpace(cancelStrValue) ? " or " : "") }" +
                                      $"{cancelStrValue}");
                }

                Console.Write(">");
                var consoleLine = Console.ReadLine();

                if (consoleLine?.ToUpper().Trim() == cancelStrValue) return (0, true);
                
                if (int.TryParse(consoleLine, out var userInt))
                {
                    if (!(userInt < min || userInt > max) || userInt == cancelIntValue)
                        return userInt == cancelIntValue ? (userInt, true) : (userInt, false);
                }
                if (consoleLine != "")
                    Console.WriteLine($"{consoleLine} is not a valid number!");
            } while (true);
        }
    }
}