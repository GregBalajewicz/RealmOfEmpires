using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.DAL
{
    public class SqlTimeOutException : Exception
    {
        public SqlTimeOutException(Exception innerException) :base("",innerException)
        {
        }
    }
}
