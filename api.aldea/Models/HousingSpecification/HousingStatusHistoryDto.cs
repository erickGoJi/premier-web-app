using api.premier.Models.Catalogos;
using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HousingSpecification
{
    public class HousingExcel
    {
        public int PropertyNo { get; set; }
        public string PropertyType { get; set; }
        public string Neighborhood { get; set; }
        public string Address { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int ParkingSpaces { get; set; }
        public int Sise { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string AssigneComments { get; set; }
    }
    public class HousingStatusHistoryDto
    {
        public int Id { get; set; }
        public int? Status { get; set; }
        public int? HousingId { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual HousingListDto Housing { get; set; }
    }
    public class HousingStatusHistorySelectDto
    {
        public int Id { get; set; }
        public int? Status { get; set; }
        public int? HousingId { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual UserDto CreatedByNavigation { get; set; }
        public virtual HousingListSelectDto Housing { get; set; }
        public virtual CatStatusHousingDto StatusNavigation { get; set; }
    }
}
