using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace CalendarControl;

public class MonthView : AbstractCalendarView
{
    public static readonly DependencyProperty DayNamesProperty = Helper.Register<IEnumerable<CalendarDayNameHeader>, MonthView>(nameof(DayNames));
    public static readonly DependencyProperty CalendarDaysProperty = Helper.Register<IEnumerable<CalendarDay>, MonthView>(nameof(CalendarDays));

    public IEnumerable<CalendarDayNameHeader> DayNames
    {
        get => (IEnumerable<CalendarDayNameHeader>)GetValue(DayNamesProperty);
        private set => SetValue(DayNamesProperty, value);
    }

    public IEnumerable<CalendarDay> CalendarDays
    {
        get => (IEnumerable<CalendarDay>)GetValue(CalendarDaysProperty);
        private set => SetValue(CalendarDaysProperty, value);
    }

    static MonthView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthView), new FrameworkPropertyMetadata(typeof(MonthView)));
    }

    public MonthView()
    {
        DateTimeFormatInfo dtf = new CultureInfo("it-IT").DateTimeFormat;
        string[] days = Enumerable.Range(1, 7)
                                  .Select(i => dtf.GetDayName((DayOfWeek)(i % 7)).ToUpper())
                                  .ToArray();

        DayNames =
        [
            new() { Name = days[0] },
            new() { Name = days[1] },
            new() { Name = days[2] },
            new() { Name = days[3] },
            new() { Name = days[4] },
            new() { Name = days[5], IsFestive = true },
            new() { Name = days[6], IsFestive = true }
        ];
    }

    private void DistributeEvents()
    {
        if (Events is not null && CalendarDays is not null)
        {
            CalendarDays = [.. CalendarDays];

            foreach (CalendarDay calendarDay in CalendarDays)
            {
                IEnumerable<IDatable> temp = Events.Where(s => s.DateOf == calendarDay.Date);
                calendarDay.Events = [.. temp];
            }
        }
    }

    private static List<CalendarDay> Calculate(int year, int month)
    {
        List<CalendarDay> days = [];
        DateTime startDate = new(year, month, 1);
        int totalDays = DateTime.DaysInMonth(startDate.Year, startDate.Month);
        DateTime firstDayOfMonth = new(startDate.Year, startDate.Month, 1);
        DateTime lastDayOfMonth = new(startDate.Year, startDate.Month, totalDays);
        DateTime prevMonth = startDate.AddMonths(-1);
        DateTime nextMonth = startDate.AddMonths(1);

        int daysToAddBefore = (int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday;
        if (daysToAddBefore < 0)
            daysToAddBefore += 7;

        int daysToAddAfter = 6 - (int)lastDayOfMonth.DayOfWeek + (int)DayOfWeek.Monday;
        if (daysToAddAfter >= 7)
            daysToAddAfter -= 7;

        int prevMonthDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        for (int i = prevMonthDays - daysToAddBefore + 1; i <= prevMonthDays; i++)
        {
            DateTime date = new(prevMonth.Year, prevMonth.Month, i);
            days.Add(new CalendarDay() { IsActive = false, Date = date });
        }

        // Current month days
        for (int i = 1; i <= totalDays; i++)
        {
            DateTime date = new(startDate.Year, startDate.Month, i);
            days.Add(new CalendarDay() { IsActive = true, Date = date });
        }

        // Next month days
        for (int i = 1; i <= daysToAddAfter; i++)
        {
            DateTime date = new(nextMonth.Year, nextMonth.Month, i);
            days.Add(new CalendarDay() { IsActive = false, Date = date });
        }

        return days;
    }

    protected override void OnEventsChanged()
    {
        DistributeEvents();
    }

    protected override void OnDateChanged()
    {
        if (CalendarDays != null)
        {
            foreach (var item in CalendarDays)
                item.Dispose();
        }

        CalendarDays = Calculate(Date.Year, Date.Month);
        DistributeEvents();
    }
}

public class CalendarDayNameHeader
{
    public string Name { get; set; } = string.Empty;
    public bool IsFestive { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

public class CalendarDay : IDisposable, INotifyPropertyChanged
{
    public DateTime Date { get; set; }
    public bool IsToday => Date.Date == DateTime.Today;
    public bool IsActive { get; set; }
    public bool IsFestive => IsFixedHoliday || IsEasterDate() || Date.DayOfWeek == DayOfWeek.Sunday || Date.DayOfWeek == DayOfWeek.Saturday;
    public object? SelectedEvent
    {
        get; set;
    }

    public ObservableCollection<IDatable> Events { get; set; } = [];
    public int Day => Date.Day;
    public string DayName => Date.DayOfWeek.ToString();
    private int Year => Date.Year;
    private bool IsFixedHoliday =>
    (Date.Month == 1 && Date.Day == 1) ||   // New Year's Day
    (Date.Month == 12 && Date.Day == 25);   // Christmas

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string propName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public void Dispose()
    {
        Events = [];
        GC.SuppressFinalize(this);
    }

    private bool IsEasterDate()
    {
        int a = Year % 19;
        int b = Year / 100;
        int c = Year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;
        return new DateTime(Year, month, day) == Date;
    }
}