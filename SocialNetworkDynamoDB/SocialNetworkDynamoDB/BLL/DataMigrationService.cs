using MongoDB.Driver;
using SocialNetworkDynamoDB.BLL;
using SocialNetworkDynamoDB.DAL;
using System.Threading.Tasks;

public class DataMigrationService
{
    private readonly IMongoCollection<Comment> _mongoCommentsCollection;
    private readonly DynamoDbService _dynamoDbService;

    public DataMigrationService(IMongoDatabase mongoDatabase, DynamoDbService dynamoDbService)
    {
        _mongoCommentsCollection = mongoDatabase.GetCollection<Comment>("comments");
        _dynamoDbService = dynamoDbService;
    }

    public async Task MigrateCommentsToDynamoDbAsync()
    {
        var mongoComments = await _mongoCommentsCollection.Find(_ => true).ToListAsync();

        foreach (var comment in mongoComments)
        {
            // Переносимо дані в DynamoDB
            await _dynamoDbService.CreateCommentAsync(
                comment.postId,
                comment.commentId,
                comment.content,
                comment.userId,
                comment.createdAt
            );
        }
    }
}
