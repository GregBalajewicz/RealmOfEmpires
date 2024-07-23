using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Collections.Generic;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Fbg.DAL;
namespace Fbg.Forum
{
   public class SqlForumsProvider : ForumsProvider
   {
    

      /// <summary>
      /// Returns an existing forum with the specified ID
      /// </summary>
       public override ForumDetails GetForumByID(int forumID,int PlayerID, string ConnectionStr)
      {
          
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                using (IDataReader reader = db.ExecuteReader("tbh_Forums_GetForumByID", new object[] { forumID, PlayerID }))
                {
                    if (reader.Read())
                        return GetForumFromReader(reader);
                    else
                        return null;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetForumByID'", e);
                ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("forumID", forumID.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
          
      }
       public override DataTable  GetForumsByClanID(int clanID, int PlayerID, bool showDeletedForums ,string ConnectionStr)
       {
           Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                DataTable dt = db.ExecuteDataSet("tbh_Forums_GetForumsByClanID", new object[] { clanID, PlayerID, showDeletedForums }).Tables[0];
                return dt;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetForumsByClanID'", e);
                ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("showDeletedForums", showDeletedForums.ToString());
                throw ex;
            }
       }

      /// <summary>
      /// Deletes a forum
      /// </summary>
       public override bool DeleteForum(int forumID, string ConnectionStr)
       {
           Database db;
           
           try
           {
               db = new DB(ConnectionStr);
               int ret = db.ExecuteNonQuery("tbh_Forums_DeleteForum", new object[] { forumID });
               return (ret == 1);
           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_DeleteForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("forumID", forumID.ToString ());

               throw ex;
           }

       }
       /// <summary>
       /// UnDeletes/restore  a forum
       /// </summary>
       public override bool UnDeleteForum(int forumID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               int ret = db.ExecuteNonQuery("tbh_Forums_UnDeleteForum", new object[] { forumID });
               return (ret == 1);
           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_UnDeleteForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("forumID", forumID.ToString());

               throw ex;
           }

       }

      /// <summary>
      /// Updates a forum
      /// </summary>
       public override bool UpdateForum(ForumDetails forum, string ConnectionStr)
      {
            Database db;

            try
            {
                db = new DB(ConnectionStr);
                int ret = db.ExecuteNonQuery("tbh_Forums_UpdateForum", new object[] { forum.ID, forum.Title, forum.Moderated, forum.Importance, forum.Description, forum.ImageUrl });
                return (ret == 1);
            }
            catch (Exception e)
            {

                BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_UpdateForum'", e);
                ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("forum.ID", forum.ID.ToString());
                ex.AdditionalInformation.Add("forum.Title", forum.Title);
                ex.AdditionalInformation.Add("forum.Moderated", forum.Moderated.ToString ());
                ex.AdditionalInformation.Add("forum.Importance", forum.Importance.ToString ());
                ex.AdditionalInformation.Add("forum.ImageUrl", forum.ImageUrl);
                ex.AdditionalInformation.Add("forum.Description", forum.Description);

                throw ex;
            }
         
      }
       public override bool UpdateForum(string Title, bool Moderated, int Importance, string ImageUrl, string Description, bool AlertClanMembers, byte SecurityLevel, int ForumID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               int ret = db.ExecuteNonQuery("tbh_Forums_UpdateForum", new object[] { ForumID, Title, Moderated, Importance, Description, ImageUrl, AlertClanMembers, SecurityLevel });
               return (ret == 1);
           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_UpdateForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("ForumID", ForumID.ToString());
               ex.AdditionalInformation.Add("Title", Title);
               ex.AdditionalInformation.Add("Moderated", Moderated.ToString ());
               ex.AdditionalInformation.Add("Importance", Importance.ToString ());
               ex.AdditionalInformation.Add("ImageUrl", ImageUrl);
               ex.AdditionalInformation.Add("Description", Description);
               ex.AdditionalInformation.Add("AlertClanMembers", AlertClanMembers.ToString());
               ex.AdditionalInformation.Add("SecurityLevel", SecurityLevel.ToString());

               throw ex;
           }
       }

