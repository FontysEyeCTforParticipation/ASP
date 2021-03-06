﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeCTforParticipation.Data;
using EyeCTforParticipation.Models;
using CryptSharp;
using System.IO;
using System.Device.Location;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace EyeCTforParticipation.Logic
{
    public class UserRepository
    {
        public IUserContext context;

        public UserRepository(IUserContext context)
        {
            this.context = context;
        }

        public UserModel Get(int userId)
        {
            return context.Get(userId);
        }

        public VolunteerModel GetVolunteer(int userId)
        {
            return context.GetVolunteer(userId);
        }

        /// <summary>
        /// Get a specific user for the RFID code.
        /// </summary>
        /// <param name="rfid">
        /// The RFID code.
        /// </param>
        /// <returns>
        /// An user.
        /// </returns>
        public UserModel Login(string rfid)
        {
            UserModel user = context.Login(rfid);
            if (user != null)
            {
                return user;
            }
            return null;
            //Throw invalid email and/or password exception
        }

        /// <summary>
        /// Get a specific user for the email and password combination.
        /// </summary>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// An user.
        /// </returns>
        public UserModel Login(string email, string password)
        {
            UserModel user = context.LoginPassword(email);
            if(user != null && CheckPassword(password, user.Password))
            {
                return user;
            }
            return null;
            //Throw invalid email and/or password exception
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="register">
        /// The data about the new user.
        /// </param>
        public int Register(RegisterModel register)
        {
            string hash = Crypter.Blowfish.Crypt(register.Password);
            bool approved = register.Role == UserRole.HelpSeeker;
            int userId = context.Register(new UserModel
            {
                Role = register.Role,
                Email = register.Email,
                Name = register.Name,
                Password = hash,
                Birthdate = register.Birthdate
            }, approved);
            if(userId >  0)
            {
                if (register.Role == UserRole.Volunteer)
                {
                    //Default location
                    GeoCoordinate location = new GeoCoordinate(52.132633, 5.291265999999999);
                    string address = "Nederland";

                    //Try to get location
                    GoogleMapsApi.Response googleMapsApi = GoogleMapsApi.Get(register.Address, "nl", "nl");
                    if(googleMapsApi != null)
                    {
                        location = googleMapsApi.Location;
                        address = googleMapsApi.Address;
                    }

                    context.RegisterVolunteer(new VolunteerModel
                    {
                        Id = userId,
                        Address = address,
                        Location = location,
                        DriversLicense = register.DriversLicense,
                        Car = register.Car
                    });
                }
                if (register.Avatar != null)
                {
                    string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\EyeCTforParticipation\\Uploads\\Avatars";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    register.Avatar.Save(directory + "\\" + userId.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
                return userId;
            }
            //Throw failed to register exception
            throw new Exception();
        }

        /// <summary>
        /// Approve a registration.
        /// </summary>
        /// <param name="userId">
        /// The user that registered.
        /// </param>
        public void ApproveRegistration(int userId)
        {

            context.ApproveRegistration(userId);

        }

        /// <summary>
        /// Send an email with a reset code.
        /// </summary>
        /// <param name="email">
        /// The email of the user that requested a reset code.
        /// </param>
        public void SendPasswordResetEmail(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the password of the user that matches the reset code.
        /// </summary>
        /// <param name="newPassword">
        /// The new password to be set.
        /// </param>
        /// <param name="resetCode">
        /// The reset code.
        /// </param>
        public void ResetPassword(string newPassword, string resetCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replace the user data with new user data.
        /// </summary>
        /// <param name="user">
        /// The new user data.
        /// </param>
        public void Profile(string name, DateTime birthdate, int userId)
        {
            context.Profile(name, birthdate, userId);
        }

        public void VolunteerProfile(string address, bool driversLicense, bool car, string about, int userId)
        {
            //Default location
            GeoCoordinate location = new GeoCoordinate(52.132633, 5.291265999999999);

            //Try to get location
            if (address != "Nederland")
            {
                GoogleMapsApi.Response googleMapsApi = GoogleMapsApi.Get(address, "nl", "nl");
                if (googleMapsApi != null)
                {
                    location = googleMapsApi.Location;
                    address = googleMapsApi.Address;
                }
            } else
            {
                address = "Nederland";
            }

            context.VolunteerProfile(address, location, driversLicense, car, about, userId);
        }

        public bool CheckPassword(string password, string hash)
        {
            return Crypter.CheckPassword(password, hash);
        }

        private string Token()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string Token(string token)
        {
            string data = context.Token(token);
            if(data == null)
            {
                throw new InvalidTokenException();
            }
            return data;
        }

        private string DictionaryToBase64(Dictionary<string, object> dictionary)
        {
            string json = JsonConvert.SerializeObject(dictionary, Formatting.None);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }

        public Dictionary<string, object> Base64ToDictionary(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        private void ConfirmEmail(string to, string name, string subject, string message, string token)
        {
            new Email("no-reply@eyectforparticipation.nl", to, subject, "<html><body style=\"font-family: sans-serif;line-height: 24px;font-size: 16px;color: #444;\"><h2 style\"font-weight: 300;\">EyeCT for Participation</h2><br />Geachte " + name + ",<br /><br />" + message + "<br />Klik <a href=\"http://eyectforparticipation.nl/user/confirm/" + token + "\" target=\"_blank\">hier</a> om dit te bevestigen.<br /><br /><br />Met vriendelijke groet,<br /><br />EyeCT for Participation</p></body></html>").Send();
        }

        public void Email(string newEmail, int userId, string url, string email, string name)
        {
            string token = Token();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["action"] = "email";
            data["url"] = url;
            data["userId"] = userId;
            data["email"] = newEmail;
            context.Token(token, DictionaryToBase64(data));
            ConfirmEmail(email,
                name,
                "Bevestig uw e-mailadres wijziging van uw account op eyectforparticipation.nl",
                "U heeft uw e-mailadres gewijzigd.",
                token);
        }

        public void Password(string password, int userId, string url, string email, string name)
        {
            string hash = Crypter.Blowfish.Crypt(password);
            string token = Token();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["action"] = "password";
            data["url"] = url;
            data["userId"] = userId;
            data["hash"] = hash;
            context.Token(token, DictionaryToBase64(data));
            ConfirmEmail(email,
                name,
                "Bevestig uw wachtwoord wijziging van uw account op eyectforparticipation.nl",
                "U heeft uw wachtwoord gewijzigd.",
                token);
        }

        public void Password(string hash, int userId)
        {
            context.Password(hash, userId);
        }

        private bool ValidatePassword(string password)
        {
            return true;
        }

        /// <summary>
        /// Remove an user.
        /// </summary>
        /// <param name="userId">
        /// The id of the user to be removed.
        /// </param>
        public void Delete(int userId)
        {
            throw new NotImplementedException();
            //this method checks every single table, from top to bottom, then back up top
        }

        /// <summary>
        /// Add a help seeker to an aid worker.
        /// </summary>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <param name="aidWorkerId">
        /// The id of the aid worker.
        /// </param>
        public void AddHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
           context.AddHelpSeeker(helpSeekerId, aidWorkerId);
        }

        /// <summary>
        /// Remove a help seeker from an aid worker.
        /// </summary>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <param name="aidWorkerId">
        /// The id of the aid worker.
        /// </param>
        public void RemoveHelpSeeker(int helpSeekerId, int aidWorkerId)
        {
            context.RemoveHelpSeeker(helpSeekerId, aidWorkerId);
        }

        /// <summary>
        /// Approve being added to an aid worker as help seeker.
        /// </summary>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <param name="aidWorkerId">
        /// The id of the aid worker.
        /// </param>
        public void ApproveAidWorker(int helpSeekerId, int aidWorkerId)
        {
            context.ChangeApproveAidWorker(helpSeekerId, aidWorkerId, true);
        }

        /// <summary>
        /// Disapprove a aid worker that was previously approved by a help seeker.
        /// </summary>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <param name="aidWorkerId">
        /// The id of the aid worker.
        /// </param>
        /// <remarks>
        /// The aid worker needs to be previously approved before it can be disapproved.
        /// </remarks>
        public void DisapproveAidWorker(int helpSeekerId, int aidWorkerId)
        {
            context.ChangeApproveAidWorker(helpSeekerId, aidWorkerId, false);
        }

        /// <summary>
        /// Get a list of help seekers for an aid worker.
        /// </summary>
        /// <param name="aidWorkerId">
        /// The id of the aid worker.
        /// </param>
        /// <returns>
        /// A list of help seekers.
        /// </returns>
        public List<UserModel> GetHelpSeekers(int aidWorkerId)
        {
            return context.GetHelpSeekers(aidWorkerId);
        }

        /// <summary>
        /// Get a list of aid workers for an help seeker.
        /// </summary>
        /// <param name="helpSeekerId">
        /// The id of the help seeker.
        /// </param>
        /// <returns>
        /// A list of aid workers.
        /// </returns>
        public List<UserModel> GetAidWorkers(int helpSeekerId)
        {
            return context.GetAidWorkers(helpSeekerId);
        }

        /// <summary>
        /// Get a list of all users
        /// </summary>
        /// <returns>
        /// A list of all users.
        /// </returns>
        public List<UserModel> Get()
        {
            return context.Get();
        }

        /// <summary>
        /// Changes zoom level for an user.
        /// </summary>
        /// <param name="zoom">
        /// The zoom level between 100 and 150.
        /// </param>
        /// <param name="userId">
        /// The id of the user.
        /// </param>
        public void Zoom(int zoom, int userId)
        {
            context.Zoom((zoom >= 100 ? (zoom <= 150 ? zoom : 150) : 100), userId);
        }

        public bool ValidateEmail(string email)
        {
            if (email != null)
            {
                Match match = Regex.Match(email, "^(?:(?:[\\w`~!#$%^&*\\-=+;:{}'|,?\\/]+(?:(?:\\.(?:\"(?:\\\\?[\\w`~!#$%^&*\\-=+;:{}'|,?\\/\\.()<>\\[\\] @]|\\\\\"|\\\\\\\\)*\"|[\\w`~!#$%^&*\\-=+;:{}'|,?\\/]+))*\\.[\\w`~!#$%^&*\\-=+;:{}'|,?\\/]+)?)|(?:\"(?:\\\\?[\\w`~!#$%^&*\\-=+;:{}'|,?\\/\\.()<>\\[\\] @]|\\\\\"|\\\\\\\\)+\"))@(?:[a-zA-Z\\d\\-]+(?:\\.[a-zA-Z\\d\\-]+)*|\\[\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\])$");
                if (match.Success)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
