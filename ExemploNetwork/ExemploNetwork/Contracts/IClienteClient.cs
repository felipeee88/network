using ExemploApiSettings.DTO.Request;
using ExemploApiSettings.DTO.Response;
using System.Threading.Tasks;

namespace ExemploApiSettings.Contracts
{
    public interface IClienteClient
    {
        Task<ClienteResponse> Inserir(ClienteRequest request);
        Task<ClienteResponse> Alterar(ClienteRequest request);
    }
}
