using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ImmigrationProfile
{
    public interface IImmigrationProfileRepository : IGenericRepository<Entities.ImmigrationProfile>
    {
        Entities.ImmigrationProfile UpdateCustom(Entities.ImmigrationProfile immigrationProfile);
        ActionResult SelectCustom(int sr);
        ActionResult document_assigne(int service_record);
        ActionResult document_assigneById(int id);
        ActionResult GetDependentImmigration(int sr);
        ActionResult immigration_library(int service_record, int service_line);
        ActionResult All_immigration_library(int service_record, int service_line);
        ActionResult immigration_libraryById(int tipo, int id);
        //App
        ActionResult SelectDocumentDependent(int assigneeId);
    }
}
