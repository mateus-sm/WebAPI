using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebAPI.Entidades;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [Route("api/cidades")]
    [ApiController]
    public class CidadesController : ControllerBase
    {
        private readonly Service.CidadeService _cidService;

        public CidadesController(Service.CidadeService cidService)
        {
            _cidService = cidService;
        }

        /// <summary>
        /// Retorna todas as cidades cadastradas no banco.
        /// </summary>
        /// <returns>Cidades encontradas</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IEnumerable<Entidades.Cidade> RetornarTodasCidades()
        {
            return _cidService.lerTodasCidades();
        }

        /// <summary>
        /// Insere manualmente uma ciadade. (Teste)
        /// </summary>
        /// <param name="cidade"></param>
        /// <returns>Bool indicando sucesso</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool CriarCidade(Entidades.Cidade cidade)
        {
            return _cidService.criarCidade(cidade);
        }

        /// <summary>
        /// Retorna uma cidade pelo id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Cidade encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult RetornarCidadePorId(int id)
        {
            try
            {
                var cidade = _cidService.lerCidadePorId(id);

                if (cidade == null)
                {
                    return NotFound();
                }

                return Ok(cidade);
            } catch (MySqlException ex)
            {
                return Problem(
                     title: "Erro inesperado",
                     detail: ex.Message,
                     statusCode: StatusCodes.Status500InternalServerError
                 );
            }

        }

        /// <summary>
        /// Retorna quantidade de cidades cadastradas no banco.
        /// </summary>
        /// <returns>Quantidade</returns>
        [HttpGet("/total")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RetornarQuantidadeCidades()
        {
            try
            {
                var quantidade = _cidService.lerQuantidadeCidades();
                return Ok(new { TotalCidades = quantidade });
            }
            catch (MySqlException ex)
            {
                return Problem(
                     title: "Erro inesperado",
                     detail: ex.Message,
                     statusCode: StatusCodes.Status500InternalServerError
                 );
            }
        }





    }
}
