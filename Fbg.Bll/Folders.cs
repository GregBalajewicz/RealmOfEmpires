using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Security;

namespace Fbg.Bll
{
    
    public class Folders
    {
        //
        // some notes
        //  the values of type FolderType CANNOT CHANGE since they are used as indexes in the _folders array
        //
        public enum FolderType :short 
        {
            Mail=0,
            Reports=1
        }
        public class CONSTS
        {

            public class FolderColumnIndex
            {
                public static int FolderID = 0;
                public static int Name = 1;
                public static int FolderType = 2;
                
            }

            public class SpecialFolderNames
            {
                public static string Starred = "STARRED";
            }
        
        }
        private DataTable dtFolders;
        private List<Folder>[] _folders = { null, null};
        private Player _player;


        internal Folders(Player player)
        {
            _player = player;
        }

        public List<Folder> GetFolders(FolderType type) 
        {
            if (_folders[(int)type] == null)
            {
                _folders[(int)type] = LoadFolders(type);
            }
            return _folders[(int)type];
        }

        private List<Folder> LoadFolders(FolderType type)
        {
            if (dtFolders == null)
            {
                dtFolders = new DataTable();
                dtFolders = DAL.Folders.GetFolders(_player.Realm.ConnectionStr, _player.ID);
           
            }

            List<Folder> fs = new List<Folder>();
            foreach (DataRow dr in dtFolders.Rows )
            {
                if ((Int16)dr[CONSTS.FolderColumnIndex.FolderType] == (Int16)type)
                {
                    fs.Add(new Folder(Convert.ToInt32(dr[CONSTS.FolderColumnIndex.FolderID]), dr[CONSTS.FolderColumnIndex.Name].ToString(), (Int16)dr[CONSTS.FolderColumnIndex.FolderType]));
                }
            }
            return fs;
        }
        public Folder GetFolderByID(int folderID,FolderType ft)
        {  
            List <Folder> fs= LoadFolders(ft);;


            foreach (Folder f in fs)
            {
                if (folderID == f.ID)
                {
                    return f;
                }
               
            }
            return null;
        }
        public int GetFolderIDByName(string folderName, FolderType ft)
        {
            List<Folder> fs = LoadFolders(ft); ;


            foreach (Folder f in fs)
            {
                if (folderName == f.Name)
                {
                    return f.ID;
                }

            }
            return -1;
        }
        public bool AddFolder(string folderName,FolderType folderFor)
        {

            bool ret= DAL.Folders.AddFolder(_player.Realm.ConnectionStr, _player.ID, folderName,(short) folderFor);
            Invalidate();
            return ret;
        }
        public bool DeleteFolder(int folderID,FolderType ft)
        {
            bool ret = DAL.Folders.DeleteFolder(_player.Realm.ConnectionStr, _player.ID, folderID,(Int16) ft);
            Invalidate();
            return ret;
        }
        public bool  UpdateFolder(int folderID,string folderName,FolderType ft)
        {
            bool ret=DAL.Folders.UpdateFolder(_player.Realm.ConnectionStr, _player.ID, folderID,folderName ,(Int16 )ft );
            Invalidate();
            return ret;
        }
        private void Invalidate()
        {
            for (int i = 0; i < _folders.Length; i++)
            {
                _folders[i] = null;
            }
            dtFolders = null;
        }
        public bool DeleteFolderAndMoveItems(int folderID, int moveTofolderID, FolderType ft)
        {
            bool ret = DAL.Folders.DeleteFolderAndMoveItems (_player.Realm.ConnectionStr, _player.ID, folderID,moveTofolderID,(Int16)ft);
            Invalidate();
            return ret;
        }
        public bool IsFolderEmpty(int folderID, FolderType  folderType)
        {
            return DAL.Folders.IsFolderEmpty(_player.Realm.ConnectionStr, _player.ID, folderID,(Int16) folderType);
        }
    }
}
