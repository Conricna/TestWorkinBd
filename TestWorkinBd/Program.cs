
using Npgsql;
using System.Data;
using System.Data.SqlClient;

namespace TestWorkinBd
{
    internal class Programs
    {
        
        static void Main(string[] args)
        {
            var cs = "Server=localhost; Port=5432; Database=testBd; UserId=postgres; Password=123456; commandTimeout=120;";
            using var con = new NpgsqlConnection(cs);
                       
            try
            {
                con.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Error connecting to the database", e);
            }
            // поиск суммы Заключённых договоров от 01.01.2023
           
            string sql = "SELECT  SUM(contractsum) AS AllContractSum FROM contract where data >= '2023-01-01' AND data < '2024-01-01' AND status = 'заключён'";
            
            using var cmd = new NpgsqlCommand(sql, con);

            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            Console.WriteLine("\nЗадание #1\n");
            while (rdr.Read())
            {                
                var Allcontractsum = rdr.GetFloat(0);

                Console.WriteLine($"Общая сумма заключённых контрактов за 2023 год:\t {Allcontractsum} \n");

            }

            con.Close();

            /////////////
            con.Open();


           string sql2 = "SELECT contract.legal_id, companyname ,SUM(contractsum) as allcontractsum " +
                "FROM contract LEFT JOIN legal ON contract.legal_id = legal.legal_id" +
                " WHERE legal.country = 'Russia' AND status = 'заключён'" +
                " Group by contract.legal_id, companyname";
            
            // сумма договоров компаний из россии
            using var cmd2 = new NpgsqlCommand(sql2, con);
            Console.WriteLine("\nЗадание #2\n");
            Console.WriteLine("Заключённые договора, юридических лиц из России:\n");
            using NpgsqlDataReader rdr2 = cmd2.ExecuteReader();

            while (rdr2.Read())
            {
                var individual_id = rdr2.GetInt32(0);
                var firstname = rdr2.GetString(1);
                var allcontractsum = rdr2.GetFloat(2);
               
                Console.WriteLine($"id: {individual_id}\t юридическое лицо: {firstname} \t Сумма всех договоров: {allcontractsum} ");
                

            }
            
            
            con.Close();


            con.Open();

            string sql3 = "SELECT contract.individual_id, firstname , email ,SUM(contractsum) as allcontractsum , DATE(data) as date " +
                 " FROM contract LEFT JOIN individual ON contract.individual_id = individual.individual_id " +
                 " WHERE status = 'заключён'" +
                 " Group by contract.individual_id,  firstname , email , contract.data" +
                 " HAVING  SUM(contractsum) > 40000 AND DATE(data) >= CURRENT_DATE - 60" +
                 " order by individual_id ASC";


            // e-mail уполномоченных лиц заключивших договора за последнии 30 дней, сумма которых выше 40 000
            using var cmd3 = new NpgsqlCommand(sql3, con);
            Console.WriteLine("\nЗадание #3\n");
            Console.WriteLine("e-mail уполномоченных лиц заключивших договора за последнии 30 дней, сумма которых выше 40 000:\n");
            using NpgsqlDataReader rdr3 = cmd3.ExecuteReader();

            while (rdr3.Read())
            {
                var individual_id = rdr3.GetInt32(0);
                var firstname = rdr3.GetString(1);
                var email = rdr3.GetString(2);
                var allcontractsum = rdr3.GetFloat(3);
                var data = rdr3.GetDateTime(4);

                Console.WriteLine($"id: {individual_id}\t email: {email}\t ");

            }
           
            con.Close();

            con.Open();
            //	Создать отчет в фориате json

            string sql5 = " SELECT individual.firstname, individual.lastname, individual.email, individual.phone, individual.dbirth " +
                 " FROM contract LEFT JOIN individual ON contract.individual_id = individual.individual_id " +
                 " LEFT JOIN legal ON contract.legal_id = legal.legal_id " +
                 " WHERE status = 'заключён' AND legal.citi = 'Moscow' " +
                 " order by contract_id ASC";

            
            using var cmd5 = new NpgsqlCommand(sql5, con);

            Console.WriteLine("\nЗадание #5\n");
            Console.WriteLine("	Создать отчет в фориате json:\n");
            using NpgsqlDataReader rdr5 = cmd5.ExecuteReader();

            Json json = new Json();

            while (rdr5.Read())
            {
                //var individual_id = rdr5.GetInt32(0);
                var firstname = rdr5.GetString(0);
                var lastname = rdr5.GetString(1);
                var email = rdr5.GetString(2);
                var phone = rdr5.GetString(3);
                var dbirth = rdr5.GetDateTime(4);

                Console.WriteLine($"имя: {firstname}\t email: {email}\t  телефон: {phone}\t день рождения: {dbirth}\t ");
                
                string writejson = json.GreateJson(firstname, lastname, email , phone, dbirth); // запись в json файл
            }
                        
            con.Close();
            Console.ReadKey();
        }
    }

}

