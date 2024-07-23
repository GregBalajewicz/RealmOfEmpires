using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Threading;

using Fbg.DAL;

namespace Fbg.Bll
{

    public  class EventHandler : GmbcBaseClass
    {
        Thread thread;
        Realm realm;
        string message; 

        public event UpdateDelegate Update;
        public delegate void UpdateDelegate(Realm r, string msg);

        public event UpdateDelegate UpdateError;
        List<long> eventsMarkedAsDoneDueToError = new List<long>();


        private static class CONSTS
        {
            public class EventsColumnIndex
            {
                public static int EventID = 0;
                public static int EventTime = 1;
                public static int Status = 2;
                public static int VillageID = 3;
                public static int BuildingTypeID = 4;
                public static int Level = 5;
                public static int CommandType = 6;
                public static int Amount = 7;
                public static int BuildingDowngradeEventID = 8;
                public static int ResearchItemID = 9;
                public static int RaidID = 10; //raid movement attack landing event
            }

            public class EventStatus
            {
                public static int Pending = 0;
                public static int Processed = 1;
            }
        }


        public EventHandler(Realm realm)
        {
            this.realm = realm;
        }



        public void Run()
        {
            if (thread != null) 
            {
                throw new BaseApplicationException("Run was called but thread is not null");
            }
            message = DateTime.Now.ToShortTimeString() + "Run()";
            TRACE.InfoLine(message);
            Update(realm, message);
            //realmEndsOn = DAL.EventHandler.GetRealmEndingTime();
            ThreadStart tStart = new ThreadStart(this.Do);
            thread = new Thread(tStart);
            thread.Name = realm.Name;
            thread.Start();
        }


        public void Stop()
        {
            Update(realm, "Stop()");
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }

        public void RunStateToggle()
        {
            if ( isRunning)
            {
                this.Stop();
            }
            else
            {
                this.Run();
            }

        }


