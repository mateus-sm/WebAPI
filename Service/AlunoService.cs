using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;

namespace WebAPI.Service
{
    public class AlunoService(Repository.AlunoRepository repository)
    {
        private readonly Repository.AlunoRepository _repository = repository;

        public void ValidaAluno(Entidades.Aluno aluno)
        {
            if (string.IsNullOrWhiteSpace(aluno.Nome))
                throw new Exception("Nome vazio.");

            // Aceita letras, espaços, hífens e apóstrofos
            if (!Regex.IsMatch(aluno.Nome, @"^[\p{L}]+([ '-][\p{L}]+)*$"))
                throw new Exception("Nome com caracteres invalidos.");

            if (aluno.Idade < 0)
                throw new Exception("Idade negativa.");

            if (aluno.CidadeId < 1 || aluno.CidadeId > 5570)
                throw new Exception("Codigo da cidade fora dos limites do IBGE.");
        }

        public bool Criar(Entidades.Aluno aluno)
        {
            ValidaAluno(aluno);
            return _repository.Criar(aluno);
        }

        public bool Alterar(Entidades.Aluno aluno)
        {
            ValidaAluno(aluno);
            return _repository.Alterar(aluno);
        }

        public Entidades.Aluno? Obter(int id)
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

        public bool SaveFoto(int id, byte[] foto)
        {
            return _repository.SalvarFoto(id, foto);
        }

        public byte[]? GetFoto(int id)
        {
            return _repository.BuscarFoto(id);
        }
    }
}
