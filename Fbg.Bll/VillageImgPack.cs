using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Bll
{
    /// <summary>
    /// Implementing this as a dictionary with string key. this is not efficient and perhaps it shoudl change to be an int (enum) key. 
    /// </summary>
    public class VillageImgPack
    {
        internal Dictionary<string, string> _images;

        public string Image(string key)
        {
            return _images[key];
        }

        public string Image(string key, string defaultValue)
        {
            string retVal = defaultValue;
            if (_images.TryGetValue(key, out retVal))
            {
                return retVal;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
