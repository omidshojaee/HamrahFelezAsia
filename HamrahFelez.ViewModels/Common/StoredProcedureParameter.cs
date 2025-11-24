using System.Data;

namespace HamrahFelez.ViewModels.Common
{
    public class StoredProcedureParameter
    {
        public string Name { get; private set; }
        public object Value { get; private set; }
        public DataTable? DataTable { get; private set; }
        public string TvpTypeName { get; private set; }
    }
}
