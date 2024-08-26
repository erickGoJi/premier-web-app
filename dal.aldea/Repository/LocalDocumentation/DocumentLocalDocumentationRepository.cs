using biz.premier.Entities;
using biz.premier.Repository.LocalDocumentation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.LocalDocumentation
{
    public class DocumentLocalDocumentationRepository :GenericRepository<DocumentLocalDocumentation>, IDocumentLocalDocumentationRepository
    {
        public DocumentLocalDocumentationRepository(Db_PremierContext context) : base(context) { }
    }
}
