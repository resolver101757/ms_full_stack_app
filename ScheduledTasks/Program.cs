using System;
using System.Data.SqlClient;
using Topshelf;
using Dapper; // Added this line

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
    private readonly string _connectionString = "Server=db; Database=UserDB; User Id=sa; Password=YourStrong@Passw0rd;Encrypt=True;TrustServerCertificate=True;";

    public void Start()
    {
        _timer = new Timer(CreateUser, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
    }

    public void Stop()
    {
        _timer?.Dispose();
    }

    private void CreateUser(object state)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                // Use a simple string for password
                string password = "DemoPassword123!";

                var userId = connection.ExecuteScalar<int>(@"
                    INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
                    OUTPUT INSERTED.Id
                    VALUES (@Username, @Email, @PasswordHash, @CreatedAt)",
                    new
                    {
                        Username = $"User_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                        Email = $"user_{Guid.NewGuid().ToString("N").Substring(0, 8)}@example.com",
                        PasswordHash = password, // Use the plain password string
                        CreatedAt = DateTime.UtcNow
                    });

            }
        }
        catch (Exception ex)
        {
            // Enhance error logging
            Console.WriteLine($"Error creating user: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}