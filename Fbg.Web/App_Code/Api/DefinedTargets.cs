using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Bll;
using Fbg.Bll.Items2;
using System.Dynamic;
using System.Data;

namespace Fbg.Bll.Api
{
    /// <summary>
    /// Summary description for Items
    /// </summary>
    public class DefinedTargets
    {

        public static string GetAll(Fbg.Bll.Player p)
        {
            return ApiHelper.RETURN_SUCCESS(p.DefinedTargets.Get());
        }

        public static string Add(Player fbgPlayer, int villageid, short typeID, DateTime? setTime, string note, int expiresInXDays, string assignedTo)
        {
            int id;
            List<DefinedTarget> list = fbgPlayer.DefinedTargets.Add(villageid, typeID, setTime, note, out id ,expiresInXDays, assignedTo);

            return ApiHelper.RETURN_SUCCESS(new { newDefinedTargetID=id, definedTargets=list });
        }
        public static string Edit(Player fbgPlayer, int definedTargetID, DateTime? setTime, string note, int expiresInXDays, string assignedTo)
        {          
            return ApiHelper.RETURN_SUCCESS(fbgPlayer.DefinedTargets.Edit(definedTargetID, setTime, note, expiresInXDays, assignedTo));
        }
        public static string Delete(Player fbgPlayer, int definedTargetID)
        {

            //return ApiHelper.RETURN_SUCCESS(fbgPlayer.DefinedTargets.Get());


            return ApiHelper.RETURN_SUCCESS(fbgPlayer.DefinedTargets.Delete(definedTargetID));
        }

        //public static string AddEdit(Player fbgPlayer, int definedTargetID, DateTime? setTime, string note)
        //{
        //    note = Utils.ClearHTMLCode(note.Trim());
        //    note = Utils.ClearInvalidChars(note);
        //    note = global::BBCodes.PreProcessBBCodes(fbgPlayer.Realm, global::BBCodes.Medium.Chat, note);
        //    return ApiHelper.RETURN_SUCCESS(fbgPlayer.DefinedTargets.Edit(definedTargetID, setTime, note));
        //}


        /// <summary>
        /// reset response to null if you want to delete it
        /// </summary>
        /// <param name="definedTargetID"></param>
        /// <param name="responseByPlayerID"></param>
        /// <param name="responseTypeID"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string AddEditDeleteResonse(Player fbgPlayer, int definedTargetID, Int16 responseTypeID, string response)
        {
            response = Utils.ClearHTMLCode(response.Trim());
            response = Utils.ClearInvalidChars(response);
            response = global::BBCodes.PreProcessBBCodes(fbgPlayer.Realm, global::BBCodes.Medium.Chat, response);

            return ApiHelper.RETURN_SUCCESS(fbgPlayer.DefinedTargets.AddEditResonse(definedTargetID, fbgPlayer.ID, responseTypeID, response));
        }


    }
}