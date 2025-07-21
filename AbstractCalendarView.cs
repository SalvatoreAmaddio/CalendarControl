using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Globalization;

namespace CalendarControl;

public abstract class AbstractCalendarView : Control
{
    public static readonly DependencyProperty AddEventCommandProperty = Helper.Register<ICommand, AbstractCalendarView>(nameof(AddEventCommand));
    public static readonly DependencyProperty EventDropCommandProperty = Helper.Register<ICommand, AbstractCalendarView>(nameof(EventDropCommand));
    public static readonly DependencyProperty SelectedEventCommandProperty = Helper.Register<ICommand, AbstractCalendarView>(nameof(SelectedEventCommand));
    public static readonly DependencyProperty DeleteCommandProperty = Helper.Register<ICommand, AbstractCalendarView>(nameof(DeleteCommand));
    public static readonly DependencyProperty EventsProperty = Helper.Register<IEnumerable<IDatable>, AbstractCalendarView>(nameof(Events), OnEventsSourcePropertyChanged);
    public static readonly DependencyProperty DateProperty = Helper.Register<DateTime, AbstractCalendarView>(nameof(Date), DateTime.Now, OnDatePropertyChanged);
    
    public static readonly DependencyProperty CultureProperty =
    Helper.Register<CultureInfo, AbstractCalendarView>(nameof(Culture), CultureInfo.CurrentUICulture, OnCulturePropertyChanged);

    public CultureInfo Culture
    {
        get => (CultureInfo)GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
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

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    private static void OnDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AbstractCalendarView control && e.NewValue is DateTime date && date > DateTime.MinValue)
        {
            control.OnDateChanged();
        }
    }

    public IEnumerable<IDatable> Events
    {
        get => (IEnumerable<IDatable>)GetValue(EventsProperty);
        set => SetValue(EventsProperty, value);
    }

    private static void OnEventsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AbstractCalendarView control)
        {
            control.OnEventsChanged();
        }
    }

    private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AbstractCalendarView control)
        {
            control.OnCultureChanged();
        }
    }

    protected abstract void OnCultureChanged();

    protected abstract void UpdateLocalizedStrings();

    protected abstract void OnEventsChanged();
    protected abstract void OnDateChanged();
}