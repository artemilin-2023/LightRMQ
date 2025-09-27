namespace LightRMQ.Example.Models;

public class UserRegisteredEvent
{
    public required string Name { get; set; }
    public required string Password { get; set; }
    public int Age { get; set; }
}
