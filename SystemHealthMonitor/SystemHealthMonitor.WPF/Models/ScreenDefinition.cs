namespace SystemHealthMonitor.WPF.Models
{
    public class ScreenDefinition 
    {
     
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public string Title { get; }
        public bool IsSettings { get; }

        public ScreenDefinition(Type viewType, Type viewModelType, string title, bool isSettings)
        {
            ViewType = viewType;
            ViewModelType = viewModelType;
            Title = title;
            IsSettings = isSettings;

        }
    }
}
