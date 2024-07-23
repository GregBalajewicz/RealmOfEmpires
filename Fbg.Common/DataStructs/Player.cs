using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Common.DataStructs
{
    public class Player
    {
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return this == (Player)obj;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(Player x, Player y)
        {
            if ((object)x == null && (object)y == null) return true;
            if ((object)x != null && (object)y != null)
            {
                if (x.ID == y.ID) return true;
            }
            return false;
        }

        public static bool operator !=(Player x, Player y)
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
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
