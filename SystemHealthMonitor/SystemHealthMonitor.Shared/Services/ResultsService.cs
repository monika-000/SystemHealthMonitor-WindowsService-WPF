using Serilog;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SystemHealthMonitor.Shared.Interfaces;
using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.Shared.Services
{
    public class ResultsService: IResultsService
    {

        private readonly string _resultsPath;
        private readonly string _directoryPath;
        private readonly ILogger<ResultsService> _logger;
        public Action? ResultsUpdated { get; set; }
        public ResultsService(ILogger<ResultsService> logger)
        {
            _directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MPCorp", "SystemHealthMonitorApp");
            _resultsPath = Path.Combine(_directoryPath, "results.json");
            _logger = logger;
        }

        public List<SystemHealthResults> GetResults()
        {
          
            List<SystemHealthResults> systemHealthResults = new List<SystemHealthResults>();
            EnsureDirecoryAndFileExists();
            foreach(var result in File.ReadLines(_resultsPath))
            {
                SystemHealthResults systemHealthResult = JsonSerializer.Deserialize<SystemHealthResults>(result);
                    
                //If older than 30 days, delete
                systemHealthResults.Add(systemHealthResult);
            }
            _logger.LogInformation("Succesfully loaded results");

            return systemHealthResults;
        }
        public void SaveLatestResults(List<SystemHealthResults> systemHealthResults)
        {
           
            EnsureDirecoryAndFileExists();
            foreach(SystemHealthResults systemHealthResult in systemHealthResults)
            {
                var json = JsonSerializer.Serialize(systemHealthResult);
                File.AppendAllText(_resultsPath, json + Environment.NewLine);
            }

            _logger.LogInformation("Succesfully saved result");

            //Invoke an event to notify Dasboard and Results that data has changed
            ResultsUpdated?.Invoke();

            _logger.LogInformation("Sent a notification about the results changing");

        }

        public void CleanUpOldResults()
        {
            string tempPath = _resultsPath + ".tmp";
            string backupPath = _resultsPath + ".bak";
            try
            {
                _logger.LogInformation("Starting old results clean up");

                DateTimeOffset cutoff = DateTimeOffset.Now.AddDays(-30);
                EnsureDirecoryAndFileExists();
         
                var cleanedUpResults = File.ReadLines(_resultsPath)
                .Where(line =>
                {
                    SystemHealthResults systemHealthResult = JsonSerializer.Deserialize<SystemHealthResults>(line);
                    return systemHealthResult.DateTime >= cutoff;
                });

                File.WriteAllLines(tempPath, cleanedUpResults);
                File.Replace(tempPath, _resultsPath, backupPath);
            }
            finally 
            {  
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                    _logger.LogInformation("Clean up successfull. Old results were deleted.");
                }
            }
        }

        public void EnsureDirecoryAndFileExists()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(_resultsPath))
            {
                File.Create(_resultsPath); 
            }
        }
    }
}
