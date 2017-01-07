using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Models;
using System.Device.Location;
using EyeCTforParticipation.Data.MemoryContext;

namespace EyeCTforParticipation.Data
{
    class HelpRequestMemoryContext : IHelpRequestContext
    {
        public List<HelpRequestModel> Search(SearchOrder order)
        {
            var results = from helpRequest in Tables.HelpRequest
                          join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                          where helpRequest.Closed == false
                          orderby
                            order == SearchOrder.DATE_ASC ? helpRequest.Date : default(DateTime),
                            order == SearchOrder.DATE_DESC ? helpRequest.Date : default(DateTime) descending
                          select new HelpRequestModel
                          {
                              Id = helpRequest.Id,
                              Title = helpRequest.Title,
                              Date = helpRequest.Date,
                              Address = helpRequest.Address,
                              Urgency = helpRequest.Urgency,
                              HelpSeeker = new UserModel
                              {
                                  Name = user.Name
                              }
                          };
            return results.ToList();
        }
        
        public List<HelpRequestModel> Search(string keywords, SearchOrder order)
        {
            var results = from helpRequest in Tables.HelpRequest
                          join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                          let matches = Functions.KeywordMatches(helpRequest.Title + helpRequest.Content, keywords, ' ')
                          where matches > 0 && helpRequest.Closed == false
                          orderby
                            order == SearchOrder.DATE_ASC ? helpRequest.Date : default(DateTime),
                            order == SearchOrder.DATE_DESC ? helpRequest.Date : default(DateTime) descending,
                            order == SearchOrder.RELEVANCE_ASC ? matches : 0,
                            order == SearchOrder.RELEVANCE_DESC ? matches : 0 descending
                          select new HelpRequestModel
                          {
                              Id = helpRequest.Id,
                              Title = helpRequest.Title,
                              Date = helpRequest.Date,
                              Address = helpRequest.Address,
                              Urgency = helpRequest.Urgency,
                              HelpSeeker = new UserModel
                              {
                                  Name = user.Name
                              }
                          };
            return results.ToList();
        }

        public List<HelpRequestModel> Search(GeoCoordinate location, int distance, SearchOrder order)
        {
            var results = from helpRequest in Tables.HelpRequest
                          join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                          let getDistance = helpRequest.Location.GetDistanceTo(location)
                          where (getDistance <= distance * 1000 || distance == 0) && helpRequest.Closed == false
                          orderby
                            order == SearchOrder.DATE_ASC ? helpRequest.Date : default(DateTime),
                            order == SearchOrder.DATE_DESC ? helpRequest.Date : default(DateTime) descending,
                            order == SearchOrder.DISTANCE_ASC ? getDistance : 0,
                            order == SearchOrder.DISTANCE_DESC ? getDistance : 0 descending
                          select new HelpRequestModel
                          {
                              Id = helpRequest.Id,
                              Title = helpRequest.Title,
                              Date = helpRequest.Date,
                              Address = helpRequest.Address,
                              Urgency = helpRequest.Urgency,
                              Distance = getDistance,
                              HelpSeeker = new UserModel
                              {
                                  Name = user.Name
                              }
                          };
            return results.ToList();
        }

        public List<HelpRequestModel> Search(string keywords, GeoCoordinate location, int distance, SearchOrder order)
        {
            var results = from helpRequest in Tables.HelpRequest
                          join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                          let matches = Functions.KeywordMatches(helpRequest.Title + helpRequest.Content, keywords, ' ')
                          let getDistance = helpRequest.Location.GetDistanceTo(location)
                          where (getDistance <= distance * 1000 || distance == 0) && matches > 0 && helpRequest.Closed == false
                          orderby
                            order == SearchOrder.DATE_ASC ? helpRequest.Date : default(DateTime),
                            order == SearchOrder.DATE_DESC ? helpRequest.Date : default(DateTime) descending,
                            order == SearchOrder.DISTANCE_ASC ? getDistance : 0,
                            order == SearchOrder.DISTANCE_DESC ? getDistance : 0 descending,
                            order == SearchOrder.RELEVANCE_ASC ? matches : 0,
                            order == SearchOrder.RELEVANCE_DESC ? matches : 0 descending
                          select new HelpRequestModel
                          {
                              Id = helpRequest.Id,
                              Title = helpRequest.Title,
                              Date = helpRequest.Date,
                              Address = helpRequest.Address,
                              Urgency = helpRequest.Urgency,
                              Distance = getDistance,
                              HelpSeeker = new UserModel
                              {
                                  Name = user.Name
                              }
                          };
            return results.ToList();
        }

