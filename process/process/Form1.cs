using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace process
{
    public partial class frmFormProc : Form
    {
        AutoResetEvent EventRefresh;
        public frmFormProc()
        {
            InitializeComponent();
            EventRefresh = new AutoResetEvent(true);
            Thread ProcThread = new Thread(ProcessListThread);
            ProcThread.IsBackground = true;
            ProcThread.Start(EventRefresh);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateProcessList();

        }

        void UpdateProcessList()
        {
            EventRefresh.Set();
        }

        void ProcessListThread(object param)
        {
            //для не пересечения с индексом другого потока
            Thread.Sleep(100);
            AutoResetEvent eventRefresh = (AutoResetEvent)param;

            Invoke((Action)delegate
            {
                lvProcList.Columns.Clear();
                lvProcList.Columns.Add("Process Name");
                lvProcList.Columns.Add("Id");
                lvProcList.Columns.Add("Count Threads");
                lvProcList.Columns.Add("Count Descrypt");
                // lvProcList.Columns.Add("Module Name");
                timerRefresh.Stop();
            });
            while (true)
            {
                eventRefresh.WaitOne();
                Process[] proc = Process.GetProcesses();
                Invoke((Action)delegate
                {
                    lvProcList.Items.Clear();
                    //чтоб не дергался list view
                    lvProcList.BeginUpdate();
                    foreach (Process p in proc)
                    {
                        //фоормирование строки для list view
                        //Добавление корневого элемента
                        ListViewItem lvl = new ListViewItem(p.ProcessName);

                        //Добавление следующих элементво в колонки
                        lvl.SubItems.Add(p.Id.ToString());
                        lvl.SubItems.Add(p.Threads.Count.ToString());
                        lvl.SubItems.Add(p.HandleCount.ToString());
                        //lvl.SubItems.Add(p.Modules[0].FileName);


                        lvProcList.Items.Add(lvl);

                    }
                    lvProcList.EndUpdate();
                });
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            UpdateProcessList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
