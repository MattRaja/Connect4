namespace GameEngine
{
    public class GameSettings
    {
        public string GameName { get; set; } = "Connect 4";
        public int BoardHeight { get; set; } = 6;
        public int BoardWidth { get; set; } = 7;
        public bool AgainstComputer { get; set; }
        public bool ComputerStarts { get; set; }
        public string Player1Name { get; set; } = "Player 1";
        public string Player2Name { get; set; } = "Player 2";
        public string? ToDoButton { get; set; }
    }
}