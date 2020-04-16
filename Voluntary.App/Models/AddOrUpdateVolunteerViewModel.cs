using System;
using System.ComponentModel.DataAnnotations;
using Voluntary.App.Data.Entities;

namespace Voluntary.App.Models
{
    public class AddOrUpdateVolunteerViewModel
    {
        public AddOrUpdateVolunteerViewModel()
        {
            
        }
        public AddOrUpdateVolunteerViewModel(Volunteer volunteer)
        {
            Id = volunteer.Id;
            FirstName = volunteer.FirstName;
            LastName = volunteer.LastName;
            BirthPlace = volunteer.BirthPlace;
            BirthDate = volunteer.BirthDate;
            Email = volunteer.Email;
            Phone = volunteer.Phone;
            CardId = volunteer.CardId;
            FirstNameAr = volunteer.FirstNameAr;
            LastNameAr = volunteer.LastNameAr;
            Address = volunteer.Address;

        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public string FirstNameAr { get; set; }
        [Required]
        public string LastNameAr { get; set; }
        public string Address { get; set; }
        public string BirthPlace { get; set; }
       [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneRequired")]

        public string Phone { get; set; }
      
        public string CardId { get; set; }
    }
}
