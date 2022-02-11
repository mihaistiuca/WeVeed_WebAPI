using System.Collections.Generic;

namespace WeVeed.Domain.Entities
{
    public static class WeVeedConstants
    {
        //public static string VideosInitialPath = "https://weveedfiles.com/videos";
        public static string VideosInitialPath = "https://weveed-dest-videos.s3-eu-west-1.amazonaws.com/converted";
        

        public static string UserTypeUser = "user";
        public static string UserTypeProducer = "producer";

        public static string GeneralChannel = "general";
        public static string EntertainmentChannel = "entertainment";
        public static string EducationalChannel = "educational";
        public static string NewsChannel = "news";
        public static string TechChannel = "tech";
        public static string TravelChannel = "travel";
        public static string FashionChannel = "fashion";
        public static string SportChannel = "sport";
        public static string KidsChannel = "kids";
        public static string CookingChannel = "cooking";
        public static string AutomotoChannel = "automoto";
        public static string GamingChannel = "gaming";
        public static string MusicChannel = "music";
        public static string VlogChannel = "vlog";

        public static List<string> Categories = new List<string>(new string[] {GeneralChannel, EntertainmentChannel, EducationalChannel, NewsChannel, TechChannel, TravelChannel,
                                                FashionChannel, SportChannel, KidsChannel, CookingChannel, AutomotoChannel, GamingChannel, MusicChannel, VlogChannel });

        public static List<string> CategoriesWithoutGeneral = new List<string>(new string[] {EntertainmentChannel, EducationalChannel, NewsChannel, TechChannel, TravelChannel,
                                                FashionChannel, SportChannel, KidsChannel, CookingChannel, AutomotoChannel, GamingChannel, MusicChannel, VlogChannel });
    }
}
