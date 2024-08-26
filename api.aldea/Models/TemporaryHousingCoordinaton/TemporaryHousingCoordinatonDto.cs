using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.TemporaryHousingCoordinaton
{
    public class TemporaryHousingCoordinatonDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? Apartment { get; set; }
        public bool? House { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool? PetFrindly { get; set; }
        public DateTime? TotalAllotedDaysStart { get; set; }
        public DateTime? TotalAllotedDaysEnd { get; set; }
        public decimal? Budget { get; set; }
        public int? Currency { get; set; }
        public int? PaymentResponsibilty { get; set; }
        public bool? ClientSecurityInspectionRequired { get; set; }
        public int? ReservationType { get; set; }
        public int? SupplierPartner { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyAddress { get; set; }
        public string Neighborhood { get; set; }
        public decimal? FinalReservationAmount { get; set; }
        public int? CurrencyFinalReservationAmount { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? CurencySecurityDeposit { get; set; }
        public int? PaymentsDue { get; set; }
        public int? TotalDays { get; set; }
        public bool? Extension { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdAdministrativeContact { get; set; }

        public int? IdPropertyType { get; set; }

        //public virtual CatCurrencyDto CurencySecurityDepositNavigation { get; set; }
        //public virtual CatCurrencyDto CurrencyFinalReservationAmountNavigation { get; set; }
        //public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual CatReservationTypeDto ReservationTypeNavigation { get; set; }
        //public virtual CatStatusDto Status { get; set; }
        //public virtual CatSupplierDto SupplierPartnerNavigation { get; set; }
        //public virtual CatDurationDto TotalTimeAllowedNavigation { get; set; }
        public virtual ICollection<CommentTemporaryHosuingDto> CommentTemporaryHosuings { get; set; }
        public virtual ICollection<DocumentTemporaryHousingCoordinatonDto> DocumentTemporaryHousingCoordinatons { get; set; }
        public virtual ICollection<ReminderTemporaryHousingCoordinatonDto> ReminderTemporaryHousingCoordinatons { get; set; }
        public virtual ICollection<StayExtensionTemporaryHousingDto> StayExtensionTemporaryHousings { get; set; }
    }
    public class TemporaryHousingCoordinatonSelectDto
    {
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public bool? Coordination { get; set; }
        public int StatusId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public bool? Apartment { get; set; }
        public bool? House { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public bool? PetFrindly { get; set; }
        public DateTime? TotalAllotedDaysStart { get; set; }
        public DateTime? TotalAllotedDaysEnd { get; set; }
        public decimal? Budget { get; set; }
        public int? Currency { get; set; }
        public int? PaymentResponsibilty { get; set; }
        public bool? ClientSecurityInspectionRequired { get; set; }
        public int? ReservationType { get; set; }
        public int? SupplierPartner { get; set; }
        public int? PropertyNo { get; set; }
        public string PropertyAddress { get; set; }
        public string Neighborhood { get; set; }
        public decimal? FinalReservationAmount { get; set; }
        public int? CurrencyFinalReservationAmount { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal? SecurityDeposit { get; set; }
        public int? CurencySecurityDeposit { get; set; }
        public int? PaymentsDue { get; set; }
        public int? TotalDays { get; set; }
        public bool? Extension { get; set; }
        public string Comments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdAdministrativeContact { get; set; }
        public int? IdPropertyType { get; set; }

        //public virtual CatCurrencyDto CurencySecurityDepositNavigation { get; set; }
        //public virtual CatCurrencyDto CurrencyFinalReservationAmountNavigation { get; set; }
        //public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        //public virtual CatReservationTypeDto ReservationTypeNavigation { get; set; }
        //public virtual CatStatusDto Status { get; set; }
        //public virtual CatSupplierDto SupplierPartnerNavigation { get; set; }
        //public virtual CatDurationDto TotalTimeAllowedNavigation { get; set; }
        public virtual ICollection<CommentTemporaryHosuingSelectDto> CommentTemporaryHosuings { get; set; }
        public virtual ICollection<DocumentTemporaryHousingCoordinatonDto> DocumentTemporaryHousingCoordinatons { get; set; }
        public virtual ICollection<ExtensionTemporaryHousingCoordinatonDto> ExtensionTemporaryHousingCoordinatons { get; set; }
        public virtual ICollection<ReminderTemporaryHousingCoordinatonDto> ReminderTemporaryHousingCoordinatons { get; set; }
        public virtual ICollection<StayExtensionTemporaryHousingDto> StayExtensionTemporaryHousings { get; set; }
    }
}
