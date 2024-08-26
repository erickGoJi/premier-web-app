using api.premier.Models.DependentInformations;
using api.premier.Models.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AssigneeInformation
{
    public class AssigneeInformationInsertDto
    {
        public int Id { get; set; }
        public string AssigneeName { get; set; }
        public int? UserId { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? SexId { get; set; }
        public DateTime? Birth { get; set; }
        public int? Age { get; set; }
      //  public int? NationalityId { get; set; }
        public int? MaritalStatusId { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public int? PolicyTypeId { get; set; }
        public string AssignmentDuration { get; set; }
        public string AssignmentDurationTime { get; set; }
        public DateTime? InitialArrival { get; set; }
        public DateTime? FinalMove { get; set; }
        public int? HomeCountryId { get; set; }
        public int? HomeCityId { get; set; }
        public string CurrentPosition { get; set; }
        public int? HostCountry { get; set; }
        public int? HostCityId { get; set; }
        public string NewPosition { get; set; }
        public bool? DependentInformation { get; set; }
        public bool? Pets { get; set; }
        public string Email { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }

        public virtual ICollection<DependentInformationDto> DependentInformations { get; set; }
        public virtual ICollection<PetDto> PetsNavigation { get; set; }
        public virtual ICollection<LanguagesSpokenDto> LanguagesSpokens { get; set; }
        public virtual ICollection<NationalityAssigneeInformationDto> NationalityAssigneeInformations { get; set; }


    }

    public class AssigneeInformationUserEditDto
    {
        public int Id { get; set; }
        public string AssigneeName { get; set; }
        public string MobilePhone { get; set; }
        public int? HomeCountryId { get; set; }
        public int? HomeCityId { get; set; }
        public string Email { get; set; }

    }

}
