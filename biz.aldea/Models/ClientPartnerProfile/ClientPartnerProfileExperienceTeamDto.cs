namespace biz.premier.Models.ClientPartnerProfile
{
    public class ClientPartnerProfileExperienceTeamDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int UserId { get; set; }
        public int IdServiceLine { get; set; }
        public string Avatar { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public int? ProfileId { get; set; }
    }
}