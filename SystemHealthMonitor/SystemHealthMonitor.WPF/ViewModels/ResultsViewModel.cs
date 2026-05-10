using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;
using SystemHealthMonitor.WPF.Commands;
namespace SystemHealthMonitor.WPF.ViewModels
{
    public class ResultsViewModel : BaseViewModel
    {
        private readonly ILogger<ResultsViewModel> _logger;
        public List<SystemHealthResults> AllResults { get; set; }
        public List<SystemHealthResults> FilteredResults { get; set; }
        private ObservableCollection<SystemHealthResults> _pageResults { get; set; }
        public ObservableCollection<SystemHealthResults> PageResults 
        {
            get => _pageResults;
            set
            {
                _pageResults = value;
                OnPropertyChanged();
            } 
        }
       
        private IResultsService _resultsService;
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand FilterResultsCommand { get; }
        public ICommand ClearFilterResultsCommand { get; }

        private int _pageSize = 4;
        private int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                OnPropertyChanged();
                LoadPage();
                SortByDateColumn();
                OnPropertyChanged(nameof(CurrentPageNumber));
            }

        }
        public string CurrentPageNumber => $"{PageIndex + 1} - {TotalPages}";
        private int _totalPages;
        public int TotalPages => (int)Math.Ceiling((double)FilteredResults.Count / _pageSize);

        private DateTime _selectedStartDate;
        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                _selectedStartDate = value;
            }
        }
        private DateTime _selectedEndDate;
        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                _selectedEndDate = value;
            }
        }

        public ResultsViewModel(IResultsService resultsService, ILogger<ResultsViewModel> logger)
        {
            _resultsService = resultsService;
            _logger = logger;
            SelectedStartDate = DateTime.Today;
            SelectedEndDate = DateTime.Today;

            _resultsService.ResultsUpdated += () => LoadData();

            PreviousPageCommand = new RelayCommand(MoveToPreviousPage);
            NextPageCommand = new RelayCommand(MoveToNextPage);
            FilterResultsCommand = new RelayCommand(FilterResults);
            ClearFilterResultsCommand = new RelayCommand(ClearFilterResults);
            LoadData();  
        }
        public void LoadPage(bool filter = false)
        {
            PageResults.Clear();
          
            List<SystemHealthResults> page = FilteredResults.Skip(PageIndex * _pageSize)
                                                .Take(_pageSize)
                                                .ToList();
              
            foreach (var item in page)
            {
                PageResults.Add(item);
            }
        }

        public void SortByDateColumn()
        {
            var view = CollectionViewSource.GetDefaultView(PageResults); 
            view.SortDescriptions.Clear(); 
            view.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));
        }

        public void MoveToPreviousPage(object obj)
        {
            if(PageIndex > 0)
            {
                PageIndex--; 
            }
        }
        public void MoveToNextPage(object obj)
        {
            if (PageIndex < TotalPages - 1)
            {
                PageIndex++;
            }
        }
        public void FilterResults(object obj)
        {
            FilteredResults = AllResults.FindAll(d => d.DateTime.Date >= SelectedStartDate 
                                                   && d.DateTime.Date <= SelectedEndDate).ToList();
            PageIndex = 0;
        }
        public void ClearFilterResults(object obj)
        {
            FilteredResults = AllResults;
            PageIndex = 0;
        }

        public override void LoadData()
        {
            try
            {
                AllResults = _resultsService.GetResults();
                FilteredResults = AllResults;
                PageResults = new ObservableCollection<SystemHealthResults>();
                LoadPage();
            }
            catch(Exception ex) 
            {
                _logger.LogError("Failed to load results. Exception: {0}", ex);
                System.Windows.MessageBox.Show("Couldn't load dashboard results. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
