using ExemploApiSettings.Connection.Contracts;
using ExemploApiSettings.Contracts;
using ExemploApiSettings.DTO;
using ExemploApiSettings.DTO.Request;
using ExemploApiSettings.DTO.Response;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ExemploApiSettings.HttpClients
{
    public class ClienteClient : IClienteClient
    {
        #region Construtor

        private ApiBaseAddresses apiBaseAddresses;
        private readonly ISubmitApi _submitApi;

        public ClienteClient(ISubmitApi submitApi,
                                IOptions<ApiBaseAddresses> apiSettings)
        {
            apiBaseAddresses = apiSettings.Value;
            _submitApi.InicializarHttp(apiBaseAddresses.Cliente);
        }

        #endregion

        #region Metodos Publicos

        public async Task<ClienteResponse> Inserir(ClienteRequest request)
        {
            var model = await _submitApi.PostWithoutTokenAsync<ClienteRequest, ClienteResponse>(request, "/Inserir").ConfigureAwait(false);
            return model;
        }

        public async Task<ClienteResponse> Alterar(ClienteRequest request)
        {
            var model = await _submitApi.PostWithoutTokenAsync<ClienteRequest, ClienteResponse>(request, "/Alterar").ConfigureAwait(false);
            return model;
        }

        #endregion
    }
}
