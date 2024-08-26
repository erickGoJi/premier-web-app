using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class RepeatTheRecurrenceRequestPaymentDto
    {
        public int Recurrence { get; set; }
        public int Day { get; set; }
    }
}
