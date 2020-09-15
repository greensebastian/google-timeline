using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Timeline.External;

namespace Model.Timeline.Data
{
    public class DbLocation
    {
        public DbLocation()
        {

        }

        public DbLocation(Transitstop transitstop)
        {
            LatitudeE7 = transitstop.latitudeE7;
            LongitudeE7 = transitstop.longitudeE7;
            Name = transitstop.name;
            PlaceId = transitstop.placeId;
        }

        public DbLocation(Location location)
        {
            LatitudeE7 = location.latitudeE7;
            LongitudeE7 = location.longitudeE7;
            Name = location.name;
            PlaceId = location.placeId;
            Address = location.address;
        }

        [Key]
        public int Id { get; set; }
        public long LatitudeE7 { get; set; }
        public long LongitudeE7 { get; set; }

        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Address { get; set; }

        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string PlaceId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DbLocation other && !string.IsNullOrWhiteSpace(PlaceId) && !string.IsNullOrWhiteSpace(other.PlaceId)){
                return PlaceId == other.PlaceId;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return PlaceId?.GetHashCode() ?? base.GetHashCode();
        }
    }
}
