using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

/// <summary>
/// Summary description for coe
/// </summary>
public class coe
{
    public coe()
    {
    }
    /// <summary>
    /// returns -1 if max title reach, 100 is level up is required, or between (0, 100) if in between 
    /// </summary>
    /// <returns></returns>
    public static double TitleProgress(Fbg.Bll.Player player)
    {
        double titleProgress = 0;
        Title nextTitle = player.Title.NextTitle;

        int curPoints = player.Points;
        if (curPoints > player.Title.MaxPoints && nextTitle != null) {
            titleProgress = 100;
        }
        else {
            int minPointsForCurTitle = 0;
            Title prevTitle = player.Title.PreviousTitle;
            if (prevTitle != null) {
                minPointsForCurTitle = prevTitle.MaxPoints + 1;
            }

            if (nextTitle != null) {
                int pointDifferenceBetweenLvls = player.Title.MaxPoints - minPointsForCurTitle + 1;
                int pointsNormalizedToCurlvl = curPoints - minPointsForCurTitle;
                titleProgress = Convert.ToInt32(Math.Floor(((double)pointsNormalizedToCurlvl / (double)pointDifferenceBetweenLvls) * 100));
                titleProgress = titleProgress < 0 ? 0 : titleProgress;
            }
            else {
                titleProgress = -1;
            }
        }

        return titleProgress;

    }


    public static void SendMessageAfterTitleAcceptIfNeeded(Fbg.Bll.Player player)
    {
        if (player.Realm.IsTemporaryTournamentRealm) {
            // none of this is for the tournament realm
            return;
        }

        if (player.Title.Level > Fbg.Bll.CONSTS.TitleLevels.Knight
            && player.Title.Level <= Fbg.Bll.CONSTS.TitleLevels.Lord
            && player.NumberOfVillages > 1
            && (player.HasFlag(Player.Flags.Misc_Got2VillagePromo) == null // this is left for compatibbility only 
                && player.User.HasFlag(User.Flags.Misc_Got2VillagePromo) == null
                )
            ) {
            if (!Fbg.Bll.utils.Admin_GiveServants(player.User.ID, 10, utils.GiveServantsReason.RewardOrPromo)) {
                // WE DO NOT Alert the player to this error but just log it 
                BaseApplicationException bex = new BaseApplicationException("Admin_GiveServants(player.User.ID, 20, utils.GiveServantsReason.RewardOrPromo) failed");
                bex.AddAdditionalInformation("player", player);
                ExceptionManager.Publish(bex);
            }
            else {
                player.User.SetFlag(User.Flags.Misc_Got2VillagePromo);
                Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(player.Realm), player.ID.ToString(), ROEResource.R_MiscMessages.GetString("email1_subject")
                    , String.Format(ROEResource.R_MiscMessages.GetString("email1_body"), player.Title.TitleName(player.Sex)), player.Name, player.Realm.ConnectionStr);
            }
        }
        if (player.Title.Level == Fbg.Bll.CONSTS.TitleLevels.Freeman) {
            Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(player.Realm), player.ID.ToString()
                , ROEResource.R_MiscMessages.GetString("email_KTitleInviteFriendsMsg_subject")
                , String.Format(ROEResource.R_MiscMessages.GetString("email_KTitleInviteFriendsMsg_body"), player.Name)
                , player.Name, player.Realm.ConnectionStr);
        }

    }

}