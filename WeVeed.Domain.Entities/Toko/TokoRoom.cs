
namespace WeVeed.Domain.Entities.Toko
{
    public class TokoRoom : EntityBase
    {
        public string RoomType { get; set; } // opendiscussion | business | drinking 

        public int RoomNumber { get; set; }

        public int NumberOfAttendants { get; set; }
    }
}
