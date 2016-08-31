using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessRule;
using Logger;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Script.Serialization;

namespace UploadPathwaysReports
{
    public partial class UploadReportService : ServiceBase
    {
        private static string createdcaseid = string.Empty;
        public UploadReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Thread.Sleep(15000);
            try
            {
                ErrorLogger.WriteActivity("UploadReportService", "OnStart", "Service Started.");
                this.RequestAdditionalTime(5000);
                System.Timers.Timer timer = new System.Timers.Timer(Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["ReportUploadTimeinHours"].ToString().Trim()) * 1000 * 60*60);
                timer.Enabled = true;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(StartService);
                // StartService();
            }
            catch (Exception ee)
            {
                ErrorLogger.WriteError("UploadReportService", "OnStart", ee.ToString().Trim());
            }
        }

        private void StartService(object sender, System.Timers.ElapsedEventArgs e)
        {
           // Thread.Sleep(15000);
            try
            {
                SerializePathWaysData().Wait();
            }
            catch (Exception exp)
            {
                ErrorLogger.WriteError("UploadReportService", "StartService", exp.ToString());
            }
        }

        private async Task SerializePathWaysData()
        {
            try
            {
                clsCall objcall = new clsCall(System.Configuration.ConfigurationSettings.AppSettings["WsBaseURL"].ToString().Trim());
                DataSet ds = new DataSet();
                ds = objcall.GetPathWaysReportData(new DateTime(2015, 12, 04), new DateTime(2015, 11, 25));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    RemoveTimezoneForDataSet(ds);
                    ErrorLogger.WriteActivity("", "", "Operation started");
                    var myResult = ds.Tables[0].AsEnumerable().Select(c => c["CallID"]).Distinct().ToList();
                    Task<Boolean>[] objtaskss = new Task<Boolean>[myResult.Count];
                    for (int j = 0; j < myResult.Count; j++)
                    {
                        clsPathWaysCaseRecord objCaseRecord = new clsPathWaysCaseRecord();
                        //objCaseRecord.siteId = 21;
                        //objCaseRecord.supplierCallRef = ds.Tables[0].Rows[0]["suppliercallRef"].ToString().Trim();
                        ////objCaseRecord.chSkillsetId = Convert.ToInt16(ds.Tables[0].Rows[0]["skillsetid"].ToString().Trim());
                        ////objCaseRecord.chUserCode = ds.Tables[0].Rows[0]["userID"].ToString().Trim();
                        ////objCaseRecord.clSkillsetId = Convert.ToInt16(ds.Tables[0].Rows[0]["skillsetid"].ToString().Trim());
                        //objCaseRecord.ageGroupId = Convert.ToInt16(ds.Tables[0].Rows[0]["ageGroupID"].ToString().Trim());
                        //objCaseRecord.genderId = Convert.ToInt16(ds.Tables[0].Rows[0]["GenderID"].ToString().Trim());
                        //objCaseRecord.partyId = Convert.ToInt16(ds.Tables[0].Rows[0]["PartyID"].ToString().Trim());
                        //objCaseRecord.releaseVersion = ds.Tables[0].Rows[0]["releaseVersion"].ToString().Trim();
                        //objCaseRecord.postcode = ds.Tables[0].Rows[0]["postCode"].ToString().Trim();

                        int callid = Convert.ToInt32(myResult[j].ToString());
                        DataView dv = new DataView(ds.Tables[0], "CallID=" + callid.ToString(), "Callid", DataViewRowState.CurrentRows);
                        int questionscount = dv.Count;
                        objCaseRecord.siteId = Convert.ToInt16(System.Configuration.ConfigurationSettings.AppSettings["siteid"].ToString().Trim());
                        objCaseRecord.supplierCallRef = dv[0]["suppliercallRef"].ToString().Trim();
                        if (ds.Tables[0].Rows[0]["skillsetid"].ToString().Trim() == "3")
                        {
                            objCaseRecord.chSkillsetId = Convert.ToInt16(dv[0]["skillsetid"].ToString().Trim());
                            objCaseRecord.chCallStartTime = Convert.ToDateTime(dv[0]["callstarttime"].ToString().Trim()).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.chCallEndTime = Convert.ToDateTime(dv[0]["DispoTime"]).AddSeconds(4).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.chDispoTime = Convert.ToDateTime(dv[0]["DispoTime"]).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.chTriageDispoCode = dv[0]["TriageDispositionCode"].ToString().Trim();
                            objCaseRecord.chFinalDispoCode = dv[0]["FinalDispositionCode"].ToString().Trim();
                            objCaseRecord.chUserCode = dv[0]["UserID"].ToString().Trim();
                            objCaseRecord.chTriageStartTime = Convert.ToDateTime(dv[0]["timein"].ToString().Trim()).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.chTriageEndTime = Convert.ToDateTime(dv[0]["DispoTime"]).AddSeconds(2).ToString("yyyy-MM-ddTHH:mm:ssK");

                        }
                        else
                        {
                            objCaseRecord.clSkillsetId = Convert.ToInt16(dv[0]["skillsetid"].ToString().Trim());
                            objCaseRecord.clCallStartTime = Convert.ToDateTime(dv[0]["callstarttime"].ToString().Trim()).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.clCallEndTime = Convert.ToDateTime(dv[0]["DispoTime"]).AddSeconds(4).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.clDispoTime = Convert.ToDateTime(dv[0]["DispoTime"]).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.clTriageDispoCode = dv[0]["TriageDispositionCode"].ToString().Trim();
                            objCaseRecord.clFinalDispoCode = dv[0]["FinalDispositionCode"].ToString().Trim();
                            objCaseRecord.clUserCode = dv[0]["UserID"].ToString().Trim();
                            objCaseRecord.clTriageStartTime = Convert.ToDateTime(dv[0]["timein"].ToString().Trim()).ToString("yyyy-MM-ddTHH:mm:ssK");
                            objCaseRecord.clTriageEndTime = Convert.ToDateTime(dv[0]["DispoTime"]).AddSeconds(2).ToString("yyyy-MM-ddTHH:mm:ssK");


                            //objCaseRecord.clSkillsetId = Convert.ToInt16(dv[0]["skillsetid"].ToString().Trim());
                            //objCaseRecord.clCallStartTime = Convert.ToDateTime(dv[0]["callstarttime"].ToString().Trim()).ToString("yyyy-MM-ddTHH:mm:ssK");
                            //objCaseRecord.clCallEndTime = Convert.ToDateTime(dv[0]["callendtime"]).ToString("yyyy-MM-ddTHH:mm:ssK");
                            //objCaseRecord.clDispoTime = Convert.ToDateTime(dv[0]["DispoTime"]).ToString("yyyy-MM-ddTHH:mm:ssK");
                            //objCaseRecord.clTriageDispoCode = dv[0]["TriageDispositionCode"].ToString().Trim();
                            //objCaseRecord.clFinalDispoCode = dv[0]["FinalDispositionCode"].ToString().Trim();
                            //objCaseRecord.clUserCode = dv[0]["UserID"].ToString().Trim();
                            //objCaseRecord.clTriageStartTime = Convert.ToDateTime(dv[0]["timein"]).AddMilliseconds(-10).ToString("yyyy-MM-ddTHH:mm:ssK");
                            //objCaseRecord.clTriageEndTime = Convert.ToDateTime(dv[questionscount - 1]["timeout"]).AddMilliseconds(10).ToString("yyyy-MM-ddTHH:mm:ssK");
                        }
                      //  objCaseRecord.chSkillsetId = Convert.ToInt16(ds.Tables[0].Rows[0]["skillsetid"].ToString().Trim());
                        //objCaseRecord.chUserCode = ds.Tables[0].Rows[0]["userID"].ToString().Trim();
                        //objCaseRecord.clSkillsetId = Convert.ToInt16(ds.Tables[0].Rows[0]["skillsetid"].ToString().Trim());
                        objCaseRecord.ageGroupId = Convert.ToInt16(dv[0]["ageGroupID"].ToString().Trim());
                        objCaseRecord.genderId = Convert.ToInt16(dv[0]["GenderID"].ToString().Trim());
                        objCaseRecord.partyId = Convert.ToInt16(dv[0]["PartyID"].ToString().Trim());
                        objCaseRecord.releaseVersion = dv[0]["releaseVersion"].ToString().Trim();
                        objCaseRecord.postcode = dv[0]["postCode"].ToString().Trim();


                        List<clsTriageItem> lsttriageitems = new List<clsTriageItem>();
                        for (int i = 0; i < dv.Count; i++)
                        {
                            lsttriageitems.Add(new clsTriageItem
                            {
                                userCode = dv[i]["UserID"].ToString().Trim(),
                                timeIn = Convert.ToDateTime(dv[i]["timeIn"]).ToString("yyyy-MM-ddTHH:mm:ssK").Trim(),
                                timeOut = Convert.ToDateTime(dv[i]["timeOut"]).ToString("yyyy-MM-ddTHH:mm:ssK").Trim(),
                                pwId = dv[i]["PWID"].ToString().Trim(),
                                orderNo = Convert.ToInt32(dv[i]["OrderNo"].ToString().Trim()),
                                quID = dv[i]["QuID"].ToString().Trim(),
                                answerNo = Convert.ToInt16(dv[i]["AnswerNo"].ToString().Trim()),
                                userComment = dv[i]["UserComment"].ToString().Trim(),
                                actionId = Convert.ToInt16(dv[i]["ActionID"].ToString().Trim())

                            });
                        }
                        objCaseRecord.listOfTriageItems = lsttriageitems;
                        var json = new JavaScriptSerializer().Serialize(objCaseRecord);
                        //Boolean result = await Posttoserver(objCaseRecord, Convert.ToInt32(myResult[j].ToString().Trim()));
                         objtaskss[j]= Posttoserver(objCaseRecord,Convert.ToInt32(myResult[j].ToString().Trim()));
                        //if (result)
                        //{
                        //    UpdatelocalDB(objCaseRecord.supplierCallRef, Convert.ToInt32(myResult[j].ToString().Trim()), createdcaseid);
                        //}

                    }
                    await Task.WhenAll(objtaskss);
                    ErrorLogger.WriteActivity("", "", "Operation finished");

                    //var json = new JavaScriptSerializer().Serialize(lstpathwayscaseRecords);
                    //  System.IO.File.WriteAllText(@"C:\testFile.txt", json);
                    // Console.WriteLine(json);
                }
                else
                {
                    ErrorLogger.WriteActivity("UploadReportService", "SerializePathWaysData", "No new Record Found");
                }
                
            }
            catch (Exception ee)
            {
                ErrorLogger.WriteError("UploadReportService", "SerializePathwaysData", ee.ToString());
            }


        }
        private static void RemoveTimezoneForDataSet(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn dc in dt.Columns)
                {

                    if (dc.DataType == typeof(DateTime))
                    {
                        dc.DateTimeMode = DataSetDateTime.Unspecified;
                    }
                }
            }
        }
        private static void UpdatelocalDB(string suppliercallref, int callid,string pathwaysreturnedid)
        {
            try
            {
                clsCall objCall = new clsCall(System.Configuration.ConfigurationSettings.AppSettings["WsBaseURL"].ToString().Trim());
                objCall.InsertUploadedPathwaysReportData(suppliercallref, callid, pathwaysreturnedid);
            }
            catch (Exception ee)
            {
                ErrorLogger.WriteError("UploadReportService", "UpdatelocalDB", ee.ToString());
            }
        }
        private static async Task<Boolean> Posttoserver(clsPathWaysCaseRecord caserecord,Int32 CadCaseID)
        {
            try
            {
                HttpClientHandler httpClientHandler = null;
                if (System.Configuration.ConfigurationSettings.AppSettings["IsNetworkProxyEnabled"].ToString().Trim().ToLower() == "true")
                {
                     httpClientHandler = new HttpClientHandler
                            {
                                Proxy = new WebProxy(System.Configuration.ConfigurationSettings.AppSettings["proxyServerAddress"].ToString().Trim(), true),
                                UseProxy = true
                            };

                }
                else
                {
                    httpClientHandler = new HttpClientHandler();
                }
                using (var client = new HttpClient(httpClientHandler))
                {
                   
                    client.BaseAddress = new Uri(System.Configuration.ConfigurationSettings.AppSettings["ServiceBaseUri"].ToString().Trim());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", System.Configuration.ConfigurationSettings.AppSettings["user"].ToString().Trim(), System.Configuration.ConfigurationSettings.AppSettings["pass"].ToString().Trim()))));
                   // client.DefaultRequestHeaders.Add("Authorization", System.Configuration.ConfigurationSettings.AppSettings["Authorization"].ToString().Trim());
                    //  client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                   // var jsonstring=JavaScriptSe
                    HttpResponseMessage response = await client.PostAsJsonAsync(System.Configuration.ConfigurationSettings.AppSettings["RequestUri"].ToString().Trim(), caserecord);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        createdcaseid = await response.Content.ReadAsStringAsync();
                        UpdatelocalDB(caserecord.supplierCallRef, CadCaseID, createdcaseid);
                        return true;
                    }
                    else
                    {
                        ErrorLogger.WriteActivity("UploadReportService", "PostToService",await response.Content.ReadAsStringAsync());
                        return false;

                    }

                }
            }

            catch (Exception ee)
            {
                ErrorLogger.WriteError("UploadReportService", "PostToService", ee.ToString());
                return false;
            }



        }


        protected override void OnStop()
        {
            ErrorLogger.WriteActivity("UploadReportService", "OnStop", "Service Stopped.");
        }
    }
}
