using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Resources;

namespace CalendarControl;

public class EventCalendarDateSetter : Control
{
    public static readonly DependencyProperty IsWeekViewProperty = Helper.Register<bool, EventCalendarDateSetter>(nameof(IsWeekView), false, OnIsToggledPropertyChanged);
    public static readonly DependencyProperty TodayContentProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(TodayContent));
    public static readonly DependencyProperty WeekStringProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(WeekString));
    public static readonly DependencyProperty WeekViewTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(WeekViewTooltip), string.Empty);
    public static readonly DependencyProperty PreviousButtonTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(PreviousButtonTooltip));
    public static readonly DependencyProperty NextButtonTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(NextButtonTooltip));
    public static readonly DependencyProperty NextYearTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(NextYearTooltip));
    public static readonly DependencyProperty PreviousYearTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(PreviousYearTooltip));
    public static readonly DependencyProperty DateVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(DateVisibility), Visibility.Collapsed);
    public static readonly DependencyProperty YearVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(YearVisibility), Visibility.Visible);
    public static readonly DependencyProperty MonthVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(MonthVisibility), Visibility.Visible);
    public static readonly DependencyProperty TodayCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(TodayCommand));
    public static readonly DependencyProperty PreviousYearCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(PreviousYearCommand));
    public static readonly DependencyProperty PreviousMonthCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(PreviousMonthCommand));
    public static readonly DependencyProperty NextYearCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(NextYearCommand));
    public static readonly DependencyProperty NextMonthCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(NextMonthCommand));
    public static readonly DependencyProperty DateProperty = Helper.Register<DateTime, EventCalendarDateSetter>(nameof(Date), DateTime.Now);
    public static readonly DependencyProperty YearProperty = Helper.Register<int, EventCalendarDateSetter>(nameof(Year), DateTime.Now.Year, (s, e) => ((EventCalendarDateSetter)s).RefreshDate());
    public static readonly DependencyProperty MonthsProperty = Helper.Register<IEnumerable<string>, EventCalendarDateSetter>(nameof(Months));

    public static readonly DependencyProperty SelectedMonthIndexProperty =
    Helper.Register<int, EventCalendarDateSetter>(nameof(SelectedMonthIndex), DateTime.Now.Month - 1, (s, e) => ((EventCalendarDateSetter)s).RefreshDate());

    public static readonly DependencyProperty CultureProperty =
    Helper.Register<CultureInfo, EventCalendarDateSetter>(nameof(Culture), CultureInfo.CurrentUICulture, OnCultureChanged);

    private readonly ResourceManager rm = new("CalendarControl.Resources.Strings", typeof(EventCalendarDateSetter).Assembly);

    public CultureInfo Culture
    {
        get => (CultureInfo)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    public string TodayContent
    {
        get => (string)GetValue(TodayContentProperty);
        set => SetValue(TodayContentProperty, value);
    }
    
    public string WeekString
    {
        get => (string)GetValue(WeekStringProperty);
        set => SetValue(WeekStringProperty, value);
    }

    public string WeekViewTooltip
    {
        get => (string)GetValue(WeekViewTooltipProperty);
        set => SetValue(WeekViewTooltipProperty, value);
    }

    public string PreviousYearTooltip
    {
        get => (string)GetValue(PreviousYearTooltipProperty);
        set => SetValue(PreviousYearTooltipProperty, value);
    }

    public string NextYearTooltip
    {
        get => (string)GetValue(NextYearTooltipProperty);
        set => SetValue(NextYearTooltipProperty, value);
    }

    public string PreviousButtonTooltip
    {
        get => (string)GetValue(PreviousButtonTooltipProperty);
        set => SetValue(PreviousButtonTooltipProperty, value);
    }

    public string NextButtonTooltip
    {
        get => (string)GetValue(NextButtonTooltipProperty);
        set => SetValue(NextButtonTooltipProperty, value);
    }

    public Visibility DateVisibility
    {
        get => (Visibility)GetValue(DateVisibilityProperty);
        set => SetValue(DateVisibilityProperty, value);
    }

    public Visibility YearVisibility
    {
        get => (Visibility)GetValue(YearVisibilityProperty);
        set => SetValue(YearVisibilityProperty, value);
    }

    public Visibility MonthVisibility
    {
        get => (Visibility)GetValue(MonthVisibilityProperty);
        set => SetValue(MonthVisibilityProperty, value);
    }

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EventCalendarDateSetter control)
        {
            control.Months = control.Culture.DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .ToArray();

            control.UpdateLocalizedStrings();
        }
    }

    private void UpdateLocalizedStrings()
    {
        PreviousYearTooltip = rm.GetString("PreviousYear", Culture) ?? "Error";
        NextYearTooltip = rm.GetString("NextYear", Culture) ?? "Error";

        if (IsWeekView)
        {
            TodayContent = rm.GetString("TodayContentWeek", Culture) ?? "Error";
            PreviousButtonTooltip = rm.GetString("PreviousWeek", Culture) ?? "Error";
            NextButtonTooltip = rm.GetString("NextWeek", Culture) ?? "Error";
            WeekViewTooltip = rm.GetString("WeekView", Culture) ?? "Error";
            WeekString = rm.GetString("Week", Culture) ?? "Error";

            DateTime firstDayOfMonth = new(Date.Year, Date.Month, 1);

            int diff = (int)firstDayOfMonth.DayOfWeek;
            int daysToSubtract = diff == 0 ? 6 : diff - 1;
            DateTime mondayOfFirstWeek = firstDayOfMonth.AddDays(-daysToSubtract);

            // Only set it if current Date is not already in the same week
            if (GetMonday(Date) != mondayOfFirstWeek)
            {
                SetCurrentValue(DateProperty, mondayOfFirstWeek);
            }
        }
        else
        {
            TodayContent = rm.GetString("TodayContentMonth", Culture) ?? "Error";
            PreviousButtonTooltip = rm.GetString("PreviousMonth", Culture) ?? "Error";
            NextButtonTooltip = rm.GetString("NextMonth", Culture) ?? "Error";
            WeekViewTooltip = rm.GetString("WeekView", Culture) ?? "Error";
            SetCurrentValue(DateProperty, new DateTime(Date.Year, Date.Month, 1));
        }
    }

    private static DateTime GetMonday(DateTime date)
    {
        int diff = (int)date.DayOfWeek;
        int daysToSubtract = diff == 0 ? 6 : diff - 1;
        return date.AddDays(-daysToSubtract).Date;
    }

    private static void OnIsToggledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EventCalendarDateSetter control && e.NewValue is bool isToggled)
        {
            if (isToggled)
            {
                control.MonthVisibility = Visibility.Collapsed;
                control.YearVisibility = Visibility.Collapsed;
                control.DateVisibility = Visibility.Visible;
            }
            else
            {
                control.MonthVisibility = Visibility.Visible;
                control.YearVisibility = Visibility.Visible;
                control.DateVisibility = Visibility.Collapsed;

                if (control.SelectedMonthIndex != control.Date.Month - 1)
                {
                    control.SelectedMonthIndex = control.Date.Month - 1;
                }
                if (control.Year != control.Date.Year)
                {
                    control.Year = control.Date.Year;
                }
            }

            control.UpdateLocalizedStrings();
        }
    }

    public bool IsWeekView
    {
        get => (bool)GetValue(IsWeekViewProperty);
        set => SetValue(IsWeekViewProperty, value);
    }

    public ICommand TodayCommand
    {
        get => (ICommand)GetValue(TodayCommandProperty);
        private set => SetValue(TodayCommandProperty, value);
    }

    public ICommand PreviousYearCommand
    {
        get => (ICommand)GetValue(PreviousYearCommandProperty);
        private set => SetValue(PreviousYearCommandProperty, value);
    }

    public ICommand PreviousMonthCommand
    {
        get => (ICommand)GetValue(PreviousMonthCommandProperty);
        private set => SetValue(PreviousMonthCommandProperty, value);
    }

    public ICommand NextYearCommand
    {
        get => (ICommand)GetValue(NextYearCommandProperty);
        private set => SetValue(NextYearCommandProperty, value);
    }

    public ICommand NextMonthCommand
    {
        get => (ICommand)GetValue(NextMonthCommandProperty);
        private set => SetValue(NextMonthCommandProperty, value);
    }

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public int SelectedMonthIndex
    {
        get => (int)GetValue(SelectedMonthIndexProperty);
        set => SetValue(SelectedMonthIndexProperty, value);
    }

    public int Year
    {
        get => (int)GetValue(YearProperty);
        set => SetValue(YearProperty, value);
    }

    public IEnumerable<string> Months
    {
        get => (IEnumerable<string>)GetValue(MonthsProperty);
        private set => SetValue(MonthsProperty, value);
    }

    static EventCalendarDateSetter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(EventCalendarDateSetter), new FrameworkPropertyMetadata(typeof(EventCalendarDateSetter)));
    }

    public EventCalendarDateSetter()
    {
        Months = new CultureInfo("it-IT").DateTimeFormat
                                         .MonthNames
                                         .Where(m => !string.IsNullOrEmpty(m))
                                         .ToArray();
        RefreshDate();

        NextMonthCommand = new RelayCommand(NextMonth);
        NextYearCommand = new RelayCommand(NextYear);
        PreviousYearCommand = new RelayCommand(PreviousYear);
        PreviousMonthCommand = new RelayCommand(PreviousMonth);
        TodayCommand = new RelayCommand(Today);
    }

    private void Today()
    {
        DateTime today = DateTime.Today;
        if (IsWeekView)
        {
            Date = today.AddDays(-(int)today.DayOfWeek + (today.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
        }
        else
        {
            SelectedMonthIndex = today.Month - 1;
            Year = today.Year;
        }
    }

    private void NextMonth()
    {
        if (IsWeekView)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)Date.DayOfWeek + 7) % 7;
            daysUntilMonday = daysUntilMonday == 0 ? 7 : daysUntilMonday;
            Date = Date.AddDays(daysUntilMonday);
        }
        else
        {
            int temp = SelectedMonthIndex + 1;
            SelectedMonthIndex = (temp > 12) ? 0 : temp;
        }
    }

    private void NextYear()
    {
        Year++;
    }

    private void PreviousYear()
    {
        Year--;
    }

    private void PreviousMonth()
    {
        if (IsWeekView)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)Date.DayOfWeek + 7) % 7;
            daysUntilMonday = daysUntilMonday == 0 ? 7 : daysUntilMonday;
            Date = Date.AddDays(-daysUntilMonday);
        }
        else
        {
            int temp = SelectedMonthIndex - 1;
            SelectedMonthIndex = (temp < 0) ? 11 : temp;
        }
    }

    private void RefreshDate()
    {
        SetCurrentValue(DateProperty, new DateTime(Year, SelectedMonthIndex + 1, 1));
    }
}