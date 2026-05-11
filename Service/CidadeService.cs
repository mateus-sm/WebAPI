using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebAPI.Service
{
    public class CidadeService(Repository.CidadeRepository repo)
    {
        private readonly Repository.CidadeRepository _cidRepository = repo;

        public void SalvarCidadesEmLote(List<Entidades.Cidade> cidades)
        {
            _cidRepository.SaveAllCities(cidades);
        }

        public void SalvarCidadesEmLote2(List<Entidades.Cidade> cidades)
        {
            _cidRepository.SaveAllCities2(cidades);
        }

        public Entidades.Cidade? MapearLinhaCSV(string linha)
        {
            if (string.IsNullOrWhiteSpace(linha))
                return null;

            var partes = linha.Split(',');
            if (partes.Length < 5) return null;

            try
            {
                return new Entidades.Cidade
                {
                    CidadeId = int.Parse(partes[0]),
                    Nome = partes[1].Trim().Trim('"'),
                    Sigla = partes[2].Trim(),
                    IBGEMunicipio = int.Parse(partes[3].Trim()),
                    Latitude = decimal.TryParse(partes[4].Trim(), CultureInfo.InvariantCulture, out var lat) ? lat : null,
                    Longitude = decimal.TryParse(partes[5].Trim(), CultureInfo.InvariantCulture, out var lon) ? lon : null
                };
            }
            catch
            {
                return null;
            }
        }

        public bool CriarCidade(Entidades.Cidade cidade)
        {
            return _cidRepository.Create(cidade);
        }

        public List<Entidades.Cidade> LerTodasCidades()
        {
            return _cidRepository.Read();
        }

        public Entidades.Cidade? LerCidadePorId(int id)
        {
            return _cidRepository.ReadById(id);
        }

        public int LerQuantidadeCidades()
        {
            return _cidRepository.Count();
        }

        public List<string> LerEstados()
        {
            return _cidRepository.ReadEstados();
        }

        public List<Entidades.Cidade> LerCidadesPorEstado(string sigla)
        {
            return _cidRepository.ReadByEstado(sigla);
        }

        public bool AtualizarCidade(Entidades.Cidade cidade)
        {
            return _cidRepository.Update(cidade);
        }

        public bool DeletarCidade(int id)
        {
            return _cidRepository.Delete(id);
        }
    }
}
