using System;

namespace WeVeed.Domain.Entities
{
    public class View : EntityBase
    {
        public string VideoId { get; set; }

        public string SessionId { get; set; }

        public DateTime ViewTime { get; set; }

        public string SeriesCategory { get; set; }
    }
}