        public bool isRunning
        {
            get
            {
                if (thread != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }


        private void Do()
        {
            DataTable dtDueEvents = null;

            while (true)
            {
                try
                {
                    //
                    // if realm is close, end it 
                    //
                    if (DateTime.Now >= realm.ClosingOn)
                    {
                        Update(realm, "realm closed. aborting.");
                        this.Stop();
                    }


                    if (DateTime.Now > realm.OpenOn)
                    {
                        //
                        // realm is opened
                        //
                        dtDueEvents = GetDueEvents();
                        Update(realm, DateTime.Now.ToString("HH:mm:ss:fffffff") + " Got events. Event count:" + dtDueEvents.Rows.Count.ToString());

                        ProcessEvents(dtDueEvents);
                    }
                    else
                    {
                        //
                        // realm is not yet opened
                        //
                        TimeSpan timeTillRealmOpens = realm.OpenOn.Subtract(DateTime.Now);
                        if (timeTillRealmOpens.TotalMilliseconds > 0)
                        {
                            int sleepFor = timeTillRealmOpens.TotalMilliseconds > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(timeTillRealmOpens.TotalMilliseconds);
                            Update(realm, "Sleep for " + sleepFor + "MS");
                            System.Threading.Thread.Sleep(sleepFor);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    try
                    {
                        BaseApplicationException bex = new BaseApplicationException("Erorr in Do()", e);
                        bex.AdditionalInformation.Add("dtDueEvents", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtDueEvents));
                        ExceptionManager.Publish(bex);
                        UpdateError(realm, "Error in DO(). Sleeeping 1 min. e.Message:" + e.Message);

                        System.Threading.Thread.Sleep(1000);
                    }
                    catch
                    {
                        //we never want this to fail... 
                    }
                }
            }

        }

        private void ProcessEvents(DataTable dtDueEvents)
        {
            DateTime eventDueDate;
            TimeSpan timeTillNextEvent;
            TimeSpan sleepTime;
            if (dtDueEvents.Rows.Count == 0)
            {
                //Update("No Events, sleeping for 4 seconds");
                System.Threading.Thread.Sleep(4000);
            }
            else
            {
                foreach (DataRow dr in dtDueEvents.Rows)
                {
                    eventDueDate = (DateTime)dr[CONSTS.EventsColumnIndex.EventTime];
                    if (eventDueDate < realm.ClosingOn) // never process event after realm is closed
                    {
                        timeTillNextEvent = eventDueDate.Subtract(DateTime.Now);
                        if (timeTillNextEvent.TotalMilliseconds <= 1000)
                        {
                            ProcessEvent(dr);
                        }
                        else
                        {
                            sleepTime = timeTillNextEvent.Subtract(new TimeSpan(0, 0, 0, 1));
                            TRACE.VerboseLine("Sleep for " + sleepTime.TotalMilliseconds + "ms");
                            System.Threading.Thread.Sleep(sleepTime);
                            ProcessEvent(dr);
                        }
                    }
                }
            }
        }

        private void ProcessEvent(DataRow dr)
        {
            long eventID = (long)dr[CONSTS.EventsColumnIndex.EventID];
            message = "Processing EventID:" +eventID.ToString() + " due on:" + ((DateTime)dr[CONSTS.EventsColumnIndex.EventTime]).ToString("HH:mm:ss:fffffff");

            try
            {

                if (!(dr[CONSTS.EventsColumnIndex.BuildingTypeID] is System.DBNull))
                {
                    message = message + " Building";
                    Update(realm, message);

                    ProcessBuildingUpgrader(dr);
                }
                else if (!(dr[CONSTS.EventsColumnIndex.CommandType] is System.DBNull))
                {
                    message = message + " Troops";
                    Update(realm, message );

                    ProcessUnitMovement(dr);
                }

                else if (!(dr[CONSTS.EventsColumnIndex.Amount] is System.DBNull))
                {
                    message = message + " CoinTran";
                    Update(realm, message );

                    ProcessTransportCoins(dr);
                }
                else if (!(dr[CONSTS.EventsColumnIndex.BuildingDowngradeEventID] is System.DBNull))
                {
                    message = message + " Downgrade";
                    Update(realm, message );

                    ProcessBuildingDowngrader(dr);
                }
                else if (!(dr[CONSTS.EventsColumnIndex.ResearchItemID] is System.DBNull))
                {
                    message = message + " research";
                    Update(realm, message );

                    ProcessResearch(dr);
                }
                else if (!(dr[CONSTS.EventsColumnIndex.RaidID] is System.DBNull))
                {
                    message = message + " Raid attack landing";
                    Update(realm, message);

                    ProcessRaidMovement(dr);
                }
                else
                {
                    throw new Exception("Unrecognized Event type!");
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {                
                BaseApplicationException ex = new BaseApplicationException("Error in " + message, e);
                ex.AdditionalInformation.Add("EventID", dr[CONSTS.EventsColumnIndex.EventID].ToString());
                ex.AddAdditionalInformation("realm.ID", realm.ID);
                ExceptionManager.Publish(ex);

                //
                // how to deal with this event?
                //  if we already got this event in "eventsMarkedAsDoneDueToError" that means it failed before therefore we set it as processed
                //  otherwise, we add it to "eventsMarkedAsDoneDueToError" and move on. if it fails again, we will mark it as processed
                //
                if (eventsMarkedAsDoneDueToError.Exists(eid => eid == eventID))
                {
                    eventsMarkedAsDoneDueToError.Remove(eventID);
                    HandleFailedEvent((long)dr[CONSTS.EventsColumnIndex.EventID]);
                }
                else
                {
                    eventsMarkedAsDoneDueToError.Add(eventID);
                    UpdateError(realm, "ERROR: Processing EventID:" + eventID.ToString() + " Failed first time. Taking no action yet");
                }
            }
        }

        private void ProcessResearch(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.CompleteResearch(realm.ConnectionStr, (Int64)dr[CONSTS.EventsColumnIndex.EventID]);
                    return;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessResearch() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }

        private void ProcessBuildingDowngrader(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.CompleteBuilingDowngrade(realm.ConnectionStr, (Int64)dr[CONSTS.EventsColumnIndex.EventID]);
                    return;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling CompleteBuilingDowngrade() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }

        private DataTable GetDueEvents()
        {
            TRACE.VerboseLine("Calling DAL.EventHandler.GetDueEvents()");
            while (true)
            {
                try
                {
                    return DAL.EventHandler.GetDueEvents(realm.ConnectionStr);
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling DAL.EventHandler.GetDueEvents() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }

            } 
        }

        private void ProcessUnitRecruitment(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.CompleteUnitRecruitment(realm.ConnectionStr, (Int64)dr[CONSTS.EventsColumnIndex.EventID]);
                    return;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessUnitRecruitment() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }

        private void ProcessUnitMovement(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.ProcessUnitMovement(realm.ConnectionStr
                        , (Int64)dr[CONSTS.EventsColumnIndex.EventID]
                        , (Fbg.Common.UnitCommand.CommandType)(short)dr[CONSTS.EventsColumnIndex.CommandType]);
                    return;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessUnitMovement() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }
        private void ProcessBuildingUpgrader(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.CompleteBuilingUpgrade(realm.ConnectionStr
                        , (Int64)dr[CONSTS.EventsColumnIndex.EventID]
                        , (int)dr[CONSTS.EventsColumnIndex.VillageID]
                        , (int)dr[CONSTS.EventsColumnIndex.BuildingTypeID]
                        , (int)dr[CONSTS.EventsColumnIndex.Level]);
                    return;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessBuildingUpgrader() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }
        private void ProcessTransportCoins(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.TransportCoinsCompleted(realm.ConnectionStr
                        , (Int64)dr[CONSTS.EventsColumnIndex.EventID]);
                    return;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessTransportCoins() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }

        private void ProcessRaidMovement(DataRow dr)
        {
            while (true)
            {
                try
                {
                    Fbg.DAL.EventHandler.ProcessRaidMovement(realm.ConnectionStr, (Int64)dr[CONSTS.EventsColumnIndex.EventID]);
                    return;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Fbg.DAL.SqlTimeOutException)
                {
                    UpdateError(realm, "Calling ProcessRaidMovement() - Got Sql Timeout. Sleeping.");
                    System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                }
            }
        }

        /// <summary>
        /// If an event failed, we simply mark it as processed to avoid trying to process the event over and over again
        /// </summary>
        /// <param name="eventID"></param>
        private void HandleFailedEvent(Int64 eventID)
        {
            UpdateError(realm, "ERROR: Processing EventID:" + eventID.ToString() + " Failed!.  Setting the event as processed");

            try
            {
                while (true)
                {
                    try
                    {
                        Fbg.DAL.EventHandler.SetEventAsProcessed(realm.ConnectionStr
                        , eventID);
                        return;
                    }
                    catch (Fbg.DAL.SqlTimeOutException)
                    {
                        UpdateError(realm, "Calling HandleFailedEvent() - Got Sql Timeout. Sleeping.");
                        System.Threading.Thread.Sleep(1000); // sleep for 1 sec
                    }
                }

            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                UpdateError(realm, "ERROR: Processing EventID:" + eventID.ToString() + " Failed!.  Setting the event as processed FAILED!");
                BaseApplicationException ex = new BaseApplicationException("Setting the event as processed FAILED!", e);
                ex.AdditionalInformation.Add("EventID", eventID.ToString());
                ExceptionManager.Publish(e);
            }


        }

    }

}
