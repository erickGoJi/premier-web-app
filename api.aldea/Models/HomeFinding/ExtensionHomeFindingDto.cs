using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class ExtensionHomeFindingDto
    {
        public int Id { get; set; }
        public int? HomeFindingId { get; set; }
        public int? RequestedBy { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? Tine { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual HomeFindingDto HomeFinding { get; set; }
    }
}
