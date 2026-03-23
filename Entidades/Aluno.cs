namespace WebAPI.Entidades
{
    public class Aluno
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public Cidade Cidade { get; set; }
    }

}
