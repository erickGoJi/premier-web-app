using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DeparturePaymentDto
    {
        public int Id { get; set; }
        public int? DepartaureId { get; set; }
        public string PaymentResponsibility { get; set; }
        public string PaymentType { get; set; }

        public virtual DepartureDto Departaure { get; set; }
    }
}
