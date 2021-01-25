using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SaveGame
    {
        [Display(Name="Game name")]
        public int SaveGameId { get; set; }
        [Display(Name="Save name")]
        [Required(ErrorMessage="Please enter a name")]
        [MinLength (1, ErrorMessage = "Min length for {0} is {1}")]
        [MaxLength (255, ErrorMessage = "Max length for {0} is {1}")]
        public string? SaveGameName { get; set; } = default!;
        public int GameDbId { get; set; }
        public GameDb? GameDb { get; set; } = default!;
        public override string ToString()
        {
            return $"{SaveGameName}";
        }
    }
}