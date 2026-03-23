namespace WebAPI.Service
{
    public class AlunoService
    {
        private readonly Repository.AlunoRepository _repository;

        public AlunoService(Repository.AlunoRepository repository)
        {
            _repository = repository;
        }

        public bool Criar(Entidades.Aluno aluno)
        {

            //aplica regra de negócio
            return _repository.Criar(aluno);

        }

        public bool Alterar(Entidades.Aluno aluno)
        {

            //aplica regra de negócio
            return _repository.Alterar(aluno);

        }

        public Entidades.Aluno Obter(int id)
        {
            return _repository.Obter(id);
        }

        public IEnumerable<Entidades.Aluno> ObterTodos()
        {
            return _repository.ObterTodos();
        }

        public IEnumerable<Entidades.Aluno> Consultar(string nome)
        {
            return _repository.Consultar(nome);
        }

        public int TotalAlunos()
        {
            return _repository.TotalAlunos();
        }


        public void Excluir(int id)
        {
            _repository.Excluir(id);

        }

        public bool AlunoExistente(string nome)
        {
            return _repository.AlunoExistente(nome);
        }

        public bool AlunoExistente(int id)
        {
            return _repository.AlunoExistente(id);
        }

        public bool saveFoto(int id, byte[] foto)
        {
            return _repository.SalvarFoto(id, foto);
        }

        public byte[]? getFoto(int id)
        {
            return _repository.BuscarFoto(id);
        }
    }
}
