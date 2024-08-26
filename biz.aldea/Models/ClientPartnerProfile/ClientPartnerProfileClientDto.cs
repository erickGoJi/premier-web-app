namespace biz.premier.Models.ClientPartnerProfile
{
    public class ClientPartnerProfileClientDto
    {
        public int Id { get; set; }
        public int IdClientFrom { get; set; }
        public int IdClientTo { get; set; }

        public virtual ClientPartnerProfileDto IdClientFromNavigation { get; set; }
        public virtual ClientPartnerProfileDto IdClientToNavigation { get; set; }
    }
}