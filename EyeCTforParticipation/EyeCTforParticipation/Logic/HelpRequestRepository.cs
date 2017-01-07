using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Data;
using EyeCTforParticipation.Models;
using System.Device.Location;
using System.Text.RegularExpressions;

namespace EyeCTforParticipation.Logic
{
    class HelpRequestRepository
    {
        IHelpRequestContext context;

        public HelpRequestRepository(IHelpRequestContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get a formatted postal code.
        /// </summary>
        /// <param name="postalCode">
        /// The original postal code.
        /// </param>
        /// <returns>
        /// The formatted postal code or null if the postal code could not be formatted.
        /// </returns>
        public string FormatPostalCode(string postalCode)
        {
            if (postalCode != null)
            {
                Match match = Regex.Match(postalCode, "([0-9]{4}).*?([a-zA-Z]{2})");
                if (match.Success)
                {
                    string numbers = match.Groups[1].ToString();
                    string letters = match.Groups[2].ToString();
                    return numbers + " " + letters.ToUpper();
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of help requests.
        /// </summary>
        /// <param name="keywords">
        /// Keywords included in search results, keywords are seperated by whitespace.
        /// </param>
        /// <param name="postalCode">
        /// The postal code used as the center of the search area. Example postal code: "5654 NE".
        /// </param>
        /// <param name="distance">
        /// The distance used as the radius of the search area, infinite distance is 0.
        /// </param>
        /// <param name="order">
        /// Order results by relevance, distance or date.
        /// </param>
        /// <returns>
        /// A list of help requests.
        /// </returns>
        public SearchResultModel Search(string keywords, string postalCode, SearchDistance distance, SearchOrder order, int userId, int skip)
        {
            // Default location
            GeoCoordinate location = null;

            // Try to get location from postal code
            GoogleMapsApi.Response googleMapsApi = GoogleMapsApi.Get(postalCode, "nl", "nl");
            if (googleMapsApi != null && postalCode != null)
            {
                location = googleMapsApi.Location;
            }

            return Search(keywords, location, distance, order, userId, skip);
        }

        /// <summary>
        /// Get a list of help requests.
        /// </summary>
        /// <param name="keywords">
        /// Keywords included in search results, keywords are seperated by whitespace.
        /// </param>
        /// <param name="location">
        /// The location used as the center of the search area.
        /// </param>
        /// <param name="distance">
        /// The distance used as the radius of the search area, infinite distance is 0 or null.
        /// </param>
        /// <param name="order">
        /// Order results by relevance, distance or date.
        /// </param>
        /// <returns>
        /// A list of help requests.
        /// </returns>
        public SearchResultModel Search(string keywords, GeoCoordinate location, SearchDistance distance, SearchOrder order, int userId, int skip)
        {
            // Maximum keywords length is 200
            keywords = keywords != null ? keywords.Length > 200 ? keywords.Substring(0, 200) : keywords.Length > 0 ? keywords : null : null;

            if (keywords != null && location != null)
            {
                return context.Search(keywords, location, (int)distance, order, userId, skip);
            }
            else if (keywords != null)
            {
                switch (order)
                {
                    case SearchOrder.DATE_ASC:
                    case SearchOrder.DATE_DESC:
                    case SearchOrder.RELEVANCE_ASC:
                    case SearchOrder.RELEVANCE_DESC:
                        return context.Search(keywords, order, userId, skip);
                    default:
                        return context.Search(keywords, SearchOrder.DATE_DESC, userId, skip);
                }
            }
            else if (location != null)
            {
                switch (order)
                {
                    case SearchOrder.DATE_ASC:
                    case SearchOrder.DATE_DESC:
                    case SearchOrder.DISTANCE_ASC:
                    case SearchOrder.DISTANCE_DESC:
                        return context.Search(location, (int)distance, order, userId, skip);
                    default:
                        return context.Search(location, (int)distance, SearchOrder.DISTANCE_ASC, userId, skip);
                }
            }
            else
            {
                switch (order)
                {
                    case SearchOrder.DATE_ASC:
                    case SearchOrder.DATE_DESC:
                        return context.Search(order, userId, skip);
                    default:
                        return context.Search(SearchOrder.DATE_DESC, userId, skip);
                }
            }
        }

        /// <summary>
        /// Get a specific help request.
        /// </summary>
        /// <param name="id">
        /// The id of the help request.
        /// </param>
        /// <returns>
        /// A help request.
        /// </returns>
        public HelpRequestModel Get(int id)
        {
            return context.Get(id);
        }



        /// <summary>
        /// Get a specific help request.
        /// </summary>
        /// <param name="id">
        /// The id of the help request.
        /// </param>
        /// <param name="volunteerId">
        /// The id of the volunteer.
        /// </param>
        /// <returns>
        /// A help request.
        /// </returns>
        public HelpRequestModel Get(int id, int volunteerId)
        {
            return context.Get(id, volunteerId);
        }

        /// <summary>
        /// Get help requests from a help seeker.
        /// </summary>
        /// <param name="userId">
        /// The id of the help seeker
        /// </param>
        /// <returns>
        /// A list of help requests.
        /// </returns>
        public List<HelpRequestModel> GetFromHelpSeeker(int userId)
        {
            return context.GetFromHelpSeeker(userId);
        }

        /// <summary>
        /// Create a new help request for a specific help seeker if the id is 0.
        /// Else if a help request already exists with the same id, the existing help request will be updated.
        /// </summary>
        /// <param name="helpRequest">
        /// The new or updated help request.
        /// </param>
        /// <returns>
        /// The created or updated help request.
        /// </returns>
        public HelpRequestModel Save(HelpRequestModel helpRequest)
        {
            //Default location
            GeoCoordinate location = new GeoCoordinate(52.132633, 5.291265999999999);
            string address = "Nederland";

            //Try to get location
            if (helpRequest.Address != address)
            {
                GoogleMapsApi.Response googleMapsApi = GoogleMapsApi.Get(helpRequest.Address, "nl", "nl");
                if (googleMapsApi != null)
                {
                    location = googleMapsApi.Location;
                    address = googleMapsApi.Address;
                }
            }

            helpRequest.Location = location;
            helpRequest.Address = address;

            if (helpRequest.Id == 0)
            {
                helpRequest.Id = context.Create(helpRequest);
            } else
            {
                context.Update(helpRequest);
            }
            return helpRequest;
        }

        /// <summary>
        /// Remove a help request.
        /// </summary>
        /// <param name="id">
        /// The id of the help request to be deleted.
        /// </param>
        /// <remarks>
        /// Messages and applications related to the help request will be deleted too!
        /// </remarks>
        public void Delete(int id)
        {
            context.Delete(id);
        }

        /// <summary>
        /// Close the help request.
        /// The help seeker and volunteer won't be able to continue sending each other messages.
        /// </summary>
        /// <param name="id">
        /// The id of the help request.
        /// </param>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        public void Close(int id, int helpSeekerId)
        {
            context.Close(id, helpSeekerId);
        }

        /// <summary>
        /// Reopen a previously closed help request.
        /// </summary>
        /// <param name="id">
        /// The id of the help request.
        /// </param>
        /// <param name="helpSeekerId">
        /// The id of the help seeker that created the help request.
        /// </param>
        public void Open(int id, int helpSeekerId)
        {
            context.Open(id, helpSeekerId);
        }

        /// <summary>
        /// Apply to a help request as volunteer.
        /// </summary>
        /// <param name="helpRequestId">
        /// The id of the help request the volunteer wants to apply to.
        /// </param>
        /// <param name="volunteerId">
        /// The id of the volunteer that wants to apply.
        /// </param>
        /// <returns>
        /// The application.
        /// </returns>
        public void Apply(int id, int volunteerId)
        {
            context.Apply(id, volunteerId);
        }

        /// <summary>
        /// Cancel the application to a help request.
        /// </summary>
        /// <param name="id">
        /// The id of the application.
        /// </param>
        /// <param name="volunteerId">
        /// The volunteer that wants to cancel the application.
        /// </param>
        public void CancelApplication(int id, int volunteerId)
        {
            context.CancelApplication(id, volunteerId);
        }

        /// <summary>
        /// Cancel the application to a help request.
        /// </summary>
        /// <param name="id">
        /// The id of the application.
        /// </param>
        /// <param name="userId">
        /// The help seeker that wants to cancel the application.
        /// </param>
        public void CancelApplicationAsHelpSeeker(int id, int userId)
        {
            context.CancelApplicationAsHelpSeeker(id, userId);
        }

        /// <summary>
        /// Get a list of applications a volunteer made.
        /// </summary>
        /// <param name="volunteerId">
        /// The id of the volunteer.
        /// </param>
        /// <returns>
        /// A list of applications for help requests by the volunteer
        /// </returns>
        public List<HelpRequestModel> GetApplications(int volunteerId)
        {
            return context.GetApplications(volunteerId);
        }

        /// <summary>
        /// Get a list of applications for a specific help request.
        /// </summary>
        /// <param name="id">
        /// The id of the help request.
        /// </param>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <returns>
        /// A list of applications for the help request created by the help seeker.
        /// </returns>
        public List<ApplicationModel> GetApplications(int id, int helpSeekerId)
        {
            return context.GetApplications(id, helpSeekerId);
        }

        public int ApplicationsCount(int id, int helpSeekerId)
        {
            return context.ApplicationsCount(id, helpSeekerId);
        }

        public bool HasApplied(int id, int volunteerId)
        {
            return context.HasApplied(id, volunteerId);
        }

        /// <summary>
        /// Let the volunteer that applied know you're interested.
        /// The help seeker and volunteer will be able to send each other messages in a interview chat.
        /// (Dutch: kennismakingsgesprek)
        /// </summary>
        /// <param name="id">
        /// The id of the application.
        /// </param>
        /// <param name="helpSeekerId">
        /// The id of the help seeker that created the help request that was applied to.
        /// </param>
        public void InterviewApplication(int id, int helpSeekerId)
        {
            context.InterviewApplication(id, helpSeekerId);
        }

        /// <summary>
        /// Approve the volunteer as final choice for the help request.
        /// The help seeker and volunteer will be able to continue sending each other messages in a chat that was previously the interview chat.
        /// </summary>
        /// <param name="applicationId">
        /// The id of the application.
        /// </param>
        /// <param name="helpSeekerId">
        /// The id of the help seeker that created the help request that was applied to.
        /// </param>
        /// <remarks>
        /// A volunteer cannot be approved if there hasn't been a interview chat.
        /// </remarks>
        public void ApproveApplication(int id, int helpSeekerId)
        {
            context.ApproveApplication(id, helpSeekerId);
        }
    }
}
