using System;
using System.Windows;
using SocialNetworkDynamoDB.DAL;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using SocialNetworkDynamoDB.BLL;
using MongoDB.Driver;

namespace SocialNetworkDynamoDB
{
    public partial class MainWindow : Window
    {
        private readonly DynamoDbService _dynamoDbService;
        private readonly SocialNetworkService _socialNetworkService;
        private readonly DataMigrationService _dataMigrationService;

        public MainWindow()
        {
            InitializeComponent();

            string accessKey = "AKIAUMYCITP4ICAC45UM"; 
            string secretKey = "y9zPEusTQyjebEpZaHftCkJ/HXZzMKNv/p8AYY3"; 

            _dynamoDbService = new DynamoDbService(accessKey, secretKey);

            _socialNetworkService = new SocialNetworkService(_dynamoDbService);

            var mongoClient = new MongoClient("mongodb://localhost:27017"); 
            var mongoDatabase = mongoClient.GetDatabase("socialNetwork"); 

            // Створюємо сервіс для міграції
            _dataMigrationService = new DataMigrationService(mongoDatabase, _dynamoDbService);
        }

        // Метод для додавання коментаря
        private async void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            string postId = "Post123"; 
            string commentId = "Comment" + Guid.NewGuid().ToString(); 
            string content = CommentContentTextBox.Text;
            string author = CommentAuthorTextBox.Text;

            try
            {
                await _dynamoDbService.CreateCommentAsync(postId, commentId, content, author, DateTime.UtcNow);
                MessageBox.Show("Comment added successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding comment: " + ex.Message);
            }
        }

        // Метод для завантаження коментарів
        private async void LoadCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            string postId = "Post123"; 
            try
            {
                var comments = await _socialNetworkService.GetComments(postId);

                if (comments.Count > 0)
                {
                    CommentsListBox.Items.Clear(); 
                    foreach (var comment in comments)
                    {
                        CommentsListBox.Items.Add(comment["Content"].S); 
                    }
                }
                else
                {
                    MessageBox.Show("No comments found for this post.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading comments: " + ex.Message);
            }
        }

        // Метод для міграції даних з MongoDB до DynamoDB
        private async void MigrateDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Викликаємо міграцію даних з MongoDB до DynamoDB
                await _dataMigrationService.MigrateCommentsToDynamoDbAsync();
                MessageBox.Show("Data migration completed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during data migration: " + ex.Message);
            }
        }
    }
}
