using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace personwatcherapi.Models
{
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }
        [Required]
        [MaxLength]
        public string Placename { get; set; }
        [Required]
        [RegularExpression(@"^[NS][0-8]\d.[0-5]\d.[0-5]\d$")]
        public string Latitude { get; set; }
        [Required]
        [RegularExpression(@"^[EW][01]\d\d.[0-5]\d.[0-5]\d$")]
        public string Longitude { get; set; }
        public double GetLatitude()
        {
            var result = Convert.ToDouble(Latitude.Substring(1, 2))
                + Convert.ToDouble(Latitude.Substring(4, 2)) / 60
                + Convert.ToDouble(Latitude.Substring(7, 2)) / 3600;
            return result * (Latitude.StartsWith("N") ? 1 : -1);
        }
        public double GetLongitude()
        {
            var result = Convert.ToDouble(Longitude.Substring(1, 3))
                + Convert.ToDouble(Longitude.Substring(5, 2)) / 60
                + Convert.ToDouble(Longitude.Substring(8, 2)) / 3600;
            return result * (Longitude.StartsWith("E") ? 1 : -1);
        }

    }
}
