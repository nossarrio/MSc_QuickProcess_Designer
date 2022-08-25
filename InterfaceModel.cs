
using Microsoft.Extensions.Logging;
//using MySql.Data.MySqlClient;
//using Npgsql;
//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace QuickProcessDesigner
{

    public class ErroLogger
    {
        ILogger<dynamic> _logger;

        public ErroLogger(ILogger<dynamic> logger)
        {
            _logger = logger;
        }

        public async void LogError(String method, String message, String linenumber, String otherinfo, string AppId, string Component)
        {
            string error = "\r\n\r\nMethod\r\n===================\r\n" + method + "\r\n\r\nMessage\r\n===================\r\n" + message + "\r\n\r\nLine Number\r\n===================\r\n" + linenumber + "\r\n\r\nOther Info\r\n===================\r\n" + otherinfo + "\r\n\r\nProcess\r\n===================\r\n" + AppId + "\r\n\r\nComponent \r\n===================\r\n" + Component;
            _logger.LogError(error);
        }
    }


    public class getComponentList_Model
    {
        public string GUID { get; set; }
    }

    public class getComponentDefinition_Model
    {
        public string path { get; set; }
    }

    public class saveComponentDefinition_Model
    {
        public string AppGUID { get; set; }
        public string Component { get; set; }
    }
    public class createAppDomain_Model
    {
        public string GUID { get; set; }
        public string DomainName { get; set; }
        public string Url { get; set; }
    }

    public class createApplication_Model
    {
        public string GUID { get; set; }
        public string Output_Path { get; set; }
        public string Dev_Path { get; set; }
        public string Url { get; set; }
        public string SessionValidationComponent { get; set; }
        public string DefaultComponent { get; set; }
        public string ThemeColor { get; set; }
        public string FontColor { get; set; }
        public string ApplicationTitle { get; set; }
        public string ApplicationName { get; set; }
        public bool LoginPage { get; set; }
        public bool MasterPage { get; set; }
        public string DefaultConnection { get; set; }
        public string DefaultDomain { get; set; }
        public string AuthenticationMode { get; set; }
        public string QuickProcessJs_Url { get; set; }
        public string QuickProcessCss_Url { get; set; }
        public string QuickProcessLoadergif_Url { get; set; }
        public string Default_FormWrapperMarkup { get; set; }
        public string Default_ListWrapperMarkup { get; set; }
        public bool Debug { get; set; }
    }

    public class AppDefinition
    {
        public string Url { get; set; }
        public string ApplicationTitle { get; set; }
        public string GUID { get; set; }
        public string Default_FormWrapperMarkup { get; set; }
        public string Default_ListWrapperMarkup { get; set; }
        public string[] ResourceGroup { get; set; }
    }

    public class Application
    {
        public string GUID { get; set; }
        public string ThemeColor { get; set; }
        public string FontColor { get; set; }
        public string Dev_Path { get; set; }
        public string Output_Path { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string ApplicationTitle { get; set; }
        public string DefaultComponent { get; set; }
        public string DefaultConnection { get; set; }
        public string DefaultDomain { get; set; }
        public string ApplicationName { get; set; }
        public string PreFetchComponents { get; set; }
        public string SessionValidationComponent { get; set; }
        public string QuickProcessJs_Url { get; set; }
        public string QuickProcessCss_Url { get; set; }
        public string QuickProcessLoadergif_Url { get; set; }
        public string Default_FormWrapperMarkup { get; set; }
        public string Default_ListWrapperMarkup { get; set; }
        public bool Debug { get; set; }
        public bool EnableAuthentication { get; set; }
        public bool EnableAuthorisation { get; set; }
        public AuthorisationDetails AuthorisationDetails { get; set; }
        public List<ConnectionList> Connections { get; set; }
        public List<ExternalDomainList> ExternalDomains { get; set; }
        public List<ComponentList> ComponentList { get; set; }
        public string PublishUrl { get; internal set; }
        public string PublishKey { get; internal set; }

        public ConnectionList getConnection(string connetionGUID)
        {
            return this.Connections.Where(con => con.GUID == connetionGUID).FirstOrDefault();
        }
    }


    public class ComponentList
    {
        public string Name { get; set; }
        public bool PreFetchComponent { get; set; }
        public List<string> ResourceGroups { get; set; }
        public string FolderPath { get; set; }
    }

    public class DesignerApplication
    {
        public string GUID { get; set; }
        public string Dev_Path { get; set; }
        public string Output_Path { get; set; }
        public string Url { get; set; }
    }


    public class getDBList_Model
    {
        public string ConnectionGUID { get; set; }
    }

    public class getTableList_Model
    {
        public string AppGUID { get; set; }
        public string ConnectionGUID { get; set; }
        public string DatabaseName { get; set; }
    }

    public class getTableColumnList_Model
    {
        public string AppGUID { get; set; }
        public string ConnectionGUID { get; set; }
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
    }

    public class getQueryColumns_Model
    {
        public string AppGUID { get; set; }
        public string ConnectionGUID { get; set; }
        public string Query { get; set; }
        public string QueryParams { get; set; }
    }

    public class  cloneComponent_Model
    {
        public string filePath { get; set; }
        public string newName { get; set; }
    }

    public class FileFolder
    {
        public string Name { get; set; }
        public FileFolder[] Children { get; set; }
    }

    public class getComponent_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string ComponentName { get; set; }
    }

    public class searchRecord_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string PageIndex { get; set; }
        public string PageSize { get; set; }
        public string SearchText { get; set; }
        public string Download { get; set; }
        public string RecordID { get; set; }
        public string DataSourceParams { get; set; }
        public string DownloadFormat { get; set; }
        public string AdvanceSearchOptions { get; set; }
        public string SortOrder { get; set; }
        public string ComponentName { get; set; }
    }

    public class Passerel_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string ComponentName { get; set; }
        public string parameters { get; set; }

    }

    public class getRecord_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string ComponentName { get; set; }
        public string RecordID { get; set; }
        public string DataSourceParams { get; set; }
    }

    public class deleteRecord_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string ComponentName { get; set; }
        public string RecordId { get; set; }
    }

    public class getDropDownList_Model
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string ComponentName { get; set; }
        public string SearchText { get; set; }
    }

    public class PasserelObject
    {
        public string AppId { get; set; }
        public string SessionId { get; set; }
        public string parameters { get; set; }
    }

    public class ConnectionList
    {
        public string GUID { get; set; }
        public string ConnectionName { get; set; }
        public string Engine { get; set; }
        public string ConnectionString { get; set; }
        public bool isDefaultConnection { get; set; }
    }

    public class ExternalDomainList
    {
        public string GUID { get; set; }
        public string DomainName { get; set; }
        public string Url { get; set; }
    }


    public class GenericResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string Result { get; set; }
    }

    public class UserSession
    {
        public string SessionID { get; set; }
        public string UserName { get; set; }
    }


    public class FormControls
    {
        public string FieldName { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string ControlType { get; set; }
        public string DataForm { get; set; }// indicates if data should be hashed or encrypted in db
        public bool Compulsory { get; set; }
        public string DataSource { get; set; }
        public ControlList[] List { get; set; }
        public bool EnableInsert { get; set; } = true;
        public bool EnableSave { get; set; } = true; //works for both insert and update
        public bool EnableUpdate { get; set; } = true;
        public bool EnableFetch { get; set; } = true;
        public bool ReadOnly { get; set; }
        public string DefaultValue { get; set; }
        public string DataType { get; set; } = "string";
        public int MaxLength { get; set; }
    }

    public class ControlList
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public static class ControlType
    {
        public static string Text = "text";
        public static string Rating = "RATING";
        public static string BadgeListGroup = "BADGE LIST GROUP";
        public static string CheckBoxList = "checkbox_list";
        public static string Toggle = "TOGGLE";
        public static string ToggleList = "TOGGLE LIST";
        public static string RadioButtonList = "RADIO BUTTON LIST";
        public static string SignPad = "SIGN PAD";
        public static string User_Phone_Validator = "USER PHONE VALIDATOR";
        public static string User_Email_Validator = "USER EMAIL VALIDATOR";
        public static string User_PIN_Validator = "USER PIN VALIDATOR";
        public static string User_Password_Validator = "USER PASSWORD VALIDATOR";

        public static string HtmlEditor = "HTML EDITOR";
        public static string CheckBox = "checkbox";
        public static string Search = "search";
        public static string CurrentDateTime = "current_date_time";
        public static string ServerField = "SERVER FIELD";
        public static string IPAddress = "IPADDRESS";
        public static string Geolocation = "GEOLOCATION";
        public static string Number = "number";
        public static string Phone = "tel";
        public static string Number_CommaSeparated = "number_cs";
        public static string Money = "money";
        public static string Text_Area = "text_area AREA";
        public static string Static_DropDown = "static_dropdown";
        public static string ColourPicker = "COLOUR PICKER";
        public static string Date = "date";
        public static string Month = "month";
        public static string Week = "week";
        public static string Range = "range";
        public static string DateTime = "datetime";
        public static string Dynamic_DropDown = "dynamic_dropdown";
        public static string Static_CheckList = "STATIC CHECKLIST";

        public static string Dynamic_ImageLookup = "DYNAMIC IMAGE LOOKUP";
        public static string Static_ImageLookup = "STATIC IMAGE LOOKUP";

        public static string Dynamic_CheckList = "DYNAMIC CHECKLIST";
        public static string AutoComplete = "AUTOCOMPLETE";
        public static string StaffLookUp = "STAFFLOOKUP";
        public static string GUID = "GUID";
        public static string FILEUPLOAD = "file";
        public static string EMAIL = "email";
        public static string HIDDEN_FIELD = "hidden";
        public static string LABEL = "LABEL";
        public static string CURRENT_USER = "current_user";
        public static string PASSWORD = "password";
        public static string TABLE_LOOKUP = "TABLE LOOKUP";
        public static string ImageUpload = "IMAGE UPLOAD";


    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class UniqueColumn
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool isIdentity { get; set; }
    }

    public class SearchResponse
    {
        public PagerSetup PagerSetup { get; set; }
        public string[,] Data { get; set; }
        public GenericResponse Response { get; set; }
    }

    public class PagerSetup
    {
        public bool FirstPage { get; set; }
        public bool LastPage { get; set; }
        public bool ReducePageIndex { get; set; }
        public bool IncreasePageIndex { get; set; }
        public string[] InViewPageIndices { get; set; }
        public int ActiveIndex { get; set; }
        public bool NextPageList { get; set; }
        public bool PrevPageList { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPages { get; set; }
        public string Columns { get; set; }
        public string UColumn { get; set; }
    }

    public class Columns
    {
        public string ColumnName { get; set; }
        public string HeaderText { get; set; }
        public bool Searchable { get; set; }
        public string DataTypeFormat { get; set; }
        public bool WrapText { get; set; }
        public bool Visible { get; set; }
        public string Sort { get; set; } = "none";
        public string Align { get; set; } = "left";
        public string Width { get; set; }
        public string Height { get; set; }
        public bool IsBound { get; set; } = true;
        public string DataType { get; set; }
        public string Template { get; set; }
    }

    public class ComponentConnection
    {
        public string Engine { get; set; }
        public string GUID { get; set; }
    }

    public class SortOrder
    {
        public int ColumnIndex { get; set; }
        public string Order { get; set; }
    }

    public static class DataSourceType
    {
        public static string Table = "table";
        public static string View = "view";
        public static string Function = "function";
        public static string Procedure = "procedure";
        public static string Query = "query";
        public static string API = "api";
    }

    public static class ComponentType
    {
        public static string HTML = "html";
        public static string TableList = "table";
        public static string CardList = "card";
        public static string Form = "form";
        public static string Query = "query";
        public static string DropDownList = "dropdown";
        public static string WebApi = "api";
    }

    public class Component
    {
        public string FileName { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool SessionBased { get; set; }
        public List<string> ResourceGroups { get; set; }
        public string About { get; set; }
        public bool AutoLoad { get; set; }
        public bool DefaultComponent { get; set; }
        public string DataSourceType { get; set; }
        public string DataSourceTable { get; set; }
        public string Schema { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string CurrentQueryString { get; set; }
        public ComponentConnection Connection { get; set; }
        public int ConnectionTimeout { get; set; } = 30;
        public string ConnectionString { get; set; }
        public bool Disabled { get; set; }
        public string WrapperMarkup { get; set; } = "@RenderComponent";
        public string ComponentEvents { get; set; }
        public string ComponentFunctions { get; set; }
        public string ThemeColor { get; set; }
        public bool IsLayoutComponent { get; set; }
        public bool PreFetchComponent { get; set; }
        public string Route { get; set; }
        public string FolderPath { get; set; }

        public string FetchMethod { get; set; }
        public string PostMethod { get; set; }
        public string FetchUrl { get; set; }
        public string PostUrl { get; set; }


        //-----------Designer Fields--------------//
        public string Designer_QueryColumns { get; set; }
        public string Designer_QueryParams { get; set; }


        //-----------List Features--------------//
        public Columns[] Columns { get; set; }
        public string CustomSelectQuery { get; set; }
        public string CustomDeleteQuery { get; set; }
        public string RecordTemplate { get; set; }
        public string RecordMenuTemplate { get; set; }
        public string loadDataMode { get; set; }
        public string onRecordClick { get; set; }
        public bool EnableSorting { get; set; }
        public bool EnablePagination { get; set; }
        public bool EnableSearch { get; set; }
        public bool EnableExport { get; set; }
        public bool ShowRecordMenu { get; set; }
        public bool ShowStripes { get; set; }
        public bool ShowHoverEffect { get; set; }
        public bool ModalForm { get; set; } = false;
        public string PaginationPosition { get; set; }
        public string NewRecordText { get; set; } = "New Record";
        public bool ShowIcon { get; set; }
        public int DefaultPageSize { get; set; }
        public bool ShowTitle { get; set; }
        public bool ModalProgress { get; set; }
        public bool ShowRecordCount { get; set; }
        public bool AutoSearch { get; set; }
        public bool ShowRefresh { get; set; } = true;
        public string EmptyRecordTemplate { get; set; }
        public string FormComponent { get; set; }

        //----------------List and Form Features--------------------//
        public UniqueColumn UniqueColumn { get; set; }
        public string FormUniqueColumn { get; set; }
        public string QueryParameters { get; set; }
        public bool EnableInsert { get; set; } = false;
        public bool EnableEdit { get; set; } = false;
        public bool EnableDelete { get; set; } = false;
        public bool EnableView { get; set; } = false;
        public bool ShowBorder { get; set; }
        public string QueryStringFilter { get; set; }


        //----------------Form Features--------------------//
        public int GridDisplay { get; set; }
        public FormControls[] Controls { get; set; }
        public string CustomInsertQuery { get; set; }
        public string CustomUpdateQuery { get; set; }


        //---------------Dropdown List Features----------// 
        public string DisplayField { get; set; }
        public string ValueField { get; set; }


        //---------------API Query Features----------//
        public string Query { get; set; }
        public string ApiUrl { get; set; }
        //public EndPoint EndPoint { get; set; }


        //---------------HTML Features----------//
        public string Markup { get; set; }
        public string LayoutPage { get; set; }
    }


    public class ApplicationDomain
    {
        public string GUID { get; set; }
        public string DomainName { get; set; }
        public string Url { get; set; }

    }


    public class AuthorisationDetails
    {
        public string AuthorisationComponent { get; set; }
        public string[] Resources { get; set; }
    }


    public class Database
    {
        public static async Task<DataTable> ExecQuery(string connectionstring, string Query, List<SqlParameter> Params)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> @params = Params;
            string engine = "ms sqlserver";

            switch (engine.ToLower())
            {
                case "ms sqlserver":
                    SqlCommand sqlCommand = new SqlCommand(Query.ToString(), new SqlConnection(connectionstring));

                    if (Params != null)
                    {
                        for (int i = 0; i < (int)@params.Count; i++)
                        {
                            if (@params[i] != null)
                            {
                                SqlParameter sqlParameter = new SqlParameter(@params[i].ParameterName, @params[i].Value);
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }
                    }

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    DataTable dMSSQLTable = new DataTable();

                    sqlDataAdapter.Fill(dMSSQLTable);
                    return dMSSQLTable;
                    break;

                //case "oracle":
                //    using (OracleConnection conn = new OracleConnection(connectionstring))
                //    {
                //        if (conn.State == 0) { conn.Open(); }
                //        OracleCommand comm = new OracleCommand(Query, conn);

                //        if (Params != null)
                //        {
                //            for (int i = 0; i < (int)@params.Count; i++)
                //            {
                //                SqlParameter sqlParameter = @params[i];
                //                comm.Parameters.Add(sqlParameter.ParameterName, sqlParameter.Value);
                //            }
                //        }

                //        OracleDataAdapter da = new OracleDataAdapter(comm);
                //        DataTable OracleTable = new DataTable();
                //        da.Fill(OracleTable);
                //        return OracleTable;
                //    }
                //    break;


                //case "my sql":
                //    Query = Query.Replace("[", "`").Replace("]", "`").Replace("isnull", "coalesce").Replace("CONVERT(date,", "DATE(date");
                //    MySqlConnection cnn = new MySqlConnection(connectionstring);
                //    MySqlCommand cmd = new MySqlCommand(Query, cnn);

                //    if (Params != null)
                //    {
                //        for (int i = 0; i < (int)@params.Count; i++)
                //        {
                //            if (@params[i] != null)
                //            {
                //                SqlParameter sqlParameter = new SqlParameter(@params[i].ParameterName, @params[i].Value);
                //                cmd.Parameters.Add(new MySqlParameter(sqlParameter.ParameterName, sqlParameter.Value));
                //            }
                //        }
                //    }

                //    MySqlDataAdapter Adapter = new MySqlDataAdapter(cmd);
                //    System.Data.DataTable MySqldTable = new System.Data.DataTable();
                //    Adapter.Fill(MySqldTable);
                //    return MySqldTable;
                //    break;

                //case "postgre sql":
                //    Query = Query.Replace("[", "").Replace("]", "").Replace("isnull", "coalesce").Replace("+", "||").Replace("CONVERT(date,", "DATE(date"); ;
                //    NpgsqlConnection pgcnn = new NpgsqlConnection(connectionstring);
                //    NpgsqlCommand pgcmd = new NpgsqlCommand(Query, pgcnn);

                //    if (Params != null)
                //    {
                //        for (int i = 0; i < (int)@params.Count; i++)
                //        {
                //            if (@params[i] != null)
                //            {
                //                SqlParameter sqlParameter = new SqlParameter(@params[i].ParameterName, @params[i].Value);
                //                pgcmd.Parameters.AddWithValue(sqlParameter.ParameterName, sqlParameter.Value);
                //            }
                //        }
                //    }

                //    NpgsqlDataAdapter pgAdapter = new NpgsqlDataAdapter(pgcmd);
                //    System.Data.DataTable pgSqldTable = new System.Data.DataTable();
                //    pgAdapter.Fill(pgSqldTable);
                //    return pgSqldTable;
                //    break;
            }

            return new DataTable();
        }

        public static async Task<DataTable> ExecQuerySchema(string connectionstring, string Query, List<SqlParameter> Params)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> @params = Params;
            string engine = "ms sqlserver";

            switch (engine.ToLower())
            {
                case "ms sqlserver":
                    SqlCommand sqlCommand = new SqlCommand(Query.ToString(), new SqlConnection(connectionstring));

                    if (Params != null)
                    {
                        for (int i = 0; i < (int)@params.Count; i++)
                        {
                            if (@params[i] != null)
                            {
                                SqlParameter sqlParameter = new SqlParameter(@params[i].ParameterName, @params[i].Value);
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }
                    }

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    DataTable dMSSQLTable = new DataTable();

                    sqlDataAdapter.FillSchema(dMSSQLTable, SchemaType.Source);
                    return dMSSQLTable;
                    break;

            }

            return new DataTable();
        }

        public static DataTable ExecQuery_Sync(string connectionstring, string Query, List<SqlParameter> Params)
        {
            DataSet ds = new DataSet();
            List<SqlParameter> @params = Params;
            string engine = "ms sqlserver";

            switch (engine.ToLower())
            {
                case "ms sqlserver":
                    SqlCommand sqlCommand = new SqlCommand(Query.ToString(), new SqlConnection(connectionstring));

                    if (Params != null)
                    {
                        for (int i = 0; i < (int)@params.Count; i++)
                        {
                            if (@params[i] != null)
                            {
                                SqlParameter sqlParameter = new SqlParameter(@params[i].ParameterName, @params[i].Value);
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                        }
                    }

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    DataTable dMSSQLTable = new DataTable();

                    sqlDataAdapter.Fill(dMSSQLTable);
                    return dMSSQLTable;
                    break;

            }

            return new DataTable();
        }

    }


}
