using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Domain.Configuration
{
    public class Configuration
    {
        public ServiceLayer ServiceLayer { get; set; }
        public SqlConnection SqlConnection { get; set; }
        public SalesForceHttp SalesForceHttp { get; set; }
        public SalesForceBusiness SalesForceBusiness { get; set; }
    }

    public class SalesForceHttp
    {
        public string Uri { get; set; } = String.Empty;
        public string VersaoAPI { get; set; } = String.Empty;
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string LastModifiedDate_Contas_Contratos { get; set; } = String.Empty;
        public string LastModifiedDate_Cotacoes { get; set; } = String.Empty;
        

    }

    public class SalesForceBusiness
    {

        public int SeriesCardCodeNumerationBR { get; set; }
        public int SeriesCardCodeNumerationEST { get; set; }

        public int GroupCodeBR_BaseBR { get; set; }
        public int GroupCodeEST_BaseBR { get; set; }

        public int GroupCode_BaseEST { get; set; }
        public string NomeBaseBR { get; set; }
        public string NomeBaseAR { get; set; }
        public string NomeBaseCH { get; set; }

    }

    public class SqlConnection
    {
        public string DataSource { get; set; } = String.Empty;
        public string UserID { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string InitialCatalog { get; set; } = String.Empty;
    }

    public class ServiceLayer
    {
        public string SessionId { get; set; } = String.Empty;
        public string Uri { get; set; } = String.Empty;       
        public string UsernameManager { get; set; } = String.Empty;      
        public string UrlFront { get; set; } = String.Empty;
        public int Language { get; set; }
        public string ApprovalUsername { get; set; } = String.Empty;
        public string ApprovalPassword { get; set; } = String.Empty;

        public string PasswordManager { get; set; } = String.Empty;
        public string CompanyDB_BR { get; set; } = String.Empty;
        public string Username_BR { get; set; } = String.Empty;
        public string Password_BR { get; set; } = String.Empty;

       
        public string CompanyDB_AR { get; set; } = String.Empty;
        public string Username_AR { get; set; } = String.Empty;
        public string Password_AR { get; set; } = String.Empty;

     
        public string CompanyDB_CH { get; set; } = String.Empty;
        public string Username_CH { get; set; } = String.Empty;
        public string Password_CH { get; set; } = String.Empty;
    }
}
