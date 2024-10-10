namespace TimelineService
{
    using System.Net.Http.Json;
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Timeline (get tweets from followed users)
            app.MapGet("/timeline/{userId}", async (int userId, HttpClient httpClient) => {
                // Call User Management Service to get followed users
                var user = await httpClient.GetFromJsonAsync<User>($"http://user-service/users/{userId}");
                if (user == null) return Results.NotFound();

                var followedTweets = new List<Tweet>();

                // Fetch tweets from followed users
                foreach (var followedUserId in user.FollowedUsers)
                {
                    var tweets = await httpClient.GetFromJsonAsync<List<Tweet>>($"http://post-service/tweets?userId={followedUserId}");
                    followedTweets.AddRange(tweets);
                }

                return Results.Ok(followedTweets);
            });

            app.Run();
        }

        public record User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public List<int> FollowedUsers { get; set; }
        }

        public record Tweet
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Content { get; set; }
        }
    }
}
