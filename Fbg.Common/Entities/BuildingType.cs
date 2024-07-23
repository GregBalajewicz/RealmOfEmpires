using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Common.Entities
{
    
    public class BuildingType
    {

        public string Name
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }
        public int MinimumLevelAllowed
        {
            get;
            set;
        }

        public int MaxLevel
        {
            get;
            set;
        }
        public string IconUrl { get; set; }

        private string _iconUrl_ThemeM;
        public string IconUrl_ThemeM {
            get
            {
                if (_iconUrl_ThemeM == null) {
                    var lastSlash = IconUrl.LastIndexOf("/");
                    var filename = IconUrl.Substring(lastSlash+1);

                    _iconUrl_ThemeM = IconUrl.Replace(filename, "m_" + filename);
                }

                return _iconUrl_ThemeM;
            }
        }


    }
}
