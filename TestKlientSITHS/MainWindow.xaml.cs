using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestKlientSITHS
{
    public enum ActionType
    {
        Undefined,
        OpenStartPage,
        OpenPatient,
        OpenForm,
        ShowFormOnly
    }

    public class Action
    {
        public ActionType ActionType { get; set; }
    }
    
    public partial class MainWindow : Window
    {
        private Process _process;

        private readonly RegisterData _registerData = new RegisterData();

        private string _patientIdentifier;

        public MainWindow()
        {
            InitializeComponent();
            UpdateValues();
        }

        private void StartPage_Button_Click(object sender, RoutedEventArgs e)
        {
            DoAction(new Action() {ActionType = ActionType.OpenStartPage});
        }

        private void Patient_Button_Click(object sender, RoutedEventArgs e)
        {
            DoAction(new Action() { ActionType = ActionType.OpenPatient });
        }

        private void Form_Button_Click(object sender, RoutedEventArgs e)
        {
            DoAction(new Action() { ActionType = ActionType.OpenForm });
        }

        private void Form_Only_Button_Click(object sender, RoutedEventArgs e)
        {
            DoAction(new Action() { ActionType = ActionType.ShowFormOnly });
        }

        private void DoAction(Action action)
        {
            try
            {
                //var foundCert = GetUserCertificate();
                var url = GetUrl(action.ActionType);
                var requestData = GetRequestData(action);
                var postData = JsonConvert.SerializeObject(requestData);
                var data = Encoding.UTF8.GetBytes(postData);
                var request = GetRequest(url, data, null);
                var response = (HttpWebResponse) request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var result = JsonConvert.DeserializeObject<RedirectResultModel>(responseString);

                OpenChrome(result.Url, data);
            }
            catch (WebException e)
            {
                var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(resp);
                var errorMessage = obj.error.message;
                MessageBox.Show(errorMessage.Value);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private HttpWebRequest GetRequest(string url, byte[] data, X509Certificate2 foundCert)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.ContentLength = data.Length;
            //request.ClientCertificates.Add(foundCert);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return request;
        }

        private RedirectModel GetRequestData(Action action)
        {
            var user = ConfigurationManager.AppSettings["Username"];
            var organization = ConfigurationManager.AppSettings["OrganizationName"];
            var position = ConfigurationManager.AppSettings["PositionShortName"];
            var role = ConfigurationManager.AppSettings["RoleShortName"];
            var register = ConfigurationManager.AppSettings["RegisterShortName"];
            var formRegister = ConfigurationManager.AppSettings["FormRegisterShortName"];
            var form = ConfigurationManager.AppSettings["FormShortName"];

            RedirectModel requestData;
            switch (action.ActionType)
            {
                case ActionType.OpenStartPage:
                    requestData = new RedirectModel()
                    {
                        UserName = user,
                        OrganizationShortName = organization,
                        PositionShortName = position,
                        RoleShortName = role,
                        RegisterShortName = register
                    };
                    break;
                case ActionType.OpenPatient:
                    requestData = new PatientRedirectModel()
                    {
                        UserName = user,
                        OrganizationShortName = organization,
                        PositionShortName = position,
                        RoleShortName = role,
                        RegisterShortName = register,
                        PatientIdentifier = _patientIdentifier
                    };
                    break;
                case ActionType.OpenForm:
                case ActionType.ShowFormOnly:
                    requestData = new FormRedirectModel()
                    {
                        UserName = user,
                        OrganizationShortName = organization,
                        PositionShortName = position,
                        RoleShortName = role,
                        RegisterShortName = register,
                        PatientIdentifier = _patientIdentifier,
                        FormRegisterShortName = formRegister,
                        FormShortName = form,
                        FormData = JsonConvert.SerializeObject(_registerData),
                        ShowFormOnly = action.ActionType == ActionType.ShowFormOnly
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            return requestData;
        }

        private string GetUrl(ActionType actionType)
        {
            var url = ConfigurationManager.AppSettings["BaseUrl"];
            if (actionType == ActionType.OpenStartPage)
            {
                url += ConfigurationManager.AppSettings["StartPageUrl"];
            }
            if (actionType == ActionType.OpenPatient)
            {
                url += ConfigurationManager.AppSettings["PatientUrl"];
            }
            else if (actionType == ActionType.OpenForm || actionType == ActionType.ShowFormOnly)
            {
                url += ConfigurationManager.AppSettings["FormUrl"];
            }
            return url;
        }

        private X509Certificate2 GetUserCertificate()
        {
            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2 foundCert = null;

            // Find the certificate we'll use to sign            
            RSACryptoServiceProvider cryptoServiceProvider = null;
            foreach (X509Certificate2 cert in my.Certificates)
            {
                if (cert.Thumbprint.Equals("d8fba59c3aec6e3a6628292c18cf4ff82fc50c7c",
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    // We found it. 
                    // Get its associated CSP and private key
                    if (cert.HasPrivateKey)
                    {
                        cryptoServiceProvider = (RSACryptoServiceProvider) cert.PrivateKey;
                        if (cryptoServiceProvider.CspKeyContainerInfo.HardwareDevice)
                        {
                            Console.WriteLine("hardware");
                        }

                        Console.WriteLine(cert.ToString());

                        foundCert = cert;
                    }
                }
            }

            if (foundCert == null)
            {
                throw new Exception("Certificate not found");
            }

            foundCert.Verify();

            return foundCert;
        }

        private void OpenChrome(string responseString, byte[] data)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C start \"\" \"{responseString}\"";
            process.StartInfo = startInfo;
            process.Start();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateValues();
        }

        private void CheckBox_Checked_Kryssruta1(object sender, RoutedEventArgs e)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            _registerData.Text10 = TextboxText10?.Text;
            _registerData.Heltal1 = TextboxHeltal1?.Text;
            _registerData.Decimal1 = TextboxDecimal1?.Text;
            _registerData.Datum1 = TextboxDatum1?.Text;
            _registerData.Kryssruta1 = CheckboxKryssruta1?.IsChecked.ToString();

            _patientIdentifier = TextboxPatientIdentifier?.Text;
        }
    }

    internal class RegisterData
    {
        public string Text10 { get; set; }
        public string Heltal1 { get; set; }
        public string Decimal1 { get; set; }
        public string Datum1 { get; set; }
        public string Kryssruta1 { get; set; }
    }
}
