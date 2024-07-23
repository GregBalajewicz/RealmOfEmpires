using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Fbg.Common
{
    public class UsersFriend
    {
        public  Guid UserID { get; set; }
        public string FacebookID { get; set; }
        public int NumOfRealms { get; set; }
    }
}
