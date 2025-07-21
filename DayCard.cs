using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Resources;

namespace CalendarControl;

public class DayCard : Control, IDisposable
{
    public static readonly DependencyProperty DeleteCommandProperty = Helper.Register<ICommand, DayCard>(nameof(DeleteCommand));
    public static readonly DependencyProperty DeleteToolTipProperty = Helper.Register<string, DayCard>(nameof(DeleteToolTip));
    public static readonly DependencyProperty EventDropCommandProperty = Helper.Register<ICommand, DayCard>(nameof(EventDropCommand));
    public static readonly DependencyProperty SelectedEventCommandProperty = Helper.Register<ICommand, DayCard>(nameof(SelectedEventCommand));
    public static readonly DependencyProperty AddEventCommandProperty = Helper.Register<ICommand, DayCard>(nameof(AddEventCommand));
    public static readonly DependencyProperty CalendarDayProperty = Helper.Register<CalendarDay, DayCard>(nameof(CalendarDay));
    public static readonly DependencyProperty CultureProperty =
    Helper.Register<CultureInfo, DayCard>(nameof(Culture), CultureInfo.CurrentUICulture, OnCulturePropertyChanged);
    
    private readonly ResourceManager rm = new("CalendarControl.Resources.Strings", typeof(EventCalendarDateSetter).Assembly);

    public string DeleteToolTip 
    {
        get => (string)GetValue(DeleteToolTipProperty);
        set => SetValue(DeleteToolTipProperty, value);
    }

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

    public CalendarDay CalendarDay
    {
        get => (CalendarDay)GetValue(CalendarDayProperty);
        set => SetValue(CalendarDayProperty, value);
    }

    private Border PART_border = null!;

    private Point _startPoint;

    private ListBox PART_eventList = null!;

    static DayCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(DayCard), new FrameworkPropertyMetadata(typeof(DayCard)));
    }

    private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DayCard control)
        {
            control.DeleteToolTip = control.rm.GetString("Delete", (CultureInfo)e.NewValue) ?? "Error";
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_eventList = (ListBox)GetTemplateChild(nameof(PART_eventList));
        PART_eventList.MouseLeftButtonUp += OnEventListMouseClicked;
        PART_eventList.PreviewMouseMove += OnEventListDragging;
        PART_eventList.Drop += OnDrop;
        PART_eventList.DragOver += OnDragOver;
        PART_border = (Border)GetTemplateChild(nameof(PART_border));
        PART_border.MouseDown += OnCardClicked;
    }

    private void OnEventListDragging(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Point position = e.GetPosition(null);
            if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (sender is ListBox listBox && listBox.SelectedItem is IDatable datable && listBox.DataContext is CalendarDay calendarDay)
                {
                    DataObject data = new(typeof((IDatable, CalendarDay)), (datable, calendarDay));
                    DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
                }
            }
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (sender is ListBox listBox && listBox.DataContext is CalendarDay targetCalendarDay)
        {
            (IDatable, CalendarDay) data = ((IDatable, CalendarDay))e.Data.GetData(typeof((IDatable, CalendarDay)));

            CalendarDay sourceCalendarDay = data.Item2;

            if (targetCalendarDay.Date == sourceCalendarDay.Date)
            {
                return;
            }

            IDatable datable = data.Item1;
            datable.DateOf = targetCalendarDay.Date;
            sourceCalendarDay.Events.Remove(datable);
            targetCalendarDay.Events.Add(datable);
            var reordered = targetCalendarDay.Events.OrderBy(e => e.StartTime).ToList();
            targetCalendarDay.Events = [.. reordered];
            targetCalendarDay.OnPropertyChanged(nameof(targetCalendarDay.Events));
            EventDropCommand.Execute(datable);
        }
    }

    private void OnCardClicked(object sender, MouseButtonEventArgs e)
    {
        CalendarDay calendarDay = (CalendarDay)PART_border.DataContext;
        AddEventCommand.Execute(calendarDay.Date);
    }

    private void OnEventListMouseClicked(object sender, MouseButtonEventArgs e)
    {
        ListBox list = (ListBox)sender;
        object selectedItem = list.SelectedItem;

        if (selectedItem != null)
            SelectedEventCommand?.Execute(selectedItem);
    }

    public void Dispose()
    {
        PART_eventList.MouseLeftButtonUp -= OnEventListMouseClicked;
        PART_eventList.PreviewMouseMove -= OnEventListDragging;
        PART_eventList.Drop -= OnDrop;
        PART_eventList.DragOver -= OnDragOver;

        PART_border.MouseDown -= OnCardClicked;
        GC.SuppressFinalize(this);
    }
}

public class DayBackgroundConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isActive = (bool)values[0];
        bool isToday = (bool)values[1];
        return isToday ? Brushes.DodgerBlue : IsActive(isActive);
    }

    private static SolidColorBrush IsActive(bool isActive)
    {
        return isActive ? Brushes.WhiteSmoke : Brushes.LightGray;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DayForegroundConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool isFestive = (bool)values[0];
        bool isToday = (bool)values[1];
        return isToday ? Brushes.WhiteSmoke : IsFestive(isFestive);
    }

    private static SolidColorBrush IsFestive(bool isFestive)
    {
        return isFestive ? Brushes.Red : Brushes.Black;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}