using ExemploApiSettings.DTO;
using System.Threading.Tasks;

namespace ExemploApiSettings.Connection.Contracts
{
    public interface ISubmitApi
    {
        void InicializarHttp(string pURLpadrao);
        Task<Resp> PostWithoutTokenAsync<Req, Resp>(Req Item, string pRequestMethod = "Login") where Req : class where Resp : BaseResponse, new();
    }
}
