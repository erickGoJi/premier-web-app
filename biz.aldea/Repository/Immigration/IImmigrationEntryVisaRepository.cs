using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Immigration
{
    public interface IImmigrationEntryVisaRepository : IGenericRepository<EntryVisa>
    {
        EntryVisa UpdateCustom(EntryVisa visa);
        EntryVisa GetEntryVisaCustom(int key);
        DocumentEntryVisa DeleteDocument(int key);
        Boolean DeleteReminder(int key);
    }
}
