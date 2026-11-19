namespace ScheduleApp.Models;

public class Lesson : Session
{
    private readonly string subject;
    private readonly string teacher;
    private readonly string group;

    public Lesson(Room room, string day, string time, string subject, string teacher, string group)
        : base(room, day, time)
    {
        this.subject = subject;
        this.teacher = teacher;
        this.group = group;
    }

    public override string GetAttribute(string name)
    {
        return name switch
        {
            "subject" => subject,
            "teacher" => teacher,
            "group" => group,
            _ => base.GetAttribute(name)
        };
    }

    public override string GetInfo() =>
        $"{Header()} | Lesson: {subject}, teacher {teacher}, group {group}";
}
