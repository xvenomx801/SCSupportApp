﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SCSupportApp.Controllers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace SCSupportApp
{
    public partial class initiate : System.Web.UI.Page
    {
        public static int siteId;
        public string siteIdString;
        public static string apiSecret = "";
        public static int accountantId;
        public string accountantIDString = "";
        public static string token = "";
        public static string returnAuthToken = "";
        public bool alpha = true;
        public static bool tokenValid = false;
        private static string partnerAPIToken { get; set; } = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            getAccrualSchema.Enabled = tokenValid;

            if (!Page.IsPostBack)
            {
                loadLabel.Text = token;
            }
            else
            {
                if (type.SelectedValue == "1") Partner.Enabled = false;
                siteIdString = Site.Text;
                siteId = toNumber(siteIdString);
                apiSecret = Secret.Text;
                sendTo.Click += new EventHandler(this.sendTo_Click);

            }

            #region create click button event handlers

            getAccrualSchema.Click += new EventHandler(getAccrualSchema_Click);
            //getAccrualBalance.Click += new EventHandler(this.getAccrualBalance_Click);
            //getAccrualActivity.Click += new EventHandler(this.getAccrualActivity_Click);
            //postUpdateAccrual.Click += new EventHandler(this.postUpdateAccrual_Click);

            getEmployeesSchema.Click += new EventHandler(this.getEmployeesSchema_Click);
            getEmployees.Click += new EventHandler(this.getEmployees_Click);
            postUpdateEmployees.Click += new EventHandler(this.postUpdateEmployees_Click);
            postEmployeeConnectMgrLogin.Click += new EventHandler(this.postEmployeeConnectMgrLogin_Click);
            postEmployeeDisconnectMangerLogin.Click += new EventHandler(this.postEmployeeDisconnectMangerLogin_Click);
            postEmployeeSetPassword.Click += new EventHandler(this.postEmployeeSetPassword_Click);
            postEmployeeResetPassword.Click += new EventHandler(this.postEmployeeResetPassword_Click);
            postEmployeeUpdatePassword.Click += new EventHandler(this.postEmployeeUpdatePassword_Click);

            getLogins.Click += new EventHandler(this.getLogins_Click);

            getPayrollActivities.Click += new EventHandler(this.getPayrollActivities_Click);
            getPayrollActivitiesPerPayPeriod.Click += new EventHandler(this.getPayrollActivitiesPerPayPeriod_Click);
            getPayrollFormats.Click += new EventHandler(this.getPayrollFormats_Click);

            getRules.Click += new EventHandler(this.getRules_Click);
            getIntegratedSchedulingRule.Click += new EventHandler(this.getIntegratedSchedulingRule_Click);
            deleteExtEmployeeIDRule.Click += new EventHandler(this.deleteExtEmployeeIDRule_Click);
            deleteIntegrationFieldsRule.Click += new EventHandler(this.deleteIntegrationFieldsRule_Click);
            postExtEmployeeIDRule.Click += new EventHandler(this.postExtEmployeeIDRule_Click);
            postIntegratedSchedulingRule.Click += new EventHandler(this.postIntegratedSchedulingRule_Click);
            postIntegrationRule.Click += new EventHandler(this.postIntegrationRule_Click);

            getTimeWorksPlusSchedules.Click += new EventHandler(this.getTimeWorksPlusSchedules_Click);

            getTimeCards.Click += new EventHandler(this.getTimeCards_Click);
            getTimeCardSummary.Click += new EventHandler(this.getTimeCardSummary_Click);
            deleteTimeCardLine.Click += new EventHandler(this.deleteTimeCardLine_Click);
            postEditTimeCardLine.Click += new EventHandler(this.postEditTimeCardLine_Click);
            postTimeCardApproval.Click += new EventHandler(this.postTimeCardApproval_Click);
            postAddTimeCardNote.Click += new EventHandler(this.postAddTimeCardNote_Click);
            postAddTimeCardPunch.Click += new EventHandler(this.postAddTimeCardPunch_Click);

            getTimeOffRequestsbyEEID.Click += new EventHandler(this.getTimeOffRequestsbyEEID_Click);
            getTimeOffRequestCategories.Click += new EventHandler(this.getTimeOffRequestCategories_Click);
            getTimeOffRequestbyEEIDdept.Click += new EventHandler(this.getTimeOffRequestbyEEIDdept_Click);
            getSchemaTimeOffRequests.Click += new EventHandler(this.getSchemaTimeOffRequests_Click);
            getSupervisorTimeOffRequests.Click += new EventHandler(this.getSupervisorTimeOffRequests_Click);
            postCreateTimeOffRequest.Click += new EventHandler(this.postCreateTimeOffRequest_Click);
            postAcceptTimeOffRequest.Click += new EventHandler(this.postAcceptTimeOffRequest_Click);
            postApproveTimeOffRequest.Click += new EventHandler(this.postApproveTimeOffRequest_Click);
            postCancelTimeOffRequest.Click += new EventHandler(this.postCancelTimeOffRequest_Click);
            postRejectTimeOffRequest.Click += new EventHandler(this.postRejectTimeOffRequest_Click);
            postUnApproveTimeOffRequest.Click += new EventHandler(this.postUnApproveTimeOffRequest_Click);

            #endregion
        }

        public static int toNumber(string str, int defaultValue = 0)
        {
            int result;
            return Int32.TryParse(str, out result) ? result : defaultValue;
        }

        protected async void sendTo_Click(object sender, EventArgs e)
        {
            siteIdString = Site.Text;
            siteId = toNumber(siteIdString);
            apiSecret = Secret.Text;
            bool partner = type.SelectedValue == "0";
            string partnerId = Partner.Text;
            if (server.SelectedValue == "1")
            {
                alpha = false;
            }
            returnAuthToken = await Controllers.token.PostAuth(Controllers.token.creator(siteId, apiSecret, partner, partnerId), alpha);

            if (!string.IsNullOrEmpty(returnAuthToken)) tokenValid = true;
            getAccrualSchema.Enabled = tokenValid;
        }

        private void catButtonHandle(object sender, EventArgs e)
        {
            var update = new Button();
            update.Click += new EventHandler(updateCat_Click);
            update.Text = $"Update Balance";
            modal_panel.Controls.Add(update);

            var getBal = new Button();
            getBal.Click += new EventHandler(getBal_Click);
            getBal.Text = $"Get Balance";
            modal_panel.Controls.Add(getBal);

            var getActivity = new Button();
            getActivity.Click += new EventHandler(getAct_Click);
            getActivity.Text = $"Activity";
            modal_panel.Controls.Add(getActivity);
        }

        private void getAct_Click(object sender, EventArgs e)
        {

        }

        private void getBal_Click(object sender, EventArgs e)
        {

        }

        private void updateCat_Click(object sender, EventArgs e)
        {

        }

        private void Load_Items(object sender, CommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
        }

        public static async Task AuthorizeAPI()
        {
            if (partnerAPIToken == null)
            {
                partnerAPIToken = await TWPSDK.GetJWTToken(apiSecret, accountantId, siteId, APIProduct.TWP_Partner);

                if(!String.IsNullOrEmpty(partnerAPIToken))
                {
                    Console.WriteLine($"Partner Authorization succeeded.");
                }
            }
        }

        #region Accruals

        protected async void getAccrualSchema_Click(object sender, EventArgs e)
        {
            siteIdString = Site.Text;
            siteId = toNumber(siteIdString);
            string values = "";

            List<TWP_AccrualsSchema> accSchema = await Controllers.TWPSDK.getAccrualSchema(siteId, returnAuthToken);

            try
            {
                foreach (TWP_AccrualsSchema value in accSchema)
                {
                    var button = new Button
                    {
                        ID = value.Category,
                        Text = value.Category,
                        CommandArgument = value.ToString()
                    };
                    button.Command += Load_Items;
                    button.CssClass = "btn btn-success";
                    resultsPanel.Controls.Add(button);
                    button.Click += new EventHandler(catButtonHandle);
                }
                accrualvalues.InnerText = $"{values}";
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Validation Client Accrual Schema: an exception occurred: {ex.Message}");
            }
        }

        protected void getAccrualBalance_Click(object sender, EventArgs e)
        {
            Controllers.TWP_AccrualsSchema Schema = new TWP_AccrualsSchema()
            {
                Category = "",
                IsHidden = "",
                Effective = "",
                Expires = "",
                Id = "",
                Value = ""
            };
            try
            {
                getAccrualSchema_Click(sender, e);
            }
            catch
            {

            }

        }
        
        protected async void getAccrualActivity_Click(object sender, EventArgs e)
        {
            List<TWP_AccrualActivities> accrualActivity = await TWPSDK.getAccrualActivity(siteId, returnAuthToken);
        }

        protected void postUpdateAccrual_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Employees

        protected async void getEmployeesSchema_Click(object sender, EventArgs e)
        {
            List<TWP_Employee> eeSchema = await TWPSDK.ListEmployees(siteId, returnAuthToken);
        }

        protected async void getEmployees_Click(object sender, EventArgs e)
        {
            List<TWP_Employee> employeesList = await Controllers.TWPSDK.ListEmployees(siteId, returnAuthToken);
            try
            {
                var eeTable = new Table();
                TableRow tRow = new TableRow();
                TableCell nameCell = new TableCell();
                TableCell IDcell = new TableCell();

                foreach (TWP_Employee employee in employeesList)
                {
                    nameCell.Text = employee.FullName;
                    IDcell.Text = employee.EmployeeCode;
                    tRow.Cells.Add(nameCell);
                    tRow.Cells.Add(IDcell);
                    eeTable.Rows.Add(tRow);
                }
                resultsPanel.Controls.Add(eeTable);
            }
            catch
            {

            }
        }

        protected async void downloadEmployees_Click(object sender, EventArgs e)
        {

            List<TWP_Employee_Schema> eeSchema = await TWPSDK.listEESchema(siteId, returnAuthToken);

            List<TWP_Employee> employees = await TWPSDK.ListEmployees(siteId, returnAuthToken);
            StringBuilder sb = new StringBuilder();

            string header = "First Name,Middle Name,Last Name,Code,Designation,Phone,Email,Start Date,Separation Date," +
                "Export Block,WebClock Enabled,Mobile Punch Enabled,Mobile Enabled,GPS Available,";

            foreach(TWP_Employee_Schema results in eeSchema)
            {
                int idCount = 0;
                foreach(TWP_Identifier id in results.Identifiers)
                {
                    ++idCount;
                    header += $"login{idCount.ToString()},";
                }

                int statesCount = 0;
                foreach(TWP_State_Schema state in results.States)
                {
                    foreach (KeyValuePair<string, string> kvp in state.Variables)
                    {
                        ++statesCount;
                        header += $"{kvp.Key.ToString()}";
                    }
                }
            }

            sb.AppendLine(header);
            string body = "";
            foreach (TWP_Employee emp in employees)
            {
                body = $"{emp.FirstName},{emp.MiddleName},{emp.LastName},{emp.EmployeeCode},{emp.Designation},{emp.Phone},{emp.Email},{emp.StartDate},,,,,,,,,,,,,,,,";
                sb.AppendLine(body);
            }

            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

            if(bytes != null)
            {
                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Length", bytes.Length.ToString());
                Response.AddHeader("Content-disposition", "attachment; filename=\"EmployeeDetail.csv" + "\"");
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }

        protected async void bulkUpsertEmployees_Click(object sender, EventArgs e)
        {

        }

        protected void postUpdateEmployees_Click(object sender, EventArgs e)
        {

        }
        
        protected void postEmployeeConnectMgrLogin_Click(object sender, EventArgs e)
        {

        }

        protected void postEmployeeDisconnectMangerLogin_Click(object sender, EventArgs e)
        {

        }

        protected void postEmployeeSetPassword_Click(object sender, EventArgs e)
        {

        }

        protected void postEmployeeResetPassword_Click(object sender, EventArgs e)
        {

        }

        protected void postEmployeeUpdatePassword_Click(object sender, EventArgs e)
        {

        }

        #endregion

        protected async void getLogins_Click(object sender, EventArgs e)
        {
            List<JObject> logins = await TWPSDK.getLogins(siteId, returnAuthToken);
        }

        #region PayrollActivities

        protected async void getPayrollActivities_Click(object sender, EventArgs e)
        {

        }

        protected void getPayrollActivitiesPerPayPeriod_Click(object sender, EventArgs e)
        {

        }

        protected async void getPayrollFormats_Click(object sender, EventArgs e)
        {
            List<string> formats = await TWPSDK.getPayrollFormats(siteId, returnAuthToken);
            var formatLabel = new Label()
            {

            };
            resultsPanel.Controls.Add(resultsLabel);
        }

        #endregion

        #region Rules

        protected async void getRules_Click(object sender, EventArgs e)
        {
            List<TWP_Rules> rules = await TWPSDK.getRules(siteId, returnAuthToken);
        }

        protected void getIntegratedSchedulingRule_Click(object sender, EventArgs e)
        {

        }

        protected void deleteExtEmployeeIDRule_Click(object sender, EventArgs e)
        {

        }

        protected void deleteIntegrationFieldsRule_Click(object sender, EventArgs e)
        {

        }

        protected void postExtEmployeeIDRule_Click(object sender, EventArgs e)
        {

        }

        protected void postIntegratedSchedulingRule_Click(object sender, EventArgs e)
        {

        }

        protected void postIntegrationRule_Click(object sender, EventArgs e)
        {

        }

        #endregion

        protected void getTimeWorksPlusSchedules_Click(object sender, EventArgs e)
        {

        }

        #region TimeCard

        protected void getTimeCards_Click(object sender, EventArgs e)
        {

        }

        protected void getTimeCardSummary_Click(object sender, EventArgs e)
        {

        }

        protected void deleteTimeCardLine_Click(object sender, EventArgs e)
        {

        }

        protected void postEditTimeCardLine_Click(object sender, EventArgs e)
        {

        }

        protected void postTimeCardApproval_Click(object sender, EventArgs e)
        {

        }

        protected void postAddTimeCardNote_Click(object sender, EventArgs e)
        {

        }

        protected void postAddTimeCardPunch_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region TOR

        protected void getTimeOffRequestsbyEEID_Click(object sender, EventArgs e)
        {

        }

        protected void getTimeOffRequestCategories_Click(object sender, EventArgs e)
        {

        }

        protected void getTimeOffRequestbyEEIDdept_Click(object sender, EventArgs e)
        {

        }

        protected async void getSchemaTimeOffRequests_Click(object sender, EventArgs e)
        {
            List<JObject> TORs = await TWPSDK.getTORCats(siteId, returnAuthToken);

            foreach (JObject Cat in TORs)
            {
                string category = Cat.ToString();
                var label = new Label
                {
                    ID = category,
                    Text = category
                };
                resultsPanel.Controls.Add(label);
            }
        }

        protected void getSupervisorTimeOffRequests_Click(object sender, EventArgs e)
        {

        }

        protected void postCreateTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        protected void postAcceptTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        protected void postApproveTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        protected void postCancelTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        protected void postRejectTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        protected void postUnApproveTimeOffRequest_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}