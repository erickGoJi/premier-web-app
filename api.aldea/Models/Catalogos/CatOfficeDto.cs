using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Catalogos
{
    public class CatOfficeDto
    {
        public int Id { get; set; }
        public string Office { get; set; }
        public string Image { get; set; }
        public string ImageExtension { get; set; }
        public int? Country { get; set; }
        public int? City { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Currency { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
