using System.Xml;
using ScheduleApp.Models;

namespace ScheduleApp.Strategies;

public class SaxSearchStrategy : IXmlSearchStrategy
{
    private Room? currentRoom;

    public List<Session> Search(string xmlPath, SearchCriteria criteria)
    {
        var result = new List<Session>();
        Dictionary<string, string> filters = criteria.GetFilters();

        using XmlReader reader = XmlReader.Create(xmlPath);
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
                continue;

            if (reader.Name == "Room")
            {
                currentRoom = Room.FromAttributes(ReadAttributes(reader));
            }
            else if (reader.Name == "Session" && currentRoom != null)
            {
                Session session = ReadSession(reader);
                if (filters.All(f => session.GetAttribute(f.Key) == f.Value))
                    result.Add(session);
            }
        }
        return result;
    }

    private Session ReadSession(XmlReader reader)
    {
        return Session.Create(currentRoom!, ReadAttributes(reader));
    }

    private static Dictionary<string, string> ReadAttributes(XmlReader reader)
    {
        var attributes = new Dictionary<string, string>();
        if (reader.MoveToFirstAttribute())
        {
            do
            {
                attributes[reader.Name] = reader.Value;
            } while (reader.MoveToNextAttribute());
            reader.MoveToElement();
        }
        return attributes;
    }
}
