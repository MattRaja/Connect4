
namespace GameEngine
{
    public class GameState
    {
        public string GameName { get; set; } = "Connect 4";
        public bool Turn { get; set; }
        public CellState[,]? Board { get; set; }

    }

        
}