        public HelpRequestModel Get(int id)
        {
            var result = from helpRequest in Tables.HelpRequest
                         where helpRequest.Id == id
                         join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                         select new HelpRequestModel
                         {
                             Id = helpRequest.Id,
                             Title = helpRequest.Title,
                             HelpSeeker = new UserModel
                             {
                                 Id = helpRequest.HelpSeeker.Id,
                                 Name = user.Name
                             },
                             Content = helpRequest.Content,
                             Date = helpRequest.Date,
                             Address = helpRequest.Address,
                             Urgency = helpRequest.Urgency,
                             Closed = helpRequest.Closed
                         };
            return result.Count() > 0 ? result.First() : null;
        }

        public List<HelpRequestModel> GetFromHelpSeeker(int userId)
        {
            var results = from helpRequest in Tables.HelpRequest
                          where helpRequest.HelpSeeker.Id == userId
                          join user in Tables.User on helpRequest.HelpSeeker.Id equals user.Id
                          select new HelpRequestModel
                          {
                              Id = helpRequest.Id,
                              Title = helpRequest.Title,
                              HelpSeeker = new UserModel
                              {
                                  Id = helpRequest.HelpSeeker.Id,
                                  Name = user.Name
                              },
                              Content = helpRequest.Content,
                              Date = helpRequest.Date,
                              Address = helpRequest.Address,
                              Urgency = helpRequest.Urgency,
                              Closed = helpRequest.Closed
                          };
            return results.ToList();
        }

        public int Create(HelpRequestModel helpRequest)
        {
            int id = Tables.HelpRequest.Count() > 0 ? Tables.HelpRequest.Max(x => x.Id) + 1 : 1;
            Tables.HelpRequest.Add(new HelpRequestModel
            {
                Id = id,
                Title = helpRequest.Title,
                Address = helpRequest.Address,
                Location = helpRequest.Location,
                Content = helpRequest.Content,
                Date = DateTime.Now,
                Closed = false,
                Urgency = helpRequest.Urgency,
                HelpSeeker = helpRequest.HelpSeeker
            });
            return helpRequest.Id;
        }

        public void Update(HelpRequestModel updatedHelpRequest)
        {
            var results = from helpRequest in Tables.HelpRequest
                          where helpRequest.Id == updatedHelpRequest.Id && helpRequest.HelpSeeker.Id == updatedHelpRequest.HelpSeeker.Id
                          select helpRequest;
            if(results.Count() == 1)
            {
                HelpRequestModel result = results.ElementAt(0);
                result.Title = updatedHelpRequest.Title;
                result.Content = updatedHelpRequest.Content;
                result.Address = updatedHelpRequest.Address;
                result.Urgency = updatedHelpRequest.Urgency;
            }
        }

        public void Delete(int id)
        {
            var applicationResults = from application in Tables.Application
                                     where application.HelpRequest.Id == id
                                     select application;
            for(int x = 0; x < applicationResults.Count(); x++)
            {
                Tables.Application.Remove(applicationResults.ElementAt(x));
            }

            var messageResults = from message in Tables.Message
                                 where Tables.Application.Any(x => x.HelpRequest.Id == id)
                                 select message;
            for (int x = 0; x < messageResults.Count(); x++)
            {
                Tables.Message.Remove(messageResults.ElementAt(x));
            }

            var results = from helpRequest in Tables.HelpRequest
                          where helpRequest.Id == id
                          select helpRequest;
            if(results.Count() == 1)
            {
                Tables.HelpRequest.Remove(results.ElementAt(0));
            }
        }

        public void Close(int id, int helpSeekerId)
        {
            var results = from helpRequest in Tables.HelpRequest
                         where helpRequest.Id == id && helpRequest.HelpSeeker.Id == helpSeekerId
                         select helpRequest;
            if (results.Count() == 1)
            {
                results.ElementAt(0).Closed = true;
            }
        }

