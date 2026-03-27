using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAPI.Entidades;
using WebAPI.Repository;
using MySql.Data.MySqlClient;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly Service.AlunoService _alunoService;

        public AlunosController(Service.AlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Recebe e armazena a foto do aluno.
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <param name="arquivo">Arquivo de imagem (multipart/form-data)</param>
        [HttpPost("{id}/foto")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UploadFoto(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            using var ms = new MemoryStream();
            arquivo.CopyTo(ms);
            byte[] foto = ms.ToArray();

            bool sucesso = _alunoService.saveFoto(id, foto);

            if (!sucesso)
                return NotFound("Aluno não encontrado.");

            return Ok("Foto salva com sucesso.");
        }

        /// <summary>
        /// Retorna a foto do aluno em Base64.
        /// </summary>
        /// <param name="id">ID do aluno</param>
        [HttpGet("{id}/foto")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult BuscarFoto(int id)
        {
            byte[]? foto = _alunoService.getFoto(id);

            if (foto == null)
                return NotFound("Foto não encontrada.");

            return Ok(new { foto = Convert.ToBase64String(foto) });
        }

        /// <summary>
        /// Cria aluno.
        /// </summary>
        /// <param name="aluno">Json</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Gravar(Entidades.Aluno aluno)
        {
            if (aluno == null)
                return BadRequest("Dados do aluno são obrigatórios.");

            try
            {
                _alunoService.Criar(aluno);
                return Ok("Aluno criado com sucesso!");
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Falha ao gravar os dados no banco.",
                    Detalhe = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
            }
        }

        /// <summary>
        /// Obter o aluno por id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Aluno cadastrado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            try
            {
                var aluno = _alunoService.Obter(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado.");
                return StatusCode(200, aluno);
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Falha ao acessar o banco de dados.",
                    Detalhe = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
            }
        }
    }
}
