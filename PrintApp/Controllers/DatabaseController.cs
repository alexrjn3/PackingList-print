using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;

namespace ScannerAp11.Controllers
{
    public class DatabaseController : Controller
    {
        //Take data from DB
        public static List<T> LoadData<T>(string sql)
        {

            using (SqlConnection cnn = new SqlConnection("Server = localhost\\SQLEXPRESS; Database= test_WEB2; Integrated Security = True;"))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }
    }
}
