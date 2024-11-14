using LostAndLocate.Chats.Models;
using LostAndLocate.Files.Models;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Users.Models;

namespace LostAndLocate.Test.UnitTests;

public static class DummyData
{
    public static User[] Users { get; } =
    {
        new()
        {
            Name = "user",
            Credentials = new Credentials(),
            Email = "user1@user.de",
            Description = "description1",
            Registration = new DateTime(2022, 1, 1, 11, 11, 0),
            Admin = true
        },
        new()
        {
            Name = "user2",
            Credentials = new Credentials(),
            Email = "user2@user.de",
            Description = "description2",
            Registration = new DateTime(2022, 1, 1, 11, 12, 0)
        },
        new()
        {
            Name = "user3",
            Credentials = new Credentials(),
            Email = "user3@user.de",
            Description = "description3",
            Registration = new DateTime(2022, 1, 1, 11, 13, 0)
        },
        new()
        {
            Name = "user4",
            Credentials = new Credentials(),
            Email = "user4@user.de",
            Description = "description4",
            Registration = new DateTime(2022, 1, 1, 11, 14, 0),
            Admin = true
        }
    };

    public static LostObject[] Objects { get; } =
    {
        new()
        {
            Name = "object",
            Coordinates = new Coordinates
            {
                Latitude = 10,
                Longitude = 20
            },
            User = Users[1],
            Description = "description1",
            Created = new DateTime(2022, 1, 1, 13, 12, 0),
        },
        new()
        {
            Name = "object2",
            Coordinates = new Coordinates
            {
                Latitude = -50,
                Longitude = 90
            },
            User = Users[1],
            Description = "description2",
            Created = new DateTime(2022, 1, 1, 13, 13, 0),
            Returned = true
        },
        new()
        {
            Name = "object",
            Coordinates = new Coordinates
            {
                Latitude = -80,
                Longitude = 120
            },
            User = Users[2],
            Description = "description3",
            Created = new DateTime(2022, 1, 1, 13, 14, 0),
        },
        new()
        {
            Name = "object4",
            Coordinates = new Coordinates
            {
                Latitude = -55,
                Longitude = 5
            },
            User = Users[3],
            Description = "description4",
            Created = new DateTime(2022, 1, 1, 13, 15, 0),
            Returned = true
        }
    };

    public static ChatMessage[] Messages { get; } =
    {
        new()
        {
            Sender = Users[0],
            Target = Users[1],
            Time = new DateTime(2022, 1, 1, 12, 10, 0),
            Message = "message1"
        },
        new()
        {
            Sender = Users[0],
            Target = Users[1],
            Time = new DateTime(2022, 1, 1, 12, 11, 0),
            Message = "message2"
        },
        new()
        {
            Sender = Users[1],
            Target = Users[2],
            Time = new DateTime(2022, 1, 1, 12, 12, 0),
            Message = "message3"
        },
        new()
        {
            Sender = Users[2],
            Target = Users[1],
            Time = new DateTime(2022, 1, 1, 12, 13, 0),
            Message = "message4"
        },
        new()
        {
            Sender = Users[1],
            Target = Users[3],
            Time = new DateTime(2022, 1, 1, 12, 14, 0),
            Message = "message5"
        }
    };

    public static Review[] Reviews { get; } =
    {
        new()
        {
            Sender = Users[0],
            Target = Users[1],
            Stars = 1,
            Description = "review1"
        },
        new()
        {
            Sender = Users[1],
            Target = Users[0],
            Stars = 3,
            Description = "review2"
        },
        new()
        {
            Sender = Users[1],
            Target = Users[2],
            Stars = 2,
            Description = "review3"
        },
        new()
        {
            Sender = Users[2],
            Target = Users[3],
            Stars = 5,
            Description = "review4"
        }
    };

    public static SavedFile[] Files { get; } =
    {
        new()
        {
            Group = "user",
            Name = "test",
            Data = new byte[] { 0x01, 0x02, 0x03 }
        },
        new()
        {
            Group = "user",
            Name = "test2",
            Data = new byte[] { 0x04, 0x05, 0x06, 0x07 }
        },
        new()
        {
            Group = "object",
            Name = "test",
            Data = new byte[] { 0x08, 0x09 }
        }
    };

    public static IEnumerable<object[]> GetUsers() => Users.Select(u => new object[] { u });

    public static IEnumerable<object[]> GetObjects() => Objects.Select(o => new object[] { o });

    public static IEnumerable<object[]> GetMessages() => Messages.Select(m => new object[] { m });

    public static IEnumerable<object[]> GetReviews() => Reviews.Select(r => new object[] { r });

    public static IEnumerable<object[]> GetFiles() => Files.Select(f => new object[] { f });
}