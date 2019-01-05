using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Definitions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Data;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using FusionCommon;
using WebFusionDataLayer;
using FusionPledgeMaintenance.View;
using System.Xml.Linq;
using System.Text;
using PledgeMaintenanceDataLayer;
using System.Data.SqlClient;

namespace FusionPledgeMaintenance.ViewModel
{
    public class PledgeAvailableViewModel : ViewModelBase
    {
        public MainViewModel MainVM { get; set; }       // must be public property for binding

        public PledgeAvailableViewModel(MainViewModel mainvm)
        {
            MainVM = mainvm;
            initializeScreen();
            registerMessages();
        }

        private void initializeScreen()
        {
            AuditInfo = new List<AuditData>();
            // initialize audit date range
            rangeStart = DateTime.Today.AddDays(-1);    // yesterday
            rangeEnd = DateTime.Today;
            PlaceholderDescription = FusionConstants.GenericPlaceholderDesc;    // start with default description
            LoadBothSides();
        }

        private void registerMessages()
        {
            //Messenger.Default.Register<LoadPledgeCodeMessage>(this, LoadActivePledgeCodes);
            //Messenger.Default.Register<LoadPledgesAvailableMessage>(this, LoadPledgesAvailable);
            //Messenger.Default.Register<PlaceholderMessage>(this, placeholderMsgHandler); 
        }

        private void placeholderMsgHandler(bool AddToPledgesAvailable)
        {
            if (AddToPledgesAvailable)
            {
                var ip = new PledgeCodeValues() { Active = true, Order = 0, PledgeCode = FusionConstants.GenericPlaceholder, Description = PlaceholderDescription };
                if (PledgesAvailable != null)
                {
                    // Only add if not already there..
                    if (PledgesAvailable[0].PledgeCode != FusionConstants.GenericPlaceholder)
                    {
                        PledgesAvailable[0].Order = 5;
                        PledgesAvailable.Insert(0, ip);
                    }
                }
                else
                {
                    PledgesAvailable = new ObservableCollection<PledgeCodeValues>();
                    PledgesAvailable.Add(ip);
                }
            }
            else  // Remove from List
            {
                if (PledgesAvailable != null)
                {
                    // find MJSPC in list
                    int found = -1;
                    for (int i=0; i < PledgesAvailable.Count; i++)
                        if (PledgesAvailable[i].PledgeCode == FusionConstants.GenericPlaceholder)
                            found = i;
                    if (found >= 0)
                        PledgesAvailable.RemoveAt(found);
                }
            }

            IsMAndJActive = AddToPledgesAvailable;
        }
        #region Bindable Properties


        private bool _isMAndJActive;
        public bool IsMAndJActive
        {
            get { return _isMAndJActive; }
            set { _isMAndJActive = value; RaisePropertyChanged(nameof(IsMAndJActive)); }
        }

        public bool IsHftwMode
        {
            get { return MainVM.OrigSettings.HftwEnabled; }
        }

        private string _PlaceholderDescription;
        public string PlaceholderDescription
        {
            get { return _PlaceholderDescription; }
            set { _PlaceholderDescription = value; RaisePropertyChanged(nameof(PlaceholderDescription)); }
        }

        public PledgeCodeValues PledgeCodeSelected { get; set; }
        private ObservableCollection<PledgeCodeValues> _pledgeCodesActive;
        public ObservableCollection<PledgeCodeValues> PledgeCodesActive
        {
            get { return _pledgeCodesActive; }
            set { _pledgeCodesActive = value; RaisePropertyChanged("PledgeCodesActive"); }
        }

        private PledgeCodeValues _pledgeAvailableSelected;
        public PledgeCodeValues PledgeAvailableSelected
        {
            get { return _pledgeAvailableSelected; }
            set
            {
                _pledgeAvailableSelected = value;
                //if (_pledgeAvailableSelected != null)
                //    Messenger.Default.Send(new NewPledgeCodeSelectedMessage(_pledgeAvailableSelected.PledgeCode));
            }
        }


        private ObservableCollection<PledgeCodeValues> _pledgesAvailable;
        public ObservableCollection<PledgeCodeValues> PledgesAvailable
        {
            get { return _pledgesAvailable; }
            set
            {
                _pledgesAvailable = value;
                RaisePropertyChanged("PledgesAvailable");
            }
        }

        #endregion


