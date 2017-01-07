using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using EyeCTforParticipation.Data.MemoryContext;
using System.Device.Location;

namespace EyeCTforParticipation.Data
{
    public class UserMemoryContext : IUserContext
    {
        public UserModel Login(string rfid)
        {
            var result = from user in Tables.User
                         where user.Rfid == rfid
                         select new UserModel
                         {
                             Id = user.Id,
                             Role = user.Role,
                             Name = user.Name,
                             Approved = user.Approved
                         };
            return result.Count() > 0 ? result.First() : null;
        }

        public UserModel LoginPassword(string email)
        {
            var result = from user in Tables.User
                         where user.Email == email
                         select new UserModel
                         {
                             Id = user.Id,
                             Role = user.Role,
                             Name = user.Name,
                             Password = user.Password,
                             Approved = user.Approved
                         };
            return result.Count() > 0 ? result.First() : null;
        }

        public int Register(UserModel user, bool approved)
        {
            int id = Tables.User.Count() > 0 ? Tables.User.Max(x => x.Id) + 1 : 1;
            Tables.User.Add(new UserModel
            {
                Id = id,
                Role = user.Role,
                Email = user.Email,
                Name = user.Name,
                Password = user.Password,
                Birthdate = user.Birthdate,
                Approved = approved
            });
            return id;
        }

        public void RegisterVolunteer(VolunteerModel volunteer)
        {
            throw new NotImplementedException();
        }

        public void ApproveRegistration(int userId)
        {
            var results = from user in Tables.User
                          where user.Id == userId
                          select user;
            if (results.Count() == 1)
            {
                UserModel result = results.ElementAt(0);
                result.Approved = true;
            }
        }
        public void Edit(UserModel updatedUser)
        {
            var results = from user in Tables.User
                          where user.Id == updatedUser.Id
                          select user;
            if (results.Count() == 1)
            {
                //(Email, Name, Password, Birthdate, Approved)
                UserModel result = results.ElementAt(0);
                result.Email = updatedUser.Email;
                result.Name = updatedUser.Name;
                result.Password = updatedUser.Password;
                result.Birthdate = updatedUser.Birthdate;
                result.Approved = updatedUser.Approved;
            }
        }
        public void Delete(int deletedUserId)
        {
            throw new NotImplementedException();
        }
        public void AddHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
            Tables.HelpSeekerAidWorker.Add(new HelpSeekerAidWorkerModel
            {
                HelpSeekerUserId = helpSeekerId,
                AidWorkerUserId = aidWorkerId,
                Approved = false
            });
        }
        public void RemoveHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
            var helpSeekerResults = from helpSeekerAidWorker in Tables.HelpSeekerAidWorker
                                     where helpSeekerId == helpSeekerAidWorker.HelpSeekerUserId && aidWorkerId == helpSeekerAidWorker.AidWorkerUserId
                                     select helpSeekerAidWorker;
            for (int x = 0; x < helpSeekerResults.Count(); x++)
            {
                Tables.HelpSeekerAidWorker.Remove(helpSeekerResults.ElementAt(x));
            }
        }
        public void ChangeApproveAidWorker(int helpSeekerId, int aidWorkerId, bool approved)
        {
            var results = from helpSeekerAidWorker in Tables.HelpSeekerAidWorker
                          where helpSeekerId == helpSeekerAidWorker.HelpSeekerUserId && aidWorkerId == helpSeekerAidWorker.AidWorkerUserId
                          select helpSeekerAidWorker;
            if (results.Count() == 1)
            {
                HelpSeekerAidWorkerModel result = results.ElementAt(0);
                result.Approved = approved;
            }
        }
        public List<UserModel> GetHelpSeekers(int aidWorkerId)
        {
            var results = from helpSeekerAidWorker in Tables.HelpSeekerAidWorker
                          join user in Tables.User on helpSeekerAidWorker.HelpSeekerUserId equals user.Id
                          where helpSeekerAidWorker.AidWorkerUserId == aidWorkerId
                          select new UserModel
                          {
                              Id = user.Id,
                              Name = user.Name
                          };
            return results.ToList();
        }
        public List<UserModel> GetAidWorkers(int HelpSeekerId)
        {
            var results = from helpSeekerAidWorker in Tables.HelpSeekerAidWorker
                          join user in Tables.User on helpSeekerAidWorker.AidWorkerUserId equals user.Id
                          where helpSeekerAidWorker.HelpSeekerUserId == HelpSeekerId
                          select new UserModel
                          {
                              Id = user.Id,
                              Name = user.Name
                          };
            return results.ToList();
        }

        public GeoCoordinate GetVolunteerLocation(int userId)
        {
            throw new NotImplementedException();
        }

        public List<UserModel> Get()
        {
            throw new NotImplementedException();
        }

        public void Zoom(int zoom, int userId)
        {
            throw new NotImplementedException();
        }

        public void Profile(string name, DateTime birthdate, int userId)
        {
            throw new NotImplementedException();
        }

        public void Password(string password, int userId)
        {
            throw new NotImplementedException();
        }

        public UserModel Get(int userId)
        {
            throw new NotImplementedException();
        }

        public string Token(string token)
        {
            throw new NotImplementedException();
        }

        public void Token(string token, string data)
        {
            throw new NotImplementedException();
        }

        bool IUserContext.Password(string hash, int userId)
        {
            throw new NotImplementedException();
        }

        public VolunteerModel GetVolunteer(int userId)
        {
            throw new NotImplementedException();
        }

        public void VolunteerProfile(string address, GeoCoordinate location, bool driversLicense, bool car, string about, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
