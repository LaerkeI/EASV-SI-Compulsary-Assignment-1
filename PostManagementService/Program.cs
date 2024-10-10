namespace PostManagementService
{
    using EasyNetQ;
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // In-memory data store for tweets
            var tweets = new List<Tweet> 
            {
                new Tweet { Id = 1, UserId = 1, Content = "Hello World!" }
            };

            // RabbitMQ setup
            var bus = RabbitHutch.CreateBus("host=rabbitmq");

            // Post a new tweet
            app.MapPost("/tweets", (Tweet tweet) => {
                tweets.Add(tweet);
                return Results.Ok(tweet);
            });

            // Like a tweet (send RabbitMQ message to Notification Service)
            app.MapPost("/tweets/{id}/like", (int id, int userId) => {
                var tweet = tweets.FirstOrDefault(t => t.Id == id);

                if (tweet is null)
                    return Results.NotFound();

                // Send message to Notification Service
                bus.PubSub.Publish(new TweetLikedMessage { TweetId = id, LikedByUserId = userId });

                return Results.Ok();
            });

            app.Run();
        }

        public record Tweet
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Content { get; set; }
        }

        public class TweetLikedMessage
        {
            public int TweetId { get; set; }
            public int LikedByUserId { get; set; }
        }
    }
}
