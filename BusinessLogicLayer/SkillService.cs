using DataAccessLayer;
using System;
using System.Linq;

namespace BusinessLogicLayer
{
    public class SkillService
    {
        private readonly SkillTrackerDBEntities db;
        public SkillService()
        {
            db = new SkillTrackerDBEntities();
        }
        public void AddSkill(SkillDTO skillDTO)
        {
            var existingSkill = db.Skills.FirstOrDefault(s => s.Name == skillDTO.Name);

            if (existingSkill != null)
            {
                return;
            }
            var newSkill = new Skill
            {
                Name = skillDTO.Name
            };

            db.Skills.Add(newSkill);
            db.SaveChanges();
        }
        public void UpdateSkill(int skillId, SkillDTO skillDTO)
        {
            var existingSkill = db.Skills.FirstOrDefault(s => s.Id == skillId);

            if (existingSkill != null)
            {
                existingSkill.Name = skillDTO.Name;
                db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Skill not found");
            }
        }
    }
}
