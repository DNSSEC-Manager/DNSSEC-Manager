using System.Collections.Generic;
using Backend.Models;

namespace Backend.ViewModels
{
    public class LogsViewModel
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageSize { get; set; }

        public int LogType { get; set; }
        public string Sort { get; set; }
        public int? DnsServerId { get; set; }

        public List<DnsServer> DnsServers { get; set; }
        public List<Log> Logs { get; set; }

    }
}
