using System;

namespace api.premier.Models.Catalogos
{
    public class NationalityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality1 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}