using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ProfileUser
{
    public class CompesationBenefitDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public int? PlaceWork { get; set; }
        public decimal? BaseCompesation { get; set; }
        public int? Currency { get; set; }
        public decimal? Taxes { get; set; }
        public int? Benefit { get; set; }
        public decimal? Ammount { get; set; }
        public int? Frequency { get; set; }
    }
    public class CompesationBenefitSelectDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public int? PlaceWork { get; set; }
        public decimal? BaseCompesation { get; set; }
        public int? Currency { get; set; }
        public decimal? Taxes { get; set; }
        public int? Benefit { get; set; }
        public decimal? Ammount { get; set; }
        public int? Frequency { get; set; }

        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual PersonalInformationDto ProfileNavigation { get; set; }
    }
}
