using System;
using System.Collections.Generic;
using System.Text;

namespace Fbg.Bll
{
    public class Distance
    {
        double distance;

        public Distance(int originX, int originY, int destinationX, int destinationY)
        {
            double x, y;
            x = System.Math.Abs(originX - destinationX);
            y = System.Math.Abs(originY - destinationY);

            x = x * x;
            y = y * y;

            distance = System.Math.Sqrt(x + y);
        }


    }
}
