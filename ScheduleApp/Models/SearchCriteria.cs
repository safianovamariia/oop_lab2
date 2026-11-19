namespace ScheduleApp.Models;

public class SearchCriteria
{
    private readonly Dictionary<string, string> filters = new();

    public bool AddFilter(string attribute, string value)
    {
        if (string.IsNullOrWhiteSpace(attribute))
            return false;

        if (string.IsNullOrWhiteSpace(value))
            filters.Remove(attribute);
        else
            filters[attribute] = value;
        return true;
    }

    public Dictionary<string, string> GetFilters() => new(filters);

    public void Clear() => filters.Clear();
}
