using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using WebAPI.Entidades;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AlunosController(Service.AlunoService alunoService) : ControllerBase
    {
        private readonly Service.AlunoService _alunoService = alunoService;

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

            bool sucesso = _alunoService.SaveFoto(id, foto);

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
            byte[]? foto = _alunoService.GetFoto(id);

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

        /// <summary>
        /// Retorna registros de todos os alunos.
        /// </summary>
        /// <remarks>
        /// Essa ação retorna uma lista de estudantes tecuperadas do banco. Se houver erro durante o acesso ao banco
        /// a resposta incluirá os detalhes. O tipo de resposta e código do status indicas o resultado a operação.
        /// </remarks>
        /// <returns>Um <see cref="IActionResult"/> contendo um 200 Ok com a respostan sendo a lista se encontrado; 404 Not
        /// Found se nenhum aluno for encontrado; ou 500 Internal Server Error se ocorrer erro acessando o banco ou se ocorrer erro interno na API
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var alunos = _alunoService.ObterTodos();
                if (alunos == null)
                    return NotFound("Alunos não encontrados.");
                return StatusCode(200, alunos);
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

        /// <summary>
        /// Atualiza as informações de um aluno existente.
        /// </summary>
        /// <remarks>Utilize este método para atualizar os dados de um aluno já cadastrado. Certifique-se
        /// de fornecer um objeto válido e completo. Em caso de falha ao acessar o banco de dados, uma mensagem
        /// detalhada será retornada no corpo da resposta.</remarks>
        /// <param name="alu">O objeto <see cref="Aluno"/> contendo os dados atualizados do aluno. Não pode ser nulo.</param>
        /// <returns>Um <see cref="IActionResult"/> que indica o resultado da operação. Retorna Status 200 (OK) com o aluno
        /// atualizado em caso de sucesso, Status 400 (BadRequest) se o parâmetro for nulo, ou Status 500
        /// (InternalServerError) em caso de erro interno.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Atualizar(Aluno alu)
        {
            if (alu == null)
                return BadRequest("Aluno é obrigatório.");

            try
            {
                if(_alunoService.Alterar(alu))
                    return Ok(alu);

                return StatusCode(500, "Erro ao tentar alterar.");    
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

        /// <summary>
        /// Obtém informações do estudante com base no nome especificado.
        /// </summary>
        /// <remarks>Este endpoint retorna um único registro de estudante correspondente ao nome fornecido. Se vários
        /// estudantes compartilharem o mesmo nome, apenas a primeira correspondência poderá ser retornada, dependendo da implementação do serviço.
        /// O formato da resposta e o tratamento de erros seguem as convenções padrão de API.</remarks>
        /// <param name="name">O nome do estudante a ser pesquisado. Não pode ser nulo ou vazio.</param>
        /// <returns>Uma resposta HTTP 200 contendo os dados do estudante, se encontrado; uma resposta HTTP 400 se o nome for nulo ou
        /// vazio; ou uma resposta HTTP 500 se ocorrer um erro interno no servidor ou no banco de dados.</returns>
        [HttpGet("nomes/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Nome vazio");

            try
            {
                var alu = _alunoService.Consultar(name);
                return Ok(alu);
            } 
            catch (MySqlException ex)
            {
                return StatusCode(500, new { Mensagem = "Erro no Banco de dados", Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno na API", Erro = ex.Message });
            }
        }

        /// <summary>
        /// Retorna a quantidade total de alunos registrados.
        /// </summary>
        /// <remarks>Utilize este endpoint para obter rapidamente o número total de alunos cadastrados no
        /// sistema. Em caso de falha no banco de dados ou erro interno, uma resposta HTTP 500 será retornada com
        /// informações sobre o erro.</remarks>
        /// <returns>Um resultado HTTP 200 contendo um objeto com a quantidade total de alunos se a operação for bem-sucedida;
        /// caso contrário, um resultado HTTP 500 com detalhes do erro.</returns>
        [HttpGet("qtdAlunos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ContarAlunos()
        {
            try
            {
                var qtd = _alunoService.TotalAlunos();
                return Ok(new {QuantidadeAlunos = qtd});
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { Mensagem = "Erro no Banco de dados", Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno na API", Erro = ex.Message });
            }
        }

        /// <summary>
        /// Exclui o aluno identificado pelo ID especificado.
        /// </summary>
        /// <remarks>Esta ação remove permanentemente o aluno do sistema. Em caso de erro no banco de
        /// dados ou falha interna, uma mensagem de erro detalhada é retornada.</remarks>
        /// <param name="id">O identificador do aluno a ser excluído. Deve ser maior ou igual a zero.</param>
        /// <returns>Um resultado de ação que indica o sucesso ou a falha da operação. Retorna 200 (OK) se o aluno for excluído
        /// com sucesso; 400 (Bad Request) se o ID for inválido; ou 500 (Internal Server Error) em caso de erro interno.</returns>
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            if (id < 0)
                return BadRequest("Id invalido, menor que zero.");

            try
            {
                _alunoService.Excluir(id);
                return Ok(new { mensagem = $"Aluno {id} excluido!"});
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { Mensagem = "Erro no Banco de dados", Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno na API", Erro = ex.Message });
            }
        }
    }
}
