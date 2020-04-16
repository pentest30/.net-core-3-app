using System;

namespace Voluntary.App.Data.Entities
{
    public class Volunteer : BaseEntity

    {
        public string FirstNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public VoluntaryTask Task { get; set; }
        public District District { get; set; }
        public Team Team { get; set; }
        public Guid? TaskId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? TeamId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CardId { get; set; }
        public string AffectedCity { get; set; }
        public string Comment { get; set; }
        public string Job { get; set; }
        public string Region { get; set; }
        public string Sector { get; set; }
        public string Neighborhood { get; set; }
        public string DistrictName { get; set; }

    }
}
