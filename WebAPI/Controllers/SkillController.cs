using BusinessLogicLayer;
using System;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class SkillController : ApiController
    {
        
        [HttpPut]
        [Route("api/Skill/{id}/{skillId}/Edit")]
        public IHttpActionResult EditSkill(int id, int skillId, UpdateUserSkillsModel editUserSkillsModel)
        {
            UserSkillService userSkillService = new UserSkillService();
            UpdateUserSkillsDTO editUserSkillsDTO = new UpdateUserSkillsDTO();
            editUserSkillsDTO.Name = editUserSkillsModel.Name;
            editUserSkillsDTO.Proficiency = editUserSkillsModel.Proficiency;
            bool result = userSkillService.EditSkill(id, skillId, editUserSkillsDTO);
            if (result)
            {
                return Ok<string>("Skills Updated Successfully!");
            }
            else
            {
                return Ok<string>("Error occured during updating Try Again..");
            }
        }

        [HttpPost]
        [Route("api/Skill/{id}/AddSkill")]
        public IHttpActionResult PostAddSkill(int id, UpdateUserSkillsModel editUserSkillsModel)
        {
            try
            {
                UserSkillService userSkillService = new UserSkillService();
                UpdateUserSkillsDTO editUserSkillsDTO = new UpdateUserSkillsDTO();
                editUserSkillsDTO.Name = editUserSkillsModel.Name;
                editUserSkillsDTO.Proficiency = editUserSkillsModel.Proficiency;
                bool result = userSkillService.AddSkillToUserSkill(id, editUserSkillsDTO);
                if (result)
                {
                    return Ok<string>("Skills Added Successfully!");
                }
                else
                {
                    return Ok<string>("Error occured during adding Try Again..");
                }

            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("api/Skill/{id}/DeleteSkill/{userSkillId}")]
        public IHttpActionResult DeleteUserSkill(int id,int userSkillId)
        {
            UserSkillService userSkillService = new UserSkillService();
            string skillName = userSkillService.DeleteUserSkill(userSkillId);
            return Ok<string>($"Deleted {skillName} from userid {id}");
        }
        [HttpPut]
        [Route("api/skill/{skillId}")]
        public IHttpActionResult PutUpdateSkill(int skillId, SkillDTO skillDTO)
        {
            try
            {
                UserSkillService userSkillService = new UserSkillService();
                userSkillService.UpdateSkill(skillId, skillDTO);
                return Ok("Skill updated successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex);
            }
        }
    }
}