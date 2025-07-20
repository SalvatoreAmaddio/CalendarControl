using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace CalendarControl;

public class WeekView : AbstractCalendarView
{
    private const int RowHeight = 60;
    private const int MinutesPerRow = 30;
    private const double PixelsPerMinute = (double)RowHeight / MinutesPerRow;
    private bool loaded = false;
    private Canvas PART_monday = null!;
    private Canvas PART_tuesday = null!;
    private Canvas PART_wednesday = null!;
    private Canvas PART_thursday = null!;
    private Canvas PART_friday = null!;
    private Canvas PART_saturday = null!;
    private Canvas PART_sunday = null!;
    private Grid PART_myGrid = null!;

    private ScrollViewer PART_scrollViewer = null!;
    private List<IDatable> events = [];

    public static readonly DependencyProperty MondayProperty = Helper.Register<string, WeekView>(nameof(Monday), string.Empty);

    public static readonly DependencyProperty TuesdayProperty = Helper.Register<string, WeekView>(nameof(Tuesday), string.Empty);

    public static readonly DependencyProperty WednesdayProperty = Helper.Register<string, WeekView>(nameof(Wednesday), string.Empty);

    public static readonly DependencyProperty ThursdayProperty = Helper.Register<string, WeekView>(nameof(Thursday), string.Empty);

    public static readonly DependencyProperty FridayProperty = Helper.Register<string, WeekView>(nameof(Friday), string.Empty);

    public static readonly DependencyProperty SaturdayProperty = Helper.Register<string, WeekView>(nameof(Saturday), string.Empty);

    public static readonly DependencyProperty SundayProperty = Helper.Register<string, WeekView>(nameof(Sunday), string.Empty);

    public string Monday
    {
        get => (string)GetValue(MondayProperty);
        set => SetValue(MondayProperty, value);
    }

    public string Tuesday
    {
        get => (string)GetValue(TuesdayProperty);
        set => SetValue(TuesdayProperty, value);
    }

    public string Wednesday
    {
        get => (string)GetValue(WednesdayProperty);
        set => SetValue(WednesdayProperty, value);
    }

    public string Thursday
    {
        get => (string)GetValue(ThursdayProperty);
        set => SetValue(ThursdayProperty, value);
    }

    public string Friday
    {
        get => (string)GetValue(FridayProperty);
        set => SetValue(FridayProperty, value);
    }

    public string Saturday
    {
        get => (string)GetValue(SaturdayProperty);
        set => SetValue(SaturdayProperty, value);
    }

    public string Sunday
    {
        get => (string)GetValue(SundayProperty);
        set => SetValue(SundayProperty, value);
    }

    public double VerticalScrollPosition => PART_scrollViewer?.VerticalOffset ?? 0;

