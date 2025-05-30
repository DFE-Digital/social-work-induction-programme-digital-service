using Dfe.Sww.Ecf.Core.DataStore.Postgres.Models;
using Dfe.Sww.Ecf.Core.Events.Models;

namespace Dfe.Sww.Ecf.Core.Tests;

public class EventInfoTests
{
    [Fact]
    public void EventSerializesCorrectly()
    {
        // Arrange
        var @e = new UserActivatedEvent()
        {
            EventId = Guid.NewGuid(),
            CreatedUtc = DateTime.UtcNow,
            RaisedBy = SystemUser.SystemUserId,
            User = new()
            {
                Email = "test.user@place.com",
                Name = "Test User",
                Roles = ["Administrator"],
                UserId = Guid.NewGuid()
            }
        };

        var eventInfo = EventInfo.Create(@e);

        // Act
        var serialized = eventInfo.Serialize();
        var deserialized = EventInfo.Deserialize(serialized);

        // Assert
        var roundTripped = Assert.IsType<EventInfo<UserActivatedEvent>>(deserialized);
        Assert.Equal(e.CreatedUtc, roundTripped.Event.CreatedUtc);
        Assert.Equal(e.RaisedBy.UserId, roundTripped.Event.RaisedBy.UserId);
        Assert.Equal(e.User.Email, roundTripped.Event.User.Email);
        Assert.Equal(e.User.Name, roundTripped.Event.User.Name);
        Assert.Equal(e.User.Roles, roundTripped.Event.User.Roles);
        Assert.Equal(e.User.UserId, roundTripped.Event.User.UserId);
    }

    [Fact]
    public void EventWithDqtUserIdSerializesRaisedByCorrectly()
    {
        // Arrange
        var @e = new UserActivatedEvent()
        {
            EventId = Guid.NewGuid(),
            CreatedUtc = DateTime.UtcNow,
            RaisedBy = RaisedByUserInfo.FromDqtUser(Guid.NewGuid(), "A DQT User"),
            User = new()
            {
                Email = "test.user@place.com",
                Name = "Test User",
                Roles = ["Administrator"],
                UserId = Guid.NewGuid()
            }
        };

        var eventInfo = EventInfo.Create(@e);

        // Act
        var serialized = eventInfo.Serialize();
        var deserialized = EventInfo.Deserialize(serialized);

        // Assert
        var roundTripped = Assert.IsType<EventInfo<UserActivatedEvent>>(deserialized);
        Assert.Equal(e.RaisedBy.DqtUserId, roundTripped.Event.RaisedBy.DqtUserId);
        Assert.Equal(e.RaisedBy.DqtUserName, roundTripped.Event.RaisedBy.DqtUserName);
    }
}
