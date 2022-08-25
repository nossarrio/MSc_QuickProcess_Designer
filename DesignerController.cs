using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


//Model.saveRecord_Model;
//Model.fetchRecord_Model;
//Model.api_Model;
//Model.deleteRecord_Model;
//Model.searchRecord_Model;
//preview specific component
//add component link to masterpage menu
//wizard

namespace QuickProcessDesigner.Controllers
{
    public class DesignerController : Controller
    {
        private ErroLogger ErroLogger;
        ILogger<DesignerController> _logger;

        public DesignerController(ILogger<DesignerController> logger)
        {
            _logger = logger;
            ErroLogger = new ErroLogger(logger);
        }


        [Route("designer/{**catchAll}")]
        [HttpGet]
        [Produces("text/html")]
        public async Task<IActionResult> Designer(string filename)
        {
            try
            {
                var app = getDevApp();

                //no appsettings.json file found
                if (app == null)
                {
                    return new ContentResult
                    {
                        ContentType = "text/html",
                        Content = await getSetupHtml()
                    };
                }

                //dev directory not found
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch (FileNotFoundException ex)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = await getSetupHtml()
                };
            }

            if (Request.Path.Value.ToString().ToLower().Contains("/designer/editor.html") == false)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = await getDesignerHtml()
                };
            }
            else
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = await getEditorHtml()
                };
            }

            return null;
        }

        private async Task<string> getSetupHtml()
        {
            using (HttpClient client = new HttpClient())
            {
                List<Tuple<string, string>> filesToDownload = new List<Tuple<string, string>>();
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/designer/setup.html", ""));

                foreach (var fileToDownload in filesToDownload)
                {
                    using (HttpResponseMessage response = await client.GetAsync(fileToDownload.Item1))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        byte[] content = new byte[streamToReadFrom.Length];
                        MemoryStream ms = new MemoryStream();
                        streamToReadFrom.CopyTo(ms);
                        content = ms.ToArray();
                        return System.Text.Encoding.UTF8.GetString(content);
                    }
                }
            }

            return "";
        }

        private async Task<string> getDesignerHtml()
        {
            using (HttpClient client = new HttpClient())
            {
                List<Tuple<string, string>> filesToDownload = new List<Tuple<string, string>>();
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/designer/index.html", ""));

                foreach (var fileToDownload in filesToDownload)
                {
                    using (HttpResponseMessage response = await client.GetAsync(fileToDownload.Item1))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        byte[] content = new byte[streamToReadFrom.Length];
                        MemoryStream ms = new MemoryStream();
                        streamToReadFrom.CopyTo(ms);
                        content = ms.ToArray();
                        return System.Text.Encoding.UTF8.GetString(content);
                    }
                }
            }

            return "";
        }

        private async Task<string> getEditorHtml()
        {
            using (HttpClient client = new HttpClient())
            {
                List<Tuple<string, string>> filesToDownload = new List<Tuple<string, string>>();
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/designer/editor.html", ""));

                foreach (var fileToDownload in filesToDownload)
                {
                    using (HttpResponseMessage response = await client.GetAsync(fileToDownload.Item1))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        byte[] content = new byte[streamToReadFrom.Length];
                        MemoryStream ms = new MemoryStream();
                        streamToReadFrom.CopyTo(ms);
                        content = ms.ToArray();
                        return System.Text.Encoding.UTF8.GetString(content);
                    }
                }
            }

            return "";
        }


        [Route("setupDirectories")]
        [HttpPost]
        public async Task<IActionResult> setupDirectories(string appName, string appTitle, string devPath, string appUrl, string defaultConnection, bool CreateFolders, bool ChangePassword, bool PassordRecovery, bool SignUp, bool UserAndRole, bool LoginAndSession, bool CreateModel, string Color, string Template, string defaultUserName, string defaultPassword)
        {
            var filesToDownload = new List<Tuple<string, string>>();
            var SetupDonload = new SetupDonload();
            string outputPath = OutputDirectory();

            try
            {
                var _app = getDevApp();
                if (Directory.Exists(_app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }


            if (appUrl.ToLower().EndsWith("designer/index.html"))
            {
                appUrl = appUrl.Substring(0, appUrl.Length - 19);
            }
            else
            {
                if (appUrl.ToLower().EndsWith("/"))
                {
                    appUrl = appUrl.Substring(0, appUrl.Length - 9);
                }
                else
                {
                    appUrl = appUrl.Substring(0, appUrl.Length - 8);
                }
            }


            //download files            
            using (HttpClient client = new HttpClient())
            {
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/form.html", "form.html"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/list.html", "list.html"));

                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/authentication_api.json", "authentication_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/authorisation_api.json", "authorisation_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/changedefaultpassword_api.json", "changedefaultpassword_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/changepassword_api.json", "changepassword_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/getCurrentUser_api.json", "getCurrentUser_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/forgotpassword_api.json", "forgotpassword_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/logout_api.json", "logout_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/resetpassword_api.json", "resetpassword_api.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/setup/components/session_api.json", "session_api.json"));

                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/userrole_dropdownlist.json", "userrole_dropdownlist.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/changepassword.json", "changepassword.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/loginpage.json", "loginpage.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/masterpage.json", "masterpage.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/user_form.json", "user_form.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/user_list.json", "user_list.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/userrole_form.json", "userrole_form.json"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/templates/" + Template + "/components/userrole_list.json", "userrole_list.json"));

                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/loginhistory_script.txt", "loginhistory_script.sql"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/usertable_script.txt", "usertable_script.sql"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/userrole_script.txt", "userrole_script.sql"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/roleresources_script.txt", "roleresources_script.sql"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/passwordreset_script.txt", "passwordreset_script.sql"));
                filesToDownload.Add(new Tuple<string, string>("https://quickprocess.azurewebsites.net/sql/saveuserrole_script.txt", "saveuserrole_script.sql"));


                foreach (var fileToDownload in filesToDownload)
                {
                    using (HttpResponseMessage response = await client.GetAsync(fileToDownload.Item1))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        byte[] content = new byte[streamToReadFrom.Length];
                        MemoryStream ms = new MemoryStream();
                        streamToReadFrom.CopyTo(ms);
                        content = ms.ToArray();
                        string file_content = System.Text.Encoding.UTF8.GetString(content);


                        if (fileToDownload.Item2.ToString() == "loginhistory_script.sql")
                            SetupDonload.loginhistory_script = file_content;

                        if (fileToDownload.Item2.ToString() == "usertable_script.sql")
                            SetupDonload.usertable_script = file_content;

                        if (fileToDownload.Item2.ToString() == "userrole_script.sql")
                            SetupDonload.userrole_script = file_content;

                        if (fileToDownload.Item2.ToString() == "roleresources_script.sql")
                            SetupDonload.roleresources_script = file_content;

                        if (fileToDownload.Item2.ToString() == "passwordreset_script.sql")
                            SetupDonload.passwordreset_script = file_content;

                        if (fileToDownload.Item2.ToString() == "saveuserrole_script.sql")
                            SetupDonload.saveuserrole_script = file_content;



                        if (fileToDownload.Item2.ToString() == "form.html")
                            SetupDonload.form_wrapper = file_content;

                        if (fileToDownload.Item2.ToString() == "list.html")
                            SetupDonload.list_wrapper = file_content;


                        if (fileToDownload.Item2.ToString() == "changepassword.json")
                            SetupDonload.changepassword = file_content;

                        if (fileToDownload.Item2.ToString() == "loginpage.json")
                            SetupDonload.loginpage = file_content.Replace("@apptitle", appTitle).Replace("@appTitle", appTitle);

                        if (fileToDownload.Item2.ToString() == "masterpage.json")
                            SetupDonload.master_component = file_content.Replace("@apptitle", appTitle).Replace("@appTitle", appTitle);

                        if (fileToDownload.Item2.ToString() == "user_form.json")
                            SetupDonload.userformpage = file_content;

                        if (fileToDownload.Item2.ToString() == "user_list.json")
                            SetupDonload.userlistpage = file_content;

                        if (fileToDownload.Item2.ToString() == "userrole_form.json")
                            SetupDonload.roleformpage = file_content;

                        if (fileToDownload.Item2.ToString() == "userrole_list.json")
                            SetupDonload.rolelistpage = file_content;


                        if (fileToDownload.Item2.ToString() == "userrole_dropdownlist.json")
                            SetupDonload.userrole_dropdownlist = file_content;

                        if (fileToDownload.Item2.ToString() == "authentication_api.json")
                            SetupDonload.authentication_api = file_content;

                        if (fileToDownload.Item2.ToString() == "authorisation_api.json")
                            SetupDonload.authorisation_api = file_content;

                        if (fileToDownload.Item2.ToString() == "changedefaultpassword_api.json")
                            SetupDonload.changedefaultpassword_api = file_content;

                        if (fileToDownload.Item2.ToString() == "changepassword_api.json")
                            SetupDonload.changepassword_api = file_content;

                        if (fileToDownload.Item2.ToString() == "forgotpassword_api.json")
                            SetupDonload.forgotpassword_api = file_content;

                        if (fileToDownload.Item2.ToString() == "getCurrentUser_api.json")
                            SetupDonload.currentuser_api = file_content;

                        if (fileToDownload.Item2.ToString() == "logout_api.json")
                            SetupDonload.logout_api = file_content;

                        if (fileToDownload.Item2.ToString() == "resetpassword_api.json")
                            SetupDonload.resetpassword_api = file_content;

                        if (fileToDownload.Item2.ToString() == "session_api.json")
                            SetupDonload.session_api = file_content;

                    }
                }
            }


            if (!Directory.Exists(devPath))
                return Content(JsonConvert.SerializeObject(new GenericResponse() { ResponseCode = "00", ResponseDescription = "Invalid root directory path" }));


            if (!Directory.Exists(outputPath))
                return Content(JsonConvert.SerializeObject(new GenericResponse() { ResponseCode = "00", ResponseDescription = "Invalid output directory path" }));


            string _devPath = devPath;
            string _outputPath = outputPath;
            devPath += "\\QuickProcess";
            outputPath += "\\QuickProcess";

            //create required direcoties
            Directory.CreateDirectory(devPath);
            Directory.CreateDirectory(outputPath);

            Directory.CreateDirectory(devPath + "\\Properties");
            Directory.CreateDirectory(devPath + "\\Components");
            Directory.CreateDirectory(outputPath + "\\Properties");
            Directory.CreateDirectory(outputPath + "\\Components");

            if (CreateFolders)
            {
                Directory.CreateDirectory(devPath + "\\Components\\Dropdown Components");
                Directory.CreateDirectory(devPath + "\\Components\\Form Components");
                Directory.CreateDirectory(devPath + "\\Components\\HTML Components");
                Directory.CreateDirectory(devPath + "\\Components\\Layout Components");
                Directory.CreateDirectory(devPath + "\\Components\\List Components");
                Directory.CreateDirectory(devPath + "\\Components\\Query API Components");
                Directory.CreateDirectory(devPath + "\\Components\\Web API Components");

                Directory.CreateDirectory(outputPath + "\\Components\\Dropdown Components");
                Directory.CreateDirectory(outputPath + "\\Components\\Form Components");
                Directory.CreateDirectory(outputPath + "\\Components\\HTML Components");
                Directory.CreateDirectory(outputPath + "\\Components\\Layout Components");
                Directory.CreateDirectory(outputPath + "\\Components\\List Components");
                Directory.CreateDirectory(outputPath + "\\Components\\Query API Components");
                Directory.CreateDirectory(outputPath + "\\Components\\Web API Components");
            }

            //create appsettings file
            Application app = new Application();
            app.ApplicationTitle = appTitle;
            app.Dev_Path = _devPath;
            app.Output_Path = _outputPath;
            app.ApplicationName = appName;
            app.Connections = new List<ConnectionList>() { new ConnectionList() { ConnectionName = "defaultConnection", ConnectionString = defaultConnection, isDefaultConnection = true, Engine = "ms sqlserver", GUID = "defaultConnection" } };
            app.ComponentList = new List<ComponentList>();
            app.AuthorisationDetails = new AuthorisationDetails() { AuthorisationComponent = "authorisation_api", Resources = new string[] { "User & Role Management", "Profile Management" } };
            app.Status = "ACTIVE";
            app.ThemeColor = (string.IsNullOrEmpty(Color)) ? "#4d90d7" : Color;
            app.FontColor = "#ffffff";
            app.Debug = true;
            app.GUID = new Guid().ToString();
            app.Url = appUrl;
            app.QuickProcessCss_Url = "https://quickprocess.azurewebsites.net/css/QuickProcess.css";
            app.QuickProcessJs_Url = "https://quickprocess.azurewebsites.net/script/QuickProcess.js";
            app.QuickProcessLoadergif_Url = "https://quickprocess.azurewebsites.net/images/qpLoader.gif";
            app.Default_FormWrapperMarkup = SetupDonload.form_wrapper;
            app.Default_ListWrapperMarkup = SetupDonload.list_wrapper;
            app.ComponentList.Add(new ComponentList() { Name = "masterpage", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Layout Components\\" : "") });
            app.DefaultConnection = "defaultConnection";

            if (LoginAndSession)
            {
                app.DefaultComponent = "loginpage";
                app.SessionValidationComponent = "session_api";

                app.EnableAuthorisation = true;
                app.EnableAuthentication = true;


                app.ComponentList.Add(new ComponentList() { Name = "user_list", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "List Components\\" : ""), ResourceGroups = new List<string>() { "User & Role Management" } });
                app.ComponentList.Add(new ComponentList() { Name = "user_form", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Form Components\\" : ""), ResourceGroups = new List<string>() { "User & Role Management" } });
                app.ComponentList.Add(new ComponentList() { Name = "userrole_list", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "List Components\\" : ""), ResourceGroups = new List<string>() { "User & Role Management" } });
                app.ComponentList.Add(new ComponentList() { Name = "userrole_form", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Form Components\\" : ""), ResourceGroups = new List<string>() { "User & Role Management" } });

                app.ComponentList.Add(new ComponentList() { Name = "authentication_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "authorisation_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "logout_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "changePassword_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : ""), ResourceGroups = new List<string>() { "Profile Management" } });
                app.ComponentList.Add(new ComponentList() { Name = "session_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "getCurrentUser_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "resetpassword_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                // app.ComponentList.Add(new ComponentList() { Name = "signup_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "forgotpassword_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });
                // app.ComponentList.Add(new ComponentList() { Name = "changedefaultpassword_api", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Query API Components\\" : "") });

                app.ComponentList.Add(new ComponentList() { Name = "userrole_dropdownlist", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "Dropdown Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "loginpage", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "HTML Components\\" : "") });
                app.ComponentList.Add(new ComponentList() { Name = "changePassword", PreFetchComponent = true, FolderPath = ((CreateFolders == true) ? "HTML Components\\" : ""), ResourceGroups = new List<string>() { "Profile Management" } });
            }

            //save appsettings file
            System.IO.File.WriteAllText(devPath + "\\Properties\\application.json", JsonConvert.SerializeObject(app));
            System.IO.File.WriteAllText(outputPath + "\\Properties\\application.json", JsonConvert.SerializeObject(app));


            //save constant files
            string saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Layout Components\\masterpage.json" : "masterpage.json");
            System.IO.File.WriteAllText(saveFilePath, SetupDonload.master_component);
            saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Layout Components\\masterpage.json" : "masterpage.json");
            System.IO.File.WriteAllText(saveFilePath, SetupDonload.master_component);


            saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\getCurrentUser_api.json" : "getCurrentUser_api.json");
            System.IO.File.WriteAllText(saveFilePath, SetupDonload.currentuser_api);
            saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\getCurrentUser_api.json" : "getCurrentUser_api.json");
            System.IO.File.WriteAllText(saveFilePath, SetupDonload.currentuser_api);


            if (LoginAndSession)
            {
                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\loginpage.json" : "loginpage.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.loginpage);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\loginpage.json" : "loginpage.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.loginpage);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\authentication_api.json" : "authentication_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.authentication_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\authentication_api.json" : "authentication_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.authentication_api);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\authorisation_api.json" : "authorisation_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.authorisation_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\authorisation_api.json" : "authorisation_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.authorisation_api);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\logout_api.json" : "logout_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.logout_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\logout_api.json" : "logout_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.logout_api);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\session_api.json" : "session_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.session_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\session_api.json" : "session_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.session_api);

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.loginhistory_script.ToString().Substring(1, SetupDonload.loginhistory_script.Length - 1), null);
                }
                catch (Exception ex) { }
            }

            if (PassordRecovery)
            {
                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\forgotpassword_api.json" : "forgotpassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.forgotpassword_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\forgotpassword_api.json" : "forgotpassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.forgotpassword_api);


                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\resetpassword_api.json" : "resetpassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.resetpassword_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\resetpassword_api.json" : "resetpassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.resetpassword_api);

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.passwordreset_script.Substring(1, SetupDonload.passwordreset_script.Length - 1), null);
                }
                catch (Exception ex) { }
            }

            if (SignUp)
            {
                //saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\signup_api.json" : "signup_api.json");
                //System.IO.File.WriteAllText(saveFilePath, SetupDonload.signup_api);
                //saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\signup_api.json" : "signup_api.json");
                //System.IO.File.WriteAllText(saveFilePath, SetupDonload.signup_api);

                //saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\signuppage.json" : "signuppage.json");
                //System.IO.File.WriteAllText(saveFilePath, SetupDonload.signuppage);
                //saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\signuppage.json" : "signuppage.json");
                //System.IO.File.WriteAllText(saveFilePath, SetupDonload.signuppage);
            }

            if (ChangePassword)
            {
                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\changepassword.json" : "changepassword.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.changepassword);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "HTML Components\\changepassword.json" : "changepassword.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.changepassword);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\changepassword_api.json" : "changepassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.changepassword_api);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Query API Components\\changepassword_api.json" : "changepassword_api.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.changepassword_api);
            }

            if (UserAndRole)
            {
                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "List Components\\user_list.json" : "user_list.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userlistpage);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "List Components\\user_list.json" : "user_list.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userlistpage);


                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Form Components\\user_form.json" : "user_form.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userformpage);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Form Components\\user_form.json" : "user_form.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userformpage);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "List Components\\userrole_list.json" : "userrole_list.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.rolelistpage);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "List Components\\userrole_list.json" : "userrole_list.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.rolelistpage);

                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Form Components\\userrole_form.json" : "userrole_form.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.roleformpage);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Form Components\\userrole_form.json" : "userrole_form.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.roleformpage);


                saveFilePath = devPath + "\\Components\\" + ((CreateFolders == true) ? "Dropdown Components\\userrole_dropdownlist.json" : "userrole_dropdownlist.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userrole_dropdownlist);
                saveFilePath = outputPath + "\\Components\\" + ((CreateFolders == true) ? "Dropdown Components\\userrole_dropdownlist.json" : "userrole_dropdownlist.json");
                System.IO.File.WriteAllText(saveFilePath, SetupDonload.userrole_dropdownlist);

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.usertable_script.Substring(1, SetupDonload.usertable_script.Length - 1), null);
                }
                catch (Exception ex) { }

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.userrole_script.Substring(1, SetupDonload.userrole_script.Length - 1), null);
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.roleresources_script.Substring(1, SetupDonload.roleresources_script.Length - 1), null);
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.saveuserrole_script.Substring(1, SetupDonload.saveuserrole_script.Length - 1), null);
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, "insert into UserRole values('Super-Admin',1) ", null);
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, "insert into UserRole_Resources values(1,'User & Role Management') ", null);
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, "insert into Users values(@UserName,@Password,'John','Doe',1,0,1,1) ",
                       new List<SqlParameter>()
                       {
                        new SqlParameter()
                        {
                            ParameterName= "@UserName", Value=defaultUserName
                        },
                        new SqlParameter()
                        {
                             ParameterName = "@Password",Value = QuickProcess.Utility.Security.HASH256(defaultPassword)
                        }
                       });
                }
                catch { }

                try
                {
                    await Database.ExecQuery(defaultConnection, SetupDonload.saveuserrole_script, null);

                }
                catch { }
            }

            if (CreateModel)
            {
                try
                {
                    await CreateTableModels("", true);
                }
                catch { }
            }


            return Content(JsonConvert.SerializeObject(new GenericResponse() { ResponseCode = "00", ResponseDescription = "Okay" }));


            //using (var fileStream = System.IO.File.Create(file1))
            //{
            //  streamToReadFrom.Seek(0, SeekOrigin.Begin);
            //  streamToReadFrom.CopyTo(fileStream);
            //}
        }


        [Route("createDropdownList")]
        [HttpPost]
        public ActionResult createDropdownList(string selectedPath, string tables, string ConnectionName, string ResourceGroups, bool SessionBased)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            try
            {
                var tableList = tables.Split(new char[] { ',' });
                foreach (var table in tableList)
                {
                    if (!string.IsNullOrEmpty(table))
                    {
                        var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == ConnectionName.ToLower()).ToList();
                        var table_structure = Database.ExecQuery(conn[0].ConnectionString, "select * from   " + table, null).Result;
                        var saveComponentDefinition_Model = new saveComponentDefinition_Model();
                        var component = new Component();
                        component.Type = ComponentType.DropDownList.ToLower();
                        component.Name = table;
                        component.DataSourceType = "table";
                        component.DataSourceTable = table;
                        component.Connection = new ComponentConnection { GUID = ConnectionName };
                        component.ConnectionTimeout = 30;
                        component.FolderPath = selectedPath;
                        component.FetchMethod = "db";
                        component.DisplayField = table_structure.Columns[1].ColumnName;
                        component.ValueField = table_structure.Columns[0].ColumnName;
                        if (string.IsNullOrEmpty(ResourceGroups) == false)
                            component.ResourceGroups = ResourceGroups.Split(new char[] { ',' }).ToList();
                        component.SessionBased = SessionBased;
                        component.loadDataMode = "automatic";

                        saveComponentDefinition_Model.Component = JsonConvert.SerializeObject(component);
                        saveComponentDefinition(saveComponentDefinition_Model);
                    }
                }
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("createQueryApiList")]
        [HttpPost]
        public ActionResult createQueryApiList(string selectedPath, string tables, string ConnectionName, string ResourceGroups, bool SessionBased)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            try
            {
                var tableList = tables.Split(new char[] { ',' });
                foreach (var table in tableList)
                {
                    if (!string.IsNullOrEmpty(table))
                    {
                        var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == ConnectionName.ToLower()).ToList();
                        var table_structure = Database.ExecQuery(conn[0].ConnectionString, "select * from @table  ", new List<SqlParameter> { new SqlParameter("@table", table) }).Result;
                        var saveComponentDefinition_Model = new saveComponentDefinition_Model();
                        var component = new Component();
                        component.Type = ComponentType.Query.ToLower();
                        component.Name = table;
                        component.Query = "select * from " + table;
                        component.DataSourceTable = table;
                        component.Connection = new ComponentConnection { GUID = ConnectionName };
                        component.ConnectionTimeout = 30;
                        component.FolderPath = selectedPath;
                        component.FetchMethod = "db";
                        if (string.IsNullOrEmpty(ResourceGroups) == false)
                            component.ResourceGroups = ResourceGroups.Split(new char[] { ',' }).ToList();
                        component.SessionBased = SessionBased;
                        component.loadDataMode = "automatic";

                        saveComponentDefinition_Model.Component = JsonConvert.SerializeObject(component);
                        saveComponentDefinition(saveComponentDefinition_Model);
                    }
                }
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("createWebApiList")]
        [HttpPost]
        public ActionResult createWebApiList(string selectedPath, string tables, string ConnectionName, string ResourceGroups, bool SessionBased, string controllerName, string operation)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            try
            {
                string ControllerCode = "";
                var tableList = tables.Split(new char[] { ',' });
                foreach (var table in tableList)
                {
                    if (!string.IsNullOrEmpty(table))
                    {
                        var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == ConnectionName.ToLower()).ToList();
                        var table_structure = Database.ExecQuery(conn[0].ConnectionString, "select * from @table  ", new List<SqlParameter> { new SqlParameter("@table", table) }).Result;
                        var saveComponentDefinition_Model = new saveComponentDefinition_Model();
                        var component = new Component();
                        component.Type = ComponentType.Query.ToLower();
                        component.Name = table;
                        component.ApiUrl = controllerName + "/" + table;
                        component.DataSourceTable = table;
                        component.Connection = new ComponentConnection();
                        component.ConnectionTimeout = 30;
                        component.FolderPath = selectedPath;
                        if (string.IsNullOrEmpty(ResourceGroups) == false)
                            component.ResourceGroups = ResourceGroups.Split(new char[] { ',' }).ToList();
                        component.SessionBased = SessionBased;
                        component.loadDataMode = "automatic";

                        saveComponentDefinition_Model.Component = JsonConvert.SerializeObject(component);
                        saveComponentDefinition(saveComponentDefinition_Model);
                    }

                    ControllerCode += "";
                }
                //create controller physical file

                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("createTableList")]
        [HttpPost]
        public ActionResult createTableList(string selectedPath, string tables, string ConnectionName, string ResourceGroups, bool SessionBased, string operation, bool createForms, bool modalForms, string layoutcomponent)
        {
            var saveComponentDefinition_Model = new saveComponentDefinition_Model();
            Application app = null;
            try
            {
                app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            if (string.IsNullOrEmpty(operation))
                operation = "";

            try
            {
                string ControllerCode = "";
                var tableList = tables.Split(new char[] { ',' });
                foreach (var table in tableList)
                {
                    if (!string.IsNullOrEmpty(table))
                    {
                        var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == ConnectionName.ToLower()).ToList();
                        var table_structure = Database.ExecQuery(conn[0].ConnectionString, "select top 1 * from " + table, null).Result;
                        var component = new Component();
                        component.DataSourceTable = table;
                        component.DataSourceType = "table";
                        component.Type = ComponentType.TableList.ToLower();
                        component.Name = table + "_List";
                        component.Title = table.Replace("_", " ");
                        component.ShowHoverEffect = true;
                        component.EnableSearch = true;
                        component.AutoSearch = true;
                        component.EnablePagination = true;
                        component.PaginationPosition = "bottom";
                        component.PreFetchComponent = true;
                        component.ShowBorder = true;
                        component.ShowRecordCount = true;
                        component.ShowRecordMenu = true;
                        component.ShowRefresh = true;
                        component.EnableSorting = true;
                        component.AutoLoad = true;
                        component.ModalProgress = true;
                        component.DefaultPageSize = 10;
                        component.DataSourceTable = table;
                        component.loadDataMode = "Automatic";
                        component.LayoutPage = layoutcomponent;


                        if (operation.ToLower().Contains("insert"))
                            component.EnableInsert = true;

                        if (operation.ToLower().Contains("update"))
                            component.EnableEdit = true;

                        if (operation.ToLower().Contains("delete"))
                            component.EnableDelete = true;

                        if (operation.ToLower().Contains("view"))
                            component.EnableView = true;



                        component.WrapperMarkup = app.Default_ListWrapperMarkup.Replace("@PageTitle", table);

                        component.Columns = new Columns[table_structure.Columns.Count];

                        int index = 0;
                        foreach (DataColumn col in table_structure.Columns)
                        {
                            Columns columnDefinition = new Columns()
                            {
                                ColumnName = col.ColumnName,
                                HeaderText = col.ColumnName.Replace("_", " "),
                                DataTypeFormat = "string",
                                Visible = true,
                                IsBound = true,
                                Sort = "none"
                            };
                            component.Columns[index] = columnDefinition;
                            index += 1;
                        }


                        String Query = " SELECT  name FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = @TABLE_NAME ";
                        var colList = Database.ExecQuery(conn[0].ConnectionString, Query, new List<SqlParameter> { new SqlParameter("@TABLE_NAME", table) });
                        if (colList.Result.Rows.Count > 0)
                        {
                            component.UniqueColumn = new UniqueColumn()
                            {
                                ColumnName = colList.Result.Rows[0][0].ToString(),
                                isIdentity = true
                            };
                        }

                        if (operation.ToLower().Contains("view") || operation.ToLower().Contains("insert") || operation.ToLower().Contains("update"))
                        {
                            component.FormComponent = table + "_Form";
                            component.ModalForm = true;
                            component.onRecordClick = "EditRecord";
                            component.FormUniqueColumn = component.UniqueColumn.ColumnName;
                        }
                        else
                            component.onRecordClick = "";

                        component.Connection = new ComponentConnection() { GUID = ConnectionName };
                        component.ConnectionTimeout = 30;
                        component.FolderPath = selectedPath;
                        if (string.IsNullOrEmpty(ResourceGroups) == false)
                            component.ResourceGroups = ResourceGroups.Split(new char[] { ',' }).ToList();
                        component.SessionBased = SessionBased;
                        component.loadDataMode = "automatic";

                        saveComponentDefinition_Model.Component = JsonConvert.SerializeObject(component);
                        saveComponentDefinition(saveComponentDefinition_Model);
                    }
                }

                if (createForms)
                    createFormList(selectedPath, tables, ConnectionName, ResourceGroups, SessionBased, operation, layoutcomponent);

                //saveComponentDefinition(saveComponentDefinition_Model);
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("createFormList")]
        [HttpPost]
        public ActionResult createFormList(string selectedPath, string tables, string ConnectionName, string ResourceGroups, bool SessionBased, string operation, string layoutcomponent)
        {
            Application app = null;
            try
            {
                app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            if (string.IsNullOrEmpty(operation))
                operation = "";

            try
            {
                string ControllerCode = "";
                var tableList = tables.Split(new char[] { ',' });
                foreach (var table in tableList)
                {
                    if (!string.IsNullOrEmpty(table))
                    {
                        var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == ConnectionName.ToLower()).ToList();
                        var table_structure = Database.ExecQuerySchema(conn[0].ConnectionString, "select top 1 * from " + table, null).Result;
                        var saveComponentDefinition_Model = new saveComponentDefinition_Model();
                        var component = new Component();
                        component.DataSourceTable = table;
                        component.DataSourceType = "table";
                        component.Type = ComponentType.Form.ToLower();
                        component.Name = table + "_Form";
                        component.Title = table.Replace("_", " ");
                        component.PreFetchComponent = true;
                        component.AutoLoad = true;
                        component.ModalProgress = true;
                        component.DataSourceTable = table;
                        component.loadDataMode = "Automatic";
                        component.LayoutPage = layoutcomponent;
                        component.FetchMethod = "db";
                        component.PostMethod = "db";


                        if (operation.ToLower().Contains("insert"))
                            component.EnableInsert = true;

                        if (operation.ToLower().Contains("update"))
                            component.EnableEdit = true;


                        component.WrapperMarkup = app.Default_FormWrapperMarkup.Replace("@PageTitle", table);

                        component.Controls = new FormControls[table_structure.Columns.Count];
                        component.QueryStringFilter = "";


                        String Query = " SELECT  name FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = @TABLE_NAME ";
                        var colList = Database.ExecQuery(conn[0].ConnectionString, Query, new List<SqlParameter> { new SqlParameter("@TABLE_NAME", table) });
                        if (colList.Result.Rows.Count > 0)
                        {
                            component.UniqueColumn = new UniqueColumn()
                            {
                                ColumnName = colList.Result.Rows[0][0].ToString(),
                                isIdentity = true
                            };
                        }


                        int index = 0;
                        string code = "";
                        foreach (DataColumn col in table_structure.Columns)
                        {
                            var ControlType = "";
                            string Compulsory = "";

                            if (col.AllowDBNull == false)
                                Compulsory = " compulsory=\"true\" ";

                            if (col.ColumnName.ToLower() != component.UniqueColumn.ColumnName.ToLower())
                            {
                                switch (col.DataType.Name.ToLower())
                                {
                                    case "int":
                                    case "int32":
                                    case "int64":
                                    case "decimal":
                                    case "float":
                                    case "double":
                                        code += " <div class=\"form-group col-md-6\">\r\n";
                                        code += "  <label>" + col.ColumnName.Replace("_", " ") + "</label> \r\n";
                                        code += "  <input id=\"" + col.ColumnName + "\"  type=\"number\" class=\"form-control\"  bindfield=\"" + col.ColumnName + "\" " + Compulsory + "  />\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "number";
                                        break;

                                    case "string":
                                    case "guid":
                                        code += " <div class=\"form-group col-md-6\">\r\n";
                                        code += "  <label>" + col.ColumnName.Replace("_", " ") + "</label> \r\n";
                                        code += "  <input id=\"" + col.ColumnName + "\"  type=\"text\" class=\"form-control\"  bindfield=\"" + col.ColumnName + "\" " + Compulsory + "   />\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "text";
                                        break;

                                    case "boolean":
                                        code += "<div class=\"form-group col-md-6\">\r\n";
                                        code += "   <div class=\"custom-control custom-switch\">\r\n";
                                        code += "       <input  type=\"checkbox\" class=\"custom-control-input\" id=\"" + col.ColumnName + "\"   bindfield=\"" + col.ColumnName + "\"  " + Compulsory + "   />\r\n";
                                        code += "       <label  class=\"custom-control-label\" for=\"" + col.ColumnName + "\"> \r\n" + col.ColumnName.Replace("_", " ");
                                        code += "   </div>\r\n\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "checkbox";
                                        break;

                                    case "datetime":
                                        code += " <div class=\"form-group col-md-6\">\r\n";
                                        code += "  <label>" + col.ColumnName.Replace("_", " ") + "</label> \r\n";
                                        code += "  <input id=\"" + col.ColumnName + "\"  type=\"date\" class=\"form-control\"  bindfield=\"" + col.ColumnName + "\"   " + Compulsory + "  />\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "date";
                                        break;

                                    case "timestamp":
                                    case "timespan":
                                        code += " <div class=\"form-group col-md-6\">\r\n";
                                        code += "  <label>" + col.ColumnName.Replace("_", " ") + "</label> \r\n";
                                        code += "  <input id=\"" + col.ColumnName + "\"  type=\"time\" class=\"form-control\"  bindfield=\"" + col.ColumnName + "\"   " + Compulsory + "  />\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "time";
                                        break;

                                    default:
                                        code += " <div class=\"form-group col-md-6\">\r\n";
                                        code += "  <label>" + col.ColumnName.Replace("_", " ") + "</label> \r\n";
                                        code += "  <input id=\"" + col.ColumnName + "\"  type=\"text\" class=\"form-control\"  bindfield=\"" + col.ColumnName + "\"  " + Compulsory + "   />\r\n";
                                        code += "</div>\r\n\r\n";
                                        ControlType = "text";
                                        break;
                                }
                            }
                            else
                            {
                                code += "  <input id=\"" + col.ColumnName + "\"  type=\"hidden\"  bindfield=\"" + col.ColumnName + "\"  />\r\n";
                                ControlType = "hidden";
                            }

                            FormControls FormControl = new FormControls()
                            {
                                FieldName = col.ColumnName,
                                Title = col.ColumnName.Replace("_", " "),
                                ControlType = ControlType,
                                Compulsory = !col.AllowDBNull
                            };
                            component.Controls[index] = FormControl;
                            index += 1;
                        }

                        component.Connection = new ComponentConnection() { GUID = ConnectionName };
                        component.ConnectionTimeout = 30;
                        component.FolderPath = selectedPath;

                        if (string.IsNullOrEmpty(ResourceGroups) == false)
                            component.ResourceGroups = ResourceGroups.Split(new char[] { ',' }).ToList();

                        component.SessionBased = SessionBased;
                        component.loadDataMode = "automatic";

                        component.WrapperMarkup = component.WrapperMarkup.Replace("@RenderComponent", code);
                        component.Markup = component.WrapperMarkup;
                        saveComponentDefinition_Model.Component = JsonConvert.SerializeObject(component);
                        saveComponentDefinition(saveComponentDefinition_Model);
                    }
                }


                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("getConnectionList")]
        [HttpPost]
        public ActionResult getConnectionList(string GUID)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                return Content(JsonConvert.SerializeObject(getDevApp().Connections));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Connection List" }));
            }
        }


        [Route("getComponentList")]
        [HttpPost]
        public ActionResult getComponentList(string AppGUID)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var app = getDevApp();
                string path = app.Dev_Path + "\\QuickProcess\\Components";
                if (System.IO.Directory.Exists(path))
                {
                    // var folder = new System.IO.DirectoryInfo(path);
                    var Components = "[";
                    //foreach (System.IO.FileInfo file in folder.GetFiles())
                    //{
                    //    try
                    //    {
                    //        var cpm = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + "\\" + file.Name));
                    //        Components += "{\"AppGUID\":\"" + AppGUID + "\",\"ComponentName\":\"" + cpm.Name + "\", \"GUID\":\"" + file.Name + "\", \"Type\":\"" + cpm.Type + "\", \"IsLayoutComponent\":\"" + cpm.IsLayoutComponent + "\"},";
                    //    }
                    //    catch (Exception ex1)
                    //    {
                    //        ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex1.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex1.StackTrace, "", "GetApplicationComponent");
                    //    }
                    //}

                    foreach (var cmpinfo in app.ComponentList)
                    {
                        try
                        {
                            var cpm = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + "\\" + cmpinfo.FolderPath + "\\" + cmpinfo.Name + ".json"));
                            Components += "{\"AppGUID\":\"" + AppGUID + "\",\"ComponentName\":\"" + cpm.Name + "\", \"GUID\":\"" + cmpinfo.Name + "\", \"Type\":\"" + cpm.Type + "\", \"IsLayoutComponent\":\"" + cpm.IsLayoutComponent + "\"},";
                        }
                        catch { }
                    }

                    Components = Components.Substring(0, Components.Length - 1) + "]";
                    return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", Result = Components }));
                }

                return Content("");
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Component List" }));
            }
        }


        [Route("getComponentFolderStructure")]
        [HttpPost]
        public ActionResult getComponentFolderStructure(string AppGUID)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var app = getDevApp();
                string path = app.Dev_Path + "\\QuickProcess\\Components";
                if (System.IO.Directory.Exists(path))
                {
                    List<ComponentList> ComponentList = new List<ComponentList>();
                    DirectoryInfo info = new DirectoryInfo(path);
                    var tree = new DynatreeItem(info, path, ComponentList);

                    try
                    {
                        var DesignerApp = getDevApp();
                        DesignerApp.ComponentList = ComponentList;
                        System.IO.File.WriteAllText(DesignerApp.Dev_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(DesignerApp));
                        System.IO.File.WriteAllText(DesignerApp.Output_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(DesignerApp));
                        QuickProcess.QuickProcess_Core.refreshFramework();
                    }
                    catch { }

                    string structure = tree.JsonToDynatree();
                    return Content(structure);
                }

                return Content("");
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Component List" }));
            }
        }


        [Route("createFolder")]
        [HttpPost]
        public ActionResult createFolder(string selectedPath, string folderName)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            try
            {
                string devpath = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath + "\\" + folderName;
                string outputpath = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath + "\\" + folderName;
                Directory.CreateDirectory(devpath);
                Directory.CreateDirectory(outputpath);
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to create folder: " + ex.Message }));
            }
        }


        [Route("renameFolder")]
        [HttpPost]
        public ActionResult renameFolder(string selectedPath, string folderName)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Can not rename root folder 'Components'" }));


            try
            {
                int pathLength = selectedPath.Split(new char[] { '/' }).Length;
                string oldFolder = selectedPath.Split(new char[] { '/' })[pathLength - 1];
                string oldDevPath = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath.Substring(0, ((selectedPath.Length - oldFolder.Length) - 1)).Replace("/", "\\");
                string oldOutputPath = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath.Substring(0, ((selectedPath.Length - oldFolder.Length) - 1)).Replace("/", "\\");

                string oldpath1 = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                string oldpath2 = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");

                string newpath1 = oldDevPath.Replace("/", "\\") + "\\" + folderName;
                string newpath2 = oldOutputPath.Replace("/", "\\") + "\\" + folderName;

                if (Directory.Exists(oldpath1))
                    Directory.Move(oldpath1, newpath1);


                if (Directory.Exists(oldpath2))
                    Directory.Move(oldpath2, newpath2);

                getComponentFolderStructure("");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to create folder: " + ex.Message }));
            }
        }


        [Route("moveComponent")]
        [HttpPost]
        public ActionResult moveComponent(string sourcePath, string destinationPath)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (sourcePath == "undefined" || sourcePath == "null")
                sourcePath = "";

            if (destinationPath == "undefined" || destinationPath == "null")
                destinationPath = "";

            try
            {
                string fileName = sourcePath.Split(new char[] { '/' })[sourcePath.Split(new char[] { '/' }).Length - 1].ToString();

                string source_devpath = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + sourcePath.Replace("/", "\\");
                string source_outputpath = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + sourcePath.Replace("/", "\\");

                string destination_devpath = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + destinationPath.Replace("/", "\\") + "\\" + fileName;
                string destination_outputpath = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + destinationPath.Replace("/", "\\") + "\\" + fileName;

                if (System.IO.File.Exists(source_devpath))
                {
                    System.IO.File.Move(source_devpath, destination_devpath);
                }

                if (System.IO.File.Exists(source_outputpath))
                {
                    System.IO.File.Move(source_outputpath, destination_outputpath);
                }

                getComponentFolderStructure("");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to create folder: " + ex.Message }));
            }
        }


        [Route("deleteFolder")]
        [HttpPost]
        public ActionResult deleteFolder(string selectedPath)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }


            if (selectedPath == "undefined" || selectedPath == "null")
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Can not delete root folder 'Components'" }));


            try
            {
                string devpath = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                string outputpath = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                Directory.Delete(devpath, true);
                Directory.Delete(outputpath, true);
                getComponentFolderStructure("");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete folder: " + ex.Message }));
            }
        }


        [Route("deleteComponent")]
        [HttpPost]
        public ActionResult deleteComponent(string selectedPath)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                string path = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                path = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                getComponentFolderStructure("");

                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to delete file: " + ex.Message }));
            }
        }


        [Route("cloneComponent")]
        [HttpPost]
        public ActionResult cloneComponent(string selectedPath, string componentName)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            if (selectedPath == "undefined" || selectedPath == "null")
                selectedPath = "";

            try
            {
                string path = getDevApp().Dev_Path + "\\QuickProcess\\Components\\" + selectedPath.Replace("/", "\\");
                int fileNameLength = selectedPath.Split(new char[] { '/' })[selectedPath.Split(new char[] { '/' }).Length - 1].ToString().Length;
                string output_path = getDevApp().Output_Path + "\\QuickProcess\\Components\\" + selectedPath.Substring(0, (selectedPath.Length - (fileNameLength + 1))).Replace("/", "\\");
                if (System.IO.File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    var component = JsonConvert.DeserializeObject<Component>(System.IO.File.ReadAllText(path));
                    component.Name = componentName;
                    component.FileName = componentName + ".json";

                    System.IO.File.WriteAllText(info.DirectoryName + "\\" + componentName + ".json", JsonConvert.SerializeObject(component));
                    System.IO.File.WriteAllText(output_path + "\\" + componentName + ".json", JsonConvert.SerializeObject(component));

                    getComponentFolderStructure("");
                }
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00" }));
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to clone Component file: " + ex.Message }));
            }
        }


        [Route("getLayoutComponentList")]
        [HttpPost]
        public ActionResult getLayoutComponentList(string AppGUID)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            string Layout = "";

            try
            {
                var app = getDevApp();
                string path = app.Dev_Path + "\\QuickProcess\\Components";
                if (System.IO.Directory.Exists(path))
                {
                    // var folder = new System.IO.DirectoryInfo(path);

                    //foreach (System.IO.FileInfo file in folder.GetFiles())
                    //{
                    //    var cpm = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + "\\" + file.Name));
                    //    if (cpm.IsLayoutComponent)
                    //        Layout += "{\"ComponentName\":\"" + cpm.Name.Split(new char[] { '.' })[0].ToString() + "\"},";
                    //}
                    foreach (var cmpinfo in app.ComponentList)
                    {
                        try
                        {
                            var cpm = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + "\\" + cmpinfo.FolderPath + "\\" + cmpinfo.Name + ".json"));
                            if (cpm.IsLayoutComponent)
                                Layout += "{\"ComponentName\":\"" + cpm.Name.Split(new char[] { '.' })[0].ToString() + "\"},";
                        }
                        catch { }
                    }
                    if (Layout != "")
                        Layout = "[" + Layout.Substring(0, Layout.Length - 1) + "]";

                    return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", Result = Layout }));

                }

                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "", Result = Layout }));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Layout List" }));
            }
        }


        [Route("getAppDefinition")]
        [HttpPost]
        public ActionResult getAppDefinition(string AppGUID)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var app = getDevApp();
                var appInfo = new AppDefinition();
                appInfo.ApplicationTitle = app.ApplicationTitle;
                appInfo.GUID = app.GUID;
                appInfo.Url = "";
                appInfo.Default_FormWrapperMarkup = app.Default_FormWrapperMarkup;
                appInfo.Default_ListWrapperMarkup = app.Default_ListWrapperMarkup;
                appInfo.ResourceGroup = app.AuthorisationDetails.Resources;

                return Content(JsonConvert.SerializeObject(app));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get App Info" }));
            }
        }


        [Route("getComponentDefinition")]
        [HttpPost]
        public ActionResult getComponentDefinition(getComponentDefinition_Model request)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                request.path = request.path.Replace("/", "\\");
                string path = getDevApp().Dev_Path + "\\QuickProcess\\Components\\";
                if (System.IO.File.Exists(path + request.path))
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path + request.path);
                    var cmp = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + request.path));
                    cmp.FileName = fileInfo.Name;
                    try
                    {
                        cmp.FolderPath = request.path.Substring(0, ((request.path.Length - fileInfo.Name.Length) - 1));
                    }
                    catch { }

                    if (cmp.Type.ToLower() == "table" || cmp.Type.ToLower() == "card" || cmp.Type.ToLower() == "form" || cmp.Type.ToLower() == "dropdown")
                    {
                        if (string.IsNullOrEmpty(cmp.FetchMethod))
                            cmp.FetchMethod = "db";

                        if (string.IsNullOrEmpty(cmp.PostMethod))
                            cmp.PostMethod = "db";
                    }

                    return Content(JsonConvert.SerializeObject(cmp));


                    //var folder = new System.IO.DirectoryInfo(path);
                    //foreach (System.IO.FileInfo file in folder.GetFiles())
                    //{
                    //    var cmp = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(path + "\\" + file.Name));
                    //    if (cmp.Name.ToLower() == request.ComponentName.ToLower())
                    //    {
                    //        cmp.FileName = file.Name;
                    //        return Content(JsonConvert.SerializeObject(cmp));
                    //    }
                    //}
                }

                return Content("");
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Component List" + ex.Message + JsonConvert.SerializeObject(request) })); ;
            }
        }


        [Route("saveComponentDefinition")]
        [HttpPost]
        public ActionResult saveComponentDefinition(saveComponentDefinition_Model request)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var app = getDevApp();
                var requestComponent = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(request.Component);
                string path = app.Dev_Path + "\\QuickProcess\\Components\\";
                string output_Path = app.Output_Path + "\\QuickProcess\\Components\\";

                //createFormList("Form Components\\", "LoginHistory", "defaultConnection", "", false, "insert,update", "masterpage");

                if (System.IO.Directory.Exists(path))
                {
                    //generate sqlquery parameters
                    if (!string.IsNullOrEmpty(requestComponent.Query))
                    {
                        string queryParams = "";
                        ArrayList paramList = new ArrayList();
                        string pattern = @"(?<!\w)@\w+";
                        Regex rg = new Regex(pattern);
                        MatchCollection matches = rg.Matches(requestComponent.Query);

                        for (int count = 0; count < matches.Count; count++)
                        {
                            if (string.IsNullOrEmpty(matches[count].Value) == false && matches[count].Value.ToLower() != "@const_sessionid" && matches[count].Value.ToLower() != "@const_username")
                            {
                                if (paramList.Contains(matches[count].Value.Replace("@", "").ToLower()) == false)
                                {
                                    queryParams += matches[count].Value.Replace("@", "") + ",";
                                    paramList.Add(matches[count].Value.Replace("@", "").ToLower());
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(queryParams))
                        {
                            queryParams = queryParams.Substring(0, queryParams.Length - 1);
                            requestComponent.Designer_QueryParams = queryParams;
                        }
                    }


                    //get unique id of form component
                    if (string.IsNullOrEmpty(requestComponent.FormComponent) == false)
                    {
                        try
                        {
                            string form_comp_path = app.ComponentList.Where(cmp => cmp.Name.ToLower() == requestComponent.FormComponent.ToLower()).First().FolderPath;
                            string full_form_component_path = app.Dev_Path + "\\QuickProcess\\Components" + form_comp_path + "\\" + requestComponent.FormComponent + ".json";
                            var cmp_obj = JsonConvert.DeserializeObject<Component>(System.IO.File.ReadAllText(full_form_component_path));
                            requestComponent.FormUniqueColumn = cmp_obj.UniqueColumn.ColumnName;
                        }
                        catch { }
                    }


                    //check if new insert or update
                    if (string.IsNullOrWhiteSpace(requestComponent.FileName) == false)
                    {
                        string filename = requestComponent.FileName;
                        requestComponent.FileName = requestComponent.Name + ".json";

                        //update existing component
                        System.IO.File.WriteAllText(path + requestComponent.FolderPath + "\\" + filename, JsonConvert.SerializeObject(requestComponent));
                        System.IO.File.WriteAllText(output_Path + requestComponent.FolderPath + "\\" + filename, JsonConvert.SerializeObject(requestComponent));

                        //rename file if necessary
                        if (filename.ToLower() != requestComponent.FileName.ToLower())
                        {
                            System.IO.File.Move(path + requestComponent.FolderPath + "\\" + filename, path + requestComponent.FolderPath + "\\" + requestComponent.FileName);
                            System.IO.File.Move(output_Path + requestComponent.FolderPath + "\\" + filename, output_Path + requestComponent.FolderPath + "\\" + requestComponent.FileName);
                        }

                        setDefaultComponent(requestComponent.FolderPath, requestComponent.Name, requestComponent);

                        return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "" }));
                    }
                    else
                    {
                        //save new component
                        requestComponent.FileName = requestComponent.Name + ".json";
                        System.IO.File.WriteAllText(path + requestComponent.FolderPath + "\\" + requestComponent.Name + ".json", JsonConvert.SerializeObject(requestComponent));
                        System.IO.File.WriteAllText(output_Path + requestComponent.FolderPath + "\\" + requestComponent.Name + ".json", JsonConvert.SerializeObject(requestComponent));

                        setDefaultComponent(requestComponent.FolderPath, requestComponent.Name, requestComponent);
                    }
                    return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "" }));
                }


                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "" }));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get Component List: " + ex.Message }));
            }
        }


        [Route("saveConnection")]
        [HttpPost]
        public ActionResult saveConnection(string GUID, string Engine, String ConnectionName, String ConnectionString)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var app = getDevApp();
                string path = app.Dev_Path + "\\QuickProcess\\Properties\\application.json";
                string output_path = app.Output_Path + "\\QuickProcess\\Properties\\application.json";

                var Application = JsonConvert.DeserializeObject<QuickProcessDesigner.Application>(System.IO.File.ReadAllText(path));
                var connection = Application.Connections.Where(conn => conn.GUID == GUID).ToList();

                if (connection.Count == 0)
                {
                    GUID = (new Guid().ToString());
                    Application.Connections.Add(new QuickProcessDesigner.ConnectionList() { GUID = GUID, Engine = Engine, ConnectionName = ConnectionName, ConnectionString = ConnectionString });
                }
                else
                {
                    connection[0].Engine = Engine;
                    connection[0].ConnectionName = ConnectionName;
                    connection[0].ConnectionString = ConnectionString;
                }

                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(Application));
                System.IO.File.WriteAllText(output_path, JsonConvert.SerializeObject(Application));

                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "" }));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to save Connection information: " + ex.Message }));
            }

        }


        [Route("saveApplication")]
        [HttpPost]
        public ActionResult saveApplication(string AppInfo)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<Application>(AppInfo);
                System.IO.File.WriteAllText(request.Dev_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(request));
                System.IO.File.WriteAllText(request.Output_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(request));
                QuickProcess.QuickProcess_Core.refreshFramework();
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "" }));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to save Application information" + ex.Message }));
            }
        }


        [Route("getTableList")]
        [HttpPost]
        public ActionResult getTableList(getTableList_Model request)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == request.ConnectionGUID.ToLower()).ToList();
                var table = Database.ExecQuery(conn[0].ConnectionString, "SELECT * FROM INFORMATION_SCHEMA.TABLES", null);
                return Content(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "getTableList");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get getTableList List" }));
            }
        }


        [Route("getTableColumnList")]
        [HttpPost]
        public ActionResult getTableColumnList(getTableColumnList_Model request)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == request.ConnectionGUID.ToLower()).ToList();
                String Query = "select COLUMN_NAME,IS_NULLABLE,CHARACTER_MAXIMUM_LENGTH,COLUMN_DEFAULT,DATA_TYPE,'' as name from INFORMATION_SCHEMA. COLUMNS where TABLE_NAME=@TABLE_NAME ";
                Query += " union ";
                Query += " SELECT '' as COLUMN_NAME,'' as IS_NULLABLE,'' as CHARACTER_MAXIMUM_LENGTH,'' as COLUMN_DEFAULT,'identity' as DATA_TYPE, name FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = @TABLE_NAME ";
                Query += " union ";
                Query += " select '' as COLUMN_NAME,'' as IS_NULLABLE,'' as CHARACTER_MAXIMUM_LENGTH,'' as COLUMN_DEFAULT,'primarykey' as DATA_TYPE, C.COLUMN_NAME as name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C  ";
                Query += " ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME  WHERE  C.TABLE_NAME= @TABLE_NAME  and T.CONSTRAINT_TYPE='PRIMARY KEY' ";

                var table = Database.ExecQuery(conn[0].ConnectionString, Query, new List<SqlParameter> { new SqlParameter("@TABLE_NAME", request.TableName) });
                foreach (DataRow row in table.Result.Rows)
                {
                    if (string.IsNullOrEmpty(row["COLUMN_DEFAULT"].ToString()) == false)
                    {
                        if (row["COLUMN_DEFAULT"].ToString().StartsWith("(N'"))
                        {
                            row["COLUMN_DEFAULT"] = row["COLUMN_DEFAULT"].ToString().Substring(3, row["COLUMN_DEFAULT"].ToString().Length - 5);
                        }

                        if (row["COLUMN_DEFAULT"].ToString().StartsWith("(("))
                        {
                            row["COLUMN_DEFAULT"] = row["COLUMN_DEFAULT"].ToString().Substring(2, row["COLUMN_DEFAULT"].ToString().Length - 4);
                        }
                    }
                }
                return Content(JsonConvert.SerializeObject(table));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get TableColumnList " }));
            }
        }


        [Route("getQueryColumns")]
        [HttpPost]
        public ActionResult getQueryColumns(getQueryColumns_Model request)
        {
            try
            {
                var app = getDevApp();
                if (Directory.Exists(app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            try
            {
                var conn = getDBConnectionList().Where(conn => conn.GUID.ToLower() == request.ConnectionGUID.ToLower()).ToList();
                String Query = request.Query;
                var Params = new List<SqlParameter>();
                string pattern = @"(?<!\w)@\w+";
                Regex rg = new Regex(pattern);
                MatchCollection matches = rg.Matches(Query);

                for (int count = 0; count < matches.Count; count++)
                {
                    if (string.IsNullOrEmpty(matches[count].Value) == false && Params.Where(param => param.ParameterName.ToLower() == matches[count].Value.ToLower()).ToList().Count() == 0)
                    {
                        Params.Add(new SqlParameter(matches[count].Value, DBNull.Value));
                    }
                }

                System.Data.DataTable table = Database.ExecQuery_Sync(conn[0].ConnectionString, Query, Params);
                string cols = "";
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    cols += col.ColumnName + ",";
                }
                if (cols != "")
                {
                    cols = cols.Substring(0, cols.Length - 1);
                }

                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", Result = cols }));
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
                return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to get QueryColumns " }));
            }
        }


        [Route("CreateTableModels")]
        [HttpGet]
        public async Task CreateTableModels(string tableList, bool updateProgram_cs)
        {
            var app = getDevApp();
            string query = "";
            string HasNoKey = "";
            string template_query = "declare @TableName sysname = '##TableName' declare @Result varchar(max) = 'public class ' + @TableName + ' \r\n{\r\n' select @Result = @Result + '\r\n public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; } ' from ( select replace(col.name, ' ', '_') ColumnName, column_id ColumnId, case typ.name when 'bigint' then 'long' when 'binary' then 'byte[]' when 'bit' then 'bool' when 'char' then 'string' when 'date' then 'DateTime' when 'datetime' then 'DateTime' when 'datetime2' then 'DateTime' when 'datetimeoffset' then 'DateTimeOffset' when 'decimal' then 'decimal' when 'float' then 'double' when 'image' then 'byte[]' when 'int' then 'int' when 'money' then 'decimal' when 'nchar' then 'string' when 'ntext' then 'string' when 'numeric' then 'decimal' when 'nvarchar' then 'string' when 'real' then 'float' when 'smalldatetime' then 'DateTime' when 'smallint' then 'short' when 'smallmoney' then 'decimal' when 'text' then 'string' when 'time' then 'TimeSpan' when 'timestamp' then 'long' when 'tinyint' then 'byte' when 'uniqueidentifier' then 'Guid' when 'varbinary' then 'byte[]' when 'varchar' then 'string' else 'string' end ColumnType, case when col.is_nullable = 1 and typ.name in ('nchar','ntext','text','varchar','nvarchar','char','bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') then '?' else '' end NullableSign from sys.columns col join sys.types typ on col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id where object_id = object_id(@TableName) ) t order by ColumnId set @Result = @Result + ' \r\n}\r\n' select @Result";
            var conn = getDBConnectionList().Where(conn => conn.ConnectionName.ToLower() == app.DefaultConnection.ToLower()).ToList();
            var table = await Database.ExecQuery(conn[0].ConnectionString, "SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='dbo'", null);

            if (Directory.Exists(app.Dev_Path + "/Model/") == false)
                Directory.CreateDirectory(app.Dev_Path + "/Model/");

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    if (row["TABLE_TYPE"].ToString() == "BASE TABLE")
                    {
                        query = template_query.Replace("##TableName", row["TABLE_NAME"].ToString());
                        string classText = (await Database.ExecQuery(conn[0].ConnectionString, query, null)).Rows[0][0].ToString();

                        string filePath = app.Dev_Path + "/Model/" + row["TABLE_NAME"].ToString() + ".cs";
                        System.IO.File.WriteAllText(filePath, classText);
                    }

                    // if (row["TABLE_TYPE"].ToString() == "VIEW")
                    // HasNoKey += "\r\nmodelBuilder.HasDefaultSchema(\"" + row["TABLE_NAME"].ToString() + "\").Entity(\"" + row["TABLE_NAME"].ToString() + "\").HasNoKey();\r\n";

                }
                catch (Exception ex) { }
            }

            //create context file
            string contextFile = "using Microsoft.EntityFrameworkCore;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Threading.Tasks;\r\n\r\n  public class " + app.ApplicationName + "Context:DbContext\r\n{\r\n   public " + app.ApplicationName + "Context(DbContextOptions<" + app.ApplicationName + "Context> options) : base(options)\r\n{\r\n\r\n}\r\n\r\n   protected override void OnModelCreating(ModelBuilder modelBuilder)\r\n{\r\nbase.OnModelCreating(modelBuilder);\r\n " + HasNoKey + "\r\n}\r\n\r\n ";

            foreach (DataRow row in table.Rows)
            {
                if (row["TABLE_TYPE"].ToString() == "BASE TABLE")
                    contextFile += "\r\npublic DbSet<" + row["TABLE_NAME"].ToString() + "> " + row["TABLE_NAME"].ToString() + " { get; set; }";
            }
            contextFile += "\r\n}\r\n";

            System.IO.File.WriteAllText(app.Dev_Path + "/Model/" + app.ApplicationName + "Context.cs", contextFile);

            if (updateProgram_cs)
            {
                try
                {
                    string program_cs = System.IO.File.ReadAllText(app.Dev_Path + "/Program.cs");
                    program_cs = "using Microsoft.EntityFrameworkCore;\r\n\r\n" + program_cs.Replace("WebApplication.CreateBuilder(args);", "WebApplication.CreateBuilder(args);\r\n\r\nvar qp = await QuickProcess.QuickProcess_Core.getApplication();\r\nstring conStr = qp.Connections.Where(conn => conn.ConnectionName.ToLower() == qp.DefaultConnection.ToLower()).FirstOrDefault().ConnectionString;\r\nbuilder.Services.AddDbContext<" + app.ApplicationName + "Context>(options => options.UseSqlServer(conStr));\r\n");
                    System.IO.File.WriteAllText(app.Dev_Path + "/Program.cs", program_cs);
                }
                catch { }
            }
        }


        [Route("PublishComponent")]
        [HttpPost]
        public ActionResult PublishComponent(string componentText)
        {
            try
            {
                var _app = getDevApp();
                if (Directory.Exists(_app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }

            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            string path = "";
            if (Directory.Exists(binDirectory + "\\"))
                path = binDirectory + "\\";

            if (Directory.Exists(rootDirectory + "\\"))
                path = rootDirectory + "\\";

            var app = getDevApp();
            if (Request.Headers["PublishKey"].ToString() == app.PublishKey)
            {
                try
                {
                    var component = Newtonsoft.Json.JsonConvert.DeserializeObject<Component>(componentText);
                    if (System.IO.File.Exists(path + "QuickProcess\\" + component.FileName) == false)
                        System.IO.File.Create(path + "QuickProcess\\" + component.FileName).Close();

                    StreamWriter sw = new StreamWriter(path + "QuickProcess\\" + component.FileName);
                    System.IO.File.WriteAllText(path + "QuickProcess\\" + component.FileName, componentText);
                }
                catch (Exception ex)
                {
                    return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "-1", ResponseDescription = "Unable to publish component " }));
                }
            }

            return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "Okay" }));
        }


        [Route("PublishComponents")]
        [HttpGet]
        public ActionResult PublishComponents()
        {
            try
            {
                var _app = getDevApp();
                if (Directory.Exists(_app.Dev_Path) == false)
                    return NotFound();
            }
            catch { }


            var app = getDevApp();
            string path = app.Dev_Path + "\\QuickProcess\\Components";

            DirectoryInfo directory = new DirectoryInfo(path);

            using (var client = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(app.PublishUrl),
                    Headers = { { "PublishKey", app.PublishKey } },
                };

                foreach (FileInfo file in directory.GetFiles())
                {
                    string json = System.IO.File.ReadAllText(file.FullName);
                    httpRequestMessage.Content = new StringContent(json);
                    var response = client.SendAsync(httpRequestMessage).Result;
                }

                //push quickprocess js and css files
            }

            return Content(JsonConvert.SerializeObject(new QuickProcessDesigner.GenericResponse() { ResponseCode = "00", ResponseDescription = "" }));

        }

        public void setDefaultComponent(string FullPath, string OldComponentName, QuickProcessDesigner.Component Component)
        {
            try
            {
                var DesignerApp = getDevApp();
                var App = JsonConvert.DeserializeObject<QuickProcessDesigner.Application>(System.IO.File.ReadAllText(DesignerApp.Dev_Path + "\\QuickProcess\\Properties\\application.json"));

                var PrevDefaultComponent = string.IsNullOrEmpty(App.DefaultComponent) ? "" : App.DefaultComponent;

                if (Component.DefaultComponent && PrevDefaultComponent.ToLower() != Component.Name.ToLower())
                {
                    //set new component as default component
                    App.DefaultComponent = Component.Name;

                    //update Previous default component to false in dev path and output path
                    if (!string.IsNullOrEmpty(PrevDefaultComponent))
                    {
                        string cpm_dev_path = DesignerApp.Dev_Path + "\\QuickProcess\\Components\\" + App.ComponentList.Where(cmp => cmp.Name.ToLower() == PrevDefaultComponent.ToLower()).FirstOrDefault().FolderPath + PrevDefaultComponent + ".json";
                        string cpm_output_path = DesignerApp.Output_Path + "\\QuickProcess\\Components\\" + App.ComponentList.Where(cmp => cmp.Name.ToLower() == PrevDefaultComponent.ToLower()).FirstOrDefault().FolderPath + PrevDefaultComponent + ".json";

                        var component = JsonConvert.DeserializeObject<QuickProcessDesigner.Component>(System.IO.File.ReadAllText(cpm_dev_path));
                        component.DefaultComponent = false;

                        System.IO.File.WriteAllText(cpm_dev_path, JsonConvert.SerializeObject(component));
                        System.IO.File.WriteAllText(cpm_output_path, JsonConvert.SerializeObject(component));
                    }
                }

                //update prefetch component list
                if (Component.PreFetchComponent)
                {
                    App.PreFetchComponents = (App.PreFetchComponents == null) ? "" : App.PreFetchComponents;

                    string PreFetchComponents = "";
                    foreach (string cmp in App.PreFetchComponents.Split(new char[] { ',' }))
                    {
                        if (!string.IsNullOrEmpty(cmp) && cmp.ToLower() != OldComponentName.ToLower())
                            PreFetchComponents += cmp + ",";
                    }
                    PreFetchComponents += Component.Name;
                    App.PreFetchComponents = PreFetchComponents;
                }

                //avoid null exception
                if (App.ComponentList == null)
                    App.ComponentList = new List<ComponentList>();

                //update ComponentList of Application object
                var componentOnList = App.ComponentList.Where(cmp => cmp.Name == OldComponentName).ToList();

                if (componentOnList == null || componentOnList.Count == 0)
                    App.ComponentList.Add(new ComponentList() { Name = Component.Name, PreFetchComponent = Component.PreFetchComponent, ResourceGroups = Component.ResourceGroups, FolderPath = FullPath });
                else
                {
                    foreach (var item in componentOnList)
                    {
                        item.Name = Component.Name;
                        item.PreFetchComponent = Component.PreFetchComponent;
                        item.ResourceGroups = Component.ResourceGroups;
                        item.FolderPath = FullPath;
                    }
                }

                System.IO.File.WriteAllText(DesignerApp.Dev_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(App));
                System.IO.File.WriteAllText(DesignerApp.Output_Path + "\\QuickProcess\\Properties\\application.json", JsonConvert.SerializeObject(App));
                QuickProcess.QuickProcess_Core.refreshFramework();
            }
            catch (Exception ex)
            {
                ErroLogger.LogError((new StackTrace()).GetFrame(0).GetMethod().ToString(), ex.Message, (new StackTrace()).GetFrame(0).GetILOffset().ToString(), DateTime.Now.ToString() + ":" + ex.StackTrace, "", "GetApplicationComponent");
            }
        }

        private List<QuickProcessDesigner.ConnectionList> getDBConnectionList()
        {
            return JsonConvert.DeserializeObject<List<QuickProcessDesigner.ConnectionList>>(JsonConvert.SerializeObject(getDevApp().Connections));
        }

        private string appPath()
        {
            string path = "";
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));

            if (Directory.Exists(binDirectory + "\\QuickProcess\\"))
                path = binDirectory + "\\QuickProcess\\";

            //consider azure
            if (Directory.Exists(rootDirectory + "\\QuickProcess\\"))
                path = rootDirectory + "\\QuickProcess\\";

            return path;
        }

        private Application getDevApp()
        {
            string path = appPath() + "Properties\\application.json";
            if (System.IO.File.Exists(path))
                return JsonConvert.DeserializeObject<QuickProcessDesigner.Application>(System.IO.File.ReadAllText(path));
            else
                return null;
        }

        private string OutputDirectory()
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            string path = "";

            if (Directory.Exists(rootDirectory))
                path = rootDirectory;

            if (Directory.Exists(binDirectory))
                path = binDirectory;

            return path;
        }

        class DynatreeItem
        {
            public string text { get; set; }
            public bool uiComponent { get; set; }
            public string icon { get; set; }
            public string path { get; set; }
            public string type { get; set; }
            public bool isFolder { get; set; }
            //public string key { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public List<DynatreeItem> nodes;

            public DynatreeItem(FileSystemInfo fsi, string rootpath, List<ComponentList> ComponentList)
            {
                text = fsi.Name.Split(new char[] { '.' })[0].ToString();
                nodes = new List<DynatreeItem>();

                if (fsi.Attributes == FileAttributes.Directory)
                {
                    isFolder = true;
                    icon = "fa fa-folder-o fa-fw";
                    type = "folder";
                    uiComponent = false;
                    try
                    {
                        path = fsi.FullName.Substring(rootpath.Length).Replace("\\", "/");
                    }
                    catch { }
                    foreach (FileSystemInfo f in (fsi as DirectoryInfo).GetFileSystemInfos())
                    {
                        nodes.Add(new DynatreeItem(f, rootpath, ComponentList));
                    }
                }
                else
                {
                    isFolder = false;
                    path = fsi.FullName.Substring(rootpath.Length).Replace("\\", "/");
                    icon = "fa fa-file-o fa-fw";
                    try
                    {
                        var component = Newtonsoft.Json.JsonConvert.DeserializeObject<Component>(System.IO.File.ReadAllText(fsi.FullName));
                        type = component.Type;

                        if (component.Type == ComponentType.TableList)
                        {
                            uiComponent = true;
                            icon = "fa fa-table fa-fw";
                        }

                        if (component.Type == ComponentType.CardList)
                        {
                            uiComponent = true;
                            icon = "fa fa-grid fa-fw";
                        }

                        if (component.Type == ComponentType.Form)
                        {
                            uiComponent = true;
                            icon = "fa fa-edit fa-fw";
                        }

                        if (component.Type == ComponentType.WebApi)
                        {
                            uiComponent = false;
                            icon = "fa fa-diagram-project fa-fw";
                        }

                        if (component.Type == ComponentType.Query)
                        {
                            uiComponent = false;
                            icon = "fa fa-database fa-fw";
                        }

                        if (component.Type == ComponentType.DropDownList)
                        {
                            uiComponent = false;
                            icon = "fa fa-list-dropdown fa-fw";
                        }

                        if (component.Type == ComponentType.HTML)
                        {
                            uiComponent = true;
                            icon = "fa fa-file-code fa-fw";
                        }


                        var ComList = new ComponentList()
                        {
                            Name = component.Name,
                            FolderPath = path.Substring(0, (path.Length - (component.FileName.Length + 1))).Replace("/", "\\"),
                            PreFetchComponent = component.PreFetchComponent,
                            ResourceGroups = component.ResourceGroups
                        };
                        ComponentList.Add(ComList);
                    }
                    catch { }

                    nodes = null;

                }
                // key = text.Replace(" ", "").ToLower();
            }

            public string JsonToDynatree()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
        }
    }
}
