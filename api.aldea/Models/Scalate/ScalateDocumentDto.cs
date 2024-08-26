using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Scalate
{
    public class ScalateDocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? ScalateServiceId { get; set; }

        public virtual ScalateServiceDto ScalateService { get; set; }
    }
}
