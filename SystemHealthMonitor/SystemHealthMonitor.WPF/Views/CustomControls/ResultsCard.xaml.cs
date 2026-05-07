using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SystemHealthMonitor.Shared.Models;


namespace SystemHealthMonitor.WPF.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for LatestResults.xaml
    /// </summary>
    public partial class ResultsCard : UserControl
    {
        public string CardTitle
        {
            get => (string)GetValue(CardTitleProperty);
           
            set
            {
                SetValue(CardTitleProperty, value);
            }
        }
        public ObservableCollection<SystemHealthResults> CardItems
        {
            get => (ObservableCollection<SystemHealthResults>)GetValue(CardItemProperty);
            set
            {
                SetValue(CardItemProperty, value);
            }
        }

        public static readonly DependencyProperty CardTitleProperty = DependencyProperty.Register("CardTitle", typeof(string), typeof(ResultsCard));
        public static readonly DependencyProperty CardItemProperty = DependencyProperty.Register("CardItems", typeof(ObservableCollection<SystemHealthResults>), typeof(ResultsCard));
        public ResultsCard()
        {
            InitializeComponent();
        }
    }
}
