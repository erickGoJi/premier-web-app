using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.HousingSpecification
{
    public interface IHousingSpecification : IGenericRepository<Entities.HousingSpecification>
    {
        Entities.HousingSpecification AddRelhousingAmenitie(Entities.HousingSpecification housingAmenitie);
        Entities.HousingSpecification UpdateRelhousingAmenitie(Entities.HousingSpecification housingAmenitie);
        int GetWorkOrderServiceId(int wos, int service);
        ActionResult GetHousingSpecification(int sr);
        ActionResult GetHousingSpecificationBySR(int sr);

        Tuple<string, string> GetWorkOrder(int wos, int service);
    }

    //public class HousingSpecificationDto
    //{
    //    public int Id { get; set; }
    //    public int? ServiceRecordId { get; set; }
    //    public string AreaInterest { get; set; }
    //    public int? PropertyTypeId { get; set; }
    //    public int? Bedroom { get; set; }
    //    public int? Bathroom { get; set; }
    //    public int? SizeId { get; set; }
    //    public int? MetricId { get; set; }
    //    public int? DesiredCommuteTime { get; set; }
    //    public decimal? Budget { get; set; }
    //    public int? CurrencyId { get; set; }
    //    public int? ContractTypeId { get; set; }
    //    public DateTime? IntendedStartDate { get; set; }
    //    public string AdditionalComments { get; set; }

    //    public virtual List<RelHousingAmenitieDto> RelHousingAmenities { get; set; }
    //}

    //public partial class RelHousingAmenitieDto
    //{
    //    public int HousingSpecificationId { get; set; }
    //    public int AmenitieId { get; set; }

    //}
}
