namespace Zinlo.Auditing.Dto
{
    public class GetEntityHistoryInput
    {
        public  string EntityId { get; set; }
        public  string EntityFullName { get; set; }
        public string PropertyName { get; set; }
    }
}