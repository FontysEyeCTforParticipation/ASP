using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using System.Device.Location;

namespace EyeCTforParticipation.Data
{
    interface IHelpRequestContext
    {
        List<HelpRequestModel> Search(SearchOrder order);
        List<HelpRequestModel> Search(string keywords, SearchOrder order);
        List<HelpRequestModel> Search(GeoCoordinate location, int distance, SearchOrder order);
        List<HelpRequestModel> Search(string keywords, GeoCoordinate location, int distance, SearchOrder order);
        HelpRequestModel Get(int id);
        List<HelpRequestModel> GetFromHelpSeeker(int userId);
        int Create(HelpRequestModel helpRequest);
        void Update(HelpRequestModel helpRequest);
        void Delete(int id);
        void Close(int id, int helpSeekerId);
        void Open(int id, int helpSeekerId);
        void Apply(int id, int volunteerId);
        void CancelApplication(int id, int volunteerId);
        void CancelApplicationAsHelpSeeker(int id, int userId);
        List<ApplicationModel> GetApplications(int volunteerId);
        List<ApplicationModel> GetApplications(int id, int helpSeekerId);
        int ApplicationsCount(int id, int helpSeekerId);
        bool HasApplied(int id, int volunteerId);
        void InterviewApplication(int id, int helpSeekerId);
        void ApproveApplication(int id, int helpSeekerId);
    }
}
