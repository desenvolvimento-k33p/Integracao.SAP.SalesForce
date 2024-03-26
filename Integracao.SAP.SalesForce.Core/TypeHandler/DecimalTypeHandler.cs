using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Core.TypeHandler
{
    public class DecimalTypeHandler : SqlMapper.TypeHandler<decimal>
    {
        public override decimal Parse(object value)
        {
            return Convert.ToDecimal(value);
        }

        public override void SetValue(IDbDataParameter parameter, decimal value)
        {
            parameter.Value = value;
        }
    }
}
