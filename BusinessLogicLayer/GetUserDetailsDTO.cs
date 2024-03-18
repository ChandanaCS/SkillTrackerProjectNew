using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class GetUserDetailsDTO
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
        public List<UpdateUserSkillsDTO> Skills { get; set; }
    }
}
