using System;
using System.Net.Http;
using System.Net.Http.Json;
using Topshelf;

class Program
{
    static void Main(string[] args)
    {
        HostFactory.Run(x =>
        {
            x.Service<UserCreationService>(s =>
            {
                s.ConstructUsing(name => new UserCreationService());
                s.WhenStarted(tc => tc.Start());
                s.WhenStopped(tc => tc.Stop());
            });
            x.RunAsLocalSystem();

            x.SetServiceName("UserCreationService");
            x.SetDisplayName("User Creation Service");
            x.SetDescription("Creates a pseudo-random user every 2 minutes");
        });
    }
}

public class UserCreationService
{
    private Timer _timer;
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "http://backend:5000/api/user";

    public UserCreationService()
    {
        _httpClient = new HttpClient();
    }

    public void Start()
    {
        _timer = new Timer(CreateUser, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
    }

    public void Stop()
    {
        _timer?.Dispose();
        _httpClient.Dispose();
    }

    private async void CreateUser(object state)
    {
        try
        {
            var newUser = new
            {
                Username = $"User_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Email = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                Password = "DemoPassword123!"
            };

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, newUser);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"User created: {newUser.Username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}