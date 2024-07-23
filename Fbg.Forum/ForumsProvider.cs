using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Fbg.Forum
{
    public abstract class ForumsProvider 
    { 
        public ForumsProvider()
        {

        }

        // methods that work with forums
        public abstract DataTable GetForumsByClanID(int clainID, int PlayerID, bool showDeletedForums, string ConnectionStr);
        public abstract ForumDetails GetForumByID(int forumID, int PlayerID,  string ConnectionStr);
        public abstract bool DeleteForum(int forumID, string ConnectionStr);
        public abstract bool UnDeleteForum(int forumID, string ConnectionStr);
        public abstract bool UpdateForum(ForumDetails forum, string ConnectionStr);
        public abstract bool UpdateForum(string Title, bool Moderated, int Importance, string ImageUrl, string Description, bool AlertClanMembers, byte SecurityLevel, int ForumID, string ConnectionStr);
        public abstract int InsertForum(ForumDetails forum, string ConnectionStr);
        public abstract int InsertForum(DateTime AddedDate, string AddedBy, string Title, bool Moderated, int Importance, string ImageUrl, string Description, int ClanID, bool AlertClanMembers, byte SecurityLevel, string ConnectionStr);
        // methods that work with posts
        public abstract DataTable  GetThreads(int forumID, string sortExpression, int pageIndex, int pageSize,int PlayerID, string ConnectionStr);
        public abstract List<PostDetails> GetThreadByID(int threadPostID,int PlayerID, string ConnectionStr);
        public abstract List<PostDetails> GetUnapprovedPosts(int clainID, string ConnectionStr);
        public abstract PostDetails GetPostByID(int postID,int PlayerID,string ConnectionStr);
        public abstract bool DeletePost(int postID, int playerID, string ConnectionStr);
        public abstract bool UpdatePost(PostDetails post, int PlayerID,string ConnectionStr);
        public abstract int InsertPost(PostDetails post, int PlayerID, string ConnectionStr);
        public abstract bool ApprovePost(int postID, string ConnectionStr);
        public abstract bool CloseThread(int threadPostID, string ConnectionStr);
        public abstract bool MoveThread(int threadPostID, int forumID, string ConnectionStr);
        public abstract bool IncrementPostViewCount(int postID, string ConnectionStr);
        public abstract bool IsPlayerInClan(int PlayerID, int ClanID, string ConnectionStr);
        
        public abstract void SetThreadAsRead(int PlayerID, int ThreadPostID, string ConnectionStr);
        public abstract void SetForumAsRead(int PlayerID, int ForumID, string ConnectionStr);

        /// <summary>
        /// Returns a valid sort expression
        /// </summary>
        protected virtual string EnsureValidSortExpression(string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression))
                return "tbh_Posts.LastPostDate DESC";

            string sortExpr = sortExpression.ToLower();
            if (!sortExpr.Equals("lastpostdate") && !sortExpr.Equals("lastpostdate asc") && !sortExpr.Equals("lastpostdate desc") &&
               !sortExpr.Equals("viewcount") && !sortExpr.Equals("viewcount asc") && !sortExpr.Equals("viewcount desc") &&
               !sortExpr.Equals("replycount") && !sortExpr.Equals("replycount asc") && !sortExpr.Equals("replycount desc") &&
               !sortExpr.Equals("addeddate") && !sortExpr.Equals("addeddate asc") && !sortExpr.Equals("addeddate desc") &&
               !sortExpr.Equals("addedby") && !sortExpr.Equals("addedby asc") && !sortExpr.Equals("addedby desc") &&
               !sortExpr.Equals("title") && !sortExpr.Equals("title asc") && !sortExpr.Equals("title desc") &&
               !sortExpr.Equals("lastpostby") && !sortExpr.Equals("lastpostby asc") && !sortExpr.Equals("lastpostby desc"))
            {
                sortExpr = "lastpostdate desc";
            }
            if (!sortExpr.StartsWith("tbh_posts"))
                sortExpr = "tbh_posts." + sortExpr;
            if (!sortExpr.StartsWith("tbh_products.lastpostdate"))
                sortExpr += ", LastPostDate DESC";
            return sortExpr;
        }

        /// <summary>
        /// Returns a new ForumDetails instance filled with the DataReader's current record data
        /// </summary>
        protected virtual ForumDetails GetForumFromReader(IDataReader reader)
        {
            return new ForumDetails(
               (int)reader["ForumID"],
               (DateTime)reader["AddedDate"],
               reader["AddedBy"].ToString(),
               reader["Title"].ToString(),
               (bool)reader["Moderated"],
               (int)reader["Importance"],
               reader["Description"].ToString(),
               reader["ImageUrl"].ToString(),
               (int)reader["ClanID"],
               (bool)reader["AlertClanMembers"],
               (byte)reader["SecurityLevel"]);
        }

        /// <summary>
        /// Returns a collection of ForumDetails objects with the data read from the input DataReader
        /// </summary>
        protected virtual List<ForumDetails> GetForumCollectionFromReader(IDataReader reader)
        {
            List<ForumDetails> forums = new List<ForumDetails>();
            while (reader.Read())
                forums.Add(GetForumFromReader(reader));
            return forums;
        }

        /// <summary>
        /// Returns a new PostDetails instance filled with the DataReader's current record data
        /// </summary>
        protected virtual PostDetails GetPostFromReader(IDataReader reader)
        {
            return GetPostFromReader(reader, true);
        }
        protected virtual PostDetails GetPostFromReader(IDataReader reader, bool readBody)
        {
            PostDetails post = new PostDetails(
               (int)reader["PostID"],
               (DateTime)reader["AddedDate"],
               reader["AddedBy"].ToString(),
               reader["AddedByIP"].ToString(),
               (int)reader["ForumID"],
               reader["ForumTitle"].ToString(),
               (int)reader["ParentPostID"],
               reader["Title"].ToString(),
               null,
               (bool)reader["Approved"],
               (bool)reader["Closed"],
               (int)reader["ViewCount"],
               (int)reader["ReplyCount"],
               (DateTime)reader["LastPostDate"],
               reader["LastPostBy"].ToString(),
                (bool)reader["Sticky"]);
            if (readBody)
                post.Body = reader["Body"].ToString();

            return post;
        }

        /// <summary>
        /// Returns a collection of PostDetails objects with the data read from the input DataReader
        /// </summary>
        protected virtual List<PostDetails> GetPostCollectionFromReader(IDataReader reader)
        {
            return GetPostCollectionFromReader(reader, true);
        }
        protected virtual List<PostDetails> GetPostCollectionFromReader(IDataReader reader, bool readBody)
        {
            List<PostDetails> posts = new List<PostDetails>();
            while (reader.Read())
                posts.Add(GetPostFromReader(reader, readBody));
            return posts;
        }
    }
}
