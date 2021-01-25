using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameDb
    {
        public int GameDbId { get; set; }
        [Required]
        [MaxLength (255)]
        public string Board { get; set; } = default!;
        // Explicit FK
        public int GameSettingsDbId { get; set; }
        public GameSettingsDb GameSettingsDb { get; set; } = default!;
        public ICollection<SaveGame>? SaveGames { get; set; }
    }
}