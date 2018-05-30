using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using TSF.UmlToolingFramework.Wrappers.EA;
using EARQ = EAAddinFramework.Requirements;

namespace EAAddinFramework.Requirements.DoorsNG
{
    public class DoorsNGProject : EARQ.Project
    {
        private DoorsNGSettings settings { get; set; }

        public DoorsNGProject(Package packageToWrap, string projectURL, DoorsNGSettings settings) : base(packageToWrap)
        {
            this.settings = settings;
            this.url = projectURL;
        }
        private HttpClient client;
        private void setHttpClient()
        {
            this.client = new HttpClient();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/rdf+xml"));
            client.DefaultRequestHeaders.Add("OSLC-Core-Version", "2.0");
            //workaround for SSL/TSL exception
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        }
        public static Project getCurrentProject(Element element, string projectURL, DoorsNGSettings settings)
        {
            var currentProjectPackage = Project.getCurrentProjectPackage(element);
            if (currentProjectPackage != null) return new DoorsNGProject(currentProjectPackage, projectURL, settings);
            //no project package found
            return null;
        }
        public string url { get; set; }

        internal HttpClient Authenticate()
        {
            if (this.client == null) setHttpClient();
            //get username and password
            var loginForm = new UserLoginForm(this.settings.defaultUserName);
            if (loginForm.ShowDialog(this.wrappedPackage.EAModel.mainEAWindow) == System.Windows.Forms.DialogResult.OK)
            {
                //call protected part
                using (var testResponse = client.GetAsync("https://localhost:9443/jts/users/Geert").Result)
                {
                    var testContent =  testResponse.Content.ReadAsStringAsync().Result;
                }

                //login form
                using (var formData = new MultipartFormDataContent())
                {
                    // Populate the form variables
                    var formVariables = new List<KeyValuePair<string, string>>();
                    formVariables.Add(new KeyValuePair<string, string>("j_username", loginForm.user));
                    formVariables.Add(new KeyValuePair<string, string>("j_password", loginForm.password));
                    var formContent = new FormUrlEncodedContent(formVariables);
                    using (var response = client.PostAsync(this.url + "/jts/j_security_check", formContent).Result)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                    }
                }
                    
            }
            return this.client;
        }
    }
}
