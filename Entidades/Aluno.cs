namespace WebAPI.Entidades
{
    public class Aluno
    {
        public int AlunoId { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public int CidadeId { get; set; }
        public byte[]? Foto { get; set; }
    }

}
