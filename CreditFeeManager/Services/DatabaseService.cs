using System.Data.SqlClient;
using CreditFeeManager.Models;

namespace CreditFeeManager.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<StudentType> GetStudentTypes()
        {
            var studentTypes = new List<StudentType>();
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Id, Name, Description FROM StudentType", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            studentTypes.Add(new StudentType
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Description = reader.GetString("Description")
                            });
                        }
                    }
                }
            }
            
            return studentTypes;
        }

        public List<ModuleType> GetModuleTypes()
        {
            var moduleTypes = new List<ModuleType>();
            
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT Id, Name, Description FROM ModulesType", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            moduleTypes.Add(new ModuleType
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("Name"),
                                Description = reader.GetString("Description")
                            });
                        }
                    }
                }
            }
            
            return moduleTypes;
        }

        public void SaveCreditFees(List<CreditFee> creditFees)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Xóa dữ liệu cũ
                        using (var command = new SqlCommand("DELETE FROM CreditFees", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Thêm dữ liệu mới
                        foreach (var fee in creditFees)
                        {
                            using (var command = new SqlCommand(
                                "INSERT INTO CreditFees (StudentTypeId, ModuleTypeId, StudyType, Fee) VALUES (@StudentTypeId, @ModuleTypeId, @StudyType, @Fee)", 
                                connection, transaction))
                            {
                                command.Parameters.AddWithValue("@StudentTypeId", fee.StudentTypeId);
                                command.Parameters.AddWithValue("@ModuleTypeId", fee.ModuleTypeId);
                                command.Parameters.AddWithValue("@StudyType", fee.StudyType);
                                command.Parameters.AddWithValue("@Fee", fee.Fee);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}