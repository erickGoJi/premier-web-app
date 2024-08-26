using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class DepartureDetailDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public DateTime? LeaseStartDate { get; set; }
        public DateTime? LeaseEndDate { get; set; }
        public decimal? ListRentPrice { get; set; }
        public int? CurrencyListRentPrice { get; set; }
        public decimal? FinalRentPrice { get; set; }
        public int? CurrencyFinalRentPrice { get; set; }
        public bool? DiplomaticCouse { get; set; }
        public bool? EarlyTerminationNotification { get; set; }
        public int? Penalty { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyPenalty { get; set; }
        public bool? PermissionCreditSecurityDeposit { get; set; }
        public int? ReturnSecurityDepositTo { get; set; }

        public virtual CatCurrencyDto CurrencyFinalRentPriceNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyListRentPriceNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyPenaltyNavigation { get; set; }
        public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual DependentInformationDto ReturnSecurityDepositToNavigation { get; set; }
    }
}
