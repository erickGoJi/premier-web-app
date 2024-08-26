namespace api.premier.Models.ClientPartnerProfile
{
    public class TermsDealDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int IdPrice { get; set; }
        public int? IdCurrency { get; set; }
        public string Volume { get; set; }
        public int Sevices { get; set; }
        public int Amount { get; set; }
        public string Fixed { get; set; }
        public string BussinesUnit { get; set; }

        public virtual ClientPartnerProfileDto IdClientPartnerProfileNavigation { get; set; }
        public virtual CatPriceTermsDealDto IdPriceNavigation { get; set; }
    }
}