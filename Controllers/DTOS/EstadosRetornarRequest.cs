namespace WebAPI.Controllers.DTOS
{
    public class EstadosRetornarRequest
    {
        public string Sigla { get; set; }

        public EstadosRetornarRequest(string sigla)
        {
            this.Sigla = sigla;
        }
    }
}
