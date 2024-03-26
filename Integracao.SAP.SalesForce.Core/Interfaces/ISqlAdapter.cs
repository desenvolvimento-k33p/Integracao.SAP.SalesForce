using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Core.Interfaces
{
   
    public interface ISqlAdapter
    {
        Task<SqlDataReader> QuerySqlDataReader(string sql, System.Data.SqlClient.SqlConnection conexao);
        Task<List<string>> QueryListString(string sql);

        Task<string> QueryReaderString(string sql);

        Task<int> QueryInsertUpdate<T>(string sql);
        //Task<IEnumerable<T>> Query<T>(string sql);

        //Task<List<T>> QueryList<T>(string sql);
        //Task<int> Execute(string sql);

        //object ExecuteSinc(string sql);
    }
}
