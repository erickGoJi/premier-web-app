namespace api.premier.Models.SchoolingSearch
{
    public class PaymentSchoolingInformationDto
    {
        public int Id { get; set; }
        public int? SchoolingInformationId { get; set; }
        public int? SchoolExpensesPayment { get; set; }
        public int? Responsible { get; set; }
        public string Description { get; set; }
        public int? Currency { get; set; }
        public decimal? Amount { get; set; }
        public int? Recurrence { get; set; }
    }
}
