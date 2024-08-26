using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DocumentRepairDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int RepairId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
