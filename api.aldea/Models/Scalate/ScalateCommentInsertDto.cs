using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Scalate
{
    public class ScalateCommentInsertDto
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public int? ScalateServiceId { get; set; }
        public DateTime? Date { get; set; }
        public int? UserFromId { get; set; }
        public int? UserToId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
