using System;
using System.Collections.Generic;

namespace WeVeed.Domain.Entities
{
    public class User : EntityBase
    {
        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // IF YOU CHANGE SOMETHING HERE, ALSO CHANGE IN WEVEED INTEGRATOR VIDEO.CS, OR THE UPLOAD OF A VIDEO WILL FAIL 


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProducerName { get; set; }

        public string ProducerDescription { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string UserType { get; set; }            // producer; user


        public DateTime ActivationDate { get; set; }

        public string ActivationCode { get; set; }

        // for reset password
        public DateTime ResetDate { get; set; }

        public string ResetToken { get; set; }

        public bool IsResetTokenActive { get; set; }
        // ------------------

        public string FacebookUid { get; set; }

        public string ProfileImageUrl { get; set; }


        public bool IsActive { get; set; }

        public string Role { get; set; }


        public string EmailContact { get; set; }

        public string FacebookContactUrl { get; set; }

        public string InstaContactUrl { get; set; }


        // IMPORTANT 
        public long NumberOfSeriesFollowers { get; set; }

        // IMPORTANT 
        public long NumberOfViewsOnVideos { get; set; }

        // IMPORTANT 
        public List<string> SeriesFollowed { get; set; } = new List<string>();


        // Login With Facebook
        public string FacebookUserId { get; set; }


        public bool IsProducerValidatedByAdmin { get; set; } = false;
    }
}
