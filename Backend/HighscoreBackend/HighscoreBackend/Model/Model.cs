using System.ComponentModel.DataAnnotations;

namespace HighscoreBackend.Model
{
    public class Highscore
    {
        public int HighscoreID { get; set; }

        [Required]
        public string Initials { get; set; }

        [Required]
        public int Score { get; set; }
    }
}
