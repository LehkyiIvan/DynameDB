using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetworkDynamoDB.DAL
{
    public class DynamoDbService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private const string TableName = "PostsAndComments"; // Назва вашої таблиці в DynamoDB

        public DynamoDbService(string accessKey, string secretKey)
        {
            var credentials = new StoredProfileAWSCredentials("Lehkyi"); // Використовуємо профіль "Lehkyi", якщо він налаштований
            _dynamoDbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName("eu-north-1"));
        }

        // Створення поста
        public async Task CreatePostAsync(string postId, string content, string author, DateTime createdDateTime)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
        {
            { "PostID", new AttributeValue { S = postId } },
            { "CommentID", new AttributeValue { S = postId } },  // Для поста CommentID дорівнює PostID
            { "Content", new AttributeValue { S = content } },
            { "Author", new AttributeValue { S = author } },
            { "CreatedDateTime", new AttributeValue { S = createdDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") } },
            { "ModifiedDateTime", new AttributeValue { S = createdDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") } }
        }
            };

            try
            {
                await _dynamoDbClient.PutItemAsync(request);
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine($"Помилка при додаванні елемента: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Загальна помилка: {e.Message}");
            }
        }

        // Створення коментаря
        public async Task CreateCommentAsync(string postId, string commentId, string content, string author, DateTime createdDateTime)
        {
            var request = new PutItemRequest
            {
                TableName = TableName,
                Item = new Dictionary<string, AttributeValue>
        {
            { "PostID", new AttributeValue { S = postId } },
            { "CommentID", new AttributeValue { S = commentId } },
            { "Content", new AttributeValue { S = content } },
            { "Author", new AttributeValue { S = author } },
            { "CreatedDateTime", new AttributeValue { S = createdDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") } },
            { "ModifiedDateTime", new AttributeValue { S = createdDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'") } }
        }
            };

            try
            {
                await _dynamoDbClient.PutItemAsync(request);
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine($"Помилка при додаванні коментаря: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Загальна помилка: {e.Message}");
            }
        }

        // Метод для редагування поста чи коментаря
        public async Task EditItemAsync(string postId, string commentId, string newContent)
        {
            var request = new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
        {
            { "PostID", new AttributeValue { S = postId } },
            { "CommentID", new AttributeValue { S = commentId } }
        },
                UpdateExpression = "SET Content = :newContent, ModifiedDateTime = :modifiedDateTime",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":newContent", new AttributeValue { S = newContent } },
            { ":modifiedDateTime", new AttributeValue { S = DateTime.UtcNow.ToString("o") } }
        }
            };

            try
            {
                await _dynamoDbClient.UpdateItemAsync(request);
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine($"Помилка при оновленні елемента: {e.Message}");
            }
        }

        // Метод для отримання коментарів за ID поста (відсортовано по ModifiedDateTime)
        public async Task<List<Dictionary<string, AttributeValue>>> GetCommentsByPostAsync(string postId)
        {
            var request = new QueryRequest
            {
                TableName = TableName,
                IndexName = "CommentsByModifiedDateTime",  // Назва GSI (якщо така є)
                KeyConditionExpression = "PostID = :postId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":postId", new AttributeValue { S = postId } }
        },
                ScanIndexForward = true // true для сортування по ModifiedDateTime в порядку зростання
            };

            try
            {
                var response = await _dynamoDbClient.QueryAsync(request);
                return response.Items;
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine($"Помилка при отриманні коментарів: {e.Message}");
                return null;
            }
        }
    }
}

