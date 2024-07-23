using System;
using System.Collections.Generic;
using System.Text;
using Fbg.Bll;
namespace Fbg.Bll
{
    public class Folder
    {
        private string _name;
        private int _id;
        private Folders.FolderType _folderType;
        /// <summary>
        /// should accept a datarow. 
        /// </summary>
        public Folder(int id,string name,Int16 folderType)
        {
            _id = id;
            _name = name;
            _folderType = (Folders.FolderType)Enum.Parse(typeof(Folders.FolderType), _folderType.ToString()); ;
        }
        public int ID
        {
            get
            {
                return _id;
            }
            

        }
        public string Name
        {
            get
            {
                return _name;
            }
            
        }
        public Folders.FolderType  FolderType
        {
            get
            {
                return _folderType;
            }
        }
       
      
    }
}
