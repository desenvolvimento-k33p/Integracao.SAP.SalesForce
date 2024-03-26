using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Infra.Repositories
{
    public class LoggerRepository : ILoggerRepository
    {
        private readonly IOptions<Configuration> _configuration;
        private readonly ISqlAdapter _hana;
        private readonly string HANA_DB;

        public LoggerRepository(IOptions<Configuration> configuration, ISqlAdapter hana)
        {
            _configuration = configuration;
            _hana = hana;

            HANA_DB = _configuration.Value.SqlConnection.InitialCatalog;//Base BR
        }

        public async Task Logger(LogIntegration logData)
        {
            try
            {
                string msg = logData.Message;// Regex.Replace(String.IsNullOrEmpty(logData.Message) ? "" : logData.Message, "[^a-zA-Z0-9. ãê]+", "");
                var response = (object)logData.ResponseObject == null ? "" : logData.ResponseObject.ToString();
                response = System.Text.RegularExpressions.Regex.Unescape(response).Replace("'", "");

                await DeleteLogs();

                var sql = $@"INSERT INTO [SBOPRODBR].dbo.[K33P_LOG_SAPSF] ( 
                ""LOGDATE"", 
                ""LOGHOUR"",               
                ""COMPANY"",               
                ""MESSAGE"",
                ""KEY_SAP"",
                ""KEY_PARC"",
                ""REQUESTOBJECT"",
                ""RESPONSEOBJECT"",
                ""OWNER"",
                ""METHOD"")
                VALUES (
                    GETDATE(),
                    cast(DATEPART(hour, GETDATE()) as varchar) + ':' + cast(DATEPART(minute, GETDATE()) as varchar) + ':' + cast(DATEPART(SECOND, GETDATE()) as varchar) ,                   
                    '{logData.Company}',                    
                    '{msg.Replace("'","")}',
                    '{logData.Key}',
                    '{logData.Key2}',
                    '{logData.RequestObject}',
                    '{response}',
                    '{logData.Owner}',
                    '{logData.Method}'
                )";



                 var result = await _hana.QueryReaderString(sql);

            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao incluir Log {e.Message}", e);
            }
        }

        private async Task DeleteLogs()
        {
            try
            {
                string sql = $@"DELETE FROM [SBOPRODBR].dbo.[K33P_LOG_SAPSF] WHERE LOGDATE < DATEADD(DAY, -20, GETDATE())";
                await _hana.QueryReaderString(sql);
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao excluir Log {e.Message}", e);
            }
        }
    }
}