        public void Open(int id, int helpSeekerId)
        {
            var results = from helpRequest in Tables.HelpRequest
                          where helpRequest.Id == id && helpRequest.HelpSeeker.Id == helpSeekerId
                          select helpRequest;
            if (results.Count() == 1)
            {
                results.ElementAt(0).Closed = false;
            }
        }

        public void Apply(int id, int volunteerId)
        {
            //Needs update check
            ApplicationModel application = new ApplicationModel
            {
                Id = Tables.Application.Count() > 0 ? Tables.Application.Max(x => x.Id) + 1 : 1,
                Volunteer = new VolunteerModel
                {
                    Id = volunteerId
                },
                Status = ApplicationStatus.NONE,
                Date = DateTime.Now,
                HelpRequest = new HelpRequestModel
                {
                    Id = id
                }
            };
            Tables.Application.Add(application);
        }

        public void CancelApplication(int id, int volunteerId)
        {
            var results = from application in Tables.Application
                          where application.Id == id && application.Volunteer.Id == volunteerId
                          select application;
            if (results.Count() == 1)
            {
                results.ElementAt(0).Status = ApplicationStatus.CANCELLED;
            }
        }

        public void CancelApplicationAsHelpSeeker(int id, int userId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.Id == id && helpRequest.HelpSeeker.Id == userId
                          select application;
            if (results.Count() == 1)
            {
                results.ElementAt(0).Status = ApplicationStatus.CANCELLED;
            }
        }

        public List<ApplicationModel> GetApplications(int volunteerId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.Volunteer.Id == volunteerId
                          select new ApplicationModel
                          {
                              Id = application.Id,
                              HelpRequest = new HelpRequestModel
                              {
                                  Id = helpRequest.Id,
                                  Title = helpRequest.Title,
                                  Urgency = helpRequest.Urgency,
                                  Closed = helpRequest.Closed
                              },
                              Status = application.Status,
                              Date = application.Date
                          };
            return results.ToList();
        }

        public List<ApplicationModel> GetApplications(int id, int helpSeekerId)
        {
            var results = from application in Tables.Application
                          join user in Tables.User on application.Volunteer.Id equals user.Id
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.HelpRequest.Id == id && helpRequest.HelpSeeker.Id == helpSeekerId
                          select new ApplicationModel
                          {
                              Id = application.Id,
                              Volunteer = new VolunteerModel
                              {
                                  Id = user.Id,
                                  Name = user.Name
                              },
                              Status = application.Status,
                              Date = application.Date
                          };
            return results.ToList();
        }

        public void InterviewApplication(int id, int helpSeekerId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.Id == id && helpRequest.HelpSeeker.Id == helpSeekerId
                          select application;
            if(results.Count() == 1)
            {
                results.ElementAt(0).Status = ApplicationStatus.INTERVIEW;
            }
        }

        public void ApproveApplication(int id, int helpSeekerId)
        {
            var results = from application in Tables.Application
                          join helpRequest in Tables.HelpRequest on application.HelpRequest.Id equals helpRequest.Id
                          where application.Id == id && helpRequest.HelpSeeker.Id == helpSeekerId
                          select application;
            if (results.Count() == 1)
            {
                results.ElementAt(0).Status = ApplicationStatus.APPROVED;
            }
        }

        public int ApplicationsCount(int id, int helpSeekerId)
        {
            throw new NotImplementedException();
        }

        public bool HasApplied(int id, int volunteerId)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(SearchOrder order, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(string keywords, SearchOrder order, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(GeoCoordinate location, int distance, SearchOrder order, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(string keywords, GeoCoordinate location, int distance, SearchOrder order, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(SearchOrder order, int userId, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(string keywords, SearchOrder order, int userId, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(GeoCoordinate location, int distance, SearchOrder order, int userId, int skip)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(string keywords, GeoCoordinate location, int distance, SearchOrder order, int userId, int skip)
        {
            throw new NotImplementedException();
        }

        public HelpRequestModel Get(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public List<HelpRequestModel> GetApplications(int volunteerId, GeoCoordinate location)
        {
            throw new NotImplementedException();
        }

        List<HelpRequestModel> IHelpRequestContext.GetApplications(int volunteerId)
        {
            throw new NotImplementedException();
        }
    }
}
