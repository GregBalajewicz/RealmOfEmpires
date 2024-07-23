using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public partial class Realm
    {
        List<GovType> _govTypes ;
        public List<GovType> GovTypes
        {
            get
            {
                if (_govTypes == null)
                {
                    _govTypes = new List<GovType>(5);
                    _govTypes.Add(new GovType() { Name = "Monarchy", ID=1 });
                    _govTypes.Add(new GovType() { Name = "Republic" , ID=2});
                    _govTypes.Add(new GovType() { Name = "Barbarian" , ID=3});
                    _govTypes.Add(new GovType() { Name = "Merchant" , ID=4});
                    _govTypes.Add(new GovType() { Name = "Theocracy" , ID=5});
                }

                return _govTypes;
                   
            }
        }
        public GovType GovType(int id)
        {
            return GovTypes.Find(delegate(GovType g) { return g.ID == id; });
        }
    }       
}
