using api.premier.Models.Catalogos;
using System;

namespace api.premier.Models.ClientPartnerProfile
{
    public class DocumentLocationCountryDto
    {
        public int Id { get; set; }
        public int IdServiceLocationCountry { get; set; }
        public int IdDocumentType { get; set; }
        public DateTime UploadDate { get; set; }
        public int Status { get; set; }
        public int Privacy { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }

    }
}