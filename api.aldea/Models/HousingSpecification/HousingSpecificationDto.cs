using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HousingSpecification
{
    public partial class HousingSpecificationDto
    {
        public HousingSpecificationDto()
        {
            RelHousingAmenities = new HashSet<RelHousingAmenitieDto>();
        }

        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public int? TypeService { get; set; }
        public string AreaInterest { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? Bedroom { get; set; }
        public int? Bathroom { get; set; }
        public string Size { get; set; }
        public int? MetricId { get; set; }
        public string DesiredCommuteTime { get; set; }
        public decimal? Budget { get; set; }
        public int? CurrencyId { get; set; }
        public int? ContractTypeId { get; set; }
        public DateTime? IntendedStartDate { get; set; }
        public int? ParkingSpace { get; set; }
        public string AdditionalComments { get; set; }

        public virtual ICollection<RelHousingAmenitieDto> RelHousingAmenities { get; set; }
    }

    public partial class RelHousingAmenitieDto
    {
        public int HousingSpecificationId { get; set; }
        public int AmenitieId { get; set; }

    }

    public class HousingSpecificationSelectDto
    {
        public int Id { get; set; }
        public int? WorkOrderServices { get; set; }
        public int? TypeService { get; set; }
        public string AreaInterest { get; set; }
        public int? PropertyTypeId { get; set; }
        public int? Bedroom { get; set; }
        public int? Bathroom { get; set; }
        public string Size { get; set; }
        public int? MetricId { get; set; }
        public string DesiredCommuteTime { get; set; }
        public decimal? Budget { get; set; }
        public int? CurrencyId { get; set; }
        public int? ContractTypeId { get; set; }
        public DateTime? IntendedStartDate { get; set; }
        public int? ParkingSpace { get; set; }
        public string AdditionalComments { get; set; }
        public string workOrder { get; set; }
        public string serviceID { get; set; }

        public virtual CatContractTypeDto ContractType { get; set; }
        public virtual CatCurrencyDto Currency { get; set; }
        public virtual CatMetricDto Metric { get; set; }
        public virtual CatPropertyTypeDto PropertyType { get; set; }
        public virtual CatTypeHousingDto TypeHousingNavigation { get; set; }
        public virtual ICollection<RelHousingAmenitieDto> RelHousingAmenities { get; set; }
    }
}
