using System;

namespace WebAPI.Models
{
    public class UserSkillModel
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> SkillId { get; set; }
        public string Proficiency { get; set; }
    }
}