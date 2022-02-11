
namespace WeVeed.Domain.Entities
{
    // ************************************************************************************
    // ************************************************************************************
    // ************************************************************************************
    // ************************************************************************************
    // ************************************************************************************
    // IF YOU CHANGE SOMETHING HERE, ALSO CHANGE IN WEVEED INTEGRATOR VIDEO.CS, OR THE UPLOAD OF A VIDEO WILL FAIL 

    public class Video : EntityBase
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public string VideoUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        // AFTER ENCODING KEY
        public string EncodedVideoKey { get; set; }



        public string SeriesId { get; set; }

        public string SeriesCategory { get; set; }


        public string UserProducerId { get; set; }

        public decimal Length { get; set; }

        
        public long NumberOfViews { get; set; }

        public int NumberOfLikes { get; set; } // for later - do not use now 


        public bool VideoGotOutOfChannels { get; set; }


        public string ControlbarThumbnailsUrl { get; set; }

        public bool IsProducerValidatedByAdmin { get; set; } = false;
    }
}
