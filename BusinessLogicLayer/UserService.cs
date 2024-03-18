using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using System.Text;

namespace BusinessLogicLayer
{
    public class UserService
    {
        private readonly IMapper mapper;
        public UserService()
        {
            var mapConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>();
            });
            mapper = mapConfig.CreateMapper();
        }

        DataAccessLayer.SkillTrackerDBEntities db = new SkillTrackerDBEntities();
        public List<UserDTO> GetListOfUsers()
        {
            DbSet<User> userDb = db.Users;

            List<UserDTO> users = new List<UserDTO>();
            foreach (var user in userDb)
            {
                if (user.IsAdmin != true)
                {
                    users.Add(MapUserToUserDTO(user));
                }
            }
            return users;
        }
        public List<UserDTO> GetListOfAdmins()
        {
            DbSet<User> userDb = db.Users;

            List<UserDTO> users = new List<UserDTO>();
            foreach (var user in userDb)
            {
                if (user.IsAdmin == true)
                {
                    users.Add(MapUserToUserDTO(user));
                }
            }
            return users;
        }
        public List<UserDTO> GetUserByIdOrName(string searchUser)
        {
            DbSet<User> userDb = db.Users;
            List<UserDTO> matchingUsers = new List<UserDTO>();

            if (int.TryParse(searchUser, out int userId))
            {
                var userById = userDb.FirstOrDefault(u => u.Id == userId);
                if (userById != null)
                {
                    matchingUsers.Add(MapUserToUserDTO(userById));
                    return matchingUsers;
                }
            }
            var userByName = userDb.Where(u => u.FullName.StartsWith(searchUser)).ToList();
            if (userByName.Any())
            {
                matchingUsers.AddRange(userByName.Select(u => MapUserToUserDTO(u)));
            }
            return matchingUsers;
        }
        public GetUserDetailsDTO GetUserDetails(int userId)
        {
            User user = db.Users.Find(userId);
            List<UserSkill> userSkills = db.UserSkills
                                        .Where(userskill => userskill.UserId == userId)
                                        .ToList();
            var userDetails = userSkills
        .Join(db.Skills, us => us.SkillId, skill => skill.Id, (us, skill) => new UpdateUserSkillsDTO
        {
            Name = skill.Name,
            Proficiency = us.Proficiency
        })
        .ToList();
            List<UpdateUserSkillsDTO> updateUserSkillsDTOs = new List<UpdateUserSkillsDTO>();
            foreach (var item in userDetails)
            {
                UpdateUserSkillsDTO updateUserSkillsDTO = new UpdateUserSkillsDTO
                {
                    Name = item.Name,
                    Proficiency = item.Proficiency
                };
                updateUserSkillsDTOs.Add(updateUserSkillsDTO);
            }

            GetUserDetailsDTO getUserDetailsDTO = new GetUserDetailsDTO();
            getUserDetailsDTO.Id = user.Id;
            getUserDetailsDTO.EmailId = user.EmailId;
            getUserDetailsDTO.Password = user.Password;
            getUserDetailsDTO.FullName = user.FullName;
            getUserDetailsDTO.DateOfBirth = user.DateOfBirth;
            Int64 contact_no = Convert.ToInt64(user.ContactNo);
            getUserDetailsDTO.ContactNo = contact_no;
            getUserDetailsDTO.Gender = user.Gender;
           
            getUserDetailsDTO.Skills = new List<UpdateUserSkillsDTO>();
            foreach (var skill in updateUserSkillsDTOs)
            {
                UpdateUserSkillsDTO skillDTO = new UpdateUserSkillsDTO
                {
                    Name = skill.Name,
                    Proficiency = skill.Proficiency
                };
                getUserDetailsDTO.Skills.Add(skillDTO);
            }
            return getUserDetailsDTO;
        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                byte[] truncatedBytes = new byte[10];
                Array.Copy(bytes, truncatedBytes, 10);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < truncatedBytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool AddUser(UserDTO newUser)
        {
            DbSet<User> userDb = db.Users;

            if (userDb.Any(u => u.EmailId == newUser.EmailId))
            {
                return false;
            }
            User user = new User();
            user.EmailId = newUser.EmailId;
            user.Password = HashPassword(newUser.Password);

            userDb.Add(user);
            db.SaveChanges();

            newUser.Id = user.Id;
            return true;
        }
        public void DeleteUser(int userId)
        {
            using (var db = new SkillTrackerDBEntities())
            {
                User user = db.Users.Find(userId);

                if (user != null)
                {
                    // Delete related UserSkills
                    var userSkills = db.UserSkills.Where(us => us.UserId == userId);
                    foreach (var userSkill in userSkills)
                    {
                        db.UserSkills.Remove(userSkill);
                    }

                    // Remove the user entity from the database
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception($"User with ID {userId} not found.");

                }
            }
        }
        public bool UpdateUserDetails(int id,UserDTO userDTO)
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Id == id);
            if (existingUser != null)
            {
                existingUser.FullName = userDTO.FullName;
                existingUser.ContactNo = userDTO.ContactNo;
                existingUser.DateOfBirth = userDTO.DateOfBirth;
                existingUser.Gender = userDTO.Gender;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool VerifyUserEmailAndDOB(string email, DateTime dob)
        {
            DbSet<User> userDb = db.Users;
            return userDb.Any(u => u.EmailId == email && u.DateOfBirth == dob);
        }
        public bool UpdatePassword(string email, string newPassword)
        {
            DbSet<User> userDb = db.Users;
            var userToUpdate = userDb.FirstOrDefault(u => u.EmailId == email);

            if (userToUpdate != null)
            {
                userToUpdate.Password = HashPassword(newPassword);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public UserDTO AuthenticateUser(string email, string password)
        {
            DbSet<User> userDb = db.Users;
            string hashedPassword = HashPassword(password);

            var authenticatedUser = userDb.FirstOrDefault(u => u.EmailId == email && u.Password == hashedPassword);

            if (authenticatedUser != null)
            {
                UserDTO userDTO = mapper.Map<UserDTO>(authenticatedUser);
                return userDTO;
            }
            return null;
        }
        public List<UserDTO> GetUsersBySkill(string skillName)
        {
            DbSet<UserSkill> userSkillDb = db.UserSkills;

            var usersWithSkill = userSkillDb
                .Where(us => us.Skill.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase))
                .Select(us => us.User).ToList();

            return usersWithSkill.Select(user => MapUserToUserDTO(user)).ToList();
        }
        private UserDTO MapUserToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                EmailId = user.EmailId
            };
        }

    }
}
