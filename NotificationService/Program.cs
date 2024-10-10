namespace NotificationService
{
    using EasyNetQ;
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // RabbitMQ setup
            var bus = RabbitHutch.CreateBus("host=rabbitmq");

            // Subscribe to messages from PostManagement (tweets liked)
            bus.PubSub.Subscribe<TweetLikedMessage>("notification-service", message => {
                Console.WriteLine($"Tweet {message.TweetId} was liked by user {message.LikedByUserId}");
            });

            app.Run();

        }

        public class TweetLikedMessage
        {
            public int TweetId { get; set; }
            public int LikedByUserId { get; set; }
        }
    }
}
