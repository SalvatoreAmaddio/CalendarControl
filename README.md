# 📅 CalendarControl

A flexible and modern WPF **Calendar Control** that supports both **Month View** and **Week View**, fully styled with [Material Design in XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit).

> 💡 See a working example here:  
👉 **[CalendarExample GitHub Repository](https://github.com/SalvatoreAmaddio/CalendarExample)**

---

## ✨ Features

- 📆 **Month View** and 🗓️ **Week View** modes
- 🎨 Built with **Material Design** for a modern, clean UI
- 🖱️ Clickable day cards to add or select events
- 🔀 **Drag and Drop** events between days
- 📦 Available via **NuGet** [`Click Here`](https://www.nuget.org/packages/CalendarControl)

---

## ⚠️ Prerequisite: Install Material Design

Before using `CalendarControl`, you **must install** the [MaterialDesignInXAML Toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) in your WPF project:

```code
<PackageReference Include="MaterialDesignThemes" Version="5.2.1" />
```

## 📦 Installation

```bash
dotnet add package CalendarControl --version 1.0.3
```

or copy and paste
```
<PackageReference Include="CalendarControl" Version="1.0.3" />
```

Once you've installed the Material Design package you must add this to your App.xaml file:
```xml
     ...
     xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
     ...
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Theme definition -->
                <materialDesign:BundledTheme BaseTheme="Light" PrimaryColor="Indigo" SecondaryColor="Lime" />

                <!-- Core styles -->
                <ResourceDictionary Source="pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesign3.Defaults.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
```

## Explaining The EventDrop Command
In the CalendarControl the EventDropCommand is invoked after an event record 
has been moved via drag-and-drop. It receives an updated IDatable object. 
The control has already modified the DateOf, StartTime, and EndTime properties, 
and the command is responsible for saving the updated data. For example in your ViewModel:
```csharp

    public ICommand EventDropCommand => new AsyncRelayCommand<IDatable>(EventDropAsync);

    private async Task EventDropAsync(IDatable? datable)
    {
        if (datable is EventModel model)
        {
            await DatabaseManager.UpdateEventAsync(model); //update in the database
        }
    }
```