      /// <summary>
      /// Creates a new forum
      /// </summary>
       public override int InsertForum(ForumDetails forum, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_InsertForum");
               db.AddInParameter(cmd,"@AddedDate", DbType.DateTime,forum.AddedDate)  ;
               db.AddInParameter(cmd, "@AddedBy", DbType.String , forum.AddedBy);
               db.AddInParameter(cmd, "@Title", DbType.String, forum.Title);
               db.AddInParameter(cmd, "@Moderated", DbType.Boolean,forum.Moderated) ;
               db.AddInParameter(cmd, "@Importance", DbType.Int32, forum.Importance);
               db.AddInParameter(cmd, "@ImageUrl", DbType.String, "http://static.realmofempires.com/images/Folder.gif");
               db.AddInParameter(cmd, "@Description", DbType.String, forum.Description);
               db.AddInParameter(cmd, "@ClanID", DbType.String, forum.ClanID);
               db.AddOutParameter(cmd, "@ForumID", System.Data.DbType.Int32, int.MaxValue);
               db.ExecuteNonQuery(cmd);
               int ForumID = (int)db.GetParameterValue(cmd, "@ForumID");
               return ForumID;

           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_InsertForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("forum.Title", forum.Title);
               ex.AdditionalInformation.Add("forum.Moderated", forum.Moderated.ToString ());
               ex.AdditionalInformation.Add("forum.Importance", forum.Importance.ToString ());
               ex.AdditionalInformation.Add("forum.ImageUrl", forum.ImageUrl);
               ex.AdditionalInformation.Add("forum.Description", forum.Description);
               ex.AdditionalInformation.Add("forum.AddedDate", forum.AddedDate.ToString());
               ex.AdditionalInformation.Add("forum.AddedBy", forum.AddedBy);
               ex.AdditionalInformation.Add("forum.ClanID", forum.ClanID.ToString ());

               throw ex;
           }
          
      }

      /// <summary>
      /// Creates a new forum
      /// </summary>
       public override int InsertForum(DateTime AddedDate, string AddedBy, string Title, bool Moderated, int Importance, string ImageUrl, string Description, int ClanID, bool AlertClanMembers, byte SecurityLevel, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_InsertForum");
              db.AddInParameter(cmd, "@AddedDate", DbType.DateTime, AddedDate);
              db.AddInParameter(cmd, "@AddedBy", DbType.String, AddedBy);
              db.AddInParameter(cmd, "@Title", DbType.String, Title);
              db.AddInParameter(cmd, "@Moderated", DbType.Boolean, Moderated);
              db.AddInParameter(cmd, "@Importance", DbType.Int32, Importance);
              db.AddInParameter(cmd, "@ImageUrl", DbType.String, "http://static.realmofempires.com/images/Folder.gif");
              db.AddInParameter(cmd, "@Description", DbType.String, Description);
              db.AddInParameter(cmd, "@ClanID", DbType.String, ClanID);
              db.AddInParameter(cmd, "@AlertClanMembers", DbType.Boolean, AlertClanMembers);
              db.AddInParameter(cmd, "@SecurityLevel", DbType.Byte, SecurityLevel);
              db.AddOutParameter(cmd, "@ForumID", System.Data.DbType.Int32, int.MaxValue);
              db.ExecuteNonQuery(cmd);
              int ForumID = (int)db.GetParameterValue(cmd, "@ForumID");
              return ForumID;

          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_InsertForum'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
            
              ex.AdditionalInformation.Add("forum.Title", Title);
              ex.AdditionalInformation.Add("forum.Moderated", Moderated.ToString ());
              ex.AdditionalInformation.Add("forum.Importance", Importance.ToString ());
              ex.AdditionalInformation.Add("forum.ImageUrl", ImageUrl);
              ex.AdditionalInformation.Add("forum.Description", Description);
              ex.AdditionalInformation.Add("forum.AddedDate", AddedDate.ToString());
              ex.AdditionalInformation.Add("forum.AddedBy", AddedBy);
              ex.AdditionalInformation.Add("forum.AlertClanMembers", AlertClanMembers.ToString());
              ex.AdditionalInformation.Add("forum.SecurityLevel", SecurityLevel.ToString());

              throw ex;
          }
        
      }
      /// <summary>
      /// Retrieves all unapproveds posts
      /// </summary>
       public override List<PostDetails> GetUnapprovedPosts(int ClanID,string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr); ;
              using (IDataReader reader = db.ExecuteReader("tbh_Forums_GetUnapprovedPosts", new object[] { ClanID }))
              {
                  return GetPostCollectionFromReader(reader);
              }

          }
          catch (Exception e)
          {
              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetUnapprovedPosts'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
              throw ex;
          }
          

       
      }

      /// <summary>
      /// Retrieves forum's approved threads by page
      /// </summary>
       public override DataTable  GetThreads(int forumID, string sortExpression, int pageIndex, int pageSize,int PlayerID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr); ;
               sortExpression = EnsureValidSortExpression(sortExpression);
               int lowerBound = pageIndex * pageSize + 1;
               int upperBound = (pageIndex + 1) * pageSize;
               DataSet ds=db.ExecuteDataSet("tbh_Forums_GetThreads", new object[] {forumID,sortExpression,lowerBound,upperBound,PlayerID  });
               if (ds.Tables.Count ==0)
               {
                   return null;
               }
               else
               {
                   return ds.Tables[0];
               }
               
               
           }
           catch (Exception e)
           {
               BaseApplicationException ex = new BaseApplicationException("Error while calling 'GetThreads'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("forumID", forumID.ToString());
               ex.AdditionalInformation.Add("sortExpression", sortExpression);
               ex.AdditionalInformation.Add("pageIndex", pageIndex.ToString());
               ex.AdditionalInformation.Add("pageSize", pageSize.ToString());
               ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
               throw ex;
           }
          
       }

      /// <summary>
      /// Retrieves the post with the specified ID
      /// </summary>
       public override PostDetails GetPostByID(int postID,int PlayerID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr); ;
              using (IDataReader reader = db.ExecuteReader("tbh_Forums_GetPostByID", new object[] { postID, PlayerID }))
              {
                  if (reader.Read())
                      return GetPostFromReader(reader);
                  else
                      return null;
              }
          }
          catch (Exception e)
          {
              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetPostByID'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("postID", postID.ToString());
              ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
              throw ex;
          }
          
         
      }

     

      /// <summary>
      /// Retrieves all posts of a given thread
      /// </summary>
      public override List<PostDetails> GetThreadByID(int threadPostID,int PlayerID,string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr); ;
              using (IDataReader reader = db.ExecuteReader("tbh_Forums_GetThreadByID", new object[] { threadPostID, PlayerID }))
              {
                  return GetPostCollectionFromReader(reader);
              }
          }
          catch (Exception e)
          {
              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetThreadByID'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("threadPostID", threadPostID.ToString());
              ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
              throw ex;
          }
        
      }

      /// <summary>
      /// Deletes a post (if the post represent the first message of a thread, 
      /// the child posts are deleted as well)
      /// </summary>
       public override bool DeletePost(int postID,int playerID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              int ret = db.ExecuteNonQuery("tbh_Forums_DeletePost", new object[] { postID ,playerID });
              return (ret >= 1);
          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_DeletePost'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("postID", postID.ToString ());

              throw ex;
          }
          
      }

      /// <summary>
      /// Inserts a new post
      /// </summary>
       public override int InsertPost(PostDetails post, int PlayerID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_InsertPost");
              db.AddInParameter(cmd, "@AddedDate", DbType.DateTime, post.AddedDate);
              db.AddInParameter(cmd, "@AddedBy", DbType.String, post.AddedBy);
              db.AddInParameter(cmd, "@AddedByIP", DbType.String, post.AddedByIP );
              db.AddInParameter(cmd, "@ForumID", DbType.Int32, post.ForumID );
              db.AddInParameter(cmd, "@ParentPostID", DbType.Int32, post.ParentPostID);
              db.AddInParameter(cmd, "@Title", DbType.String, post.Title);
              db.AddInParameter(cmd, "@Body", DbType.String, post.Body);
              db.AddInParameter(cmd, "@BodyForChat", DbType.String, post.BodyForChat);
              db.AddInParameter(cmd, "@Approved", DbType.Boolean, post.Approved);
              db.AddInParameter(cmd, "@Closed", DbType.Boolean, post.Closed);
              db.AddInParameter(cmd, "@Sticky", DbType.Boolean, post.Sticky);
              db.AddInParameter(cmd, "@PlayerID", DbType.Int32, PlayerID );
              db.AddOutParameter(cmd, "@PostID", System.Data.DbType.Int32, int.MaxValue);
              db.ExecuteNonQuery(cmd);
              int postID = (int)db.GetParameterValue(cmd, "@PostID");
              return postID;

          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_InsertPost'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("post.Title", post.Title);
              ex.AdditionalInformation.Add("post.Body", post.Body);
              ex.AdditionalInformation.Add("post.Approved", post.Approved.ToString ());
              ex.AdditionalInformation.Add("post.Closed", post.Closed.ToString());
              ex.AdditionalInformation.Add("post.Sticky", post.Sticky.ToString());
              ex.AdditionalInformation.Add("post.ForumID", post.ForumID.ToString());
              ex.AdditionalInformation.Add("post.ParentPostID", post.ParentPostID.ToString());
              ex.AdditionalInformation.Add("post.AddedDate", post.AddedDate.ToString());
              ex.AdditionalInformation.Add("post.AddedBy", post.AddedBy);
              ex.AdditionalInformation.Add("post.AddedByIP", post.AddedByIP);
              ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
              

              throw ex;
          }
         
      }

      /// <summary>
      /// Updates an post
      /// </summary>
       public override bool UpdatePost(PostDetails post,int PlayerID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_UpdatePost");
              db.AddInParameter(cmd, "@PostID", DbType.Int32, post.ID);
              db.AddInParameter(cmd, "@Title", DbType.String, post.Title);
              db.AddInParameter(cmd, "@Body", DbType.String, post.Body);
              db.AddInParameter(cmd, "@Sticky", DbType.Boolean, post.Sticky);
              db.AddInParameter(cmd, "@PlayerID", DbType.Int32, PlayerID);
          
              int ret=db.ExecuteNonQuery(cmd);
              if (ret > 0)
              {
                  return true;
              }
              else
              {
                  return false;
              }


          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_UpdatePost'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("post.ID", post.ID.ToString());
              ex.AdditionalInformation.Add("post.Title", post.Title);
              ex.AdditionalInformation.Add("post.Body", post.Body);
              ex.AdditionalInformation.Add("post.Sticky", post.Sticky.ToString());
              ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());


              throw ex;
          }
          
          
      }

      /// <summary>
      /// Approves a post
      /// </summary>
       public override bool ApprovePost(int postID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_ApprovePost");
              db.AddInParameter(cmd, "@PostID", DbType.Int32, postID);

              int ret = db.ExecuteNonQuery(cmd);
              return (ret >= 1);


          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_ApprovePost'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("postID", postID.ToString());


              throw ex;
          }
          
          
         
      }

      /// <summary>
      /// Increments the ViewCount of the specified post
      /// </summary>
       public override bool IncrementPostViewCount(int postID, string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_IncrementViewCount");
              db.AddInParameter(cmd, "@PostID", DbType.Int32, postID);

              int ret = db.ExecuteNonQuery(cmd);
              return (ret == 1);


          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_IncrementViewCount'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("postID", postID.ToString());


              throw ex;
          }
      }

      /// <summary>
      /// Closes a thread
      /// </summary>
      public override bool CloseThread(int threadPostID,string ConnectionStr)
      {
          Database db;

          try
          {
              db = new DB(ConnectionStr);
              System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_CloseThread");
              db.AddInParameter(cmd, "@ThreadPostID", DbType.Int32 , threadPostID );

              int ret = db.ExecuteNonQuery(cmd);
              return (ret == 1);


          }
          catch (Exception e)
          {

              BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_CloseThread'", e);
              ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
              ex.AdditionalInformation.Add("threadPostID", threadPostID.ToString());


              throw ex;
          }
        
      }

      /// <summary>
      /// Moves a thread (the parent post and all its child posts) to a different forum
      /// </summary>
       public override bool MoveThread(int threadPostID, int forumID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_MoveThread");
               db.AddInParameter(cmd, "@ThreadPostID", DbType.Int32, threadPostID);
               db.AddInParameter(cmd, "@ForumID", DbType.Int32, forumID );
               int ret = db.ExecuteNonQuery(cmd);
               return (ret >= 1);


           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_MoveThread'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("ThreadPostID",threadPostID.ToString());
               ex.AdditionalInformation.Add("forumID", forumID.ToString());


               throw ex;
           }
          
      }

      /// <summary>
      /// Security Functions
      /// </summary>
       public override bool IsPlayerInClan( int PlayerID,int ClanID,string ConnectionStr)
       {
             Database db;

             try
             {
                 db = new DB(ConnectionStr);
                 if ((int)db.ExecuteScalar("tbh_Forums_IsPlayerInClan", new object[] { ClanID,PlayerID }) == 0)
                 {
                     return false;
                 }
                 else
                 {
                     return true;
                 }
             }
             catch (Exception e)
             {

                 BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_IsPlayerInClan'", e);
                 ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
                 ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                 ex.AdditionalInformation.Add("ClanID", ClanID.ToString());


                 throw ex;
             }
           
       }
    
       public override void SetThreadAsRead(int PlayerID, int ThreadPostID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_SetThreadAsRead");
               db.AddInParameter(cmd, "@PlayerID", DbType.Int32, PlayerID);
               db.AddInParameter(cmd, "@ThreadPostID", DbType.Int32, ThreadPostID);
                db.ExecuteNonQuery(cmd);

           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_SetThreadAsRead'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("ThreadPostID", ThreadPostID.ToString());
               ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());


               throw ex;
           }
           
       }

       public override void SetForumAsRead(int PlayerID, int ForumID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("tbh_Forums_SetForumAsRead");
               db.AddInParameter(cmd, "@PlayerID", DbType.Int32, PlayerID);
               db.AddInParameter(cmd, "@ForumID", DbType.Int32, ForumID);
               db.ExecuteNonQuery(cmd);

           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_SetForumAsRead'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("ThreadPostID", ForumID.ToString());
               ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());


               throw ex;
           }

       }

       public DataTable GetSharedForumByClanID(int clanID, int PlayerID, bool showDeletedForums, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr); ;
               DataTable dt = db.ExecuteDataSet("tbh_Forums_GetSharedForumsByClanID", new object[] { clanID, PlayerID, showDeletedForums }).Tables[0];
               return dt;
           }
           catch (Exception e)
           {
               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetSharedForumsByClanID'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
               ex.AdditionalInformation.Add("showDeletedForums", showDeletedForums.ToString());
               throw ex;
           }
       }

       public object GetWhiteListClansByClanID(int clanID, int playerID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr); ;
               DataTable dt = db.ExecuteDataSet("tbh_Forums_GetWhiteListClansByClanID", new object[] { clanID, playerID }).Tables[0];
               return dt;
           }
           catch (Exception e)
           {
               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetWhiteListClansByClanID'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
               
               throw ex;
           }
       }

       public bool DeleteWhiteListClan(int whileListClanID, int clanID,int playerID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               return Convert.ToBoolean( db.ExecuteScalar("tbh_Forums_DeleteWhiteListClan", new object[] { whileListClanID, clanID, playerID }));
               
           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_DeleteWhiteListClan'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("whileListClanID", whileListClanID.ToString());
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("playerID", playerID.ToString());

               throw ex;
           }
       }

       public bool AddWhiteListClan(string clanName, int clanID, int playerID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               return Convert.ToBoolean(db.ExecuteScalar("tbh_Forums_InsertWhiteListClan", new object[] { clanName, clanID, playerID }));
              
           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_InsertWhiteListClan'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("whileListClanName", clanName);
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("playerID", playerID.ToString());

               throw ex;
           }
       }
       public DataSet GetSharedForumsWithClansByClanID(int clanID, int playerID,  string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr); ;
               DataSet ds = db.ExecuteDataSet("tbh_Forums_GetSharedForumsWithClansByClanID", new object[] { clanID, playerID });
               return ds;
           }
           catch (Exception e)
           {
               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_GetSharedForumsWithClansByClanID'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
              
               throw ex;
           }
       }
       public bool AddClanNameToSharedForum(int forumID, string clanName, int clanID, int playerID, string ConnectionStr)
       {

           Database db;

           try
           {
               db = new DB(ConnectionStr);
               return Convert.ToBoolean( db.ExecuteScalar("tbh_Forums_InsertClanToSharedForum", new object[] { forumID, clanName, clanID,playerID }));

           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_InsertClanToSharedForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("forumID", forumID.ToString());
               ex.AdditionalInformation.Add("clanName", clanName);
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("playerID", playerID.ToString());

               throw ex;
           }
       }

       public bool RemoveClanFromSharedForum(int sharedForumID, int sharedWithClanID, int clanID, int playerID, string ConnectionStr)
       {
           Database db;

           try
           {
               db = new DB(ConnectionStr);
               return Convert.ToBoolean( db.ExecuteScalar("tbh_Forums_DeleteClanFromSharedForum", new object[] { sharedForumID, sharedWithClanID, clanID, playerID }));

           }
           catch (Exception e)
           {

               BaseApplicationException ex = new BaseApplicationException("Error while calling 'tbh_Forums_DeleteClanFromSharedForum'", e);
               ex.AdditionalInformation.Add("ConnectionStr", ConnectionStr);
               ex.AdditionalInformation.Add("sharedforumID", sharedForumID.ToString());
               ex.AdditionalInformation.Add("sharedWithClanID", sharedWithClanID.ToString());
               ex.AdditionalInformation.Add("clanID", clanID.ToString());
               ex.AdditionalInformation.Add("playerID", playerID.ToString());

               throw ex;
           }
       }
   }
}
