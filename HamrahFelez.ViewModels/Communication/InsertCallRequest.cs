namespace HamrahFelez.ViewModels
{
    public class InsertCallRequest
    {
        public int fkOptionCommunicationType { get; set; }
        public int fkOptionCallDirection { get; set; }
        public int fkOptionDialStatus { get; set; }
        public string InternalNumber { get; set; }
        public string ExternalNumber { get; set; }
    }
}
