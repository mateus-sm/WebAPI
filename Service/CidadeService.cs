namespace WebAPI.Service
{
    public class CidadeService
    {
        private readonly Repository.CidadeRepository _cidRepository;

        public CidadeService(Repository.CidadeRepository repo)
        {
            _cidRepository = repo;   
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
    }
}
