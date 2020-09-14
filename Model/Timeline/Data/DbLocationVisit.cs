using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model.Timeline.External;

namespace Model.Timeline.Data
{
    public class DbLocationVisit
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string SemanticType { get; set; }
        public virtual DbLocation Location { get; set; }
        public int LocationId { get; set; }
        public int? DbActivitySegmentId { get; set; }

        public DbLocationVisit()
        {

        }

        public DbLocationVisit(Location location)
        {
            Location = new DbLocation(location);
            SemanticType = location.semanticType;
        }

        public DbLocationVisit(Transitstop transitstop)
        {
            Location = new DbLocation(transitstop);
        }

    }
}
