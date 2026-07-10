using TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Reports;

namespace TransitNova.Domain.Entities.MainEntities
{
    public class ReportRequest : AggregateRoot<Guid>
    {
        public string? FilePath { get; private set; }
        public ReportStatus ReportStatus { get; private set; }
        public string ReportKey { get; private set; } = string.Empty;
        public string PayloadJson { get; private set; } = "{}";
        public string? ErrorMessage { get; private set; }
        public int FileSize { get; private set; }
        public Guid RequestedBy { get; private set; }
        public DateTime? StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private ReportRequest()
        {
        }

        private ReportRequest(string reportKey, string payloadJson, Guid requestedBy)
        {
            Id = Guid.CreateVersion7();
            ReportKey = reportKey;
            PayloadJson = payloadJson;
            RequestedBy = requestedBy;
            ReportStatus = ReportStatus.Pending;
        }

        public static ReportRequest CreateReport(string reportKey, string payloadJson, Guid requestedBy)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reportKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);

            var report = new ReportRequest(reportKey, payloadJson, requestedBy);
            report.RaiseDomainEvent(new ReportRequestCreatedEvent(report.Id, report.ReportKey));
            return report;
        }

        public void MarkAsStarted()
        {
            EnsureReadyToStart();
            ReportStatus = ReportStatus.InProgress;
            StartedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ReportRequestStartedEvent(Id));
        }

        public void MarkAsCompleted(string filePath, int fileSize)
        {
            EnsureReadyToComplete();
            ReportStatus = ReportStatus.Completed;
            FilePath = filePath;
            FileSize = fileSize;
            CompletedAt = DateTime.UtcNow;
            RaiseDomainEvent(new ReportRequestCompletedEvent(Id, RequestedBy));
        }

        private void EnsureReadyToStart()
        {
            if (ReportStatus != ReportStatus.Pending)
            {
                throw new InvalidOperationException("Report request can only be started if it is in pending status.");
            }

            if (StartedAt != null)
            {
                throw new InvalidOperationException("Report request has already been started.");
            }
        }

        private void EnsureReadyToComplete()
        {
            if (ReportStatus != ReportStatus.InProgress)
            {
                throw new InvalidOperationException("Report request can only be completed if it is in progress.");
            }

            if (CompletedAt != null)
            {
                throw new InvalidOperationException("Report request has already been completed.");
            }
        }
    }
}
