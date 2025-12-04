namespace HamrahFelez.ViewModels
{
    public class View_WebUser
    {
        public long pkUser { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string HashedPassword { get; set; }
        public int? AttendanceID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string BirthdayP { get; set; }
        public int? fkOptionSex { get; set; }
        public string OptionSex { get; set; }
        public string NationalCode { get; set; }
        public string RegistrationNumber { get; set; }
        public string EconomicCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? fkOrganizationalChart { get; set; }
        public int? fkOrganizationalChartParent { get; set; }
        public int fkOptionApiRole { get; set; }
        public string OptionApiRole { get; set; }
        public string fkAshkhasChilds { get; set; }
    }
}
