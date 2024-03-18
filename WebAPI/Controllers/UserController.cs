using AutoMapper;
using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class UserController : ApiController
    {
        private readonly IMapper mapper;
        public UserController()
        {
            var mapConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDTO, UserModel>();
                cfg.CreateMap<GetUserDetailsDTO, GetUserDetailsModel>()
            .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills));
                cfg.CreateMap<UpdateUserSkillsDTO, UpdateUserSkillsModel>();
                cfg.CreateMap<UserModel, DisplayModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.EmailId, opt => opt.MapFrom(src => src.EmailId));
            });
            mapper = mapConfig.CreateMapper();
        }

        UserService userBusiness = new UserService();
        public List<DisplayModel> GetAllUsers()        //View -> Admin
        {
            List<UserDTO> users = userBusiness.GetListOfUsers();
            List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
            List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(userModelList);          
            return displayModels;
        }

        [Route("api/user/getalladmins")]
        public List<DisplayModel> GetAllAdmins()       //View -> Admin
        {
            List<UserDTO> admins = userBusiness.GetListOfAdmins();
            List<UserModel> adminModelList = mapper.Map<List<UserModel>>(admins);
            List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(adminModelList);
            return displayModels;
        }

        [Route("api/user/getuserbyidorname")]
        public IHttpActionResult GetUserByIdOrName(string searchUser)       //View -> Admin
        {
            List<UserDTO> foundUsers = userBusiness.GetUserByIdOrName(searchUser);

            if (foundUsers.Any())
            {
                List<UserModel> users = mapper.Map<List<UserModel>>(foundUsers);
                List<DisplayModel> displayModel = mapper.Map<List<DisplayModel>>(users);
                
                return Ok(displayModel);
            }
            else
            {
                return Ok<string>("User not found");
            }
        }

        [Route("api/user/{id}")]
        public IHttpActionResult GetUserDetails(int id)         //View -> Admin & User(Id)
        {
            GetUserDetailsDTO getUserDetailsDTOs = userBusiness.GetUserDetails(id);
            GetUserDetailsModel getUserDetailsModel = mapper.Map<GetUserDetailsModel>(getUserDetailsDTOs);
            return Ok(getUserDetailsModel);
        }

        [HttpPost]
        [Route("api/user/PostNewUser")]
        public IHttpActionResult PostNewUser([FromBody] UserModel newUser)      //View -> Admin
        {
            if (!ModelState.IsValid)
            {
                return Ok<string>($"Validation failed");
            }
            UserDTO userDTO = new UserDTO
            {
                EmailId = newUser.EmailId,
                Password = newUser.Password,
            };
            bool result = userBusiness.AddUser(userDTO);
            if (result)
            {
                return Ok($"User having User ID: {userDTO.Id} added successfully.");
            }
            else
            {
                return Ok<string>("User with the same EmailId already exists.");
            }
        }
        [HttpDelete]
        [Route("api/user/{id}/Delete")]
        public IHttpActionResult DeleteUser(int id)         //View -> Admin
        {
            try
            {
                UserService userBusiness = new UserService();
                userBusiness.DeleteUser(id);

                return Ok($"User with ID {id} is deleted.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpPut]
        [Route("api/User/{id}/Edit")]
        public IHttpActionResult UpdateUserDetails(int id,UserModel userModel)         //View -> Admin & User(Id)
        {
            List<string> errorMessages = new List<string>();
            if (string.IsNullOrWhiteSpace(userModel.FullName))
            {
                errorMessages.Add("Full Name is required.");
            }
            const string contactNumberPattern = @"^\d{10}$";
            string contact_number = Convert.ToString(userModel.ContactNo);
            if (!Regex.IsMatch(contact_number, contactNumberPattern))
            {
                errorMessages.Add("Contact Number must be exactly 10 digits.");
            }
            if (errorMessages.Any())
            {
                return BadRequest(string.Join(Environment.NewLine, errorMessages));
            }
            UserDTO userDTO = new UserDTO();
            userDTO.FullName = userModel.FullName;
            userDTO.ContactNo = userModel.ContactNo;
            userDTO.DateOfBirth = userModel.DateOfBirth;
            userDTO.Gender = userModel.Gender;

            bool result = userBusiness.UpdateUserDetails(id, userDTO);
            if (result)
            {
                return Ok<string>("Details Updated Successfully!");
            }
            else
            {
                return Ok<string>("Error occured during updating Try Again..");
            }
        }
        [HttpGet]
        [Route("api/user/getusersbyskill")]
        public IHttpActionResult GetUsersBySkill(string skillName)
        {
            List<UserDTO> users = userBusiness.GetUsersBySkill(skillName);

            if (users.Any())
            {
                List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
                List<DisplayModel> displayUsers = mapper.Map<List<DisplayModel>>(userModelList);

                return Ok(displayUsers);
            }
            else
            {
                return Ok("No users found with the specified skill.");
            }
        }
        [HttpPost]
        [Route("api/user/forgotpassword")]
        public IHttpActionResult ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            string email = forgotPasswordModel.EmailId;
            DateTime dob = forgotPasswordModel.DateOfBirth;
            string newPassword = forgotPasswordModel.NewPassword;
            string confirmPassword = forgotPasswordModel.ConfirmPassword;

            if (newPassword != confirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            if (userBusiness.VerifyUserEmailAndDOB(email, dob))
            {
                if (userBusiness.UpdatePassword(email, newPassword))
                {
                    return Ok("Password updated successfully.");
                }
                else
                {
                    return InternalServerError();
                }
            }
            else
            {
                return BadRequest("Invalid email or date of birth.");
            }
        }

        [HttpPost]
        [Route("api/user/postloginuser")]
        public IHttpActionResult PostUserLogin(UserModel loginUser)         //View -> Admin & User
        {
            string email = loginUser.EmailId;
            string password = loginUser.Password;

            UserDTO authenticatedUser = userBusiness.AuthenticateUser(email, password);

            if (authenticatedUser != null)
            {
                if (authenticatedUser.IsAdmin)
                {
                    List<UserDTO> users = userBusiness.GetListOfUsers();
                    List<UserModel> userModelList = mapper.Map<List<UserModel>>(users);
                    List<DisplayModel> displayModels = mapper.Map<List<DisplayModel>>(userModelList);
                    return Ok(displayModels);
                }
                else
                {
                    UserModel userModel = mapper.Map<UserModel>(authenticatedUser);
                    return Ok(userModel);
                }
            }
            else
            {
                return BadRequest("Invalid Email or Password");
            }
        }
    }
}
