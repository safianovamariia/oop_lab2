namespace ScheduleApp.Models;

public class Schedule
{
    private readonly List<Room> rooms = new();

    public List<Room> GetRooms() => rooms;
}
