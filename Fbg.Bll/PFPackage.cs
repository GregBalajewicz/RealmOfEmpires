using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public class PFPackage
    {
        int _id;

        int _cost;

        float _duration;

        public float  Duration
        {
            get { return _duration; }
        }
        public int Id
        {
            get { return _id; }
        }
        public int Cost
        {
            get { return _cost; }
        }

        public PFPackage(int id, int cost, float duration)
        {
            _id = id;
            _cost = cost;
            _duration = duration;
        }
    }
}
