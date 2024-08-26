using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AreaOrientation
{
    public class HousingAreaOrientationDto
    {
        public int Id { get; set; }
        public int? AreaOrientationId { get; set; }
        public int? PropertyNo { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public int? Bedroom { get; set; }
        public int? Bathroom { get; set; }
        public int? ParkingSpace { get; set; }
        public decimal? Price { get; set; }
        public int? CurrencyId { get; set; }
        public string AdditionalComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AreaOrientationDto AreaOrientation { get; set; }
        public virtual CatCurrencyDto Currency { get; set; }
        public virtual CatStatusHousingDto StatusNavigation { get; set; }
        public virtual ICollection<RelHousingAreaOrientationDto> RelHousingAreaOrientations { get; set; }
    }
}
