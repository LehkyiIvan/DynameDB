using System;

namespace SocialNetworkDynamoDB.BLL
{
    public class Comment
    {
        public string _id { get; set; } 
        public string commentId { get; set; }  
        public string postId { get; set; }  
        public string userId { get; set; }  
        public string content { get; set; }  
        public DateTime createdAt { get; set; }  
    }
}
