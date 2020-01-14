using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace azureFareTypes
{
    public class FareType
    {
        public int FareTypeId { get; set; }
        public string FareTypeAbbr { get; set; }
        public string Description { get; set; }
        public float? FlatFareAmt { get; set; }
        public float? FlatRate { get; set; }
        public float? DistanceRate { get; set; }
        public float? PolygonRate { get; set; }
        public float? TimeRate { get; set; }
        public int? PolyTypeId { get; set; }
        public string PrepaidReq { get; set; }
        public double? FareAdjustDiffZon { get; set; }
        public double? FareAdjustSameZon { get; set; }
        public double? DistanceLimitSameZon { get; set; }
        public double? DistanceLimitDiffZon { get; set; }
        public short? NumCode { get; set; }
        public short? FareMode { get; set; }
        public short? FareCalcType { get; set; }
        public int? VariationOf { get; set; }
        public float? MinFare { get; set; }
        public float? MaxFare { get; set; }
        public short? RoundFare { get; set; }
        public int? DistanceLimit { get; set; }
        public short? RoundDirection { get; set; }
        public float? Accuracy { get; set; }
        public int? InActive { get; set; }
        [Column("Timestamp")]
        public DateTime SourceTimestamp { get; set; }
    }
}
