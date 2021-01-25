﻿using System;
 using System.Collections.Generic;
 using System.ComponentModel;
 using System.Linq;
 using DAL;

 namespace MenuSystem
{
    public class Menu
    {
        private readonly int _menuLevel;

        private const string MenuCommandExit = "X";
        private const string MenuCommandReturnToPrevious = "P";
        private const string MenuCommandReturnToMain = "M";
        
        private Dictionary<string, MenuItem> _menuItemsDictionary;

        public Menu(int menuLevel = 0)
        {
            _menuLevel = menuLevel;
            _menuItemsDictionary = new Dictionary<string, MenuItem>();
        }
        
        public string? Title { get; set; }

        public Dictionary<string, MenuItem> MenuItemsDictionary
        {
            get => _menuItemsDictionary;
            set
            {
                _menuItemsDictionary = value;

                if (_menuLevel >= 2)
                {
                    _menuItemsDictionary.Add(MenuCommandReturnToPrevious, 
                        new MenuItem(){Title = "Return to Previous Menu"});
                }
                if (_menuLevel >= 1)
                {
                    _menuItemsDictionary.Add(MenuCommandReturnToMain, 
                        new MenuItem(){Title = "Return to Main Menu"});
                }
                _menuItemsDictionary.Add(MenuCommandExit, 
                    new MenuItem(){ Title = "Exit"});
            }
        }

        public void UpdateDict(Dictionary<string, MenuItem> dict)
        {
            MenuItemsDictionary = new Dictionary<string, MenuItem>();
            MenuItemsDictionary = dict;
        }

        public int GetMenuLevel()
        {
            return _menuLevel;
        }
        
        public string Run()
        {
            string command;
            
            do
            {
                Console.WriteLine(Title);
                Console.WriteLine(string.Concat(Enumerable.Repeat("=", Title!.Length)));
         
                foreach (var menuItem in MenuItemsDictionary)
                {
                    Console.Write(menuItem.Key);
                    Console.Write(" ");
                    Console.WriteLine(menuItem.Value);
                }
                
                Console.WriteLine("----------");
                Console.Write(">");

                command = Console.ReadLine()?.Trim().ToUpper() ?? "";


                var returnCommand = "";

                if (MenuItemsDictionary.ContainsKey(command))
                {
                    var menuItem = MenuItemsDictionary[command];
                    if (menuItem.CommandToExecute != null)
                    {
                        returnCommand = menuItem.CommandToExecute(); // run the command 
                    }
                }



                if (returnCommand == MenuCommandExit)
                {
                    command = MenuCommandExit;
                }
                
                if (returnCommand == MenuCommandReturnToMain)
                {
                    command = MenuCommandReturnToMain;
                }

                if (command == MenuCommandReturnToMain)
                {
                    if (_menuLevel < 1)
                    {
                        command = "";
                    }
                }

                if (command == MenuCommandReturnToPrevious && _menuLevel < 2)
                {
                    command = "";
                }
                
            } while (command != MenuCommandExit && 
                     command != MenuCommandReturnToMain && 
                     command != MenuCommandReturnToPrevious);

            
            return command;
        }
    }
}