using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Symbol.XamarinEMDK; 
using Symbol.XamarinEMDK.Barcode;
using Android.Util;

using System.Xml;
using System.IO;

using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace GettingStartedTutorial
{
    [Activity(Label = "Expoyer", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity, EMDKManager.IEMDKListener
    {
        private EMDKManager emdkManager = null;
        private ProfileManager profileManager = null;
        private String profileName = "ClockProfile";
        private TextView tvStatus = null;

        private TextView statusView = null;
        private TextView dataView = null;
         
        private BarcodeManager barcodeManager = null;
        private Scanner scanner = null;

        #region Ciclo de vida
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            tvStatus = FindViewById<TextView>(Resource.Id.tbEstadoPerfil);

            // Get our button from the layout resource,
            // and attach an event to it
            ImageButton button = FindViewById<ImageButton>(Resource.Id.MyButton);

            button.Click += delegate { ApplyProfile(); };

            statusView = FindViewById<TextView>(Resource.Id.statusViewTxt);
            dataView   = FindViewById<TextView>(Resource.Id.DataViewTxt);

            EMDKResults results = EMDKManager.GetEMDKManager(Android.App.Application.Context, this);
            if (results.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                tvStatus.Text = "Status: EMDKManager object creation failed ...";
            }
            else
            {
                tvStatus.Text = "Status: EMDKManager object creation succeeded ...";
            }

      
        }

        protected override void OnResume()
        {
            base.OnResume();
            InitScanner();
        }

        protected override void OnPause()
        {
            base.OnPause();
            DeinitScanner();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (profileManager != null)
            {
                profileManager = null;
            }

            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        #endregion

        void EMDKManager.IEMDKListener.OnOpened(EMDKManager emdkManager)
        {
            tvStatus.Text = "Estado: EMDK funcionando correctamente ...";

            this.emdkManager = emdkManager;
            InitScanner();
            try
            {
                profileManager = (ProfileManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Profile);
            }
            catch (Exception e)
            {
                tvStatus.Text = "Status: Exception <" + e.Message + ">";
            }
        }
        void EMDKManager.IEMDKListener.OnClosed()
        {
            tvStatus.Text = "Status: EMDK Open failed unexpectedly. Please close and restart the application ...";

            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        #region Metodos
        void displayStatus(String status)
        {

            if (Looper.MainLooper.Thread == Java.Lang.Thread.CurrentThread())
            {
                statusView.Text = status;
            }
            else
            {
                RunOnUiThread(() => statusView.Text = status);
            }
        }
         
        void displaydata(string data)
        {

            if (Looper.MainLooper.Thread == Java.Lang.Thread.CurrentThread())
            {
                dataView.Text += (data + "\n");
            }
            else
            {
                RunOnUiThread(() => dataView.Text += data + "\n");
            }
        }

        void ApplyProfile()
        {
            if (profileManager != null)
            {
                EMDKResults results = profileManager.ProcessProfile(profileName, ProfileManager.PROFILE_FLAG.Set, new String[] { "" });
                if (results.StatusCode == EMDKResults.STATUS_CODE.Success)
                {
                    tvStatus.Text = "Status: Perfil aplicado satisfactoriamente ...";
                }
                else if (results.StatusCode == EMDKResults.STATUS_CODE.CheckXml)
                {
                    //Inspect the XML response to see if there are any errors, if not report success

                    using (XmlReader reader = XmlReader.Create(new StringReader(results.StatusString)))
                    {
                        String checkXmlStatus = "Status:\n\n";
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    switch (reader.Name)
                                    {
                                        case "parm-error":
                                            checkXmlStatus += "Parm Error:\n";
                                            checkXmlStatus += reader.GetAttribute("name") + " - ";
                                            checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                            break;
                                        case "characteristic-error":
                                            checkXmlStatus += "characteristic Error:\n";
                                            checkXmlStatus += reader.GetAttribute("type") + " - ";
                                            checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                            break;
                                    }
                                    break;
                            }
                        }

                        if (checkXmlStatus == "Status:\n\n")
                        {
                            tvStatus.Text = "Status: Profile applied successfully ...";
                        }
                        else
                        {
                            tvStatus.Text = checkXmlStatus;
                        }

                    }
                }
                else
                {
                    tvStatus.Text = "Status: Profile initialization failed ... " + results.StatusCode;
                }
            }
            else
            {
                tvStatus.Text = "Status: profileManager is null ...";
            }
        }
        void InitScanner()
        {
            if (emdkManager != null)
            {

                if (barcodeManager == null)
                {
                    try
                    {

                        //Get the feature object such as BarcodeManager object for accessing the feature.
                        barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                        scanner = barcodeManager.GetDevice(BarcodeManager.DeviceIdentifier.Default);

                        if (scanner != null)
                        {

                            //Attahch the Data Event handler to get the data callbacks.
                            scanner.Data += scanner_Data;

                            //Attach Scanner Status Event to get the status callbacks.
                            scanner.Status += scanner_Status;

                            scanner.Enable();

                            //EMDK: Configure the scanner settings
                            ScannerConfig config = scanner.GetConfig();
                            config.SkipOnUnsupported = ScannerConfig.SkipOnUnSupported.None;
                            config.ScanParams.DecodeLEDFeedback = true;
                            config.ReaderParams.ReaderSpecific.ImagerSpecific.PickList = ScannerConfig.PickList.Enabled;
                            config.DecoderParams.Code39.Enabled = true;
                            config.DecoderParams.Code128.Enabled = false;
                            scanner.SetConfig(config);

                        }
                        else
                        {
                            displayStatus("Failed to enable scanner.\n");
                        }
                    }
                    catch (ScannerException e)
                    {
                        displayStatus("Error: " + e.Message);
                    }
                    catch (Exception ex)
                    {
                        displayStatus("Error: " + ex.Message);
                    }
                }
            }
        }

        void DeinitScanner()
        {
            if (emdkManager != null)
            {

                if (scanner != null)
                {
                    try
                    {

                        scanner.Data -= scanner_Data;
                        scanner.Status -= scanner_Status;

                        scanner.Disable();


                    }
                    catch (ScannerException e)
                    {
                        Log.Debug(this.Class.SimpleName, "Exception:" + e.Result.Description);
                    }
                }

                if (barcodeManager != null)
                {
                    emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode);
                }
                barcodeManager = null;
                scanner = null;
            }
        }

        #endregion

        #region Eventos del scanner
        void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    EnviarPostLocal(0, data.LabelType.ToString(), data.Data);
                    displaydata(data.LabelType + " : " + data.Data);
                }
            }
        }

        void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            String statusStr = "";

            //EMDK: The status will be returned on multiple cases. Check the state and take the action.
            StatusData.ScannerStates state = e.P0.State;

            if (state == StatusData.ScannerStates.Idle)
            {
                statusStr = "El escáner está inactivo y listo para leer.";
                try
                {
                    if (scanner.IsEnabled && !scanner.IsReadPending)
                    {
                        scanner.Read();
                    }
                }
                catch (ScannerException e1)
                {
                    statusStr = e1.Message;
                }
            }
            if (state == StatusData.ScannerStates.Waiting)
            {
                statusStr = "Aprete el gatillo para escanear";
               
            }
            if (state == StatusData.ScannerStates.Scanning)
            {
                statusStr = "Escaneando en progreso...";
            }
            if (state == StatusData.ScannerStates.Disabled)
            {
                statusStr = "Scanner disabled";
            }
            if (state == StatusData.ScannerStates.Error)
            {
                statusStr = "Error: occurrio un error durante el escaneo";

            }
            displayStatus(statusStr);
        }

        #endregion


        private async void EnviarPostLocal(int id, string codigo, string tipo)
        {
            using (var client = new HttpClient())
            {
                // Create a new post  
                var novoPost = new Producto
                {
                    Codigo = codigo,
                    Tipo = tipo
                };

                // create the request content and define Json  
                var json = JsonConvert.SerializeObject(novoPost);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //  send a POST request  
                var uri = "https://webappexpoyer.azurewebsites.net/productos";
                var result = await client.PostAsync(uri, content);

                // on error throw a exception  
                result.EnsureSuccessStatusCode();
                /* Esto esta comentado por si recibe un resultado
                // handling the answer  
                var resultString = await result.Content.ReadAsStringAsync();
                var post = JsonConvert.DeserializeObject<Post>(resultString);

                // display the output in TextView  
                resultado.Text = post.ToString();
                */
            }
        }




    }
}

