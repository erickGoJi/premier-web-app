using System;

namespace biz.premier.Models.ClientPartnerProfile
{
    public class DocumentOfficeInformationDto
    {
        public int Id { get; set; }
        public int IdOfficeInformation { get; set; }
        public int IdDocumentType { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }

        public virtual OfficeInformationDto IdOfficeInformationNavigation { get; set; }
    }
}