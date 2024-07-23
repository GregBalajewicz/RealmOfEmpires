using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Fbg.Bll.Api.CommandTroops
{
    public class UnitMovements
    {
        Fbg.Bll.Player _player;        

         public UnitMovements(Fbg.Bll.Player player)
        {
            _player = player;
        }



         public string ToggleHide(long eventID)
         {
             int curHiddenState = Fbg.Bll.UnitMovements.ToggleHide(_player, eventID);

             return ApiHelper.RETURN_SUCCESS(new
             {
                 eventID = eventID,
                 curHiddenState = curHiddenState
             });
         }

         public string GetDetails(long eventID)
         {
             DataTable tbl = Fbg.Bll.UnitMovements.GetUnitMovementDetails(_player, eventID);
             var troops = tbl.AsEnumerable().Select(
                    unit => new
                    {
                        id = unit.Field<int>(Fbg.Bll.UnitMovements.CONSTS.UnitMovementDetColIndex.UnitTypeId),
                        count = unit.Field<int>(Fbg.Bll.UnitMovements.CONSTS.UnitMovementDetColIndex.UnitCount),
                    }
                );
             
             return ApiHelper.RETURN_SUCCESS(new
             {
                 eventID = eventID,
                 troops = troops
             });
         }


         public string Cancel(long eventID)
         {
             if (Fbg.Bll.UnitMovements.Cancel(_player, eventID))
             {
                 return ApiHelper.RETURN_SUCCESS(new
                 {
                     eventID = eventID
                 });
             }

             // signal failure
             return ApiHelper.RETURN_SUCCESS(new
             {
                 eventID = 0
             });
         }
    }
}
