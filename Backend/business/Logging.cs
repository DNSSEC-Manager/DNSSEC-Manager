using Backend.Models;
using System;

namespace Backend.Business
{
    public class Logging
    {

        public static Log LogJob(Job job, LogType logType, string message, string rawMessage = "")
        {
            var newLog = new Log
            {
                JobId = job.Id,
                CreatedAt = DateTime.Now,
                DomainId = job.DomainId,
                DnsServerId = job.DnsServerId,
                Message = message,
                LogType = logType,
                RawMessage = rawMessage
            };
            return newLog;
        }

        public static Log LogDomain(Domain domain, LogType logType, string message, string rawMessage = "")
        {
            var newLog = new Log
            {
                CreatedAt = DateTime.Now,
                DomainId = domain.Id,
                DnsServerId = domain.DnsServerId,
                Message = message,
                LogType = logType,
                RawMessage = rawMessage
            };
            return newLog;
        }

        public static Log LogGeneral(LogType logType, string message, string rawMessage = "")
        {
            var newLog = new Log
            {
                CreatedAt = DateTime.Now,
                Message = message,
                LogType = logType,
                RawMessage = rawMessage
            };
            return newLog;
        }
    }
}
