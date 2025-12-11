using System.Data;

namespace HamrahFelez.ViewModels
{
    public class StoredProcedureParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DataTable? DataTable { get; set; }
        public string TvpTypeName { get; set; }
    }
}