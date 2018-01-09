using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.DataModel
{
    public static class MediaDB
    {
        public static SqlConnection GetConnection()
        {
            string cs =
                ConfigurationManager.ConnectionStrings["MediaDBConnectionString"].ConnectionString;
            SqlConnection cn = new SqlConnection(cs);
            return cn;
        }
    }
}
