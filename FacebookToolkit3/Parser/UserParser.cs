using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using Facebook.Entity;
using Facebook.Parser;
using Facebook.Utility;

namespace Facebook.Parser {
    internal sealed class UserParser
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private UserParser() { }

        /// <summary>
        /// Uses DOM parsing to constitute a PhotoTag data object given the xml returned from facebook
        /// </summary>
        internal static User ParseUser(XmlNode node)
        {
            User user = new User();
            user.UserId = XmlHelper.GetNodeText(node, "uid");
            user.FirstName = XmlHelper.GetNodeText(node, "first_name");
            user.LastName = XmlHelper.GetNodeText(node, "last_name");
            user.Name = XmlHelper.GetNodeText(node, "name");

//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "pic")))
//            {
//                user.PictureUrl = new Uri(XmlHelper.GetNodeText(node, "pic"));
//            }
//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "pic_small")))
//            {
//                user.PictureSmallUrl = new Uri(XmlHelper.GetNodeText(node, "pic_small"));
//            }
//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "pic_big")))
//            {
//                user.PictureBigUrl = new Uri(XmlHelper.GetNodeText(node, "pic_big"));
//            }
            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "pic_square")))
            {
                user.PictureSquareUrl = new Uri(XmlHelper.GetNodeText(node, "pic_square"));
            }

//            user.Religion = XmlHelper.GetNodeText(node, "religion");
//            DateTime tempDate;
//#if NETCF
//            try
//            {
//                tempDate = DateTime.Parse(XmlHelper.GetNodeText(node, "birthday"));
//                user.Birthday = tempDate;
//            }
//            catch
//            {
//            }
//#else
//            if (DateTime.TryParse(XmlHelper.GetNodeText(node, "birthday"), out tempDate))
//            {
//                user.Birthday = tempDate;
//            }
//#endif

//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "profile_update_time"))  && double.Parse(XmlHelper.GetNodeText(node, "profile_update_time")) > 0)
//            {
//                user.ProfileUpdateDate = DateHelper.ConvertDoubleToDate(double.Parse(XmlHelper.GetNodeText(node, "profile_update_time"), CultureInfo.InvariantCulture));
//            }


            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "sex")))
            {
                if (Enum.IsDefined(typeof(Gender), XmlHelper.GetNodeText(node, "sex")))
                {
                    user.Sex = (Gender)Enum.Parse(typeof(Gender), XmlHelper.GetNodeText(node, "sex"), true);
                }
                else
                {
                    switch (XmlHelper.GetNodeText(node, "sex").ToUpper())
                    {
                        case "MALE":
                            user.Sex = Gender.Male;
                            break;
                        case "FEMALE":
                            user.Sex = Gender.Female;
                            break;
                        default:
                            user.Sex = Gender.Unknown;
                            break;
                    }
                }
            }

//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "relationship_status")))
//            {
//                if (Enum.IsDefined(typeof(RelationshipStatus), XmlHelper.GetNodeText(node, "relationship_status")))
//                {
//                    user.RelationshipStatus = (RelationshipStatus)Enum.Parse(typeof(RelationshipStatus), XmlHelper.GetNodeText(node, "relationship_status").Replace(" ", "").Replace("'", ""), true);
//                }
//                else
//                {
//                    user.RelationshipStatus = RelationshipStatus.Unknown;
//                }
//            }

//            user.SignificantOtherId = XmlHelper.GetNodeText(node, "significant_other_id");

//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(node, "political")))
//            {
//                //user.PoliticalView = (PoliticalView)Enum.Parse(typeof(PoliticalView), XmlHelper.GetNodeText(node, "political").Replace(" ", ""), true);
//            }

//            user.Activities = XmlHelper.GetNodeText(node, "activities");
//            user.Interests = XmlHelper.GetNodeText(node, "interests");
//            user.Music = XmlHelper.GetNodeText(node, "music");
//            user.TVShows = XmlHelper.GetNodeText(node, "tv");
//            user.Movies = XmlHelper.GetNodeText(node, "movies");
//            user.Books = XmlHelper.GetNodeText(node, "books");
//            user.Quotes = XmlHelper.GetNodeText(node, "quotes");
//            user.AboutMe = XmlHelper.GetNodeText(node, "about_me");


