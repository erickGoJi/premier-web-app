using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HousingSpecification
{
    public class DocumentHousingDto
    {
        public int Id { get; set; }
        public int? HousingId { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }
        public int? CreationBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual HousingListDto Housing { get; set; }
    }
}
