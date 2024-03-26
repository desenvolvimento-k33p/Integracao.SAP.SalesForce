using Dapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Core.TypeHandler;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


//using SqlConnection = System.Data.SqlClient.SqlConnection;

namespace Integracao.SAP.SalesForce.Core.Adapters
{
    public class SqlAdapter : ISqlAdapter
    {
        private readonly string _urlConnection;

        public SqlAdapter(IOptions<Configuration> configurations)
        {

            Domain.Configuration.SqlConnection cfgFile = configurations.Value.SqlConnection;
            _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";
        }

        public async Task<string> QueryReaderString(string sql)
        {
            //dynamic Data;
            System.Data.SqlClient.SqlConnection conexao = null;
            SqlDataReader leitorDados = null;
            string ret = "";

            try
            {

                using (conexao = new System.Data.SqlClient.SqlConnection(_urlConnection))
                {
                    await conexao.OpenAsync();

                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = sql;

                        leitorDados = await comando.ExecuteReaderAsync();



                        while (leitorDados.Read())
                        {
                            ret = leitorDados.GetValue(0).ToString();
                        }


                        leitorDados.Close();

                    }

                    await conexao.CloseAsync();

                }

            }
            catch (Exception e)
            {
                throw new Exception("HanaAdapter QueryReaderString", e);
            }
            //finally
            //{
            //    await conexao.CloseAsync();
            //}
            return ret;

        }

        public async Task<SqlDataReader> QuerySqlDataReader(string sql, System.Data.SqlClient.SqlConnection conexao)
        {
            //dynamic Data;
            //System.Data.SqlClient.SqlConnection conexao = null;
            SqlDataReader leitorDados = null;
            string ret = "";

            try
            {
                //conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                await conexao.OpenAsync();
                SqlCommand comando = new SqlCommand();
                comando.Connection = conexao;
                comando.CommandText = sql;

                leitorDados = await comando.ExecuteReaderAsync();
                //await conexao.CloseAsync();

                //using (conexao = new System.Data.SqlClient.SqlConnection(_urlConnection))
                //{
                //    await conexao.OpenAsync();

                //    using (SqlCommand comando = new SqlCommand())
                //    {
                //        comando.Connection = conexao;
                //        comando.CommandText = sql;

                //        leitorDados = await comando.ExecuteReaderAsync();

                //        //leitorDados.Close();

                //    }

                //    //await conexao.CloseAsync();

                //}

            }
            catch (Exception e)
            {
                throw new Exception("HanaAdapter QuerySqlDataReader", e);
            }
            //finally
            //{
            //    await conexao.CloseAsync();
            //}
            return leitorDados;

        }

        public async Task<int> QueryInsertUpdate<T>(string sql)
        {
            int Data;
            System.Data.SqlClient.SqlConnection conexao;

            try
            {

                using (conexao = new System.Data.SqlClient.SqlConnection(_urlConnection))
                {
                    await conexao.OpenAsync();

                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = sql;
                        Data = comando.ExecuteNonQuery();

                    }

                    await conexao.CloseAsync();

                }

            }
            catch (Exception e)
            {
                throw new Exception("HanaAdapter QueryInsertUpdate", e);
            }
            //finally
            //{
            //    await conexao.CloseAsync();
            //}

            return Data;
        }

        public async Task<List<string>> QueryListString(string sql)
        {
            //dynamic Data;
            System.Data.SqlClient.SqlConnection conexao = null;
            SqlDataReader leitorDados = null;
            List<string> ret = new List<string>();
            //int i = 0;
            try
            {

                using (conexao = new System.Data.SqlClient.SqlConnection(_urlConnection))
                {
                    await conexao.OpenAsync();

                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = sql;

                        leitorDados = await comando.ExecuteReaderAsync();



                        while (leitorDados.Read())
                        {
                            for (int x = 0; x < leitorDados.FieldCount; x++)
                                ret.Add(leitorDados.GetValue(x).ToString());
                            //i++;
                        }


                        leitorDados.Close();

                    }

                    await conexao.CloseAsync();

                }

            }
            catch (Exception e)
            {
                throw new Exception("HanaAdapter QueryListString", e);
            }
            //finally
            //{
            //    await conexao.CloseAsync();
            //}
            return ret;

        }




    }
}
