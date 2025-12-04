using HamrahFelez.Repositories;
using HamrahFelez.ViewModels;

namespace HamrahFelez.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ICommunicationRepository _communicationRepository;

        public CommunicationService(ICommunicationRepository communicationRepository)
        {
            _communicationRepository = communicationRepository;
        }

        public async Task<BaseStoredProcedureResponse> InsertCallAsync(InsertCallRequest request)
        {
            return await _communicationRepository.InsertCallAsync(request);
        }
    }
}
