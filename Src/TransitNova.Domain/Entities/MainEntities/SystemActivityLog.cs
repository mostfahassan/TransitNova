
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class SystemActivityLog
    {
        public long Id { get; private set; }

        public ActivityAction Action { get; private set; }

        public ActivityEntityType EntityType { get; private set; }

        public string Description { get; private set; } = string.Empty;

        public Guid? PerformedByUserId { get; private set; }

        public string PerformedByName { get; private set; } = string.Empty;

        public DateTime OccurredAt { get; private set; }

        private SystemActivityLog()
        {

        }

        private SystemActivityLog(ActivityAction action, ActivityEntityType entityType, string description, Guid? performedByUserId, string performedByName)
        {
            Action = action;
            EntityType = entityType;
            Description = description;
            PerformedByUserId = performedByUserId;
            PerformedByName = performedByName;
            OccurredAt = DateTime.UtcNow;
        }

        public static SystemActivityLog AddLog(ActivityAction action, ActivityEntityType entityType, string description, Guid? performedByUserId, string performedByName)
        {
            return new SystemActivityLog(
                action,
                entityType,
                description,
                performedByUserId,
                performedByName);
        }
    }



}
