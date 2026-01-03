using Microsoft.Data.SqlClient;
using Verifile.Models;

namespace Verifile.Repositories
{
    public class PersonRepository
    {
        private readonly string _connectionString;

        public PersonRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> SavePersonAsync(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO person (given_name, family_name, lastupdate)
                VALUES (@GivenName, @FamilyName, @LastUpdate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@GivenName", person.GivenName ?? string.Empty);
            command.Parameters.AddWithValue("@FamilyName", person.FamilyName ?? string.Empty);
            command.Parameters.AddWithValue("@LastUpdate", 
                person.LastUpdateDate ?? DateTime.UtcNow);

            int newId = (int)await command.ExecuteScalarAsync();
            return newId;
        }
    }
}