using System;
using Warranty.Core.Extensions;

namespace Warranty.LotusExtract
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Columns;

    public class ServiceCallDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        public void Import(string fileName, string marketList, string archivedFileName = null)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.ServiceCallImports", sc))
                    cmd.ExecuteNonQuery();

                using (var file = new StreamReader(fileName))
                {
                    var columnCount = GetColumnIndexes(file);

                    var row = "";
                    string line;

                    while ((line = file.ReadLine()) != null)
                    {

                        line = line
                            .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString())
                            .TrimStart('"')
                            .TrimEnd('"');

                        row += line;
                        if (row.Split(_fieldDelimiter).Count() >= columnCount)
                        {
                            var items = row
                                .Split(_fieldDelimiter)
                                .ToList();

                            var sql = @"INSERT INTO imports.ServiceCallImports (JobAddress, Area, AreaPresCode, Assigned_By, Assigned_To, BldNo, Breaks, Builder, BuilderEmp_Num, CallCompnotification, Call_Comments, Call_Num, Cause_1, Cause_10, Cause_2, Cause_3, Cause_4, Cause_5, Cause_6, Cause_7, Cause_8, Cause_9, CDate_1, CDate_10, CDate_2, CDate_3, CDate_4, CDate_5, CDate_6, CDate_7, CDate_8, CDate_9, City, CityCode, CityCode_Short, Close_Date, CommNum, CommNum_Long, Community, Comp_Date, Contact, Date_C, Date_Conv, Date_Conv_1, Date_O, Date_Open, Descript_1, Descript_10, Descript_2, Descript_3, Descript_4, Descript_5, Descript_6, Descript_7, Descript_8, Descript_9, DisplayAuthor, DivCode, Division, DocAuthor, Duration, Duration_1, Duration_Pending, Duration_Pending_1, DurUnder7, Elevation, EmailContact, Fax_Num, GetCloseDate, Hbr5Close, Homeowner, Home_Phone, HOSig, Item1, Item10, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Items, Items_Complete, Job_Num, Last_Modified, LegalDesc, lookupserver, ModifiedBy, MonthComp, MonthQtr, MonthSort, Notification_0, Notification_1, Notification_2, Notification_Call_Items, Notification_Homeowner, NumCalls, Orig_CompDate, Other_Phone, OwnNum, PCode_1, PCode_10, PCode_2, PCode_3, PCode_4, PCode_5, PCode_6, PCode_7, PCode_8, PCode_9, JobPlan, PlanType, PlanTypeDesc, Plan_Name, Printed_Name, Project, ProjectCode, RequestCalc, ResCode_1, ResCode_10, ResCode_10_1, ResCode_1_1, ResCode_2, ResCode_2_1, ResCode_3, ResCode_3_1, ResCode_4, ResCode_4_1, ResCode_5, ResCode_5_1, ResCode_6, ResCode_6_1, ResCode_7, ResCode_7_1, ResCode_8, ResCode_8_1, ResCode_9, ResCode_9_1, Root_1, Root_10, Root_10_1, Root_1_1, Root_2, Root_2_1, Root_3, Root_3_1, Root_4, Root_4_1, Root_5, Root_5_1, Root_6, Root_6_1, Root_7, Root_7_1, Root_8, Root_8_1, Root_9, Root_9_1, SalesEmp_Num, Sales_Consultant, SateliteCode, Select2YearComp, Server_Name, StateCode, Swing, CallType, unid, UnitNo, User_Defined, VendorCode_1, VendorCode_10, VendorCode_2, VendorCode_3, VendorCode_4, VendorCode_5, VendorCode_6, VendorCode_7, VendorCode_8, VendorCode_9, VendorName_1, VendorName_10, VendorName_2, VendorName_3, VendorName_4, VendorName_5, VendorName_6, VendorName_7, VendorName_8, VendorName_9, VendorPhoneNum_1, VendorPhoneNum_10, VendorPhoneNum_2, VendorPhoneNum_3, VendorPhoneNum_4, VendorPhoneNum_5, VendorPhoneNum_6, VendorPhoneNum_7, VendorPhoneNum_8, VendorPhoneNum_9, WorkPhone_1, WorkPhone_2, WSFU, WSFU_1, WsrEmp_Num, YearComp, ZipCode, ReturnValue)
                                        SELECT @JobAddress, @Area, @AreaPresCode, @Assigned_By, @Assigned_To, @BldNo, @Breaks, @Builder, @BuilderEmp_Num, @CallCompnotification, @Call_Comments, @Call_Num, @Cause_1, @Cause_10, @Cause_2, @Cause_3, @Cause_4, @Cause_5, @Cause_6, @Cause_7, @Cause_8, @Cause_9, @CDate_1, @CDate_10, @CDate_2, @CDate_3, @CDate_4, @CDate_5, @CDate_6, @CDate_7, @CDate_8, @CDate_9, @City, @CityCode, @CityCode_Short, @Close_Date, @CommNum, @CommNum_Long, @Community, @Comp_Date, @Contact, @Date_C, @Date_Conv, @Date_Conv_1, @Date_O, @Date_Open, @Descript_1, @Descript_10, @Descript_2, @Descript_3, @Descript_4, @Descript_5, @Descript_6, @Descript_7, @Descript_8, @Descript_9, @DisplayAuthor, @DivCode, @Division, @DocAuthor, @Duration, @Duration_1, @Duration_Pending, @Duration_Pending_1, @DurUnder7, @Elevation, @EmailContact, @Fax_Num, @GetCloseDate, @Hbr5Close, @Homeowner, @Home_Phone, @HOSig, @Item1, @Item10, @Item2, @Item3, @Item4, @Item5, @Item6, @Item7, @Item8, @Item9, @Items, @Items_Complete, @Job_Num, @Last_Modified, @LegalDesc, @lookupserver, @ModifiedBy, @MonthComp, @MonthQtr, @MonthSort, @Notification_0, @Notification_1, @Notification_2, @Notification_Call_Items, @Notification_Homeowner, @NumCalls, @Orig_CompDate, @Other_Phone, @OwnNum, @PCode_1, @PCode_10, @PCode_2, @PCode_3, @PCode_4, @PCode_5, @PCode_6, @PCode_7, @PCode_8, @PCode_9, @JobPlan, @PlanType, @PlanTypeDesc, @Plan_Name, @Printed_Name, @Project, @ProjectCode, @RequestCalc, @ResCode_1, @ResCode_10, @ResCode_10_1, @ResCode_1_1, @ResCode_2, @ResCode_2_1, @ResCode_3, @ResCode_3_1, @ResCode_4, @ResCode_4_1, @ResCode_5, @ResCode_5_1, @ResCode_6, @ResCode_6_1, @ResCode_7, @ResCode_7_1, @ResCode_8, @ResCode_8_1, @ResCode_9, @ResCode_9_1, @Root_1, @Root_10, @Root_10_1, @Root_1_1, @Root_2, @Root_2_1, @Root_3, @Root_3_1, @Root_4, @Root_4_1, @Root_5, @Root_5_1, @Root_6, @Root_6_1, @Root_7, @Root_7_1, @Root_8, @Root_8_1, @Root_9, @Root_9_1, @SalesEmp_Num, @Sales_Consultant, @SateliteCode, @Select2YearComp, @Server_Name, @StateCode, @Swing, @TypeOfSomething, @unid, @UnitNo, @User_Defined, @VendorCode_1, @VendorCode_10, @VendorCode_2, @VendorCode_3, @VendorCode_4, @VendorCode_5, @VendorCode_6, @VendorCode_7, @VendorCode_8, @VendorCode_9, @VendorName_1, @VendorName_10, @VendorName_2, @VendorName_3, @VendorName_4, @VendorName_5, @VendorName_6, @VendorName_7, @VendorName_8, @VendorName_9, @VendorPhoneNum_1, @VendorPhoneNum_10, @VendorPhoneNum_2, @VendorPhoneNum_3, @VendorPhoneNum_4, @VendorPhoneNum_5, @VendorPhoneNum_6, @VendorPhoneNum_7, @VendorPhoneNum_8, @VendorPhoneNum_9, @WorkPhone_1, @WorkPhone_2, @WSFU, @WSFU_1, @WsrEmp_Num, @YearComp, @ZipCode, @ReturnValue";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@JobAddress", items[ServiceCallColumns.Address].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Area", items[ServiceCallColumns.Area].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresCode", items[ServiceCallColumns.AreaPresCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Assigned_By", items[ServiceCallColumns.Assigned_By].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Assigned_To", items[ServiceCallColumns.Assigned_To].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@BldNo", items[ServiceCallColumns.BldNo].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Breaks", items[ServiceCallColumns.Breaks].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Builder", items[ServiceCallColumns.Builder].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@BuilderEmp_Num", items[ServiceCallColumns.BuilderEmp_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CallCompnotification", items[ServiceCallColumns.CallCompnotification].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Call_Comments", items[ServiceCallColumns.Call_Comments].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Call_Num", items[ServiceCallColumns.Call_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_1", items[ServiceCallColumns.Cause_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_10", items[ServiceCallColumns.Cause_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_2", items[ServiceCallColumns.Cause_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_3", items[ServiceCallColumns.Cause_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_4", items[ServiceCallColumns.Cause_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_5", items[ServiceCallColumns.Cause_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_6", items[ServiceCallColumns.Cause_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_7", items[ServiceCallColumns.Cause_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_8", items[ServiceCallColumns.Cause_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Cause_9", items[ServiceCallColumns.Cause_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_1", items[ServiceCallColumns.CDate_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_10", items[ServiceCallColumns.CDate_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_2", items[ServiceCallColumns.CDate_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_3", items[ServiceCallColumns.CDate_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_4", items[ServiceCallColumns.CDate_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_5", items[ServiceCallColumns.CDate_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_6", items[ServiceCallColumns.CDate_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_7", items[ServiceCallColumns.CDate_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_8", items[ServiceCallColumns.CDate_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CDate_9", items[ServiceCallColumns.CDate_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@City", items[ServiceCallColumns.City].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CityCode", items[ServiceCallColumns.CityCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CityCode_Short", items[ServiceCallColumns.CityCode_Short].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Close_Date", items[ServiceCallColumns.Close_Date].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CommNum", items[ServiceCallColumns.CommNum].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CommNum_Long", items[ServiceCallColumns.CommNum_Long].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Community", items[ServiceCallColumns.Community].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Comp_Date", items[ServiceCallColumns.Comp_Date].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Contact", items[ServiceCallColumns.Contact].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Date_C", items[ServiceCallColumns.Date_C].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Date_Conv", items[ServiceCallColumns.Date_Conv].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Date_Conv_1", items[ServiceCallColumns.Date_Conv_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Date_O", items[ServiceCallColumns.Date_O].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Date_Open", items[ServiceCallColumns.Date_Open].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_1", items[ServiceCallColumns.Descript_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_10", items[ServiceCallColumns.Descript_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_2", items[ServiceCallColumns.Descript_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_3", items[ServiceCallColumns.Descript_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_4", items[ServiceCallColumns.Descript_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_5", items[ServiceCallColumns.Descript_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_6", items[ServiceCallColumns.Descript_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_7", items[ServiceCallColumns.Descript_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_8", items[ServiceCallColumns.Descript_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Descript_9", items[ServiceCallColumns.Descript_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DisplayAuthor", items[ServiceCallColumns.DisplayAuthor].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DivCode", items[ServiceCallColumns.DivCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Division", items[ServiceCallColumns.Division].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DocAuthor", items[ServiceCallColumns.DocAuthor].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Duration", items[ServiceCallColumns.Duration].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Duration_1", items[ServiceCallColumns.Duration_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Duration_Pending", items[ServiceCallColumns.Duration_Pending].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Duration_Pending_1", items[ServiceCallColumns.Duration_Pending_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DurUnder7", items[ServiceCallColumns.DurUnder7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Elevation", items[ServiceCallColumns.Elevation].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@EmailContact", items[ServiceCallColumns.EmailContact].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Fax_Num", items[ServiceCallColumns.Fax_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@GetCloseDate", items[ServiceCallColumns.GetCloseDate].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Hbr5Close", items[ServiceCallColumns.Hbr5Close].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Homeowner", items[ServiceCallColumns.Homeowner].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Home_Phone", items[ServiceCallColumns.Home_Phone].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@HOSig", items[ServiceCallColumns.HOSig].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item1", items[ServiceCallColumns.Item1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item10", items[ServiceCallColumns.Item10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item2", items[ServiceCallColumns.Item2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item3", items[ServiceCallColumns.Item3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item4", items[ServiceCallColumns.Item4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item5", items[ServiceCallColumns.Item5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item6", items[ServiceCallColumns.Item6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item7", items[ServiceCallColumns.Item7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item8", items[ServiceCallColumns.Item8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Item9", items[ServiceCallColumns.Item9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Items", items[ServiceCallColumns.Items].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Items_Complete", items[ServiceCallColumns.Items_Complete].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Job_Num", items[ServiceCallColumns.Job_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Last_Modified", items[ServiceCallColumns.Last_Modified].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@LegalDesc", items[ServiceCallColumns.LegalDesc].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@lookupserver", items[ServiceCallColumns.lookupserver].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ModifiedBy", items[ServiceCallColumns.ModifiedBy].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@MonthComp", items[ServiceCallColumns.MonthComp].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@MonthQtr", items[ServiceCallColumns.MonthQtr].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@MonthSort", items[ServiceCallColumns.MonthSort].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Notification_0", items[ServiceCallColumns.Notification].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Notification_1", items[ServiceCallColumns.Notification_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Notification_2", items[ServiceCallColumns.Notification_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Notification_Call_Items", items[ServiceCallColumns.Notification_Call_Items].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Notification_Homeowner", items[ServiceCallColumns.Notification_Homeowner].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@NumCalls", items[ServiceCallColumns.NumCalls].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Orig_CompDate", items[ServiceCallColumns.Orig_CompDate].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Other_Phone", items[ServiceCallColumns.Other_Phone].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@OwnNum", items[ServiceCallColumns.OwnNum].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_1", items[ServiceCallColumns.PCode_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_10", items[ServiceCallColumns.PCode_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_2", items[ServiceCallColumns.PCode_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_3", items[ServiceCallColumns.PCode_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_4", items[ServiceCallColumns.PCode_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_5", items[ServiceCallColumns.PCode_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_6", items[ServiceCallColumns.PCode_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_7", items[ServiceCallColumns.PCode_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_8", items[ServiceCallColumns.PCode_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PCode_9", items[ServiceCallColumns.PCode_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@JobPlan", items[ServiceCallColumns.Plan].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PlanType", items[ServiceCallColumns.PlanType].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PlanTypeDesc", items[ServiceCallColumns.PlanTypeDesc].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Plan_Name", items[ServiceCallColumns.Plan_Name].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Printed_Name", items[ServiceCallColumns.Printed_Name].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Project", items[ServiceCallColumns.Project].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ProjectCode", items[ServiceCallColumns.ProjectCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@RequestCalc", items[ServiceCallColumns.RequestCalc].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_1", items[ServiceCallColumns.ResCode_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_10", items[ServiceCallColumns.ResCode_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_10_1", items[ServiceCallColumns.ResCode_10_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_1_1", items[ServiceCallColumns.ResCode_1_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_2", items[ServiceCallColumns.ResCode_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_2_1", items[ServiceCallColumns.ResCode_2_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_3", items[ServiceCallColumns.ResCode_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_3_1", items[ServiceCallColumns.ResCode_3_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_4", items[ServiceCallColumns.ResCode_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_4_1", items[ServiceCallColumns.ResCode_4_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_5", items[ServiceCallColumns.ResCode_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_5_1", items[ServiceCallColumns.ResCode_5_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_6", items[ServiceCallColumns.ResCode_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_6_1", items[ServiceCallColumns.ResCode_6_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_7", items[ServiceCallColumns.ResCode_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_7_1", items[ServiceCallColumns.ResCode_7_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_8", items[ServiceCallColumns.ResCode_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_8_1", items[ServiceCallColumns.ResCode_8_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_9", items[ServiceCallColumns.ResCode_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_9_1", items[ServiceCallColumns.ResCode_9_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_1", items[ServiceCallColumns.Root_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_10", items[ServiceCallColumns.Root_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_10_1", items[ServiceCallColumns.Root_10_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_1_1", items[ServiceCallColumns.Root_1_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_2", items[ServiceCallColumns.Root_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_2_1", items[ServiceCallColumns.Root_2_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_3", items[ServiceCallColumns.Root_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_3_1", items[ServiceCallColumns.Root_3_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_4", items[ServiceCallColumns.Root_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_4_1", items[ServiceCallColumns.Root_4_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_5", items[ServiceCallColumns.Root_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_5_1", items[ServiceCallColumns.Root_5_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_6", items[ServiceCallColumns.Root_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_6_1", items[ServiceCallColumns.Root_6_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_7", items[ServiceCallColumns.Root_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_7_1", items[ServiceCallColumns.Root_7_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_8", items[ServiceCallColumns.Root_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_8_1", items[ServiceCallColumns.Root_8_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_9", items[ServiceCallColumns.Root_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Root_9_1", items[ServiceCallColumns.Root_9_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SalesEmp_Num", items[ServiceCallColumns.SalesEmp_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Sales_Consultant", items[ServiceCallColumns.Sales_Consultant].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SateliteCode", items[ServiceCallColumns.SateliteCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Select2YearComp", items[ServiceCallColumns.Select2YearComp].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Server_Name", items[ServiceCallColumns.Server_Name].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@StateCode", items[ServiceCallColumns.State].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Swing", items[ServiceCallColumns.Swing].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@TypeOfSomething", items[ServiceCallColumns.Type].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@unid", items[ServiceCallColumns.unid].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@UnitNo", items[ServiceCallColumns.UnitNo].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@User_Defined", items[ServiceCallColumns.User_Defined].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_1", items[ServiceCallColumns.VendorCode_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_10", items[ServiceCallColumns.VendorCode_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_2", items[ServiceCallColumns.VendorCode_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_3", items[ServiceCallColumns.VendorCode_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_4", items[ServiceCallColumns.VendorCode_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_5", items[ServiceCallColumns.VendorCode_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_6", items[ServiceCallColumns.VendorCode_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_7", items[ServiceCallColumns.VendorCode_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_8", items[ServiceCallColumns.VendorCode_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_9", items[ServiceCallColumns.VendorCode_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_1", items[ServiceCallColumns.VendorName_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_10", items[ServiceCallColumns.VendorName_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_2", items[ServiceCallColumns.VendorName_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_3", items[ServiceCallColumns.VendorName_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_4", items[ServiceCallColumns.VendorName_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_5", items[ServiceCallColumns.VendorName_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_6", items[ServiceCallColumns.VendorName_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_7", items[ServiceCallColumns.VendorName_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_8", items[ServiceCallColumns.VendorName_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_9", items[ServiceCallColumns.VendorName_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_1", items[ServiceCallColumns.VendorPhoneNum_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_10", items[ServiceCallColumns.VendorPhoneNum_10].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_2", items[ServiceCallColumns.VendorPhoneNum_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_3", items[ServiceCallColumns.VendorPhoneNum_3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_4", items[ServiceCallColumns.VendorPhoneNum_4].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_5", items[ServiceCallColumns.VendorPhoneNum_5].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_6", items[ServiceCallColumns.VendorPhoneNum_6].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_7", items[ServiceCallColumns.VendorPhoneNum_7].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_8", items[ServiceCallColumns.VendorPhoneNum_8].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_9", items[ServiceCallColumns.VendorPhoneNum_9].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone_1", items[ServiceCallColumns.WorkPhone_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone_2", items[ServiceCallColumns.WorkPhone_2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WSFU", items[ServiceCallColumns.WSFU].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WSFU_1", items[ServiceCallColumns.WSFU_1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WsrEmp_Num", items[ServiceCallColumns.WsrEmp_Num].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@YearComp", items[ServiceCallColumns.YearComp].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ZipCode", items[ServiceCallColumns.ZipCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ReturnValue", items[ServiceCallColumns.Return].MaxLength(4000)));

                                cmd.ExecuteNonQuery();
                            }

                            row = "";
                        }
                        else
                        {
                            row += "\r\n";
                        }
                    }
                }

                if (!archivedFileName.IsNullOrEmpty() && File.Exists(archivedFileName))
                {
                    using (var file = new StreamReader(archivedFileName))
                    {
                        var columnCount = GetColumnIndexes(file);

                        var row = "";
                        string line;

                        while ((line = file.ReadLine()) != null)
                        {

                            line = line
                                .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString())
                                .TrimStart('"')
                                .TrimEnd('"');

                            row += line;
                            if (row.Split(_fieldDelimiter).Count() >= columnCount)
                            {
                                var items = row
                                    .Split(_fieldDelimiter)
                                    .ToList();

                                var sql =
                                    @"INSERT INTO imports.ServiceCallImports (JobAddress, Area, AreaPresCode, Assigned_By, Assigned_To, BldNo, Breaks, Builder, BuilderEmp_Num, CallCompnotification, Call_Comments, Call_Num, Cause_1, Cause_10, Cause_2, Cause_3, Cause_4, Cause_5, Cause_6, Cause_7, Cause_8, Cause_9, CDate_1, CDate_10, CDate_2, CDate_3, CDate_4, CDate_5, CDate_6, CDate_7, CDate_8, CDate_9, City, CityCode, CityCode_Short, Close_Date, CommNum, CommNum_Long, Community, Comp_Date, Contact, Date_C, Date_Conv, Date_Conv_1, Date_O, Date_Open, Descript_1, Descript_10, Descript_2, Descript_3, Descript_4, Descript_5, Descript_6, Descript_7, Descript_8, Descript_9, DisplayAuthor, DivCode, Division, DocAuthor, Duration, Duration_1, Duration_Pending, Duration_Pending_1, DurUnder7, Elevation, EmailContact, Fax_Num, GetCloseDate, Hbr5Close, Homeowner, Home_Phone, HOSig, Item1, Item10, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Items, Items_Complete, Job_Num, Last_Modified, LegalDesc, lookupserver, ModifiedBy, MonthComp, MonthQtr, MonthSort, Notification_0, Notification_1, Notification_2, Notification_Call_Items, Notification_Homeowner, NumCalls, Orig_CompDate, Other_Phone, OwnNum, PCode_1, PCode_10, PCode_2, PCode_3, PCode_4, PCode_5, PCode_6, PCode_7, PCode_8, PCode_9, JobPlan, PlanType, PlanTypeDesc, Plan_Name, Printed_Name, Project, ProjectCode, RequestCalc, ResCode_1, ResCode_10, ResCode_10_1, ResCode_1_1, ResCode_2, ResCode_2_1, ResCode_3, ResCode_3_1, ResCode_4, ResCode_4_1, ResCode_5, ResCode_5_1, ResCode_6, ResCode_6_1, ResCode_7, ResCode_7_1, ResCode_8, ResCode_8_1, ResCode_9, ResCode_9_1, Root_1, Root_10, Root_10_1, Root_1_1, Root_2, Root_2_1, Root_3, Root_3_1, Root_4, Root_4_1, Root_5, Root_5_1, Root_6, Root_6_1, Root_7, Root_7_1, Root_8, Root_8_1, Root_9, Root_9_1, SalesEmp_Num, Sales_Consultant, SateliteCode, Select2YearComp, Server_Name, StateCode, Swing, CallType, unid, UnitNo, User_Defined, VendorCode_1, VendorCode_10, VendorCode_2, VendorCode_3, VendorCode_4, VendorCode_5, VendorCode_6, VendorCode_7, VendorCode_8, VendorCode_9, VendorName_1, VendorName_10, VendorName_2, VendorName_3, VendorName_4, VendorName_5, VendorName_6, VendorName_7, VendorName_8, VendorName_9, VendorPhoneNum_1, VendorPhoneNum_10, VendorPhoneNum_2, VendorPhoneNum_3, VendorPhoneNum_4, VendorPhoneNum_5, VendorPhoneNum_6, VendorPhoneNum_7, VendorPhoneNum_8, VendorPhoneNum_9, WorkPhone_1, WorkPhone_2, WSFU, WSFU_1, WsrEmp_Num, YearComp, ZipCode, ReturnValue)
                                        SELECT @JobAddress, @Area, @AreaPresCode, @Assigned_By, @Assigned_To, @BldNo, @Breaks, @Builder, @BuilderEmp_Num, @CallCompnotification, @Call_Comments, @Call_Num, @Cause_1, @Cause_10, @Cause_2, @Cause_3, @Cause_4, @Cause_5, @Cause_6, @Cause_7, @Cause_8, @Cause_9, @CDate_1, @CDate_10, @CDate_2, @CDate_3, @CDate_4, @CDate_5, @CDate_6, @CDate_7, @CDate_8, @CDate_9, @City, @CityCode, @CityCode_Short, @Close_Date, @CommNum, @CommNum_Long, @Community, @Comp_Date, @Contact, @Date_C, @Date_Conv, @Date_Conv_1, @Date_O, @Date_Open, @Descript_1, @Descript_10, @Descript_2, @Descript_3, @Descript_4, @Descript_5, @Descript_6, @Descript_7, @Descript_8, @Descript_9, @DisplayAuthor, @DivCode, @Division, @DocAuthor, @Duration, @Duration_1, @Duration_Pending, @Duration_Pending_1, @DurUnder7, @Elevation, @EmailContact, @Fax_Num, @GetCloseDate, @Hbr5Close, @Homeowner, @Home_Phone, @HOSig, @Item1, @Item10, @Item2, @Item3, @Item4, @Item5, @Item6, @Item7, @Item8, @Item9, @Items, @Items_Complete, @Job_Num, @Last_Modified, @LegalDesc, @lookupserver, @ModifiedBy, @MonthComp, @MonthQtr, @MonthSort, @Notification_0, @Notification_1, @Notification_2, @Notification_Call_Items, @Notification_Homeowner, @NumCalls, @Orig_CompDate, @Other_Phone, @OwnNum, @PCode_1, @PCode_10, @PCode_2, @PCode_3, @PCode_4, @PCode_5, @PCode_6, @PCode_7, @PCode_8, @PCode_9, @JobPlan, @PlanType, @PlanTypeDesc, @Plan_Name, @Printed_Name, @Project, @ProjectCode, @RequestCalc, @ResCode_1, @ResCode_10, @ResCode_10_1, @ResCode_1_1, @ResCode_2, @ResCode_2_1, @ResCode_3, @ResCode_3_1, @ResCode_4, @ResCode_4_1, @ResCode_5, @ResCode_5_1, @ResCode_6, @ResCode_6_1, @ResCode_7, @ResCode_7_1, @ResCode_8, @ResCode_8_1, @ResCode_9, @ResCode_9_1, @Root_1, @Root_10, @Root_10_1, @Root_1_1, @Root_2, @Root_2_1, @Root_3, @Root_3_1, @Root_4, @Root_4_1, @Root_5, @Root_5_1, @Root_6, @Root_6_1, @Root_7, @Root_7_1, @Root_8, @Root_8_1, @Root_9, @Root_9_1, @SalesEmp_Num, @Sales_Consultant, @SateliteCode, @Select2YearComp, @Server_Name, @StateCode, @Swing, @TypeOfSomething, @unid, @UnitNo, @User_Defined, @VendorCode_1, @VendorCode_10, @VendorCode_2, @VendorCode_3, @VendorCode_4, @VendorCode_5, @VendorCode_6, @VendorCode_7, @VendorCode_8, @VendorCode_9, @VendorName_1, @VendorName_10, @VendorName_2, @VendorName_3, @VendorName_4, @VendorName_5, @VendorName_6, @VendorName_7, @VendorName_8, @VendorName_9, @VendorPhoneNum_1, @VendorPhoneNum_10, @VendorPhoneNum_2, @VendorPhoneNum_3, @VendorPhoneNum_4, @VendorPhoneNum_5, @VendorPhoneNum_6, @VendorPhoneNum_7, @VendorPhoneNum_8, @VendorPhoneNum_9, @WorkPhone_1, @WorkPhone_2, @WSFU, @WSFU_1, @WsrEmp_Num, @YearComp, @ZipCode, @ReturnValue";

                                using (var cmd = new SqlCommand(sql, sc))
                                {
                                    cmd.Parameters.Add(new SqlParameter("@JobAddress", items[ArchivedServiceCallColumns.Address].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Area", items[ArchivedServiceCallColumns.Area].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@AreaPresCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Assigned_By",
                                        items[ArchivedServiceCallColumns.Assigned_By].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Assigned_To",
                                        items[ArchivedServiceCallColumns.Assigned_To].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@BldNo", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Breaks", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Builder", items[ArchivedServiceCallColumns.Builder].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@BuilderEmp_Num",
                                        items[ArchivedServiceCallColumns.BuilderEmp_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CallCompnotification",
                                        items[ArchivedServiceCallColumns.CallCompnotification].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Call_Comments",
                                        items[ArchivedServiceCallColumns.Call_Comments].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Call_Num", items[ArchivedServiceCallColumns.Call_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_1", items[ArchivedServiceCallColumns.Cause_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_10", items[ArchivedServiceCallColumns.Cause_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_2", items[ArchivedServiceCallColumns.Cause_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_3", items[ArchivedServiceCallColumns.Cause_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_4", items[ArchivedServiceCallColumns.Cause_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_5", items[ArchivedServiceCallColumns.Cause_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_6", items[ArchivedServiceCallColumns.Cause_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_7", items[ArchivedServiceCallColumns.Cause_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_8", items[ArchivedServiceCallColumns.Cause_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Cause_9", items[ArchivedServiceCallColumns.Cause_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_1", items[ArchivedServiceCallColumns.CDate_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_10", items[ArchivedServiceCallColumns.CDate_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_2", items[ArchivedServiceCallColumns.CDate_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_3", items[ArchivedServiceCallColumns.CDate_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_4", items[ArchivedServiceCallColumns.CDate_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_5", items[ArchivedServiceCallColumns.CDate_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_6", items[ArchivedServiceCallColumns.CDate_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_7", items[ArchivedServiceCallColumns.CDate_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_8", items[ArchivedServiceCallColumns.CDate_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CDate_9", items[ArchivedServiceCallColumns.CDate_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@City", items[ArchivedServiceCallColumns.City].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CityCode", items[ArchivedServiceCallColumns.CityCode].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CityCode_Short", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Close_Date",
                                        items[ArchivedServiceCallColumns.Close_Date].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CommNum", items[ArchivedServiceCallColumns.CommNum].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CommNum_Long", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Community",
                                        items[ArchivedServiceCallColumns.Community].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Comp_Date",
                                        items[ArchivedServiceCallColumns.Comp_Date].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Contact", items[ArchivedServiceCallColumns.Contact].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Date_C", items[ArchivedServiceCallColumns.Date_C].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Date_Conv",
                                        items[ArchivedServiceCallColumns.Date_Conv].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Date_Conv_1",
                                        items[ArchivedServiceCallColumns.Date_Conv_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Date_O", items[ArchivedServiceCallColumns.Date_O].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Date_Open",
                                        items[ArchivedServiceCallColumns.Date_Open].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_1",
                                        items[ArchivedServiceCallColumns.Descript_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_10",
                                        items[ArchivedServiceCallColumns.Descript_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_2",
                                        items[ArchivedServiceCallColumns.Descript_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_3",
                                        items[ArchivedServiceCallColumns.Descript_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_4",
                                        items[ArchivedServiceCallColumns.Descript_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_5",
                                        items[ArchivedServiceCallColumns.Descript_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_6",
                                        items[ArchivedServiceCallColumns.Descript_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_7",
                                        items[ArchivedServiceCallColumns.Descript_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_8",
                                        items[ArchivedServiceCallColumns.Descript_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Descript_9",
                                        items[ArchivedServiceCallColumns.Descript_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DisplayAuthor",
                                        items[ArchivedServiceCallColumns.DisplayAuthor].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DivCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Division", items[ArchivedServiceCallColumns.Division].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DocAuthor",
                                        items[ArchivedServiceCallColumns.DocAuthor].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Duration", items[ArchivedServiceCallColumns.Duration].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Duration_1",
                                        items[ArchivedServiceCallColumns.Duration_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Duration_Pending",
                                        items[ArchivedServiceCallColumns.Duration_Pending].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Duration_Pending_1",
                                        items[ArchivedServiceCallColumns.Duration_Pending_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DurUnder7", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Elevation",
                                        items[ArchivedServiceCallColumns.Elevation].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@EmailContact", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Fax_Num", items[ArchivedServiceCallColumns.Fax_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@GetCloseDate",
                                        items[ArchivedServiceCallColumns.Close_Date].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Hbr5Close",
                                        items[ArchivedServiceCallColumns.Hbr5Close].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Homeowner",
                                        items[ArchivedServiceCallColumns.Homeowner].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Home_Phone",
                                        items[ArchivedServiceCallColumns.Home_Phone].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@HOSig", items[ArchivedServiceCallColumns.HOSig].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item1", items[ArchivedServiceCallColumns.Item1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item10", items[ArchivedServiceCallColumns.Item10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item2", items[ArchivedServiceCallColumns.Item2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item3", items[ArchivedServiceCallColumns.Item3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item4", items[ArchivedServiceCallColumns.Item4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item5", items[ArchivedServiceCallColumns.Item5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item6", items[ArchivedServiceCallColumns.Item6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item7", items[ArchivedServiceCallColumns.Item7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item8", items[ArchivedServiceCallColumns.Item8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Item9", items[ArchivedServiceCallColumns.Item9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Items", items[ArchivedServiceCallColumns.Items].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Items_Complete",
                                        items[ArchivedServiceCallColumns.Items_Complete].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Job_Num", items[ArchivedServiceCallColumns.Job_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Last_Modified",
                                        items[ArchivedServiceCallColumns.Last_Modified].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@LegalDesc",
                                        items[ArchivedServiceCallColumns.LegalDesc].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@lookupserver", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ModifiedBy",
                                        items[ArchivedServiceCallColumns.ModifiedBy].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@MonthComp", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@MonthQtr", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@MonthSort", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Notification_0",
                                        items[ArchivedServiceCallColumns.Notification].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Notification_1",
                                        items[ArchivedServiceCallColumns.Notification_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Notification_2",
                                        items[ArchivedServiceCallColumns.Notification_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Notification_Call_Items",
                                        items[ArchivedServiceCallColumns.Notification_Call_Items].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Notification_Homeowner",
                                        items[ArchivedServiceCallColumns.Notification_Homeowner].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@NumCalls", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Orig_CompDate", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Other_Phone",
                                        items[ArchivedServiceCallColumns.Other_Phone].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@OwnNum", items[ArchivedServiceCallColumns.OwnNum].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_1", items[ArchivedServiceCallColumns.PCode_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_10", items[ArchivedServiceCallColumns.PCode_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_2", items[ArchivedServiceCallColumns.PCode_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_3", items[ArchivedServiceCallColumns.PCode_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_4", items[ArchivedServiceCallColumns.PCode_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_5", items[ArchivedServiceCallColumns.PCode_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_6", items[ArchivedServiceCallColumns.PCode_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_7", items[ArchivedServiceCallColumns.PCode_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_8", items[ArchivedServiceCallColumns.PCode_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PCode_9", items[ArchivedServiceCallColumns.PCode_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@JobPlan", items[ArchivedServiceCallColumns.Plan].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PlanType", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@PlanTypeDesc", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Plan_Name",
                                        items[ArchivedServiceCallColumns.Plan_Name].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Printed_Name",
                                        items[ArchivedServiceCallColumns.Printed_Name].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Project", items[ArchivedServiceCallColumns.Project].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ProjectCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@RequestCalc",
                                        items[ArchivedServiceCallColumns.RequestCalc].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_1",
                                        items[ArchivedServiceCallColumns.ResCode_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_10",
                                        items[ArchivedServiceCallColumns.ResCode_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_10_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_1_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_2",
                                        items[ArchivedServiceCallColumns.ResCode_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_2_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_3",
                                        items[ArchivedServiceCallColumns.ResCode_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_3_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_4",
                                        items[ArchivedServiceCallColumns.ResCode_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_4_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_5",
                                        items[ArchivedServiceCallColumns.ResCode_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_5_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_6",
                                        items[ArchivedServiceCallColumns.ResCode_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_6_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_7",
                                        items[ArchivedServiceCallColumns.ResCode_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_7_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_8",
                                        items[ArchivedServiceCallColumns.ResCode_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_8_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_9",
                                        items[ArchivedServiceCallColumns.ResCode_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ResCode_9_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_1", items[ArchivedServiceCallColumns.Root_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_10", items[ArchivedServiceCallColumns.Root_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_10_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_1_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_2", items[ArchivedServiceCallColumns.Root_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_2_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_3", items[ArchivedServiceCallColumns.Root_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_3_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_4", items[ArchivedServiceCallColumns.Root_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_4_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_5", items[ArchivedServiceCallColumns.Root_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_5_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_6", items[ArchivedServiceCallColumns.Root_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_6_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_7", items[ArchivedServiceCallColumns.Root_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_7_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_8", items[ArchivedServiceCallColumns.Root_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_8_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Root_9", items[ArchivedServiceCallColumns.Root_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Root_9_1", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@SalesEmp_Num",
                                        items[ArchivedServiceCallColumns.SalesEmp_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Sales_Consultant",
                                        items[ArchivedServiceCallColumns.Sales_Consultant].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@SateliteCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Select2YearComp", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Server_Name", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@StateCode", items[ArchivedServiceCallColumns.State].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Swing", items[ArchivedServiceCallColumns.Swing].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@TypeOfSomething",
                                        items[ArchivedServiceCallColumns.Type].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@unid", items[ArchivedServiceCallColumns.unid].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@UnitNo", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@User_Defined",
                                        items[ArchivedServiceCallColumns.User_Defined].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_1",
                                        items[ArchivedServiceCallColumns.VendorCode_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_10",
                                        items[ArchivedServiceCallColumns.VendorCode_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_2",
                                        items[ArchivedServiceCallColumns.VendorCode_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_3",
                                        items[ArchivedServiceCallColumns.VendorCode_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_4",
                                        items[ArchivedServiceCallColumns.VendorCode_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_5",
                                        items[ArchivedServiceCallColumns.VendorCode_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_6",
                                        items[ArchivedServiceCallColumns.VendorCode_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_7",
                                        items[ArchivedServiceCallColumns.VendorCode_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_8",
                                        items[ArchivedServiceCallColumns.VendorCode_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorCode_9",
                                        items[ArchivedServiceCallColumns.VendorCode_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_1",
                                        items[ArchivedServiceCallColumns.VendorName_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_10",
                                        items[ArchivedServiceCallColumns.VendorName_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_2",
                                        items[ArchivedServiceCallColumns.VendorName_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_3",
                                        items[ArchivedServiceCallColumns.VendorName_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_4",
                                        items[ArchivedServiceCallColumns.VendorName_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_5",
                                        items[ArchivedServiceCallColumns.VendorName_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_6",
                                        items[ArchivedServiceCallColumns.VendorName_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_7",
                                        items[ArchivedServiceCallColumns.VendorName_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_8",
                                        items[ArchivedServiceCallColumns.VendorName_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorName_9",
                                        items[ArchivedServiceCallColumns.VendorName_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_1",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_10",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_10].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_2",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_3",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_4",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_4].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_5",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_5].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_6",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_6].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_7",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_7].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_8",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_8].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_9",
                                        items[ArchivedServiceCallColumns.VendorPhoneNum_9].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WorkPhone_1",
                                        items[ArchivedServiceCallColumns.WorkPhone_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WorkPhone_2",
                                        items[ArchivedServiceCallColumns.WorkPhone_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WSFU", items[ArchivedServiceCallColumns.WSFU].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WSFU_1", items[ArchivedServiceCallColumns.WSFU_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WsrEmp_Num",
                                        items[ArchivedServiceCallColumns.WsrEmp_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@YearComp", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@ZipCode", items[ArchivedServiceCallColumns.ZipCode].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ReturnValue", items[ArchivedServiceCallColumns.Return].MaxLength(4000)));

                                    cmd.ExecuteNonQuery();
                                }

                                row = "";
                            }
                            else
                            {
                                row += "\r\n";
                            }
                        }
                    }
                }
            }
        }

        private static int GetColumnIndexes(StreamReader file)
        {
            var line = file.ReadLine();
            if (line != null)
            {
                var rowUnquoted = line
                    .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString(CultureInfo.InvariantCulture))
                    .TrimStart('"')
                    .TrimEnd('"');

                return rowUnquoted.Split(_fieldDelimiter).Count();
            }

            return 0;
        }
    }
}
