using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fbg.Bll;
using System.Data;
using System.Diagnostics;


using Gmbc.Common.Diagnostics;

namespace Fbg.EventHandlerWin2
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<realmEH> ehs;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer_lisclear;
        System.Windows.Threading.DispatcherTimer dispatcherTimer_heartbeat;
        ListView lvErrors;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartOrReInit()
        {
            try
            {
                //
                // do clean up
                //
                Fbg.Bll.Realms.ADMINONLY_REINIT();
                if (ehs != null)
                {
                    foreach (realmEH eh in ehs)
                    {
                        eh.eventHandler.Stop();
                    }
                }
                tabs.Items.Clear();

                ehs = new List<realmEH>(Fbg.Bll.Realms.AllRealms.Count);

                realmEH oneEH;
                TabItem ti;
                ListView lv;



                //
                // erorr tab
                //
                ti = new TabItem() { Name = "Errors", Header = "Errors" };
                lvErrors = new ListView();
                lvErrors.Name = "events_errors";
                lvErrors.View = new GridView();
                ((GridView)lvErrors.View).Columns.Add(new GridViewColumn() { Header = "Time", DisplayMemberBinding = new Binding("time") });
                ((GridView)lvErrors.View).Columns.Add(new GridViewColumn() { Header = "Realm", DisplayMemberBinding = new Binding("realm") });
                ((GridView)lvErrors.View).Columns.Add(new GridViewColumn() { Header = "Message", DisplayMemberBinding = new Binding("Message") });
                ti.Content = lvErrors;
                tabs.Items.Add(ti);
                //
                // realms tab
                //
                foreach (Realm r in Fbg.Bll.Realms.AllRealms)
                {
                    if (r.ClosingOn > DateTime.Now)
                    {
                        oneEH = new realmEH();
                        ehs.Add(oneEH);

                        oneEH.eventHandler = new Bll.EventHandler(r);
                        oneEH.r = r;


                        // object realmContainer = { Name = "ddd" };
                        ti = new TabItem() { Name = "r" + r.Tag, Header = r.Tag};
                        lv = new ListView();
                        lv.Name = "events_r" + r.Tag;
                        lv.View = new GridView();
                        ((GridView)lv.View).Columns.Add(new GridViewColumn() { Header = "Time", DisplayMemberBinding = new Binding("time") });
                        ((GridView)lv.View).Columns.Add(new GridViewColumn() { Header = "Message", DisplayMemberBinding = new Binding("Message") });
                        ti.Content = lv;
                        oneEH.lv = lv;
                        tabs.Items.Add(ti);
                        oneEH.ti = ti;



                        oneEH.eventHandler.Run();

                        Button b = new Button();
                    }

                }


                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new System.EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();


                dispatcherTimer_lisclear = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer_lisclear.Tick += new System.EventHandler(dispatcherTimer_lisclear_Tick);
                dispatcherTimer_lisclear.Interval = new TimeSpan(0, 30, 0);
                dispatcherTimer_lisclear.Start();

                dispatcherTimer_heartbeat = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer_heartbeat.Tick += new System.EventHandler(dispatcherTimer_heartbeat_Tick);
                dispatcherTimer_heartbeat.Interval = new TimeSpan(0, 0, 5);
                dispatcherTimer_heartbeat.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(ex);
            }
        }

        void dispatcherTimer_heartbeat_Tick(object sender, EventArgs e)
        {
            UpdateIsRunningIndicators();
        }

        private void UpdateIsRunningIndicators()
        {
            try {
            foreach (realmEH oneEH in ehs)
            {
                if (!oneEH.eventHandler.isRunning && oneEH.r.ClosingOn > DateTime.Now)
                {
                    oneEH.ti.Header = oneEH.r.Tag + "!";
                }
                else
                {
                    oneEH.ti.Header = oneEH.r.Tag;
                }
            }
            if (tabs.Items.Count > 0)
            {
                ((TabItem)tabs.Items[0]).Header = string.Format("Errors({0})", lvErrors.Items.Count);
            }
            }catch{};
        }
       

        void dispatcherTimer_lisclear_Tick(object sender, EventArgs e)
        {
            if (tabs.Items.Count > 0 ) {

                if (lvErrors.Items.Count > 5000)
                {
                    lvErrors.Items.Clear();
                }
            }
            foreach (realmEH oneEH in ehs)
            {
                if (oneEH.lv.Items.Count > 1000)
                {
                    oneEH.lv.Items.Clear();
                } 
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.IsEnabled = false;
            try
            {


                foreach (realmEH oneEH in ehs)
                {
                    //Trace.Listeners.Remove(oneEH.listener);

                    //DataTable dt = oneEH.msgs.CopyTo( .Copy();

                    //oneEH.dt = new DataTable();
                    //oneEH.listener = new DataTableListener(oneEH.dt);
                    //Trace.Listeners.Add(oneEH.listener);

                    //DataRow[] rows = dt.Select("", "time asc");


                    //ListViewItem lvi;
                    //foreach (DataRow dr in rows)
                    //{
                    //    var data = new
                    //    {
                    //        time = ((DateTime)dr[0]).ToString("HH:mm:ss:ffff")
                    //        ,
                    //        Category = (string)dr[1],
                    //        Message = (string)dr[2]
                    //    };
                    //    lvi = new ListViewItem();
                    //    lvi.Content = ((DateTime)dr[0]).ToString("HH:mm:ss:ffff");
                    //    //lvi.SubItems.Add((string)dr[1]);
                    //    //lvi.SubItems.Add((string)dr[2]);

                    //    oneEH.lv.Items.Insert(0, data);
                    //}

                    //dt = null;

                    
                    foreach (string s in oneEH.msgs)
                    {
                        var data = new
                        {
                            time = DateTime.Now.ToString("dd-MMM HH:mm:ss:ffff")
                            ,                           
                            Message = s
                        };
                       
                        oneEH.lv.Items.Insert(0, data);
                    }
                    oneEH.msgs = new List<string>(10);


                    foreach (string s in oneEH.msgs_errors)
                    {
                        var data = new
                        {
                            time = DateTime.Now.ToString("dd-MMM HH:mm:ss:ffff")
                            ,realm = oneEH.r.ID.ToString()
                            ,Message = s
                        };

                        lvErrors.Items.Insert(0, data);
                    }
                    oneEH.msgs_errors = new List<string>(10);
                    
                }


            }
            catch (Exception eX)
            {
                try
                {
                    Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(eX);
                }
                catch
                {
                    // we do not want this to fail no matter what... 
                }
            }
            dispatcherTimer.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (realmEH oneEH in ehs)
            {
                if (oneEH.eventHandler.isRunning)
                {
                    oneEH.eventHandler.Stop();
                }
            }
        }

        private void StartReInit_Click(object sender, RoutedEventArgs e)
        {
          
            StartOrReInit();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.SelectedIndex != -1 && tabs.SelectedIndex != 0)
            {
                TabItem ti = (TabItem)tabs.Items[tabs.SelectedIndex];

                int realmID = Convert.ToInt32(((string)ti.Header).Replace("!", ""));

                realmEH oneEH = ehs.Find(r => r.r.ID == realmID);

                if (oneEH != null)
                {
                    oneEH.eventHandler.RunStateToggle();                    
                }
            }
        }

        private void clearlist_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.SelectedIndex != -1 && tabs.SelectedIndex != 0)
            {
                TabItem ti = (TabItem)tabs.Items[tabs.SelectedIndex];

                int realmID = Convert.ToInt32(((string)ti.Header).Replace("!", ""));

                realmEH oneEH = ehs.Find(r => r.r.ID == realmID);

                if (oneEH != null)
                {
                    oneEH.lv.Items.Clear();
                }
            }
        }
    }



    class realmEH
    {
        private Fbg.Bll.EventHandler _eventHandler;
        public TabItem ti;
        //public DataTable dt;
        //public DataTableListener listener;
        public ListView lv;
        public Realm r;
        public List<string> msgs;
        public List<string> msgs_errors;

        public realmEH()
        {
            msgs = new List<string>(10);
            msgs_errors = new List<string>(10);
        }

        public Fbg.Bll.EventHandler eventHandler
        {
            get
            {
                return _eventHandler;
            }
            set
            {
                _eventHandler = value;
                _eventHandler.Update += new Bll.EventHandler.UpdateDelegate(Update);
                _eventHandler.UpdateError += new Bll.EventHandler.UpdateDelegate(UpdateError);
            }
        }

        public void Update(Realm r, string message)
        {
            msgs.Add(message);
        }

        public void UpdateError(Realm r, string message)
        {
            msgs.Add(message);
            msgs_errors.Add(message);
        }
    }

    

   
}
