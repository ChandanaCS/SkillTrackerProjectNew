using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class GetUserDetailsModel
    {
        [Key]
        public int Id { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<Int64> ContactNo { get; set; }
        public string Gender { get; set; }
        public List<UpdateUserSkillsModel> Skills { get; set; }
    }
}