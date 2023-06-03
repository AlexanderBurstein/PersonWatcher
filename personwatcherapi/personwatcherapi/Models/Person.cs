using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
        [Range(0, 360)]
        [DefaultValue(0)]
        public int MercuryPos { get; set; }
        [Required]
        [Range(0, 360)]
        [DefaultValue(0)]
        public int UranusPos { get; set; }
        [Required]
        public DateTime NextStart { get; set; }
        [Required]
        public int PlaceId { get; set; }
        [Required]
        [DefaultValue(0)]
        public int EventPredictability { get; set; }
        [ForeignKey("PlaceId")]
        public Place Place { get; set; }
        [NotMapped]
        public string ExtraInfo { get; set; }
        public int HowCloseToSunAndMoon(int[] contactPoints, int[] oppositePoints)
        {
            int contacts = (contactPoints.Contains(SunPos) ? 1 : 0) +
                (contactPoints.Contains(MoonPos) ? 1 : 0) +
                (contactPoints.Contains(MercuryPos) ? 1 : 0) +
                (contactPoints.Contains(VenusPos) ? 1 : 0) +
                (contactPoints.Contains(MarsPos) ? 1 : 0) +
                (contactPoints.Contains(JupiterPos) ? 1 : 0) +
                (contactPoints.Contains(SaturnPos) ? 1 : 0) +
                (contactPoints.Contains(UranusPos) ? 1 : 0) +
                (contactPoints.Contains(NeptunePos) ? 1 : 0);
            int opposites = (oppositePoints.Contains(SunPos) ? 1 : 0) +
                (oppositePoints.Contains(MoonPos) ? 1 : 0) +
                (oppositePoints.Contains(MercuryPos) ? 1 : 0) +
                (oppositePoints.Contains(VenusPos) ? 1 : 0) +
                (oppositePoints.Contains(MarsPos) ? 1 : 0) +
                (oppositePoints.Contains(JupiterPos) ? 1 : 0) +
                (oppositePoints.Contains(SaturnPos) ? 1 : 0) +
                (oppositePoints.Contains(UranusPos) ? 1 : 0) +
                (oppositePoints.Contains(NeptunePos) ? 1 : 0);
            return Math.Max(contacts, opposites);
        }
    }
    public enum EventType
    {
        Single, TeamLead, TeamPlayer
    }
}
