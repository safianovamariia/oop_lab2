using System.Xml.Linq;
using System.Xml.Xsl;
using ScheduleApp.Models;
using ScheduleApp.Strategies;

namespace ScheduleApp;

public class ScheduleController
{
    private static readonly string[] WeekDays =
        { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    private string xmlFilePath = "";
    private string htmlFilePath = "";
    private IXmlSearchStrategy? strategy;
    private readonly SearchCriteria criteria = new();

    public Dictionary<string, List<string>> OpenFile(string path)
    {
        XDocument document = XDocument.Load(path);
        XElement root = document.Root ?? throw new InvalidOperationException("The XML file is empty.");

        var schedule = new Schedule();
        foreach (XElement roomElement in root.Elements("Room"))
        {
            Room room = Room.FromAttributes(ToDictionary(roomElement));
            foreach (XElement sessionElement in roomElement.Elements("Session"))
                Session.Create(room, ToDictionary(sessionElement));
            schedule.GetRooms().Add(room);
        }

        xmlFilePath = path;
        criteria.Clear();

        List<Session> sessions = schedule.GetRooms().SelectMany(r => r.GetSessions()).ToList();
        return new Dictionary<string, List<string>>
        {
            ["room"] = schedule.GetRooms().Select(r => r.Number).Distinct().OrderBy(n => n).ToList(),
            ["day"] = sessions.Select(s => s.GetAttribute("day")).Distinct().OrderBy(DayOrder).ToList(),
            ["teacher"] = ValuesOf(sessions.Where(s => s is Lesson).ToList(), "teacher"),
            ["group"] = ValuesOf(sessions, "group"),
            ["subject"] = ValuesOf(sessions, "subject")
        };
    }

    private static List<string> ValuesOf(List<Session> sessions, string attribute) =>
        sessions.Select(s => s.GetAttribute(attribute))
                .Where(v => v != "")
                .Distinct()
                .OrderBy(v => v)
                .ToList();

    public bool AddFilter(string attribute, string value)
    {
        return criteria.AddFilter(attribute, value);
    }

    public bool SetStrategy(string strategyName)
    {
        strategy = strategyName switch
        {
            "SAX API" => new SaxSearchStrategy(),
            "DOM API" => new DomSearchStrategy(),
            "LINQ to XML" => new LinqSearchStrategy(),
            _ => null
        };
        return strategy != null;
    }

    public List<Session> Search()
    {
        if (xmlFilePath == "")
            throw new InvalidOperationException("Open an XML file first.");
        if (strategy == null)
            throw new InvalidOperationException("Choose the analysis method: SAX API, DOM API or LINQ to XML.");

        return strategy.Search(xmlFilePath, criteria);
    }

    public string HtmlFilePath => htmlFilePath;

    public bool TransformToHtml(string xslPath, string htmlPath)
    {
        if (xmlFilePath == "")
            throw new InvalidOperationException("Open an XML file first.");

        string xslFullPath = Path.Combine(AppContext.BaseDirectory, xslPath);
        string htmlFullPath = Path.Combine(Path.GetDirectoryName(xmlFilePath) ?? "", htmlPath);

        var transform = new XslCompiledTransform();
        transform.Load(xslFullPath);
        transform.Transform(xmlFilePath, htmlFullPath);

        htmlFilePath = htmlFullPath;
        return File.Exists(htmlFullPath);
    }

    public void Clear()
    {
        criteria.Clear();
        strategy = null;
    }

    private static int DayOrder(string day)
    {
        int index = Array.IndexOf(WeekDays, day);
        return index < 0 ? int.MaxValue : index;
    }

    private static Dictionary<string, string> ToDictionary(XElement element) =>
        element.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value);
}
