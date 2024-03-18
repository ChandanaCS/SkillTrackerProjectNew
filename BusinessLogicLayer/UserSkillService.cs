using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class UserSkillService
    {
        public int AddSkillToSkillTable(string skillName)
        {
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            Skill skill = new Skill();
            skill.Name = skillName;
            db.Skills.Add(skill);
            db.SaveChanges();
            Skill newSkill = db.Skills.FirstOrDefault(s => s.Name == skillName);
            int newSkillId = newSkill.Id;
            return newSkillId;
        }
        private bool IsUserSkillExists(int userId, string skillName)
        {
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            return db.UserSkills.Any(us => us.UserId == userId && us.Skill.Name == skillName);
        }
        public bool AddSkillToUserSkill(int userId, UpdateUserSkillsDTO editUserSkillsDTO)
        {

            if (IsUserSkillExists(userId, editUserSkillsDTO.Name))
            {
                throw new ArgumentException("The user already has this skill.");
            }
            int newSkillId;
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            Skill skill = db.Skills.FirstOrDefault(s => s.Name == editUserSkillsDTO.Name);
            if (skill == null)
            {
                newSkillId = AddSkillToSkillTable(editUserSkillsDTO.Name);
            }
            else
            {
                newSkillId = skill.Id;
            }
            UserSkill userSkill = new UserSkill();
            userSkill.UserId = userId;
            userSkill.SkillId = newSkillId;
            userSkill.Proficiency = editUserSkillsDTO.Proficiency;
            db.UserSkills.Add(userSkill);
            db.SaveChanges();
            return true;

        }

        public bool EditSkill(int userId, int skillId, UpdateUserSkillsDTO editUserSkillsDTO)
        {
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            User user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return false;
            }
            UserSkill userSkill = db.UserSkills.FirstOrDefault(us => us.UserId == userId && us.SkillId == skillId);
            if (userSkill != null)
            {
                Skill skill = db.Skills.FirstOrDefault(s => s.Id == skillId);
                if ((skill.Name == editUserSkillsDTO.Name) && (userSkill.Proficiency != editUserSkillsDTO.Proficiency))
                {
                    userSkill.Proficiency = editUserSkillsDTO.Proficiency;
                }
                else
                {
                    return false;
                }

                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public string DeleteUserSkill(int userSkillId)
        {
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            UserSkill userSkill = db.UserSkills.Find(userSkillId);
            int userSkill_SkillId = (int)userSkill.SkillId;
            Skill skill = db.Skills.FirstOrDefault(s => s.Id == userSkill_SkillId);
            string userSkillName = skill.Name;
            db.UserSkills.Remove(userSkill);
            db.SaveChanges();
            return userSkillName;
        }
       
        public void UpdateSkill(int skillId, SkillDTO skillDTO)
        {
            DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
            // Find the skill by its ID
            var existingSkill = db.Skills.FirstOrDefault(s => s.Id == skillId);

            if (existingSkill != null)
            {
                // Update the skill name
                existingSkill.Name = skillDTO.Name;

                // Save changes to the database
                db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Skill not found");
            }
        }
    }
}
