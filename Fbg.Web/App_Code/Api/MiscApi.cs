using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Web.Security;
using System.Data;
using BDA.Achievements;
using System.Collections;

namespace Fbg.Bll.Api
{
    public class MiscApi
    {

        private static RecoveryEmailUpdateFailreReasons RecoveryEmail_canUseThisEmail(string email, MembershipUser loggedInMembershipUser)
        {
            email = email.ToLower(); // better to do this to ensure no stupid case caused mismatches

            // does this player already have this exact email?
            if (loggedInMembershipUser.Email == email)
            {
                return RecoveryEmailUpdateFailreReasons.FAIL_emailNotDifferentThanCurrent;
            }

            // does some user already use this email in their username ?
            MembershipUser userWithNewEmail = Membership.GetUser(email);
            if (userWithNewEmail != null || (userWithNewEmail != null && userWithNewEmail.UserName != loggedInMembershipUser.UserName))
            {
                return RecoveryEmailUpdateFailreReasons.FAIL_emailAlredyInUse; // email already in use in form of username
            }

            // does some user already use this email as their email ?
            string usernameOfUserWithNewEmail = Membership.GetUserNameByEmail(email);
            if (usernameOfUserWithNewEmail != null || (usernameOfUserWithNewEmail != null && usernameOfUserWithNewEmail != loggedInMembershipUser.UserName))
            {
                return RecoveryEmailUpdateFailreReasons.FAIL_emailAlredyInUse; // email already in use by some other user in the system
            }

            if (String.Equals(loggedInMembershipUser.UserName, loggedInMembershipUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                // for tactica account, we do not allow chaning email for now
                if (!String.Equals(loggedInMembershipUser.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    return RecoveryEmailUpdateFailreReasons.FAIL_cannotChangeTacticaEmail;
                }
            }

            return RecoveryEmailUpdateFailreReasons.OK_nofailure;
        }

        enum RecoveryEmailUpdateFailreReasons
        {
            OK_nofailure = 0,
            FAIL_emailAlredyInUse,
            FAIL_emailNotDifferentThanCurrent,
            FAIL_cannotChangeTacticaEmail
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string RecoveryEmail(string email, MembershipUser loggedInMembershipUser, Fbg.Bll.User user, Fbg.Bll.Player player, string requestURL)
        {

            bool sent = false;
            bool updated = false;
            RecoveryEmailUpdateFailreReasons updateFailureReasonCode = RecoveryEmailUpdateFailreReasons.OK_nofailure;
            email = email.ToLower(); // better to do this to ensure no stupid case caused mismatches
            try
            {
                updateFailureReasonCode = RecoveryEmail_canUseThisEmail(email, loggedInMembershipUser);

                //
                // if the email is OK (is not used by someone else) then send a verification email.
                //  we resend it even if the email was exactly the same as before. this way, player always get verification email 
                //
                if (updateFailureReasonCode == RecoveryEmailUpdateFailreReasons.OK_nofailure
                    || updateFailureReasonCode == RecoveryEmailUpdateFailreReasons.FAIL_emailNotDifferentThanCurrent)
                {
                    try
                    {
                        Mailer mailer = new Mailer(Config.awsAccessKey, Config.awsSecretKey);
                        string url = requestURL;
                        int slashslash = url.IndexOf("//") + 2;
                        int slash = url.IndexOf("/", slashslash);
                        string host = url.Substring(0, slash);
                        Guid id = (Guid)loggedInMembershipUser.ProviderUserKey;
                        sent = mailer.SendRecoveryEmailVerification(host, id.ToString(), email, player.Name, player.Realm.Name, Config.addressToBCCSomeEmailsTo);
                    }
                    catch (Exception e)
                    {
                        BaseApplicationException bex = new BaseApplicationException("error sending email verification email", e);
                        bex.AddAdditionalInformation("user.ID", user.ID);
                        bex.AddAdditionalInformation("email", email);
                        Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(bex);
                    }
                }

                if (updateFailureReasonCode == RecoveryEmailUpdateFailreReasons.OK_nofailure)
                {
                    //
                    // update the email 
                    loggedInMembershipUser.Email = email;
                    user.SetRecoveryEmailState(Fbg.Bll.User.RecoveryEmailState.Unverified);
                    Membership.UpdateUser(loggedInMembershipUser);
                    //
                    // CRITICAL! no calls can be made after this, that could trigger a reload since username has changed!!
                    //
                    updated = true;

                    FormsAuthentication.SignOut();
                }


            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in RecoveryEmail()", e);
                bex.AddAdditionalInformation("user.ID", user.ID);
                bex.AddAdditionalInformation("email", email);
                Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(bex);
            }
            return ApiHelper.RETURN_SUCCESS(new
            {
                sent,
                updated,
                v2_wasVerificationEmailSent = sent,
                v2_wasEmailUpdated = updated,
                v2_wasEmailUpdated_updateFailureReasonCode = updateFailureReasonCode.ToString(),
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FbgPlayer"></param>
        /// <param name="villageID"></param>
        /// <returns>see return value of FbgPlayer.Clan.ClaimVillage()</returns>
        public static string ClaimVillage(Player FbgPlayer, string villageID)
        {
            int result=0;
            if (FbgPlayer.Clan != null)
            {
                result = FbgPlayer.Clan.ClaimVillage(Convert.ToInt32(villageID));
            }


            return ApiHelper.RETURN_SUCCESS(new
            {
                claimResults = result,
                villageID = villageID
            });
        }

                /// <summary>
        /// 
        /// </summary>
        /// <param name="FbgPlayer"></param>
        /// <param name="villageID"></param>
        /// <returns>see return value of FbgPlayer.Clan.ClaimVillage_Unclaim()</returns>
        public static string ClaimVillage_Unclaim(Player FbgPlayer, string villageID)
        {
            int result=0;
            if (FbgPlayer.Clan != null)
            {
                result = FbgPlayer.Clan.ClaimVillage_Unclaim(Convert.ToInt32(villageID));
            }


            return ApiHelper.RETURN_SUCCESS(new
            {
                claimResults = result,
                villageID = villageID
            });
        }


        /// <summary>
        /// Returning Dynamic list here instead of list of UserAvatars so API doesnt freak out
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<dynamic> AvatarsGetListAll(Guid userID)
        {

            List<dynamic> avatarList = new List<dynamic>();

            DataSet userAvatarData = Fbg.DAL.User.AvatarData(userID);
            DataTable avatarTable = userAvatarData.Tables[0];
            DataTable userUnlockedAvatars = userAvatarData.Tables[1];

            int Status = 0;

            foreach (DataRow row in avatarTable.Rows)
            {

                Avatar AV = new Avatar(row);
                if (AV.AvatarType == 1) //automatic unlocking of all type 1 avatars
                { 
                    Status = 1;
                }
                else //type 2 and 3 need ownership lookup
                {    
                    bool unlocked = userUnlockedAvatars.AsEnumerable().Any(unlockedRow => Convert.ToInt32(unlockedRow["AvatarID"]) == AV.AvatarID);
                    Status = unlocked ? 1 : 0;
                }

                UserAvatar UAV = new UserAvatar(AV, Status);

                avatarList.Add(new
                {
                    id = UAV.AvatarID,
                    type = UAV.AvatarType,
                    imageUrlS = UAV.ImageUrlS,
                    imageUrlL = UAV.ImageUrlL,
                    info = UAV.Info,
                    cost = UAV.Cost,
                    status = UAV.Status
                });

            }

            return avatarList;

        }

        /// <summary>
        /// For purchasing an avatar and unlocking it for your user account.
        /// </summary>
        /// <param name="bllUser"></param>
        /// <param name="purchaseAvatarID"></param>
        /// <returns></returns>
        public static string AvatarsPurchase(Fbg.Bll.User bllUser, int purchaseAvatarID)
        {

            //avatar existance sanity check
            Avatar av = Realms.Avatars.GetAvatar(purchaseAvatarID);
            if(av == null){
                return "error: avatar not found";
            }
            if (av.Cost < 1) {
                return "error: avatar not for purchase";
            }

            int result;

            //get current avatar and user ownership status 
            DataSet userAvatarData = Fbg.DAL.User.AvatarData(bllUser.ID);
            //DataTable avatarTable = userAvatarData.Tables[0];
            DataTable userUnlockedAvatars = userAvatarData.Tables[1];

            //we want to make sure user doesnt already have the avatar
            bool unlocked = userUnlockedAvatars.AsEnumerable().Any(unlockedRow => Convert.ToInt32(unlockedRow["AvatarID"]) == purchaseAvatarID);
            if (unlocked)
            {
                result = 1; //FAIL CODE 1: already unlocked avatar
            }
            else {
                if (bllUser.Credits >= av.Cost)
                {
                    bllUser.UseCredits(av.Cost, 34, 0); 
                    Realms.Avatars.unlockUserAvatarByAvatarID(bllUser, purchaseAvatarID);
                    result = 0;
                }
                else {
                    result = 2; //FAIL CODE 2: not enough credits
                }
            }

            return Api.ApiHelper.GetJsonSerializer.Serialize(new
            {
                success = true,
                @object = new
                {
                    result = result,
                    avatarID = purchaseAvatarID,
                    credits = bllUser.Credits, //updated user credits
                    newList = MiscApi.AvatarsGetListAll(bllUser.ID) //updated user avatars list
                }

            });



        }




        public static string GetNExtRecommendeQuest(Player FbgPlayer)
        {

            return ApiHelper.RETURN_SUCCESS(new
            {
                nextQuests = GetNextRecommendeQuestRaw(FbgPlayer)
            });
        }


        /// <summary>
        /// used in player refresh
        /// </summary>
        /// <param name="FbgPlayer"></param>
        /// <returns></returns>
        public static IEnumerable GetNextRecommendeQuestRaw(Player FbgPlayer)
        {

            if (FbgPlayer.NumberOfVillages > 1)
            {
                return new List<object>();
            }
            return FbgPlayer.Quests2.NextQuestsInProgression.Select(r => new { Tag = r.Tag, ID = r.ID , Goal = r.Goal, Title = r.Title }) ;
        }


        public static string Quest_GetRewardForCompletedQuest(Player FbgPlayer, string questTag)
        {
            Quest q = FbgPlayer.Quests_CompleteAQuest(questTag);
            return ApiHelper.RETURN_SUCCESS(new
            {
                rewardedQuest = q == null ? "" : q.Tag
            });
        }


        public static string VideoAdd_ResetIsAvailableFlag(User user )
        {
            user.SetFlag(Fbg.Bll.User.Flags.VideAd_IsAvailable, "viewinitiated");
            return ApiHelper.RETURN_SUCCESS(new
            {
                
            });
        }

    }
}