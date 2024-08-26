using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.EducationalBackground
{
    public class EducationalBackgroundDto
    {
        public int Id { get; set; }
        public int? ImmigrationProfileId { get; set; }
        public string Institution { get; set; }
        public string FieldStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Degree { get; set; }
        public string ListProfessionalLicenses { get; set; }
        public virtual ImmigrationProfileInsertDto ImmigrationProfile { get; set; }

    }
}
