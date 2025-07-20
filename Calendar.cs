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

    static Calendar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(typeof(Calendar)));
    }
}
