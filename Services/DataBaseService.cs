using System.Data.SqlClient;
using AmigoSecreto.Models;

namespace AmigoSecreto.Services;

public class DataBaseService
{
    private const string ConnectionString = "Server=DESKTOP-0JTQ3HI;Database=amg_secreto;Trusted_Connection=True;";

    public void AdicionarParticipante(string nome, string email, string presente, string mensagem)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string query = "INSERT INTO participantes (nome, email, presente, mensagem) VALUES (@nome, @email, @presente, @mensagem)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@presente", presente);
                cmd.Parameters.AddWithValue("@mensagem", mensagem);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Participante> ObterParticipantes()
    {
        var participantes = new List<Participante>();

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string query = "SELECT id_part, nome, email, presente, mensagem FROM participantes";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    participantes.Add(new Participante
                    {
                        Id = Convert.ToInt32(reader["id_part"]),
                        Nome = reader["nome"].ToString(),
                        Email = reader["email"].ToString(),
                        Presente = reader["presente"].ToString(),
                        Mensagem = reader["mensagem"].ToString()
                    });
                }
            }
        }

        return participantes;
    }

    public List<Sorteio> ObterSorteios()
    {
        var sorteios = new List<Sorteio>();

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            string query = @"
                SELECT p1.nome AS Participante, p2.nome AS AmigoSecreto
                FROM sorteios
                JOIN participantes p1 ON sorteios.id_part = p1.id_part
                JOIN participantes p2 ON sorteios.id_amigo_secreto = p2.id_part";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sorteios.Add(new Sorteio
                    {
                        Participante = reader["Participante"].ToString(),
                        AmigoSecreto = reader["AmigoSecreto"].ToString()
                    });
                }
            }
        }

        return sorteios;
    }

    public void RealizarSorteio(List<Participante> participantes)
    {
        var random = new Random();

        foreach (var participante in participantes)
        {
            // Garante que o amigo secreto seja diferente do próprio participante
            var amigoSecreto = participantes[random.Next(participantes.Count)];
            while (amigoSecreto.Id == participante.Id)
            {
                amigoSecreto = participantes[random.Next(participantes.Count)];
            }

            // Inserir no banco de dados
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO sorteios (id_part, id_amigo_secreto) VALUES (@id_part, @id_amigo_secreto)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id_part", participante.Id);
                    cmd.Parameters.AddWithValue("@id_amigo_secreto", amigoSecreto.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}