using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ILogger<DashboardViewModel> _logger;
        private IResultsService _resultsService;
        private ObservableCollection<SystemHealthResults> _results;
        private ObservableCollection<SystemHealthResults> _latestResults;
        private ObservableCollection<SystemHealthResults> _worstResults;
        public ObservableCollection<SystemHealthResults> Results
        {
            get => _results;
            set
            {
                _results = value;
                OnPropertyChanged();
            }

        }
        public ObservableCollection<SystemHealthResults> LatestResults
        { 
            get => _latestResults;
            set
            {
                _latestResults = value;
                OnPropertyChanged(); //Automatically update UI with latest results
            }
        }

        public ObservableCollection<SystemHealthResults> WorstResults 
        {   get => _worstResults;
            set
            {
                _worstResults = value;
                OnPropertyChanged();
            }
        }

        public DashboardViewModel(IResultsService resultsService, ILogger<DashboardViewModel> logger)
        {
            _logger = logger;
            _resultsService = resultsService;
            _resultsService.ResultsUpdated += () => LoadData();
            LoadData();
        }

        private List<SystemHealthResults> GetLatestResults()
        {
           List<SystemHealthResults> latestResults = Results
            .OrderByDescending(d => d.DateTime)
            .GroupBy(m => m.Metric)
            .Select(r => r.First()).ToList();
            
            return latestResults;
        }

        private List<SystemHealthResults> GetWorstResults()
        {
            List<SystemHealthResults> worstResults = Results
                .GroupBy(m => m.Metric)
                .Select(r => new SystemHealthResults
                {
                    Metric = r.Key,
                    Value = r.Average(v => decimal.Parse(v.Value)).ToString()
                })
                .OrderByDescending(v => v.Value)
                .Take(3)
                .ToList();

            return worstResults;
        }

        public override void LoadData()
        {
            try
            {
                Results = new ObservableCollection<SystemHealthResults>(_resultsService.GetResults());
                LatestResults = new ObservableCollection<SystemHealthResults>(GetLatestResults());
                WorstResults = new ObservableCollection<SystemHealthResults>(GetWorstResults());
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load results. Exception: {0}", ex);
                System.Windows.MessageBox.Show("Couldn't load dashboard results. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
