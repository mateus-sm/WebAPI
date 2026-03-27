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

        public bool SalvarFoto(int id, byte[] foto)
        {
            bool sucesso = false;
            MySql.Data.MySqlClient.MySqlTransaction? transacao = null;

            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    transacao = _context.GetConexao().BeginTransaction();
                    cmd.Transaction = transacao;

                    cmd.CommandText = "UPDATE Aluno SET Foto = @Foto WHERE AlunoId = @AlunoId";
                    cmd.Parameters.AddWithValue("@Foto", foto);
                    cmd.Parameters.AddWithValue("@AlunoId", id);

                    cmd.ExecuteNonQuery();
                    transacao.Commit();
                    sucesso = true;
                }
            }
            catch (MySqlException)
            {
                transacao?.Rollback();
                throw;
            }

            return sucesso;
        }

        public byte[]? BuscarFoto(int id)
        {
            using (var cmd = _context.GetConexao().CreateCommand())
            {
                cmd.CommandText = "SELECT Foto FROM Aluno WHERE AlunoId = @AlunoId";
                cmd.Parameters.AddWithValue("@AlunoId", id);

                var dr = cmd.ExecuteReader();

                if (dr.Read() && !dr.IsDBNull(dr.GetOrdinal("Foto")))
                {
                    var foto = (byte[])dr["Foto"];
                    dr.Close();
                    return foto;
                }

                dr.Close();
                return null;
            }
        }

        public bool Criar(Entidades.Aluno aluno)
        {
            bool sucesso = false;
            try
            {
                using (var cmd = _context.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"insert Aluno(AlunoId, Nome, Idade, CidadeId, Foto) 
                                        values (@AlunoId, @Nome, @Idade, @CidadeId, @Foto)";

                    cmd.Parameters.AddWithValue("@AlunoId", aluno.AlunoId);
                    cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                    cmd.Parameters.AddWithValue("@Idade", aluno.Idade);
                    cmd.Parameters.AddWithValue("@CidadeId", aluno.CidadeId);
                    cmd.Parameters.AddWithValue("@Foto", aluno.Foto);

                    //insert, update, delete e sp
                    cmd.ExecuteNonQuery();
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
                    cmd.Parameters.AddWithValue("@AlunoId", aluno.AlunoId);
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        cmd.CommandText = @"insert AlunoHistorico(AlunoId, Nome, Idade, CidadeId) 
                                           values (@AlunoId, @Nome, @Idade, @CidadeId) ";

                        cmd.Parameters.AddWithValue("@AlunoId", aluno.AlunoId);
                        cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                        cmd.Parameters.AddWithValue("@Idade", aluno.Idade);
                        cmd.Parameters.AddWithValue("@CidadeId", aluno.CidadeId);
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
                    cmd.Parameters.AddWithValue("@Idade", aluno.Idade);
                    cmd.Parameters.AddWithValue("@CidadeId", aluno.CidadeId);
                    cmd.Parameters.AddWithValue("@AlunoId", aluno.AlunoId);

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

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            aluno = Map(dr);
                        }
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
            aluno.AlunoId = dr.GetInt32("AlunoId");
            aluno.Nome = dr.GetString("Nome");
            aluno.Idade = dr.GetInt32("Idade");
            aluno.CidadeId = dr.GetInt32("CidadeId");
            aluno.Foto = dr["Foto"] as byte[];
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
