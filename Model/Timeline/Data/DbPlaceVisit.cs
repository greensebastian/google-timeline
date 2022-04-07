using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Model.Timeline.External;
using Common;

namespace Model.Timeline.Data
{
    public class DbPlaceVisit
    {
        [Key]
        public int Id { get; set; }

        public virtual DbLocationVisit LocationVisit { get; set; }
        [ForeignKey("LocationVisit")]
        public int? LocationVisitId { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string Confidence { get; set; }
        public long CenterLatE7 { get; set; }
        public long CenterLngE7 { get; set; }
        public double CenterLat { get => CoordinateUtil.ToDegrees(CenterLatE7); }
        public double CenterLng { get => CoordinateUtil.ToDegrees(CenterLngE7); }

        [ForeignKey("DbPlaceVisitId")]
        public virtual List<DbPlaceVisit> ChildVisits { get; set; }
        public int? DbPlaceVisitId { get; set; }

        public DbPlaceVisit()
        {

        }

        public DbPlaceVisit(Placevisit visit)
        {
            StartDateTime = visit.duration.startTimestamp;
            EndDateTime = visit.duration.endTimestamp;
            CenterLatE7 = visit.centerLatE7;
            CenterLngE7 = visit.centerLngE7;
            Confidence = visit.placeConfidence;
            LocationVisit = new DbLocationVisit(visit.location);
            ChildVisits = visit.childVisits?.Select(childVisit => new DbPlaceVisit(childVisit, visit.duration.startTimestamp, visit.duration.endTimestamp)).ToList() ?? new List<DbPlaceVisit>();
        }

        public DbPlaceVisit(Childvisit visit, DateTime startTimestamp, DateTime endTimestamp)
        {
            StartDateTime = startTimestamp;
            EndDateTime = endTimestamp;
            CenterLatE7 = visit.centerLatE7;
            CenterLngE7 = visit.centerLngE7;
            Confidence = visit.placeConfidence;
            LocationVisit = new DbLocationVisit(visit.location);
        }

        public override int GetHashCode()
        {
            var dateString = StartDateTime.ToString(CultureInfo.InvariantCulture) + EndDateTime.ToString(CultureInfo.InvariantCulture);
            return dateString.GetHashCode();
        }
    }
}
