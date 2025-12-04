namespace HamrahFelez.ViewModels.Common
{
    public class BaseStoredProcedureResponse
    {
        public BaseStoredProcedureResponse(int _result, string _message)
        {
            Result = _result;
            Message = _message;
            Success = _result > 0;
        }

        public int Result { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
