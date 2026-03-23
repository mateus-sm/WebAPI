using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebAPI.Entidades;

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
        /// Importa cidades de um arquivo .csv e insere no banco de dados.
        /// </summary>
        /// <param name="usarLote">Set <c>true</c> para processar 1000 linhas por vez.</param>
        /// <returns>Mensagem de confirmação</returns>
        [HttpPost("/importar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ReceberArquivoCSV(IFormFile arquivo, [FromQuery] bool usarLote)
        {
            // 1. Valida se o arquivo realmente chegou e se não está vazio
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado ou o arquivo está vazio.");
            }

            // 2. Valida se a extensão é realmente um .csv
            if (!arquivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Formato inválido. Por favor, envie um arquivo .csv");
            }

            try
            {
                // 3. Abre um fluxo de leitura (Stream) para ler o conteúdo do arquivo
                using (var stream = arquivo.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    // Lê o arquivo inteiro como uma única string
                    // var conteudo = reader.ReadToEnd();

                    // Lê linha por linha e transforma em lista
                    var cidadesParaSalvar = new List<Entidades.Cidade>();
                    int numeroLinha = 1;

                    if (!reader.EndOfStream)
                    {
                        reader.ReadLine(); // Pula a primeira linha
                    }

                    while (!reader.EndOfStream)
                    {
                        var linha = reader.ReadLine();
                        var cidade = _cidService.MapearLinhaCSV(linha);

                        if (cidade == null)
                        {
                            return StatusCode(400, $"Erro de formatação na linha {numeroLinha}: {linha}. Correção necessária para prosseguir.");
                        }

                        cidadesParaSalvar.Add(cidade);
                        numeroLinha++;
                    }

                    // 4. Fase de Inserção (Tudo ou Nada)
                    try
                    {
                        if (usarLote)
                        {
                            _cidService.SalvarCidadesEmLote2(cidadesParaSalvar);
                        } else
                        {
                            _cidService.SalvarCidadesEmLote(cidadesParaSalvar);
                        }
                        return Ok(new { Mensagem = $"{cidadesParaSalvar.Count} cidades importadas com sucesso!" });
                    }
                    catch (MySqlException ex)
                    {
                        // Esse throw vem direto do repository
                        return StatusCode(500, new
                        {
                            Erro = "Falha ao gravar os dados no banco.",
                            Detalhe = ex.Message
                        });
                    }
                    catch (Exception ex)
                    {
                        // Qualquer outro throw
                        return StatusCode(500, $"Erro interno na API: {ex.Message}");
                    }

                }
            }
            catch (Exception ex)
            {
                // Retorna erro 500 caso algo dê errado na leitura
                return StatusCode(500, $"Erro interno ao processar o arquivo: {ex.Message}");
            }
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
        public IActionResult RetornarTodasCidades()
        {
            try
            {
                var cidades = _cidService.lerTodasCidades();

                if (cidades == null || !cidades.Any())
                {
                    return NotFound("Nenhuma cidade encontrada no banco de dados.");
                }

                return Ok(cidades);
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Falha ao buscar os dados no banco.",
                    Detalhe = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
            }
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
        public IActionResult CriarCidade(Entidades.Cidade cidade)
        {
            try
            {
                _cidService.criarCidade(cidade);
                return StatusCode(210, "Criado com sucesso");
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
            } 
            catch (MySqlException ex)
            {
                return Problem(
                     title: "Erro inesperado",
                     detail: ex.Message,
                     statusCode: StatusCodes.Status500InternalServerError
                 );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
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
        /// Retorna lista de estados cadastradas no banco.
        /// </summary>
        /// <returns>Lista com itens</returns>
        [HttpGet("/estados")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RetornarEstados()
        {
            try
            {
                return Ok(_cidService.lerEstados());
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
        /// Retorna lista de cidades filtradas por estados.
        /// </summary>
        /// <returns>Lista com itens</returns>
        [HttpGet("/estado/{uf}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RetornarCidadesPorEstados(string uf)
        {
            //Validações
            if (string.IsNullOrWhiteSpace(uf))
            {
                return BadRequest("A UF não pode ser nula ou vazia.");
            }
            uf = uf.Trim();
            if (uf.Length != 2)
            {
                return BadRequest("A UF deve conter exatamente 2 caracteres (ex: SP, RJ).");
            }

            //Fluxo
            try
            {
                List<Entidades.Cidade> cidades = _cidService.lerCidadesPorEstado(uf);

                if (cidades == null || cidades.Count == 0)
                {
                    return StatusCode(404, $"Cidades não encontradas: {uf}.");
                }

                return Ok(cidades);    
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
        /// Alterar cidade existente no banco.
        /// </summary>
        /// <param name="cidade"></param>
        /// <returns>Flag de cofnirmação</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AlterarCidade(Entidades.Cidade cidade)
        {
            try
            {
                bool flag = _cidService.atualizarCidade(cidade);
                return Ok("Alterado com sucesso.");
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Falha ao alterar os dados no banco.",
                    Detalhe = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletar cidade existente no banco.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Flag de confirmação</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletarCidade(int id)
        {
            try
            {
                bool flag = _cidService.deletarCidade(id);
                return Ok("Deletado com sucesso.");
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new
                {
                    Erro = "Falha ao alterar os dados no banco.",
                    Detalhe = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno na API: {ex.Message}");
            }
        }

        //Template fluxo APIs
        //try
        //{
        //    //Chamada service
        //    return Ok("");
        //}
        //catch (MySqlException ex)
        //{
        //    return StatusCode(500, new
        //    {
        //        Erro = "Falha ao gravar os dados no banco.",
        //        Detalhe = ex.Message
        //    });
        //}
        //catch (Exception ex)
        //{
        //    return StatusCode(500, $"Erro interno na API: {ex.Message}");
        //}
    }
}
