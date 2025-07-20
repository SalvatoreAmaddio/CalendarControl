using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace CalendarControl;

public class EventCalendarDateSetter : Control
{
    public static readonly DependencyProperty IsToggledProperty = Helper.Register<bool, EventCalendarDateSetter>(nameof(IsToggled), false, OnIsToggledPropertyChanged);    
    public static readonly DependencyProperty TodayContentProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(TodayContent), "Mese Corrente");
    public static readonly DependencyProperty PreviousButtonTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(PreviousButtonTooltip), "Mese Precedente");
    public static readonly DependencyProperty NextButtonTooltipProperty = Helper.Register<string, EventCalendarDateSetter>(nameof(NextButtonTooltip), "Prossimo Mese");
    public static readonly DependencyProperty DateVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(DateVisibility), Visibility.Collapsed);
    public static readonly DependencyProperty YearVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(YearVisibility), Visibility.Visible);
    public static readonly DependencyProperty MonthVisibilityProperty = Helper.Register<Visibility, EventCalendarDateSetter>(nameof(MonthVisibility), Visibility.Visible);
    public static readonly DependencyProperty TodayCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(TodayCommand));
    public static readonly DependencyProperty PreviousYearCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(PreviousYearCommand));
    public static readonly DependencyProperty PreviousMonthCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(PreviousMonthCommand));
    public static readonly DependencyProperty NextYearCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(NextYearCommand));
    public static readonly DependencyProperty NextMonthCommandProperty = Helper.Register<ICommand, EventCalendarDateSetter>(nameof(NextMonthCommand));
    public static readonly DependencyProperty DateProperty = Helper.Register<DateTime, EventCalendarDateSetter>(nameof(Date));
    public static readonly DependencyProperty YearProperty = Helper.Register<int, EventCalendarDateSetter>(nameof(Year), DateTime.Now.Year, (s, e) => ((EventCalendarDateSetter)s).RefreshDate());
    public static readonly DependencyProperty MonthsProperty = Helper.Register<IEnumerable<string>, EventCalendarDateSetter>(nameof(Months));
    public static readonly DependencyProperty SelectedMonthIndexProperty = 
    Helper.Register<int, EventCalendarDateSetter>(nameof(SelectedMonthIndex), DateTime.Now.Month - 1, (s, e) => ((EventCalendarDateSetter)s).RefreshDate());
    public string TodayContent
    {
        get => (string)GetValue(TodayContentProperty);
        set => SetValue(TodayContentProperty, value);
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

    private static void OnIsToggledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EventCalendarDateSetter control && e.NewValue is bool isToggled)
        {
            if (isToggled)
            {
                control.MonthVisibility = Visibility.Collapsed;
                control.YearVisibility = Visibility.Collapsed;
                control.DateVisibility = Visibility.Visible;
                control.NextButtonTooltip = "Prossima Settimana";
                control.PreviousButtonTooltip = "Settimana Precedente";
                control.TodayContent = "Settimana Corrente";
            }
            else
            {
                control.TodayContent = "Mese Corrente";
                control.PreviousButtonTooltip = "Mese Precedente";
                control.NextButtonTooltip = "Prossimo Mese";
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
        }
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
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
        if (IsToggled)
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
        if (IsToggled)
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
        if (IsToggled)
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
        Date = new(Year, SelectedMonthIndex + 1, 1);
    }
}