    static WeekView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekView), new FrameworkPropertyMetadata(typeof(WeekView)));
    }

    public WeekView()
    {
        UpdateHeaders();
        this.SizeChanged += (s, e) =>
        {
            DrawAppointments();
            DrawCurrentTimeLine();
        };
    }

    public void ScrollToVerticalOffset(double? offset = null)
    {
        PART_scrollViewer?.ScrollToVerticalOffset(offset ?? VerticalScrollPosition);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_monday = (Canvas)GetTemplateChild(nameof(PART_monday));
        PART_tuesday = (Canvas)GetTemplateChild(nameof(PART_tuesday));
        PART_wednesday = (Canvas)GetTemplateChild(nameof(PART_wednesday));
        PART_thursday = (Canvas)GetTemplateChild(nameof(PART_thursday));
        PART_friday = (Canvas)GetTemplateChild(nameof(PART_friday));
        PART_saturday = (Canvas)GetTemplateChild(nameof(PART_saturday));
        PART_sunday = (Canvas)GetTemplateChild(nameof(PART_sunday));
        PART_scrollViewer = (ScrollViewer)GetTemplateChild(nameof(PART_scrollViewer));
        PART_myGrid = (Grid)GetTemplateChild(nameof(PART_myGrid));
        loaded = true;
    }

    private void UpdateHeaders()
    {
        events = GenerateWeeklyEmptySlots();

        DateTime today = Date;
        int daysToMonday = ((int)today.DayOfWeek + 6) % 7; // Sunday = 0 → Monday = 1
        DateTime monday = today.AddDays(-daysToMonday);

        for (int dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            DateTime currentDay = monday.AddDays(dayOffset);
            string dayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentDay.ToString("dddd d", new CultureInfo("it-IT")));

            switch (currentDay.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Sunday = dayName;
                    break;
                case DayOfWeek.Monday:
                    Monday = dayName;
                    break;
                case DayOfWeek.Tuesday:
                    Tuesday = dayName;
                    break;
                case DayOfWeek.Wednesday:
                    Wednesday = dayName;
                    break;
                case DayOfWeek.Thursday:
                    Thursday = dayName;
                    break;
                case DayOfWeek.Friday:
                    Friday = dayName;
                    break;
                case DayOfWeek.Saturday:
                    Saturday = dayName;
                    break;
            }
        }
    }

    private List<IDatable> GenerateWeeklyEmptySlots()
    {
        List<IDatable> events = [];

        // Start from the Monday of the current week
        DateTime today = Date;
        int daysToMonday = ((int)today.DayOfWeek + 6) % 7; // Sunday = 0 → Monday = 1
        DateTime monday = today.AddDays(-daysToMonday);

        // For each day from Monday to Sunday
        for (int dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            DateTime currentDay = monday.AddDays(dayOffset);

            for (int i = 0; i < 48; i++) // 48 half-hour slots
            {
                TimeSpan start = TimeSpan.FromMinutes(i * 30);
                TimeSpan end = start.Add(TimeSpan.FromMinutes(30));
                events.Add(new PlaceholderEvent(currentDay, start, end));
            }
        }

        return events;
    }

    private void RemoveTextBlockEvents(object? item)
    {
        if (item is Border border && border.Child is TextBlock txt)
        {
            txt.MouseUp -= OnSlotMouseUp;
            txt.PreviewMouseMove -= OnTxtPreviewMouseMove;
            txt.DragEnter -= OnDragOver;
            txt.Drop -= OnDrop;
        }
    }

    private void Unsubscribe()
    {
        foreach (var item in PART_monday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_tuesday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_wednesday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_thursday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_friday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_saturday.Children)
            RemoveTextBlockEvents(item);

        foreach (var item in PART_sunday.Children)
            RemoveTextBlockEvents(item);
    }

    private void DrawAppointments()
    {
        if (!loaded)
            return;

        Unsubscribe();
        ClearAll();

        List<IDatable> events = [.. this.events];
        events.AddRange(Events);

        foreach (IDatable evt in events)
        {
            double top = evt.StartTime.TotalMinutes * PixelsPerMinute;
            double height = (evt.EndTime - evt.StartTime).TotalMinutes * PixelsPerMinute;

            Canvas canvas = GetCanvas(evt.DateOf);

            Border border;

            if (evt.Title.Length > 0)
            {
                border = EventSlot(canvas.ActualWidth, height, evt);
            }
            else
            {
                border = Placeholder(canvas.ActualWidth, height, evt);
            }

            Canvas.SetTop(border, top);
            Canvas.SetLeft(border, 0); // Adjust left if you want multi-column layout
            canvas.Children.Add(border);
        }
    }

    private Border EventSlot(double width, double height, IDatable evt)
    {
        Grid grid = new();

        Button button = new()
        {
            Command = DeleteCommand,
            CommandParameter = evt,
            Padding = new(0),
            ToolTip = "Elimina",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Content = new PackIcon() { Kind = PackIconKind.CloseThick }
        };

        TextBlock txt = new()
        {
            Cursor = Cursors.Hand,
            ToolTip = "Clicca per Aggiungere/Modificare",
            Text = $"{evt.Title}\n{evt.Location}\n{evt.StartTime:hh\\:mm} - {evt.EndTime:hh\\:mm}",
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(6),
            Tag = evt
        };

        txt.PreviewMouseMove += OnTxtPreviewMouseMove;
        txt.MouseUp += OnSlotMouseUp;

        grid.Children.Add(txt);
        grid.Children.Add(button);

        Border border = new()
        {
            Background = Brushes.LightSkyBlue,
            Width = width,
            Height = height,
            CornerRadius = new(4),
            BorderBrush = Brushes.DodgerBlue,
            BorderThickness = new(1),
            Child = grid
        };

        return border;
    }

    private void OnTxtPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && sender is TextBlock txt)
        {
            DragDrop.DoDragDrop(txt, new DataObject(typeof(TextBlock), txt), DragDropEffects.Move);
        }
    }

    private Border Placeholder(double width, double height, IDatable datable)
    {
        TextBlock txt = new()
        {
            Tag = datable,
            Cursor = Cursors.Hand,
            ToolTip = "Clicca per Aggiungere/Modificare",
            Text = string.Empty,
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(6),
            AllowDrop = true
        };

        txt.MouseUp += OnSlotMouseUp;
        txt.DragEnter += OnDragOver;
        txt.Drop += OnDrop;

        Border border = new()
        {
            Background = Brushes.Transparent,
            Width = width,
            Height = height,
            CornerRadius = new(0),
            BorderBrush = Brushes.Black,
            BorderThickness = new(.1),
            Child = txt
        };

        return border;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(TextBlock)) && sender is TextBlock txt)
        {
            TextBlock droppedEvent = (TextBlock)e.Data.GetData(typeof(TextBlock));

            Border? targetBorder = FindVisualParent<Border>(txt);
            Canvas? targetCanvas = FindVisualParent<Canvas>(targetBorder);

            if (targetBorder != null && targetCanvas != null)
            {
                double left = Canvas.GetLeft(targetBorder);
                double top = Canvas.GetTop(targetBorder);

                IDatable sourceDatable = (IDatable)droppedEvent.Tag;
                IDatable targetDatabale = (IDatable)((TextBlock)targetBorder.Child).Tag;

                sourceDatable.StartTime = targetDatabale.StartTime;
                sourceDatable.EndTime = targetDatabale.EndTime;
                sourceDatable.DateOf = targetDatabale.DateOf;

                Border newSlot = EventSlot(targetBorder.Width, targetBorder.Height, sourceDatable);
                targetCanvas.Children.Add(newSlot);
                Canvas.SetLeft(newSlot, left);
                Canvas.SetTop(newSlot, top);

                Border? sourceBorder = FindVisualParent<Border>(droppedEvent);
                Canvas? sourceCanvas = FindVisualParent<Canvas>(sourceBorder);
                RemoveTextBlockEvents(sourceBorder);
                sourceCanvas?.Children?.Remove(sourceBorder);
                EventDropCommand.Execute(sourceDatable);
            }
        }
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Move;
        e.Handled = true;
    }

    private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child != null)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent is T parentAsT)
                return parentAsT;

            child = parent;
        }
        return null;
    }

    private void OnSlotMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBlock txt)
        {
            if (txt.Tag is IDatable datable)
            {
                if (datable.Id > 0)
                    SelectedEventCommand.Execute(datable);
                else
                    AddEventCommand.Execute(datable);
            }
        }
    }

    private void ClearAll()
    {
        PART_monday.Children.Clear();
        PART_tuesday.Children.Clear();
        PART_wednesday.Children.Clear();
        PART_thursday.Children.Clear();
        PART_friday.Children.Clear();
        PART_saturday.Children.Clear();
        PART_sunday.Children.Clear();
    }

    private Canvas GetCanvas(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => PART_monday,
            DayOfWeek.Tuesday => PART_tuesday,
            DayOfWeek.Wednesday => PART_wednesday,
            DayOfWeek.Thursday => PART_thursday,
            DayOfWeek.Friday => PART_friday,
            DayOfWeek.Saturday => PART_saturday,
            DayOfWeek.Sunday => PART_sunday,
            _ => throw new InvalidOperationException("Invalid day")
        };
    }

    private void DrawCurrentTimeLine()
    {
        if (!loaded)
            return;

        TimeSpan now = DateTime.Now.TimeOfDay;
        double top = now.TotalMinutes * PixelsPerMinute;

        Line line = new()
        {
            X1 = 0,
            X2 = PART_monday.ActualWidth + PART_tuesday.ActualWidth + PART_wednesday.ActualWidth + PART_thursday.ActualWidth + PART_friday.ActualWidth + PART_saturday.ActualWidth + PART_sunday.ActualWidth, // Use the actual width of the canvas
            Y1 = top,
            Y2 = top,
            Stroke = Brushes.DodgerBlue,
            StrokeThickness = 2,
        };

        PART_monday.Children.Add(line);

        PART_scrollViewer.ScrollToVerticalOffset(top - 100);
    }

    protected override void OnEventsChanged()
    {
        DrawAppointments();
        DrawCurrentTimeLine();
    }

    protected override void OnDateChanged()
    {
        UpdateHeaders();
    }
}

public class PlaceholderEvent(DateTime dateOf, TimeSpan startTime, TimeSpan endTime) : IDatable
{
    public DateTime DateOf { get; set; } = dateOf;
    public TimeSpan StartTime { get; set; } = startTime;
    public TimeSpan EndTime { get; set; } = endTime;
    public string Title { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{StartTime:hh\\:mm} - {Title}";
    }
}