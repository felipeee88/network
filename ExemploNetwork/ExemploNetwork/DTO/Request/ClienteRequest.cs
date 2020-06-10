using System;

namespace ExemploApiSettings.DTO.Request
{
    public class ClienteRequest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataDeNascimento { get; set; }
    }
}
