using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public class PF
    {
        int _id;
        string _desc;
  
        public string Desc
        {
            get { return _desc; }
        }
        public int Id
        {
            get { return _id; }
        }
       
        public PF(int id, string desc)
        {
            _id = id;
            _desc = desc;
        }
    }
}
