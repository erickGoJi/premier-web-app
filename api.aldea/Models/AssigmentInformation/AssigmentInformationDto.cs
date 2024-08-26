using api.premier.Models.Catalogos;
using api.premier.Models.ImmigrationProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AssigmentInformation
{
    public class AssigmentInformationDto
    {
        public int Id { get; set; }
        public string LegalNameHomeCountry { get; set; }
        public string LocationHome { get; set; }
        public string CurrentJobPositionTitle { get; set; }
        public DateTime? EmploymentFrom { get; set; }
        public DateTime? EmploymentTo { get; set; }
        public string LegalNameHostCountry { get; set; }
        public string LocationHost { get; set; }
        public string NewJobPositionTitle { get; set; }
        public string NewJobResponsibilitie { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public string LenghtAssigment { get; set; }
        public string CurrentGrossSalary { get; set; }
        public int? CurrencyCurrentGrossSalary { get; set; }
        public string NewGrossSalary { get; set; }
        public int? CurrencyNewGrossSalary { get; set; }
        public string PayrollLocation { get; set; }
        public string PayrollLocationSecond { get; set; }
        public bool? SplitPayrollApplicable { get; set; }
        public string HiringManager { get; set; }
        public string HiringManagerPhone { get; set; }
        public string HiringManagerEmail { get; set; }
        public string LegalRepresentative { get; set; }
        public string SpecificAtention { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTypeExtension { get; set; }
        public string LicenseDriver { get; set; }
        public string LicenseDriverExtension { get; set; }
        public virtual CatCurrencyDto CurrencyCurrentGrossSalaryNavigation { get; set; }
        public virtual CatCurrencyDto CurrencyNewGrossSalaryNavigation { get; set; }
        public virtual ICollection<ImmigrationProfileInsertDto> ImmigrationProfiles { get; set; }


    }
}
