using System.Xml.Linq;
using ScheduleApp.Models;

namespace ScheduleApp.Strategies;

public class LinqSearchStrategy : IXmlSearchStrategy
{
    private readonly Dictionary<XElement, Room> rooms = new();

    public List<Session> Search(string xmlPath, SearchCriteria criteria)
    {
        XDocument document = XDocument.Load(xmlPath);
        rooms.Clear();

        return BuildQuery(document, criteria)
            .Select(ToSession)
            .ToList();
    }

    private IEnumerable<XElement> BuildQuery(XDocument document, SearchCriteria criteria)
    {
        Dictionary<string, string> filters = criteria.GetFilters();

        IEnumerable<XElement> roomElements = document.Root!.Elements("Room");
        if (filters.TryGetValue("room", out string? number))
            roomElements = from r in roomElements
                           where (string?)r.Attribute("number") == number
                           select r;

        IEnumerable<XElement> sessions = roomElements.Elements("Session");
        foreach (var filter in filters.Where(f => f.Key != "room"))
        {
            string name = filter.Key;
            string value = filter.Value;
            if (name == "teacher")
                sessions = from s in sessions
                           where (string?)s.Attribute("teacher") == value || (string?)s.Attribute("user") == value
                           select s;
            else
                sessions = from s in sessions
                           where (string?)s.Attribute(name) == value
                           select s;
        }
        return sessions;
    }

    private Session ToSession(XElement element)
    {
        XElement roomElement = element.Parent!;
        if (!rooms.TryGetValue(roomElement, out Room? room))
        {
            room = Room.FromAttributes(roomElement.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value));
            rooms[roomElement] = room;
        }
        return Session.Create(room, element.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value));
    }
}