        #region Commands and Handlers
        public ICommand UpdatePledgeAvailCmd { get { return new RelayCommand(UpdatePledgesAvailable); } }
        public ICommand PutPlaceHolderAtTopCmd { get { return new RelayCommand(PutPlaceHolderAtTopHandler); } }
        public ICommand RemovePlaceHolderAtTopCmd { get { return new RelayCommand(RemovePlaceHolderAtTopHandler); } }
        public ICommand btnMoveLeft { get { return new RelayCommand(MoveLeft); } }
        public ICommand btnMoveRight { get { return new RelayCommand(MoveRight); } }
        public ICommand btnMoveUp { get { return new RelayCommand(MoveUp); } }
        public ICommand btnMoveDown { get { return new RelayCommand(MoveDown); } }
        public ICommand btnReloadData { get { return new RelayCommand(LoadBothSides); } }
        public ICommand AuditCmd { get { return new RelayCommand(AuditHandler); } }
        public ICommand RefreshAuditCmd { get { return new RelayCommand(RefreshAuditWindowHandler); } }
        public ICommand CloseAuditCmd { get { return new RelayCommand(CloseAuditHandler); } }


        #region AUDIT
        // binding vars for AuditPanel dates
        private DateTime _rangeStart;
        public DateTime rangeStart
        {
            get { return _rangeStart; }
            set { _rangeStart = value; RaisePropertyChanged("rangeStart"); }
        }

        private DateTime _rangeEnd;
        public DateTime rangeEnd
        {
            get { return _rangeEnd; }
            set { _rangeEnd = value; RaisePropertyChanged("rangeEnd"); }
        }


        private AuditPanel auditWindow;

        private void AuditHandler()
        {
            auditWindow = new AuditPanel() { DataContext = this };
            RefreshAuditWindowHandler();
            auditWindow.ShowDialog();
        }


        private void LoadBothSides()
        {
            LoadActivePledgeCodes();
            LoadPledgesAvailable();
        }

        private void RefreshAuditWindowHandler()
        {
            //AuditInfo = PledgeMaintenanceDataLayer.PledgeMaintenanceAccessStatic.DA.GetAuditInfoFromFusion("", rangeStart, rangeEnd);
            AuditInfo = PledgeMaintenanceDataLayer.PledgeMaintenanceAccessStatic.DA.GetAuditInfo(rangeStart, rangeEnd, PledgeMaintAuditTypes.PledgeAvailable);
        }


        private void CloseAuditHandler()
        {
            auditWindow.Close();
        }

        private List<AuditData> _auditInfo;
        public List<AuditData> AuditInfo
        {
            get { return _auditInfo; }
            set
            {
                _auditInfo = value;
                RaisePropertyChanged("AuditInfo");
            }
        }
        #endregion

        private void MoveLeft()
        {
            if (PledgeAvailableSelected != null)
            {
                if (PledgeAvailableSelected.PledgeCode == FusionConstants.GenericPlaceholder)
                    placeholderMsgHandler(false);               // remove it from the available list
                else
                {
                    for (int j = 0; j < PledgeCodesActive.Count; ++j)
                    {
                        if (PledgeCodesActive[j].PledgeCode.CompareTo(PledgeAvailableSelected.PledgeCode) > 0)
                        {
                            PledgeCodesActive.Insert(j, PledgeAvailableSelected);
                            break;
                        }
                    }
                    PledgesAvailable.Remove(PledgeAvailableSelected);
                }
            }
        }

        private void MoveRight()
        {
            if (PledgeCodeSelected != null)
            {
                PledgesAvailable.Insert(0, PledgeCodeSelected);
                PledgeCodesActive.Remove(PledgeCodeSelected);
            }
        }

        private void MoveUp()
        {
            if (PledgeAvailableSelected != null && PledgesAvailable.IndexOf(PledgeAvailableSelected) > 0)
            {
                int index = PledgesAvailable.IndexOf(PledgeAvailableSelected);
                PledgesAvailable.Insert(index - 1, PledgeAvailableSelected);
                PledgesAvailable.RemoveAt(index + 1);
                Messenger.Default.Send<PledgeAvailableIndexMessage>(new PledgeAvailableIndexMessage(index - 1));
                //var lb = App.Current.Windows[0].FindName("lbPledgesAvailable") as ListBox;
                //if (lb!=null) 
                //    lb.SelectedItem = lb.Items.GetItemAt(index - 1);
            }
        }

