using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using System.Device.Location;

namespace EyeCTforParticipation.Data
{
    public interface IUserContext
    {
        UserModel Get(int userId);
        VolunteerModel GetVolunteer(int userId);
        UserModel Login(string rfid);
        UserModel LoginPassword(string email);
        int Register(UserModel user, bool approved);
        void RegisterVolunteer(VolunteerModel volunteer);
        void ApproveRegistration(int userId);
        void Profile(string name, DateTime birthdate, int userId);
        void VolunteerProfile(string address, GeoCoordinate location, bool driversLicense, bool car, string about, int userId);
        string Token(string token);
        void Token(string token, string data);
        bool Password(string hash, int userId);
        void Delete(int userId);
        void AddHelpSeeker(int helpSeekerId, int aidWorkerId);
        void RemoveHelpSeeker(int helpSeekerId, int aidWorkerId);
        void ChangeApproveAidWorker(int helpSeekerId, int aidWorkerId, bool approved);
        List<UserModel> GetHelpSeekers(int aidWorkerId);
        List<UserModel> GetAidWorkers(int HelpSeekerId);
        List<UserModel> Get();
        void Zoom(int zoom, int userId);
    }
}
