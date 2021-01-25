using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameSettingsDb
    {
        public int GameSettingsDbId { get; set; }
        [Display(Name="Number of rows")]
        [Required(ErrorMessage="Please enter the {0}")]
        [Range(6, 255, ErrorMessage = "{0} has to be between {1} and {2}")]
        public int? BoardHeight { get; set; } = 6;
        [Display(Name="Number of columns")]
        [Required(ErrorMessage="Please enter the {0}")]
        [Range(7, 255, ErrorMessage = "{0} has to be between {1} and {2}")]
        public int? BoardWidth { get; set; } = 7;

        [Display(Name="Against computer")]
        public bool AgainstComputer { get; set; }
        [Display(Name="Computer starts")]
        [Required] public bool ComputerStarts { get; set; }
        [Display(Name="Name")]
        [Required(ErrorMessage="Please enter a name")]
        [MinLength (1, ErrorMessage = "Min length for {0} is {1}")]
        [MaxLength (64, ErrorMessage = "Max length for {0} is {1}")] 
        public string? Player1Name { get; set; }
        [Display(Name="Name")]
        [Required(ErrorMessage="Please enter a name")]
        [MinLength (1, ErrorMessage = "Min length for {0} is {1}")]
        [MaxLength (64, ErrorMessage = "Max length for {0} is {1}")]
        public string? Player2Name { get; set; }
        public string? ToDoButton { get; set; }
        public ICollection<GameDb>? GameDbs { get; set; }
    }
}