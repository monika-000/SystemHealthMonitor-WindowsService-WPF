using SystemHealthMonitor.Shared.Models;

namespace SystemHealthMonitor.Shared.Interfaces
{
    public interface IResultsService
    {
        //void UpdateLatestResultsInWPF(decimal cpuUsage, decimal memoryUsage, decimal diskIO, Dictionary<string, decimal> freeDiskSpace, decimal networkTotalUsage, decimal incomingPacketsLoss, decimal outgoingPacketsLoss);
        List<SystemHealthResults> GetResults();
        void SaveLatestResults(List<SystemHealthResults> systemHealthResults);
        void CleanUpOldResults();
        Action? ResultsUpdated { get; set; }
    }
}
