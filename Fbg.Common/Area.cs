using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fbg.Common
{
    public class Area
    {
        private Area(int areaNumber)
        {
            StartOfArea = new Point();
            EndOfArea = new Point();
            AreaNumber = areaNumber;
        }
        /// <summary>
        /// bottom left corner of area. inclusive cordinate
        /// </summary>
        public Point StartOfArea;
        /// <summary>
        /// Top right corner of the area. inclusive cordinate
        /// </summary>
        public Point EndOfArea;
        public int AreaNumber;



        public static Area GetArea(int areaNumber)
        {
            Fbg.Common.Area area = new Fbg.Common.Area(areaNumber);

            //Get Ring
            double ring = Convert.ToInt32(Math.Floor(Math.Sqrt(areaNumber - 1) / 2));

            //Get fraction of the spiral on the ring
            double fractionOfSpiral = (areaNumber - (4 * ring * ring)) / (8 * ring + 4);
            //This if statement needs to "fall through" 
            //Otherwise you need to put an AND statement to bound fractionOfSpiral
            if (fractionOfSpiral <= .25)
            {
                area.StartOfArea.X = Convert.ToInt32(100 * (-ring - 1));
                area.StartOfArea.Y = Convert.ToInt32(100 * Math.Round(-ring - 1 + (2 * ring + 1) * fractionOfSpiral * 4, MidpointRounding.AwayFromZero));
            }
            else if (fractionOfSpiral <= .50)
            {
                area.StartOfArea.X = Convert.ToInt32(100 * Math.Round(-ring - 1 + (2 * ring + 1) * (fractionOfSpiral - 0.25) * 4, MidpointRounding.AwayFromZero));
                area.StartOfArea.Y = Convert.ToInt32(100 * ring);
            }
            else if (fractionOfSpiral <= .75)
            {
                area.StartOfArea.X = Convert.ToInt32(100 * ring);
                area.StartOfArea.Y = Convert.ToInt32(100 * Math.Round(ring - (2 * ring + 1) * (fractionOfSpiral - 0.5) * 4, MidpointRounding.AwayFromZero));
            }
            else if (fractionOfSpiral <= 1)
            {
                area.StartOfArea.X = Convert.ToInt32(100 * Math.Round(ring - (2 * ring + 1) * (fractionOfSpiral - 0.75) * 4, MidpointRounding.AwayFromZero));
                area.StartOfArea.Y = Convert.ToInt32(100 * (-ring - 1));
            }

            area.EndOfArea.X = area.StartOfArea.X + 99;
            area.EndOfArea.Y = area.StartOfArea.Y + 99;

            return area;

        }

        public static int GetAreaNumberFromCords(int x, int y)
        {
            int regionNumber=0;
            double ring =0;
            int xbase=Convert.ToInt32(Math.Floor((double)x/100));
            int ybase = Convert.ToInt32(Math.Floor((double)y / 100));

            ring = Math.Floor(Math.Max(Math.Abs((x + 0.5) / 100), Math.Abs((y + 0.5) / 100)));

            if (xbase == - ring - 1 && ybase > - ring - 1)
            {
                regionNumber = Convert.ToInt32((2 * ring) * (2 * ring) + Math.Floor((double)y / 100) + ring + 1);
            }
            else if (xbase >= -ring - 1 && ybase == ring)
            {
                regionNumber = Convert.ToInt32((2 * ring) * (2 * ring) + (2 * ring + 1) + Math.Floor((double)x / 100) + ring + 1);
            }
            else if (xbase == ring && ybase < ring)
            {
                regionNumber = Convert.ToInt32((2 * ring) * (2 * ring) + (4 * ring + 1) - (Math.Floor((double)y / 100)) + ring + 1);
            }
            else // was ELSE IF (xbase < ring && ybase == -ring - 1)
            {
                regionNumber = Convert.ToInt32((2 * ring) * (2 * ring) + (6 * ring + 1) - (Math.Floor((double)x / 100)) + ring + 2);
            }
            				
	            return regionNumber;
        }
    }
}
