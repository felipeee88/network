using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ExemploApiSettings.DTO
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = true;
            Message = "OK";
            StatusCode = HttpStatusCode.OK;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
