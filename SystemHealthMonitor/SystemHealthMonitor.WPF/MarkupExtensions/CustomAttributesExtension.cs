using System.ComponentModel;
using System.Reflection;
using System.Windows.Markup;

namespace SystemHealthMonitor.WPF.MarkupExtensions
{
    public class CustomAttributesExtension : MarkupExtension
    {

        public Type Type { get; set; }
        public string Property { get; set; }
        public enum AttributeMode 
        { 
            DisplayName, 
            Description 
        }
        public AttributeMode Mode { get; set; } = AttributeMode.DisplayName;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if(Type == null || Property == null) return null;

            var prop = Type.GetProperty(Property);
            if (prop == null)
                return Property; 

            switch (Mode) 
            { case AttributeMode.DisplayName: 
                    return prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? Property; 
                case AttributeMode.Description: return prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? Property; 
                default: return Property; 
            }
        }

    }
}
