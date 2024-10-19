using MongoDB.Bson;
using Realms;

namespace Models
{

    public partial class PaymentType_PaymentSchedule : IEmbeddedObject
    {
        public int? DailyScheduleEveryXDays { get; set; }

        public string? ScheduleType { get; set; }

        public IList<int> WeeklyScheduleDays { get; }

        public int? WeeklyScheduleEveryXWeeks { get; set; }

        public int? YearlyScheduleDay { get; set; }

        public int? YearlyScheduleMonth { get; set; }
    }

}
