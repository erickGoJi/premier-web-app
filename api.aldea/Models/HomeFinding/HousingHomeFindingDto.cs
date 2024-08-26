using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class HousingHomeFindingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public string AreaInterest { get; set; }
        public int? ContractTypeId { get; set; }
        public int? DesiredPropertyType { get; set; }
        public int? Bedroom { get; set; }
        public int? Bathroom { get; set; }
        public decimal? Budget { get; set; }
        public int? CurrencyId { get; set; }
        public int? SizeId { get; set; }
        public int? MetricId { get; set; }
        public int? DesiredCommuteTime { get; set; }
        public DateTime? IntendedStartDate { get; set; }
        public int? ParkingSpaceId { get; set; }
        public string AdditionalComments { get; set; }

        public virtual CatContractTypeDto ContractType { get; set; }
        public virtual CatCurrencyDto Currency { get; set; }
        public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual CatMetricDto Metric { get; set; }
        public virtual CatSizeDto Size { get; set; }
        public virtual ICollection<RelHousingHomeFindingDto> RelHousingHomeFindings { get; set; }
    }
}
