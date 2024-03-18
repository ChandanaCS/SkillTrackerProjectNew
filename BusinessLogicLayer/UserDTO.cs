using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer
{
    public class UserDTO
    {
        [Key]
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<decimal> ContactNo { get; set; }
        public string Gender { get; set; }
    }
}
