using System;

namespace WeVeed.Application.Dtos
{
    public class CommentDisplayUiDto
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string UserId { get; set; }

        public DateTime CommentTime { get; set; }


        public string UserName { get; set; }

        public string UserProfileImageUrl { get; set; }

        public bool UserIsProducer { get; set; }
    }
}
