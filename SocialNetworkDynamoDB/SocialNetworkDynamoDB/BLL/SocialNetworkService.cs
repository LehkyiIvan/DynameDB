using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; 
using SocialNetworkDynamoDB.DAL;

namespace SocialNetworkDynamoDB.BLL
{
    public class SocialNetworkService
    {
        private readonly DynamoDbService _dynamoDbService;

        public SocialNetworkService(DynamoDbService dynamoDbService)
        {
            _dynamoDbService = dynamoDbService;
        }

        public async Task CreatePost(string postId, string content, string author)
        {
            await _dynamoDbService.CreatePostAsync(postId, content, author, DateTime.UtcNow);
        }

        public async Task CreateComment(string postId, string commentId, string content, string author)
        {
            await _dynamoDbService.CreateCommentAsync(postId, commentId, content, author, DateTime.UtcNow);
        }

        public async Task EditPostOrComment(string postId, string commentId, string newContent)
        {
            await _dynamoDbService.EditItemAsync(postId, commentId, newContent);
        }

        public async Task<List<Dictionary<string, AttributeValue>>> GetComments(string postId)
        {
            return await _dynamoDbService.GetCommentsByPostAsync(postId);
        }
    }
}
