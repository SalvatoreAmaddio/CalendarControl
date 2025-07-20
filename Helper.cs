using System.Windows;

namespace CalendarControl;

internal static class Helper
{
    public static DependencyProperty Register<T, C>(string propertyName, T defaultValue)
    {
        return DependencyProperty.Register(
                  propertyName,
                  typeof(T),
                  typeof(C),
                  new PropertyMetadata(defaultValue));
    }

    public static DependencyProperty Register<T, C>(string propertyName, T defaultValue, PropertyChangedCallback callBack)
    {
        return DependencyProperty.Register(
                  propertyName,
                  typeof(T),
                  typeof(C),
                  new PropertyMetadata(defaultValue, callBack));
    }

    public static DependencyProperty Register<T, C>(string propertyName, PropertyChangedCallback callBack)
    {
        return DependencyProperty.Register(
                  propertyName,
                  typeof(T),
                  typeof(C),
                  new PropertyMetadata(callBack));
    }

    public static DependencyProperty Register<T, C>(string propertyName)
    {
        return DependencyProperty.Register(
                  propertyName,
                  typeof(T),
                  typeof(C),
                  new PropertyMetadata());
    }
}
