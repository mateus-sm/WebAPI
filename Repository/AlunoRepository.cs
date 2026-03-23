using MySql.Data.MySqlClient;
using WebAPI.Entidades;

namespace WebAPI.Repository
{
    public class AlunoRepository
    {
        private readonly MySqlDbContext _context;

        public AlunoRepository(MySqlDbContext context)
        {
            _context = context;
        }


        public bool Criar(Entidades.Aluno aluno)
        {

            bool sucesso = false;
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"insert Aluno(Nome, Idade, CidadeId) 
                                        values (@Nome, @Idade, @CidadeId) ";


                    cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                    cmd.Parameters.AddWithValue("@Idade", aluno.Id);
                    cmd.Parameters.AddWithValue("@CidadeId", aluno.Cidade.CidadeId);

                    //insert, update, delete e sp
                    cmd.ExecuteNonQuery();

                    aluno.Id = (int)cmd.LastInsertedId;

                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return sucesso;

        }

        public bool Alterar(Entidades.Aluno aluno)
        {

            bool sucesso = false;
            MySql.Data.MySqlClient.MySqlTransaction? transacao = null;
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    transacao = _context.GetConexao().BeginTransaction();

                    cmd.CommandText = "select * from Aluno where AlunoId = @AlunoId";
                    cmd.Parameters.AddWithValue("@AlunoId", aluno.Id);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        cmd.CommandText = @"insert AlunoHistorico(AlunoId, Nome, Idade, CidadeId) 
                                           values (@AlunoId, @Nome, @Idade, @CidadeId) ";

                        cmd.Parameters.AddWithValue("@AlunoId", aluno.Id);
                        cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                        cmd.Parameters.AddWithValue("@Idade", aluno.Id);
                        cmd.Parameters.AddWithValue("@CidadeId", aluno.Cidade.CidadeId);
                        cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                        //insert, update, delete e sp
                        cmd.ExecuteNonQuery();
                    }


                    cmd.Parameters.Clear();
                    cmd.CommandText = @"update Aluno 
                                        set Nome = @Nome, 
                                            Idade = @Idade,
                                            CidadeId = @CidadeId
                                        where AlunoId = @AlunoId";

                    cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                    cmd.Parameters.AddWithValue("@Idade", aluno.Id);
                    cmd.Parameters.AddWithValue("@CidadeId", aluno.Cidade.CidadeId);
                    cmd.Parameters.AddWithValue("@AlunoId", aluno.Id);

                    //insert, update, delete e sp
                    cmd.ExecuteNonQuery();
                    transacao.Commit();
                    sucesso = true;
                }
            }
            catch (MySqlException ex)
            {
                transacao?.Rollback();

                //serilog...
                throw;
            }

            return sucesso;

        }

        public void Excluir(int id)
        {
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"delete from Aluno where AlunoId = " + id;

                    //insert, update, delete e sp
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }
        }


        public Entidades.Aluno Obter(int id)
        {

            Entidades.Aluno aluno = null;
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select * 
                                        from Aluno 
                                        where AlunoId = " + id;

                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        aluno = Map(dr);
                    }
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return aluno;

        }

        public IEnumerable<Entidades.Aluno> ObterTodos()
        {

            List<Entidades.Aluno> alunos = new();
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select * 
                                        from Aluno";

                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        alunos.Add(Map(dr));
                    }
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return alunos;

        }

        public IEnumerable<Entidades.Aluno> Consultar(string nome)
        {

            List<Entidades.Aluno> alunos = new();
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select * 
                                        from Aluno
                                        where Nome like @Nome";

                    cmd.Parameters.AddWithValue("@Nome", nome + "%");
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        alunos.Add(Map(dr));
                    }
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return alunos;

        }

        public Entidades.Aluno Map(MySql.Data.MySqlClient.MySqlDataReader dr)
        {
            Aluno aluno = new Aluno();
            //aluno.Id = Convert.ToInt32(dr["AlunoId"]);
            //aluno.Nome = dr["Nome"].ToString();
            aluno.Id = dr.GetInt32("AlunoId");
            aluno.Nome = dr.GetString("Nome");
            aluno.Idade = dr.GetInt32("Idade");
            aluno.Cidade = new Cidade()
            {
                CidadeId = dr.GetInt32("CidadeId")
            };

            return aluno;
        }


        public int TotalAlunos()
        {
            int total = 0;

            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select count(*) from Aluno";

                    var aux = cmd.ExecuteScalar();

                    total = Convert.ToInt32(aux);
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return total;

        }

        public bool AlunoExistente(string nome)
        {
            int total = 0;

            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select count(*) 
                                        from Aluno 
                                        where Nome = @Nome";

                    cmd.Parameters.AddWithValue("@Nome", nome);
                    var aux = cmd.ExecuteScalar();

                    total = Convert.ToInt32(aux);
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return total == 1;

        }

        public bool AlunoExistente(int id)
        {
            int total = 0;

            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"select count(*) 
                                        from Aluno
                                        where AlunoId = @AlunoId";

                    cmd.Parameters.AddWithValue("@AlunoId", id);
                    var aux = cmd.ExecuteScalar();

                    total = Convert.ToInt32(aux);
                }
            }
            catch (MySqlException ex)
            {

                //serilog...
                throw;
            }

            return total == 1;

        }
    }
}
