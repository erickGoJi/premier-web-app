using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.PropertyReport
{
    public class PhotosInventoryDto
    {
        public int Id { get; set; }
        public int? SectionInventory { get; set; }
        public string PhotoName { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual SectionInventoryDto SectionInventoryNavigation { get; set; }
    }
}
