﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace biz.premier.Entities
{
    public partial class RequestInformationDocument
    {
        public int Id { get; set; }
        public int? DocumentType { get; set; }
        public int? RelationshipId { get; set; }
        public string FileRequest { get; set; }
        public int? RequestInformationId { get; set; }

        public virtual CatDocumentType DocumentTypeNavigation { get; set; }
        public virtual DependentInformation Relationship { get; set; }
        public virtual RequestInformation RequestInformation { get; set; }
    }
}