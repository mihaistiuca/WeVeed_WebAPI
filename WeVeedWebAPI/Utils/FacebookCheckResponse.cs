
namespace WeVeedWebAPI.Utils
{
    public class FacebookCheckResponse
    {
        public FacebookCheckResponseData Data { get; set; }
    }

    public class FacebookCheckResponseData
    {
        public string App_Id { get; set; }

        public string Type { get; set; }

        public string Application { get; set; }

        public bool Is_Valid { get; set; }

        public string User_Id { get; set; }
    }
}
