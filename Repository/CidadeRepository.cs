using MySql.Data.MySqlClient;
using WebAPI.Entidades;

namespace WebAPI.Repository
{
    public class CidadeRepository
    {
        private readonly MySqlDbContext _conexao;

        public CidadeRepository(MySqlDbContext conexao)
        {
            _conexao = conexao;
        }

        public bool Create(Entidades.Cidade cidade)
        {
            bool flag = false;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO aluno20.Cidades (Nome, Sigla, IBGEMunicipio, Latitude, Longitude)
                                         VALUES (@Nome, @Sigla, @IBGEMunicipio, @Latitude, @Longitude);";

                    cmd.Parameters.AddWithValue("@Nome", cidade.Nome);
                    cmd.Parameters.AddWithValue("@Sigla", cidade.Sigla);
                    cmd.Parameters.AddWithValue("@IBGEMunicipio", cidade.IBGEMunicipio);
                    cmd.Parameters.AddWithValue("@Latitude", cidade.Latitude);
                    cmd.Parameters.AddWithValue("@Longitude", cidade.Longitude);

                    cmd.ExecuteNonQuery();
                    cidade.CidadeId = (int)cmd.LastInsertedId;
                    
                    flag = true;
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return flag;
        }

        public List<Entidades.Cidade> Read()
        {
            List<Entidades.Cidade> cid = new();
            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM aluno20.Cidades";
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        cid.Add(Map(dr));
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }
            return cid;
        }

        public Entidades.Cidade ReadById(int id)
        {
            Cidade cid = null;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM aluno20.Cidades WHERE CidadeId = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        cid = Map(dr);
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return cid;
        }

        public int Count()
        {
            int count = 0;

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM aluno20.Cidades";
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return count;
        }

        public List<Controllers.DTOS.EstadosRetornarRequest> ReadEstados()
        {
            List<Controllers.DTOS.EstadosRetornarRequest> estados = new();

            try
            {
                using (var cmd = _conexao.GetConexao().CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT Sigla FROM aluno20.Cidades";
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        estados.Add(new Controllers.DTOS.EstadosRetornarRequest(
                            dr.GetString("Sigla")
                        ));
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw;
            }

            return estados;
        }

        public Entidades.Cidade Map(MySql.Data.MySqlClient.MySqlDataReader dr)
        {
            Cidade cid = new Cidade();
            cid.CidadeId = dr.GetInt32("CidadeId");
            cid.Nome = dr.GetString("Nome");
            cid.Sigla = dr.GetString("Sigla");
            cid.IBGEMunicipio = dr.GetInt32("IBGEMunicipio");
            cid.Latitude = dr.GetDecimal("Latitude");
            cid.Longitude = dr.GetDecimal("Longitude");
            return cid;
        }
    }
}
