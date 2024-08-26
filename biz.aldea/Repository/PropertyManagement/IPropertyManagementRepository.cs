using System;

namespace biz.premier.Repository.PropertyManagement
{
    public interface IPropertyManagementRepository : IGenericRepository<Entities.PropertyManagement>
    {
        Entities.PropertyManagement GetCustom(int key);
        Entities.PropertyManagement UpdateCustom(Entities.PropertyManagement propertyManagement,int key);
        bool DeleteReminder(int key);
        bool DeleteDocument(Entities.DocumentPropertyManagement key);
        Entities.DocumentPropertyManagement FindDocument(int key);
        Tuple<int, string> FindPhoto(int key, int type);
        bool DeletePhoto(int key, int type);
    }
}