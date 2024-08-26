using System.ComponentModel;
using api.premier.Extensions;

namespace api.premier.Models.Countries
{
    public class CountriesGenericDto
    {
        public int Id { get; set; }
        [DefaultValue("na")]
        public string Name { get; set; }
    }
}