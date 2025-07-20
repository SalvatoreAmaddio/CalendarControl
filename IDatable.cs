namespace CalendarControl;

public interface IDatable
{
    int Id { get; set; }
    DateTime DateOf { get; set; }
    TimeSpan StartTime { get; set; }
    TimeSpan EndTime { get; set; }
    string Title { get; set; }
    string Location { get; set; }
}