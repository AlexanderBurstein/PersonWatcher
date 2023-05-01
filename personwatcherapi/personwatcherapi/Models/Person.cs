using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personwatcherapi.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public EventType EventType { get; set; }
        [Required]
        [Range(0,360)]
        public int SunPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int MoonPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int VenusPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int MarsPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int JupiterPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int SaturnPos { get; set; }
        [Required]
        [Range(0, 360)]
        public int NeptunePos { get; set; }
        [Required]
        public DateTime NextStart { get; set; }
        [Required]
        public int PlaceId { get; set; }
        [ForeignKey("PlaceId")]
        public Place Place { get; set; }
    }
    public enum EventType
    {
        Single, TeamLead, TeamPlayer
    }
}
