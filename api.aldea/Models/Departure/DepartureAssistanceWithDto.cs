using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Departure
{
    public class DepartureAssistanceWithDto
    {
        public int Id { get; set; }
        public int? DepartaureId { get; set; }
        public int? AssistanceWith { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string OtherSpecify { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatAssitanceWithDto AssistanceWithNavigation { get; set; }
        public virtual DepartureDto DepartaureNavigation { get; set; }
    }
}
