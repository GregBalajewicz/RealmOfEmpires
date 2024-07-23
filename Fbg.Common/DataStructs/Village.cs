using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common.DataStructs
{
    public class Village
    {
        public override string ToString()
        {
            return String.Format("{0}({1},{2})", _name, XCord, YCord);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this == (Village)obj;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(Village x, Village y)
        {
            if ((object)x == null && (object)y == null) return true;
            if ((object)x != null && (object)y != null)
            {
                if (x.ID == y.ID) return true;
            }
            return false;
        }

        public static bool operator !=(Village x, Village y)
        {
            if ((object)x == null && (object)y == null) return false;
            if ((object)x != null && (object)y != null)
            {
                if (x.ID != y.ID) return true;
            }
            else
            {
                return true;
            }
            return false;
        }

        int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        int _xCord;

        public int XCord
        {
            get { return _xCord; }
            set { _xCord = value; }
        }
        int _yCord;

        public int YCord
        {
            get { return _yCord; }
            set { _yCord = value; }
        }
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        Player _owner;

        internal Player Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
        public string NameFull
        {
            get { return String.Format("{0}({1},{2})", _name, XCord, YCord); }
        }
    
    }
}
