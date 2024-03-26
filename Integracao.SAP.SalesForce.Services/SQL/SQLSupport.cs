using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.SQL
{
    public static class SQLSupport
    {
        public static string GetConsultas(string nomeConsulta)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var targetName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith($".{nomeConsulta}.sql"));

            using (var stream = assembly.GetManifestResourceStream(targetName))
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return result;
            }

        }
    }
}
