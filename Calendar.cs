using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalendarControl;

public class Calendar : Control
{
    public static readonly DependencyProperty IsToggledProperty = Helper.Register<bool, Calendar>(nameof(IsToggled), false);
    public static readonly DependencyProperty DateProperty = Helper.Register<DateTime, Calendar>(nameof(Date));
    public static readonly DependencyProperty IsLoadingProperty = Helper.Register<bool, Calendar>(nameof(IsLoading));
    public static readonly DependencyProperty AddEventCommandProperty = Helper.Register<ICommand, Calendar>(nameof(AddEventCommand));
    public static readonly DependencyProperty EventDropCommandProperty = Helper.Register<ICommand, Calendar>(nameof(EventDropCommand));
    public static readonly DependencyProperty SelectedEventCommandProperty = Helper.Register<ICommand, Calendar>(nameof(SelectedEventCommand));
    public static readonly DependencyProperty DeleteCommandProperty = Helper.Register<ICommand, Calendar>(nameof(DeleteCommand));
    public static readonly DependencyProperty EventsProperty = Helper.Register<IEnumerable<IDatable>, Calendar>(nameof(Events));
    public static readonly DependencyProperty HeaderMarginProperty = Helper.Register<Thickness, Calendar>(nameof(HeaderMargin));

    private EventCalendarDateSetter PART_eventCalendarDateSetter = null!;
    private WeekView PART_weekView = null!;
    private MonthView PART_monthView = null!;

    public Thickness HeaderMargin
    {
        get => (Thickness)GetValue(HeaderMarginProperty);
        set => SetValue(HeaderMarginProperty, value);
    }

    public ICommand EventDropCommand
    {
        get => (ICommand)GetValue(EventDropCommandProperty);
        set => SetValue(EventDropCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public ICommand SelectedEventCommand
    {
        get => (ICommand)GetValue(SelectedEventCommandProperty);
        set => SetValue(SelectedEventCommandProperty, value);
    }

    public ICommand AddEventCommand
    {
        get => (ICommand)GetValue(AddEventCommandProperty);
        set => SetValue(AddEventCommandProperty, value);
    }

    public IEnumerable<IDatable> Events
    {
        get => (IEnumerable<IDatable>)GetValue(EventsProperty);
        set => SetValue(EventsProperty, value);
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }
    public double VerticalScrollPosition => PART_weekView.VerticalScrollPosition;

    static Calendar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(typeof(Calendar)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_eventCalendarDateSetter = GetTemplateChild("PART_eventCalendarDateSetter") as EventCalendarDateSetter ?? throw new InvalidOperationException("PART_eventCalendarDateSetter not found in template.");
        PART_weekView = GetTemplateChild("PART_weekView") as WeekView ?? throw new InvalidOperationException("PART_weekView not found in template.");
        PART_monthView = GetTemplateChild("PART_monthView") as MonthView ?? throw new InvalidOperationException("PART_monthView not found in template.");
    }
    public void ScrollIntoView(double? offset = null)
    {
        if (PART_weekView.Visibility == Visibility.Visible)
        {
            PART_weekView.ScrollToVerticalOffset(offset);
        }
    }
}