        private void MoveDown()
        {
            if (PledgeAvailableSelected != null && PledgesAvailable.IndexOf(PledgeAvailableSelected) < PledgesAvailable.Count - 1)
            {
                int index = PledgesAvailable.IndexOf(PledgeAvailableSelected);
                PledgesAvailable.Insert(index + 2, PledgeAvailableSelected);
                PledgesAvailable.RemoveAt(index);
                Messenger.Default.Send<PledgeAvailableIndexMessage>(new PledgeAvailableIndexMessage(index + 1));
                //var lb = App.Current.Windows[0].FindName("lbPledgesAvailable") as ListBox;
                //if (lb!=null)
                //    lb.SelectedItem = lb.Items.GetItemAt(index + 1);
            }
        }

        private void UpdatePledgesAvailable()
        {
            int minCount = (MainVM.FusionEnvironment == "PROD") ? 1 : 0;  // handle case of PRODAPPS being added automatically in PROD
            if (MainVM.PMTargetsSelected.Count <= minCount)
            {
                MessageBox.Show("You have not selected any target databases", "Warning", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (PledgesAvailable.Count == 0)
            {
                MessageBoxResult mbr =
                    MessageBox.Show("There are ZERO segments in your available list!\r\nClick OK to save anyway.  Click Cancel to discontinue saving.", "IMPORTANT", MessageBoxButton.OKCancel);
                if (mbr != MessageBoxResult.OK)
                    return;
            }

            if (IsMAndJActive && PledgesAvailable[0].PledgeCode!= FusionConstants.GenericPlaceholder)   // MJSPC must be at the top to save
            {
                MessageBox.Show("The temporary pledge code (MJSPC) must be at the top.\r\nPlease move it to the top or deactivate it before saving.", "IMPORTANT", MessageBoxButton.OK);
                return;
            }

            string xml = GetPledgeAvailableXML();           // determine XML once
            StringBuilder sb = new StringBuilder();
            foreach (DBConnection conn in MainVM.PMTargetsSelected) // save in each selected target DB
            {
                if (!conn.IsMySql)        // skip MySql DB for now -- we can't send our xml to MYSQL
                {
                    DataAccess.ConnectionString = conn.ConnStr;
                    bool bOK = DataAccess.SavePledgeMaintXML(xml);
                    sb.AppendFormat("{0} Pledges Available to {1}\r\n\n", bOK ? "SUCCESSFULLY saved" : "ERROR saving", conn.ConnInfo2);
                }
            }

            // =========== Update WEB second ============
            if (MainVM.WEB_DB.IsSelected)
            {
                try
                {
                    WebFusionAccessStatic.DA.TrunkPledgesAvailable();
                    foreach (var pledge in PledgesAvailable)
                    {
                        pledge.Order = PledgesAvailable.IndexOf(pledge) * 10;
                        WebFusionAccessStatic.DA.InsertPledgeAvailable(pledge);
                    }
                    sb.AppendFormat("SUCCESSFULLY saved Pledges Available to web: {0}\r\n", WebFusionAccessStatic.DA.DisplayConnection());

                    // update desription for the MJSPC record
                    WebFusionAccessStatic.DA.UpdatePledgeMasterWeb("MJSPC","11111111",PlaceholderDescription,PlaceholderDescription,false);
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("ERROR saving Pledges Available to web: {0}\r\n{1}\r\n", WebFusionAccessStatic.DA.DisplayConnection(), ex.Message);
                    return;
                }
            }

            MessageBox.Show(sb.ToString(), "Save results", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        //===============================================================================
        public string GetPledgeAvailableXML()
        {
            string currentUserName = Environment.UserName.ToUpper();

            XElement availXml = new XElement("PledgesAvailable",
                new XElement("UserName", currentUserName),
                new XElement("XmlDateTime", DateTime.Now.ToString()));

            // =========== Loop through PledgeAppeals ============
            if (PledgesAvailable != null)
            {
                int order = 10;
                foreach (var pledge in PledgesAvailable)
                {
                    availXml.Add(new XElement("PledgeAvailable",
                        new XElement("PledgeOrder", order),
                        new XElement("PledgeCode", pledge.PledgeCode)));
                    order += 10;
                }
            }

            XElement PMRoot = new XElement("IMGroot", availXml);

            if (IsMAndJActive)
            {
                XElement tempPledgeCodeXml = new XElement("TempPledgeCode",     // for saving the MJSPC description
                    new XElement("UserName", currentUserName),
                    new XElement("XmlDateTime", DateTime.Now.ToString()),
                    new XElement("Description", PlaceholderDescription));
                Globals.InsertXml(PMRoot, tempPledgeCodeXml, "IMGroot");        // insert it into root xml
            }

            return PMRoot.ToString(SaveOptions.DisableFormatting);
        }

        #endregion


        #region Helper Methods
        // @@@@@@ This loads the pledgecode list from the PledgeMaster table, for the left side of the screen @@@@@@@@@@@@@@@@@@@@@
        //   loads from PledgeMaintenance db for the environment
        public void LoadActivePledgeCodes()        // (LoadPledgeCodeMessage msg)
        {
            PledgeMaintenanceAccessStatic.DA.Initialize(MainVM.FusionEnvironment);
            string sqlCmd = "SELECT [PledgeCode],[Description] FROM [PledgeMaster]   WHERE PledgeCode not in (SELECT PledgeCode FROM PledgeAvailable) AND PledgeCode<>'MJSPC'  ORDER BY [PledgeCode]";
            DataSet ds = PledgeMaintenanceAccessStatic.DA.RetrieveSqlData(sqlCmd);
            if (ds != null)
            {
                List<PledgeCodeValues> lst = new List<PledgeCodeValues>(from r in ds.Tables[0].AsEnumerable()
                                                                        select new PledgeCodeValues()
                                                                        {
                                                                            PledgeCode = r.Field<string>("PledgeCode"),
                                                                            Description = r.Field<string>("Description")
                                                                        });
                if (lst != null)
                    PledgeCodesActive = new ObservableCollection<PledgeCodeValues>(lst);
            }
        }


        // This loads the list of pledgecodes for the rigth side
        private void LoadPledgesAvailable()      //   (LoadPledgesAvailableMessage msg)
        {
            int order = 0;
            List<PledgesAvailableInfo> plTemp = PledgeMaintenanceAccessStatic.DA.GetActivePledgeSegments(false);
            if (plTemp != null)
            {
                List<PledgeCodeValues> ls1 = new List<PledgeCodeValues>();
                plTemp.ForEach(rec => ls1.Add(new PledgeCodeValues() { Order = order++, PledgeCode = rec.PledgeCode, Description = rec.Description }));
                PledgesAvailable = new ObservableCollection<PledgeCodeValues>(ls1);
                IsMAndJActive = (PledgesAvailable != null && PledgesAvailable.Count > 0 && PledgesAvailable[0].PledgeCode == Definitions.FusionConstants.GenericPlaceholder);
            }
            else
                PledgesAvailable = new ObservableCollection<PledgeCodeValues>();
        }

        public void PutPlaceHolderAtTopHandler()
        {
            // check that there are no MJSPC records in the DB already
            int tempCount = 0;
            try
            {
                //Globals.ConnStr = MainVM.FUSION_DB.ConnStr;
                SqlConnection sConn = Globals.GetSqlConn(MainVM.FUSION_REPL_DB.ConnStr);
                string sqlCmd = string.Format("SELECT count(PledgeCode) FROM Pledge where [PledgeCode] = '{0}'", FusionConstants.GenericPlaceholder);
                tempCount = Globals.RetrieveIntFromDB(sConn, sqlCmd);
            }
            catch { }       // do nothing

            MessageBoxResult mbr = MessageBoxResult.None;
            if (tempCount > 0)
                mbr = MessageBox.Show($"There are already {tempCount} MJSPC pledges waiting to be converted!\r\nClick OK to activate MJSPC anyway.  Click Cancel to leave it inactive.", "WARNING - EXISTING MJSPC PLEDGES", MessageBoxButton.OKCancel);
            if (tempCount <= 0 || mbr == MessageBoxResult.OK)
                placeholderMsgHandler(true);
        }

        
        public void RemovePlaceHolderAtTopHandler()
        {
            placeholderMsgHandler(false);
        }


        private void CleanUI()
        {
            PledgeMaintenanceDataLayer.PledgeMaintenanceAccessStatic.DA.Initialize(MainVM.FusionEnvironment);
        }

        #endregion
    }

}

