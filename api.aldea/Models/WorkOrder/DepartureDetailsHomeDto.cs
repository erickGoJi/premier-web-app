using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class DepartureDetailsHomeDto
    {
        public int Id { get; set; }
        public bool? DiplomaticClause { get; set; }
        public int? EarlyTerminationNotification { get; set; }
        public int? Penalty { get; set; }
        public string Amount { get; set; }
        public int? Currency { get; set; }
        public bool? Permission { get; set; }
        public int? ReturnSecurityDepositTo { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? SecurityDepositReturn { get; set; }
        public int IdDeparture { get; set; }
        public int? IdServiceDetail { get; set; }
        public DateTime? DepartureNotificationLandlord { get; set; }

        public bool? Visible { get; set; }

        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual HousingListDto IdNavigation { get; set; }
    }
}
