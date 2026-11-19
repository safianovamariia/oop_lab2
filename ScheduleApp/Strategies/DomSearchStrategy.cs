using System.Xml;
using ScheduleApp.Models;

namespace ScheduleApp.Strategies;

public class DomSearchStrategy : IXmlSearchStrategy
{
    private readonly Dictionary<XmlNode, Room> rooms = new();

    public List<Session> Search(string xmlPath, SearchCriteria criteria)
    {
        var document = new XmlDocument();
        document.Load(xmlPath);
        rooms.Clear();

        var result = new List<Session>();
        XmlNodeList? nodes = document.SelectNodes(BuildXPath(criteria));
        if (nodes == null)
            return result;

        foreach (XmlNode node in nodes)
            result.Add(ToSession(node));
        return result;
    }

    private string BuildXPath(SearchCriteria criteria)
    {
        string roomCondition = "";
        string sessionConditions = "";

        foreach (var filter in criteria.GetFilters())
        {
            if (filter.Key == "room")
                roomCondition = $"[@number='{filter.Value}']";
            else if (filter.Key == "teacher")
                sessionConditions += $"[@teacher='{filter.Value}' or @user='{filter.Value}']";
            else
                sessionConditions += $"[@{filter.Key}='{filter.Value}']";
        }
        return $"/*/Room{roomCondition}/Session{sessionConditions}";
    }

    private Session ToSession(XmlNode node)
    {
        XmlNode roomNode = node.ParentNode!;
        if (!rooms.TryGetValue(roomNode, out Room? room))
        {
            room = Room.FromAttributes(GetAttributes(roomNode));
            rooms[roomNode] = room;
        }
        return Session.Create(room, GetAttributes(node));
    }

    private static Dictionary<string, string> GetAttributes(XmlNode node)
    {
        var attributes = new Dictionary<string, string>();
        foreach (XmlAttribute attribute in node.Attributes!)
            attributes[attribute.Name] = attribute.Value;
        return attributes;
    }
}
