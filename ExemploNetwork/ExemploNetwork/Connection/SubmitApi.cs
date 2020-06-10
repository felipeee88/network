using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ExemploApiSettings.Connection.Contracts;
using ExemploApiSettings.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ExemploApiSettings.Connection
{
    public class SubmitApi : ISubmitApi
    {
        #region Construtor

        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private OptionHttpBasic OptionHttp { get; set; }

        public SubmitApi(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        #endregion

        #region Metodos Publicos

        public void InicializarHttp(string pURLpadrao)
        {
            ValidarConfiguracaoHttp(pURLpadrao);
            PrepararConfiguracaoHttp(pURLpadrao);
            InstanciarHttp();
        }

        public async Task<Resp> PostWithoutTokenAsync<Req, Resp>(Req Item, string pRequestMethod = "Login")
            where Req : class
            where Resp : BaseResponse, new()
        {
            Resp responseFail = new Resp();

            var json = JsonConvert.SerializeObject(Item);
            var response = await CreatePostAsync(pRequestMethod, json);

            return RetornarStatusCode<Resp>(response.StatusCode, response, pRequestMethod);
        }

        #endregion

        #region Metodos Privados

        private async Task<HttpResponseMessage> CreatePostAsync(string pRequestMethod, string json)
        {
            return await _client.PostAsync(string.Format("{0}{1}", _client.BaseAddress, pRequestMethod), new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        }

        private Resp RetornarStatusCode<Resp>(HttpStatusCode responseStatusCode, HttpResponseMessage response, string pRequestMethod) where Resp : BaseResponse, new()
        {
            Resp responseFail = new Resp();

            var conteudo = GerarResponseJson(response);
            var retorno = GerarResponseRetornoSemBase<Resp>(response, conteudo);

            switch (responseStatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.IMUsed:
                    return retorno;
                case HttpStatusCode.NotFound:
                    responseFail.StatusCode = HttpStatusCode.NotFound;
                    responseFail.Message = string.Format("Url da conexão não encotrada : {0} ContentResponse : Message: {1} StatusCode: {2} Success: {3} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo, retorno.StatusCode, retorno.Success);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.BadRequest:
                    responseFail.StatusCode = HttpStatusCode.BadRequest;
                    responseFail.Message = string.Format("Erro ao processar a requisição {0} ContentResponse : Message: {1} StatusCode: {2} Success: {3} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo, retorno.StatusCode, retorno.Success);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.Forbidden:
                    responseFail.StatusCode = HttpStatusCode.Forbidden;
                    responseFail.Message = string.Format("Erro ao processar a requisição, acesso negado : {0}{1} ContentResponse :{2} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.InternalServerError:
                    responseFail.StatusCode = HttpStatusCode.InternalServerError;
                    responseFail.Message = string.Format("Erro ao processar a requisição: {0}{1} ContentResponse :{2}", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    responseFail.StatusCode = HttpStatusCode.MethodNotAllowed;
                    responseFail.Message = string.Format("Erro ao processar metodo URL do Token, metodo não permitido : {0}{1} ContentResponse :{2} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    responseFail.StatusCode = HttpStatusCode.ServiceUnavailable;
                    responseFail.Message = string.Format("Erro ao processar a requisição, servidor indisponível : {0}{1} ContentResponse :{2} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
                case HttpStatusCode.Unauthorized:
                    responseFail.StatusCode = HttpStatusCode.Unauthorized;
                    responseFail.Message = string.Format("Erro ao processar a requisição, acesso não autorizado: {0}{1} ContentResponse :{2} ", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
                default:
                    responseFail.StatusCode = HttpStatusCode.InternalServerError;
                    responseFail.Message = string.Format("Erro ao processar a requisição: {0}{1} ContentResponse :{2}", _client.BaseAddress, pRequestMethod.Trim(), conteudo);
                    responseFail.Success = false;
                    break;
            }

            return responseFail;
        }

        private void ValidarConfiguracaoHttp(string pURLpadrao)
        {
            if (String.IsNullOrEmpty(pURLpadrao)) throw new Exception(string.Format(" Url de conexao Token '{0}' não informada, favor inserir parametro no request", pURLpadrao));
            if (_client == null) throw new Exception(string.Format(" Instancia do HttpClient não inicializada '{0}' ", _client));
        }

        private void PrepararConfiguracaoHttp(string pURLpadrao)
        {
            OptionHttp = new OptionHttpBasic()
            {
                URLBase = pURLpadrao
            };
        }

        private void InstanciarHttp()
        {
            _client.BaseAddress = new Uri(OptionHttp.URLBase);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static string GerarResponseJson(HttpResponseMessage response)
        {
            string conteudo = "";
            conteudo = response.Content.ReadAsStringAsync().Result;
            return conteudo;
        }

        private Resp GerarResponseRetornoSemBase<Resp>(HttpResponseMessage response, string conteudo) where Resp : class, new()
        {
            Resp retorno = null;

            if (response.Content != null)
            {
                retorno = JsonConvert.DeserializeObject<Resp>(conteudo);
            }

            return retorno;
        }

        private void ValidarChamadatHttp()
        {
            if (OptionHttp == null) throw new Exception(string.Format(" Voce não Inicializou a instancia HTTP antes de realizar a chamada da API, favor inicializar a instancia HTTP "));
        }

        private HttpResponseMessage CreateGet(string pRequestMethod)
        {
            return _client.GetAsync(string.Format("{0}{1}", _client.BaseAddress, pRequestMethod.Trim())).Result;
        }

        #endregion
    }
}
