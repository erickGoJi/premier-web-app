using biz.premier.Entities;

namespace biz.premier.Repository.Other
{
    public interface IOtherRepository : IGenericRepository<Entities.Other>
    {
        Entities.Other GetCustom(int key);
        Entities.Other UpdateCustom(Entities.Other other, int key);
        bool DeleteReminder(int key);
        DocumentOther FindDocument(int key);
        bool DeleteDocument(DocumentOther documentOther);
    }
}