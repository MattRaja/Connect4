using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class DefaultSettings
    {
        public int DefaultSettingsId { get; set; }
        [Required]
        [MinLength (6)]
        [MaxLength (255)]
        public int DefaultBoardHeight { get; set; } = default!;
        [Required]
        [MinLength (6)]
        [MaxLength (255)]
        public int DefaultBoardWidth { get; set; } = default!;
        [Required]
        [MinLength (1)]
        [MaxLength (255)]
        public string GameName { get; set; } = default!;
    }
}