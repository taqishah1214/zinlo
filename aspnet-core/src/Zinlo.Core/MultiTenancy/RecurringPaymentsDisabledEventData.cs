using Abp.Events.Bus;

namespace Zinlo.MultiTenancy
{
    public class RecurringPaymentsDisabledEventData : EventData
    {
        public int TenantId { get; set; }

        public int EditionId { get; set; }
    }
}
