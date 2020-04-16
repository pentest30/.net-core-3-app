using System.ComponentModel.DataAnnotations;

namespace Voluntary.App.Models
{
    public class AddDistrictViewModel
    {
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }
        public string NameAr { get; set; }
        [Required(ErrorMessage = "StreetRequired")]
        public string Street { get; set; }
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "CityRequired")]
        public string City { get; set; }
        public string Department { get; set; }
    }
}
