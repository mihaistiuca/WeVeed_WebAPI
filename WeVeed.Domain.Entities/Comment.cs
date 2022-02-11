using System;

namespace WeVeed.Domain.Entities
{
    public class Comment : EntityBase
    {
        public string Text { get; set; }

        public string VideoId { get; set; }

        public string UserId { get; set; }

        public DateTime CommentTime { get; set; }
    }
}
