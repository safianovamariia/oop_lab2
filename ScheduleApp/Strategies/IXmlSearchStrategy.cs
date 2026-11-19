using ScheduleApp.Models;

namespace ScheduleApp.Strategies;

public interface IXmlSearchStrategy
{
    List<Session> Search(string xmlPath, SearchCriteria criteria);
}
