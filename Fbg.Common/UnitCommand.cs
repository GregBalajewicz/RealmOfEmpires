using System;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Common
{
    public class UnitCommand : Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection
    {
        public enum CommandType
        {
            Support=0
            ,Attack=1
            ,Return=2
           , Recall = 3
        }

        public class Units  
        { 
            /// <summary>
            /// 
            /// </summary>
            /// <param name="utID"></param>
            /// <param name="sendCount"></param>
            /// <param name="targetBuildingTypeID">Pass -1 if no building is targetted</param>
            public Units(int utID, int sendCount, int targetBuildingTypeID)
            { 
                this.utID = utID;
                this.sendCount = sendCount;
                this.targetBuildingTypeID = targetBuildingTypeID;
            }

            public Units()
            {
            }

            public int utID;
            public int sendCount;
            /// <summary>
            /// -1 if no building is targetted, otherwise ID of the building. 
            /// </summary>
            public int targetBuildingTypeID;
        }


        public CommandType command;
        public int targetVillageID;
        public int originVillageID;
        public List<Units> unitsSent;
        public double UnitDesertionFactor;

        #region ISerializableToNameValueCollection Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = String.Format("Command[{0}]", command.ToString());

                if (col == null)
                {
                    ExceptionManager.Publish("Error in UnitCommand.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    col.Add(pre + "targetVillageID", targetVillageID.ToString());
                    col.Add(pre + "originVillageID", originVillageID.ToString());

                    if (unitsSent == null)
                    {
                        col.Add(pre + "unitsSent", "null");
                    }
                    else
                    {
                        foreach (Units u in unitsSent)
                        {
                            col.Add(pre + "unitsSent.utID", u.utID.ToString());
                            col.Add(pre + "unitsSent.sendCount", u.sendCount.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Village.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion
    }
}
