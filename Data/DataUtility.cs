using ContactHarbor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ContactHarbor.Data;

public static class DataUtility
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
    }

    private static string BuildConnectionString(string databaseUrl)
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Disable,
            TrustServerCertificate = true
        };
        return builder.ToString();
    }


    public static async Task ManageDataAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var dbContextService = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManagerService = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        await dbContextService.Database.MigrateAsync();
		
		Console.WriteLine("Starting to seed demo user...");
		await SeedDemoUserAsync(userManagerService, configuration);
		Console.WriteLine("Demo user seeding completed.");

		Console.WriteLine("Starting to seed demo user data...");
		await SeedDemoUserDataAsync(dbContextService, userManagerService, webHostEnvironment);
		Console.WriteLine("Demo user data seeding completed.");
	}

    private static async Task SeedDemoUserAsync(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        try
        {
			AppUser demoUser = new AppUser()
			{
				UserName = "demo@contactharbor.com",
				Email = "demo@contactharbor.com",
				FirstName = "Demo",
				LastName = "User",
				EmailConfirmed = true
			};

			await userManager.CreateAsync(demoUser, configuration["DemoUserPassword"] ?? Environment.GetEnvironmentVariable("DEMO_USER_PASSWORD")!);
        }
        catch(Exception ex)
        {
			Console.WriteLine($"Unable to seed the demo user account. {ex.Message}");
        }
    }

    public static async Task SeedDemoUserDataAsync(ApplicationDbContext dbContext, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        var demoUser = await userManager.FindByEmailAsync("demo@contactharbor.com");


        if (demoUser is not null)
        {
			List<Category> categoryList = await dbContext.Categories.Where(c => c.AppUserId == demoUser.Id).ToListAsync();

			if (!categoryList.Any())
            {
			    var categories = new List<Category>
			    {
				    new Category { Id = Guid.NewGuid(), Name = "Friends", AppUser = demoUser },
				    new Category { Id = Guid.NewGuid(), Name = "Family", AppUser = demoUser },
				    new Category { Id = Guid.NewGuid(), Name = "Baseball Team", AppUser = demoUser },
				    new Category { Id = Guid.NewGuid(), Name = "Co-Worker", AppUser = demoUser },
				    new Category { Id = Guid.NewGuid(), Name = "Neighbors", AppUser = demoUser }
			    };

				await dbContext.Categories.AddRangeAsync(categories);
				await dbContext.SaveChangesAsync();
				categoryList = await dbContext.Categories.Where(c => c.AppUserId == demoUser.Id).ToListAsync();
		    }
            
            if (!await dbContext.Contacts.AnyAsync(c => c.AppUserId == demoUser.Id))
            {
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

				var random = new Random();
				DateTime RandomDayFunc() => new DateTime(random.Next(1950, 2002), random.Next(1, 13), random.Next(1, 29));


				List<Contact> contacts = new List<Contact>
			{
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "John",
					LastName = "Doe",
					Email = "john.doe@contactharbor.com",
					PhoneNumber = "1234567890",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "123 Main St",
					City = "Anytown",
					State = "FL",
					ZipCode = "12345",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M1.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[0]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "James",
					LastName = "Smith",
					Email = "james.smith@contactharbor.com",
					PhoneNumber = "2345678901",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "456 Maple Dr",
					City = "Sometown",
					State = "AL",
					ZipCode = "23456",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M2.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[1], categoryList[2] }
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Robert",
					LastName = "Johnson",
					Email = "robert.johnson@contactharbor.com",
					PhoneNumber = "3456789012",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "789 Pine Ave",
					City = "Otherplace",
					State = "PA",
					ZipCode = "34567",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M3.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[3]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Michael",
					LastName = "Williams",
					Email = "michael.williams@contactharbor.com",
					PhoneNumber = "4567890123",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "1011 Birch Blvd",
					City = "Cityplace",
					State = "WI",
					ZipCode = "45678",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M4.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[4]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "William",
					LastName = "Brown",
					Email = "william.brown@contactharbor.com",
					PhoneNumber = "5678901234",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "1213 Cedar Ct",
					City = "Townville",
					State = "MD",
					ZipCode = "56789",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M5.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[4]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "David",
					LastName = "Jones",
					Email = "david.jones@contactharbor.com",
					PhoneNumber = "6789012345",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "1415 Dogwood Dr",
					City = "Burgplace",
					State = "MV",
					ZipCode = "67890",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M6.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[4], categoryList[3] }
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Richard",
					LastName = "Miller",
					Email = "richard.miller@contactharbor.com",
					PhoneNumber = "7890123456",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "1617 Elm St",
					City = "Villeshire",
					State = "OH",
					ZipCode = "78901",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M7.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[3]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Charles",
					LastName = "Davis",
					Email = "charles.davis@contactharbor.com",
					PhoneNumber = "8901234567",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "1819 Fir Ave",
					City = "Hamletburg",
					State = "KY",
					ZipCode = "89012",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M8.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[2]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Joseph",
					LastName = "Garcia",
					Email = "joseph.garcia@contactharbor.com",
					PhoneNumber = "9012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "2021 Grove Blvd",
					City = "Metropolis",
					State = "WA",
					ZipCode = "90123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "M9.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[1]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Mary",
					LastName = "Smith",
					Email = "mary.smith@contactharbor.com",
					PhoneNumber = "9012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "222 Main St",
					City = "Anytown",
					State = "MO",
					ZipCode = "90123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F1.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Jennifer",
					LastName = "Johnson",
					Email = "jennifer.johnson@contactharbor.com",
					PhoneNumber = "8012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "333 Maple Dr",
					City = "Sometown",
					State = "MI",
					ZipCode = "80123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F2.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Linda",
					LastName = "Williams",
					Email = "linda.williams@contactharbor.com",
					PhoneNumber = "7012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "444 Pine Ave",
					City = "Otherplace",
					State = "NY",
					ZipCode = "70123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F3.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[4]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Patricia",
					LastName = "Brown",
					Email = "patricia.brown@contactharbor.com",
					PhoneNumber = "6012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "555 Birch Blvd",
					City = "Cityplace",
					State = "NJ",
					ZipCode = "60123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F4.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[2]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Elizabeth",
					LastName = "Jones",
					Email = "elizabeth.jones@contactharbor.com",
					PhoneNumber = "5012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "666 Cedar Ct",
					City = "Townville",
					State = "NC",
					ZipCode = "50123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F5.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[3]}
				},
				new Contact
				{
					Id = Guid.NewGuid(),
					FirstName = "Susan",
					LastName = "Miller",
					Email = "susan.miller@contactharbor.com",
					PhoneNumber = "4012345678",
					DateOfBirth = RandomDayFunc().ToUniversalTime(),
					Address1 = "777 Dogwood Dr",
					City = "Burgplace",
					State = "SC",
					ZipCode = "40123",
					ImageData = await File.ReadAllBytesAsync(Path.Combine(webHostEnvironment.WebRootPath, "img/demo", "F6.jpg")),
					ImageType = "image/jpeg",
					Created = DateTimeOffset.UtcNow,
					AppUserId = demoUser!.Id,
					Categories = new List<Category> { categoryList[1]}
				}
			};

				await dbContext.Contacts.AddRangeAsync(contacts);
				await dbContext.SaveChangesAsync();
			}
        }
    }
}
