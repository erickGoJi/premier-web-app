using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.EmailSend
{
    public class EmailSendDto
    {
        public int Id { get; set; }
        public int ServiceLineId { get; set; }
        public int? ServiceRecordId { get; set; }
        public int EmailId { get; set; }
        public DateTime Date { get; set; }
        public bool Completed { get; set; }

    }
}
