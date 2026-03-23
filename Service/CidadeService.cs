using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebAPI.Service
{
    public class CidadeService
    {
        private readonly Repository.CidadeRepository _cidRepository;

        public CidadeService(Repository.CidadeRepository repo)
        {
            _cidRepository = repo;   
        }

        public void SalvarCidadesEmLote(List<Entidades.Cidade> cidades)
        {
            _cidRepository.SaveAllCities(cidades);
        }

        public void SalvarCidadesEmLote2(List<Entidades.Cidade> cidades)
        {
            _cidRepository.SaveAllCities2(cidades);
        }

        public Entidades.Cidade MapearLinhaCSV(string linha)
        {
            if (string.IsNullOrWhiteSpace(linha)) return null;

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

        public bool criarCidade(Entidades.Cidade cidade)
        {
            return _cidRepository.Create(cidade);
        }

        public List<Entidades.Cidade> lerTodasCidades()
        {
            return _cidRepository.Read();
        }

        public Entidades.Cidade lerCidadePorId(int id)
        {
            return _cidRepository.ReadById(id);
        }

        public int lerQuantidadeCidades()
        {
            return _cidRepository.Count();
        }

        public List<string> lerEstados()
        {
            return _cidRepository.ReadEstados();
        }
    }
}
