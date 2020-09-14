using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.Timeline.Data
{
    public class TimelineData
    {
        [Key]
        public int Id { get; set; }
        public virtual List<DbPlaceVisit> PlaceVisits { get; set; } = new List<DbPlaceVisit>();
        public virtual List<DbActivitySegment> ActivitySegments { get; set; } = new List<DbActivitySegment>();
    }
}
