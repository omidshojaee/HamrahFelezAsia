using HamrahFelez.ViewModels;

namespace HamrahFelez.Repositories
{
    public interface ICommunicationRepository
    {
        Task<BaseStoredProcedureResponse> InsertCallAsync(InsertCallRequest request);
    }
}
