using HamrahFelez.ViewModels;
using HamrahFelez.Utilities;

namespace HamrahFelez.Repositories
{
    public class CommunicationRepository : ICommunicationRepository
    {
        public async Task<BaseStoredProcedureResponse> InsertCallAsync(InsertCallRequest request)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            var response = await DataAccessManager.ExecuteStoredProcedureAsync(
                procedure: "dbo.Sp_CrudCommunication",
                cancellationToken: cts.Token,
                parameters:
                [
                    new StoredProcedureParameter { Name = "mod", Value = (int)Enums.CrudMod.Insert },
                    new StoredProcedureParameter { Name = "pkCommunication", Value = -1 },
                    new StoredProcedureParameter { Name = "ParentID", Value = -1 },
                    new StoredProcedureParameter { Name = "fkOptionCommunicationType", Value = request.fkOptionCommunicationType },
                    new StoredProcedureParameter { Name = "fkOptionCallDirection", Value = request.fkOptionCallDirection },
                    new StoredProcedureParameter { Name = "fkOptionDialStatus", Value = request.fkOptionDialStatus },
                    new StoredProcedureParameter { Name = "fkOptionLeadSource", Value = -1 },
                    new StoredProcedureParameter { Name = "InternalNumber", Value = request.InternalNumber },
                    new StoredProcedureParameter { Name = "ExternalNumber", Value = request.ExternalNumber },
                    new StoredProcedureParameter { Name = "InternalEmail", Value = "" },
                    new StoredProcedureParameter { Name = "ExternalEmail", Value = "" },
                    new StoredProcedureParameter { Name = "MessageBody", Value = "" },
                    new StoredProcedureParameter { Name = "DescriptionEmployee", Value = "" },
                    new StoredProcedureParameter { Name = "DescriptionCustomer", Value = "" },
                    new StoredProcedureParameter { Name = "fkOptionOwnCallReason", Value = -1 },
                    new StoredProcedureParameter { Name = "fkOptionClientCallReason", Value = -1 },
                    new StoredProcedureParameter { Name = "fkOptionCallStatus", Value = 1556 },
                    new StoredProcedureParameter { Name = "IsActive", Value = 1 },
                    new StoredProcedureParameter { Name = "CreateUser", Value = 536499 },
                    new StoredProcedureParameter { Name = "InsertUser", Value = 536499 },
                    new StoredProcedureParameter { Name = "InsertIP", Value = "192.168.50.4" }
                ]);

            return response;
        }
    }
}
