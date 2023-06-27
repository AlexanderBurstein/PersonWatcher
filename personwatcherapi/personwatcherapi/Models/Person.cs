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
        [NotMapped]
        public double HowClose { get; set; }
        public int HowCloseToSunAndMoon(int[] contactPoints, int[] oppositePoints)
        {
            var (contacts, opposites) = HowCloseAndFar(contactPoints, oppositePoints);
            return Math.Max(contacts, opposites);
        }
        public (int, int) HowCloseAndFar(int[] contactPoints, int[] oppositePoints)
        {
            int contacts = GetPowerOfPoint(SunPos, contactPoints) +
                GetPowerOfPoint(MoonPos, contactPoints) +
                GetPowerOfPoint(MercuryPos, contactPoints) +
                GetPowerOfPoint(VenusPos, contactPoints) +
                GetPowerOfPoint(MarsPos, contactPoints) +
                GetPowerOfPoint(JupiterPos, contactPoints) +
                GetPowerOfPoint(SaturnPos, contactPoints) +
                GetPowerOfPoint(UranusPos, contactPoints) +
                GetPowerOfPoint(NeptunePos, contactPoints);
            int opposites = GetPowerOfPoint(SunPos, oppositePoints) +
                GetPowerOfPoint(MoonPos, oppositePoints) +
                GetPowerOfPoint(MercuryPos, oppositePoints) +
                GetPowerOfPoint(VenusPos, oppositePoints) +
                GetPowerOfPoint(MarsPos, oppositePoints) +
                GetPowerOfPoint(JupiterPos, oppositePoints) +
                GetPowerOfPoint(SaturnPos, oppositePoints) +
                GetPowerOfPoint(UranusPos, oppositePoints) +
                GetPowerOfPoint(NeptunePos, oppositePoints);
            return (contacts, opposites);
        }
        private int GetPowerOfPoint(int pos, int[] checkingPoints)
        {
            int result = 0;
            if (checkingPoints.Contains(pos))
            {
                result = checkingPoints.Count() - Array.IndexOf(checkingPoints, pos);
            }
            return result;
        }
    }
    public enum EventType
    {
        Single, TeamLead, TeamPlayer
    }
}                                                                                                                    