namespace HamrahFelez.ViewModels.Common
{
    public class BaseStoredProcedureResponse
    {
        public BaseStoredProcedureResponse(int _result, string _message)
        {
            result = _result;
            message = _message;
            success = _result > 0;
        }
        public int result { get; private set; }
        public string message { get; private set; }
        public bool success { get; private set; }
    }
}