//            int tempInt = 0;
//#if NETCF
//            try
//            {
//                tempInt = int.Parse(XmlHelper.GetNodeText(node, "notes_count"));
//                user.NotesCount = tempInt;
//            }
//            catch
//            {
//            }

//            try
//            {
//                tempInt = int.Parse(XmlHelper.GetNodeText(node, "wall_count"));
//                user.WallCount = tempInt;
//            }
//            catch
//            {
//            }
//#else
//            if (int.TryParse(XmlHelper.GetNodeText(node, "notes_count"), out tempInt))
//            {
//                user.NotesCount = tempInt;
//            }
//            if (int.TryParse(XmlHelper.GetNodeText(node, "wall_count"), out tempInt))
//            {
//                user.WallCount = tempInt;
//            }
//#endif

//            XmlNodeList statusNodeList = ((XmlElement)node).GetElementsByTagName("status");
//            user.Status.Message = XmlHelper.GetNodeText(statusNodeList[statusNodeList.Count-1], "message");
//            if (!String.IsNullOrEmpty(XmlHelper.GetNodeText(statusNodeList[statusNodeList.Count - 1], "time")))
//            {
//                user.Status.Time = DateHelper.ConvertDoubleToDate(double.Parse(XmlHelper.GetNodeText(statusNodeList[statusNodeList.Count - 1], "time"), CultureInfo.InvariantCulture));
//            }
            
//            XmlElement xmlElement = node as XmlElement;

//            //affiliations
//            NetworkParser.ParseNetworks(xmlElement.GetElementsByTagName("affiliations")[0], user.Affiliations);

//            //meeting_sex
//            user.InterestedInGenders = ParseInterestedInGenders(xmlElement.GetElementsByTagName("meeting_sex")[0]);

//            //interested_in
//            user.InterstedInRelationshipTypes = ParseRelationshipTypes(xmlElement.GetElementsByTagName("meeting_for")[0]);

//            //hometown_location
//            user.HometownLocation = LocationParser.ParseLocation(xmlElement.GetElementsByTagName("hometown_location")[0]);

//            //curent_location
//            user.CurrentLocation = LocationParser.ParseLocation(xmlElement.GetElementsByTagName("current_location")[0]);

//            //school_history
//            user.SchoolHistory = SchoolHistoryParser.ParseSchoolHistory(xmlElement.GetElementsByTagName("hs_info")[0], xmlElement.GetElementsByTagName("education_history")[0]);

//            //work_history
//            user.WorkHistory = WorkParser.ParseWorkHistory(xmlElement.GetElementsByTagName("work_history")[0]);


            return user;
        }
        /// <summary>
        /// Uses DOM parsing to constitute a collection of relationshiptype object given the xml returned from facebook
        /// </summary>
        private static Collection<LookingFor> ParseRelationshipTypes(XmlNode node)
        {
            Collection<LookingFor> relationshipTypeList = new Collection<LookingFor>();

            foreach (XmlNode seekingNode in ((XmlElement)node).GetElementsByTagName("seeking"))
            {
                try
                {
                    relationshipTypeList.Add((LookingFor)Enum.Parse(typeof(LookingFor), ((XmlElement)seekingNode).InnerText.Replace(" ", "").Replace("'", ""), true));
                }
                catch (ArgumentException)
                {
                    // if there was no enum for this relationship type, we set it to Unknown
                    relationshipTypeList.Add(LookingFor.Unknown);
                }
            }
            return relationshipTypeList;

        }
        /// <summary>
        /// Uses DOM parsing to constitute a collection of genderlist object given the xml returned from facebook
        /// </summary>
        private static Collection<Gender> ParseInterestedInGenders(XmlNode node)
        {
            Collection<Gender> genderList = new Collection<Gender>();

            foreach (XmlNode sexNode in ((XmlElement)node).GetElementsByTagName("sex"))
            {
                if (Enum.IsDefined(typeof(Gender), ((XmlElement)sexNode).InnerText))
                {
                    genderList.Add((Gender)Enum.Parse(typeof(Gender), ((XmlElement)sexNode).InnerText, true));
                }
                else
                {
                    genderList.Add(Gender.Unknown);
                }
            }
            return genderList;

        }
        
    }
}