using HamrahFelez.ViewModels;

namespace HamrahFelez.Services
{
    public interface ICommunicationService
    {
        Task<BaseStoredProcedureResponse> InsertCallAsync(InsertCallRequest request);
    }
}
