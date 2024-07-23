using System;
using System.Collections.ObjectModel;

namespace Facebook.Entity {
    [Serializable]
    public class HigherEducation
    {
        #region Private Data

        private string _school;
        private int _classYear;
        private Collection<string> _concentration;

        #endregion PrivateData

        #region Properties

        /// <summary>
        /// The name of the school
        /// </summary>
        public string School
        {
            get { return _school; }
            set { _school = value; }
        }

        /// <summary>
        /// Collection of concentrations
        /// </summary>
        public Collection<string> Concentration
        {
            get
            {
                if (_concentration == null)
                    _concentration = new Collection<string>();

                return _concentration; 
            }
        }

        /// <summary>
        /// Graduation year
        /// </summary>
        public int ClassYear
        {
            get { return _classYear; }
            set { _classYear = value; }
        }

        #endregion Properties

        /// <summary>
        /// Default constructor
        /// </summary>
        public HigherEducation()
        {
        }
    }
}