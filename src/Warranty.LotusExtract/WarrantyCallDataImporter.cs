using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using NHibernate;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Warranty.LotusExtract.Columns;
using Warranty.LotusExtract.Security;

namespace Warranty.LotusExtract
{
    public class WarrantyCallDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        private static ISession _session;

        public WarrantyCallDataImporter()
        {
            var sessionFactory = new Core.DataAccess.ConfigurationFactory().CreateConfigurationWithAuditing(new ImporterUserSession()).BuildSessionFactory();
            _session = sessionFactory.OpenSession();
        }

        public void Import(string fileName, string marketList)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.WarrantyCallImports", sc))
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

                            var sql = @"INSERT INTO imports.WarrantyCallImports 
                                        SELECT @JobAddress, @Area, @AreaPresCode, @Assigned_By, @Assigned_To, @BldNo, @Breaks, @Builder, @BuilderEmp_Num, @CallCompnotification, @Call_Comments, @Call_Num, @Cause_1, @Cause_10, @Cause_2, @Cause_3, @Cause_4, @Cause_5, @Cause_6, @Cause_7, @Cause_8, @Cause_9, @CDate_1, @CDate_10, @CDate_2, @CDate_3, @CDate_4, @CDate_5, @CDate_6, @CDate_7, @CDate_8, @CDate_9, @City, @CityCode, @CityCode_Short, @Close_Date, @CommNum, @CommNum_Long, @Community, @Comp_Date, @Contact, @Date_C, @Date_Conv, @Date_Conv_1, @Date_O, @Date_Open, @Descript_1, @Descript_10, @Descript_2, @Descript_3, @Descript_4, @Descript_5, @Descript_6, @Descript_7, @Descript_8, @Descript_9, @DisplayAuthor, @DivCode, @Division, @DocAuthor, @Duration, @Duration_1, @Duration_Pending, @Duration_Pending_1, @DurUnder7, @Elevation, @EmailContact, @Fax_Num, @GetCloseDate, @Hbr5Close, @Homeowner, @Home_Phone, @HOSig, @Item1, @Item10, @Item2, @Item3, @Item4, @Item5, @Item6, @Item7, @Item8, @Item9, @Items, @Items_Complete, @Job_Num, @Last_Modified, @LegalDesc, @lookupserver, @ModifiedBy, @MonthComp, @MonthQtr, @MonthSort, @Notification_0, @Notification_1, @Notification_2, @Notification_Call_Items, @Notification_Homeowner, @NumCalls, @Orig_CompDate, @Other_Phone, @OwnNum, @PCode_1, @PCode_10, @PCode_2, @PCode_3, @PCode_4, @PCode_5, @PCode_6, @PCode_7, @PCode_8, @PCode_9, @JobPlan, @PlanType, @PlanTypeDesc, @Plan_Name, @Printed_Name, @Project, @ProjectCode, @RequestCalc, @ResCode_1, @ResCode_10, @ResCode_10_1, @ResCode_1_1, @ResCode_2, @ResCode_2_1, @ResCode_3, @ResCode_3_1, @ResCode_4, @ResCode_4_1, @ResCode_5, @ResCode_5_1, @ResCode_6, @ResCode_6_1, @ResCode_7, @ResCode_7_1, @ResCode_8, @ResCode_8_1, @ResCode_9, @ResCode_9_1, @Root_1, @Root_10, @Root_10_1, @Root_1_1, @Root_2, @Root_2_1, @Root_3, @Root_3_1, @Root_4, @Root_4_1, @Root_5, @Root_5_1, @Root_6, @Root_6_1, @Root_7, @Root_7_1, @Root_8, @Root_8_1, @Root_9, @Root_9_1, @SalesEmp_Num, @Sales_Consultant, @SateliteCode, @Select2YearComp, @Server_Name, @StateCode, @Swing, @TypeOfSomething, @unid, @UnitNo, @User_Defined, @VendorCode_1, @VendorCode_10, @VendorCode_2, @VendorCode_3, @VendorCode_4, @VendorCode_5, @VendorCode_6, @VendorCode_7, @VendorCode_8, @VendorCode_9, @VendorName_1, @VendorName_10, @VendorName_2, @VendorName_3, @VendorName_4, @VendorName_5, @VendorName_6, @VendorName_7, @VendorName_8, @VendorName_9, @VendorPhoneNum_1, @VendorPhoneNum_10, @VendorPhoneNum_2, @VendorPhoneNum_3, @VendorPhoneNum_4, @VendorPhoneNum_5, @VendorPhoneNum_6, @VendorPhoneNum_7, @VendorPhoneNum_8, @VendorPhoneNum_9, @WorkPhone_1, @WorkPhone_2, @WSFU, @WSFU_1, @WsrEmp_Num, @YearComp, @ZipCode, @ReturnValue";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@JobAddress", items[WarrantyCallColumns.Address]));
                                cmd.Parameters.Add(new SqlParameter("@Area", items[WarrantyCallColumns.Area]));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresCode", items[WarrantyCallColumns.AreaPresCode]));
                                cmd.Parameters.Add(new SqlParameter("@Assigned_By", items[WarrantyCallColumns.Assigned_By]));
                                cmd.Parameters.Add(new SqlParameter("@Assigned_To", items[WarrantyCallColumns.Assigned_To]));
                                cmd.Parameters.Add(new SqlParameter("@BldNo", items[WarrantyCallColumns.BldNo]));
                                cmd.Parameters.Add(new SqlParameter("@Breaks", items[WarrantyCallColumns.Breaks]));
                                cmd.Parameters.Add(new SqlParameter("@Builder", items[WarrantyCallColumns.Builder]));
                                cmd.Parameters.Add(new SqlParameter("@BuilderEmp_Num", items[WarrantyCallColumns.BuilderEmp_Num]));
                                cmd.Parameters.Add(new SqlParameter("@CallCompnotification", items[WarrantyCallColumns.CallCompnotification]));
                                cmd.Parameters.Add(new SqlParameter("@Call_Comments", items[WarrantyCallColumns.Call_Comments]));
                                cmd.Parameters.Add(new SqlParameter("@Call_Num", items[WarrantyCallColumns.Call_Num]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_1", items[WarrantyCallColumns.Cause_1]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_10", items[WarrantyCallColumns.Cause_10]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_2", items[WarrantyCallColumns.Cause_2]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_3", items[WarrantyCallColumns.Cause_3]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_4", items[WarrantyCallColumns.Cause_4]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_5", items[WarrantyCallColumns.Cause_5]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_6", items[WarrantyCallColumns.Cause_6]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_7", items[WarrantyCallColumns.Cause_7]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_8", items[WarrantyCallColumns.Cause_8]));
                                cmd.Parameters.Add(new SqlParameter("@Cause_9", items[WarrantyCallColumns.Cause_9]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_1", items[WarrantyCallColumns.CDate_1]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_10", items[WarrantyCallColumns.CDate_10]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_2", items[WarrantyCallColumns.CDate_2]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_3", items[WarrantyCallColumns.CDate_3]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_4", items[WarrantyCallColumns.CDate_4]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_5", items[WarrantyCallColumns.CDate_5]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_6", items[WarrantyCallColumns.CDate_6]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_7", items[WarrantyCallColumns.CDate_7]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_8", items[WarrantyCallColumns.CDate_8]));
                                cmd.Parameters.Add(new SqlParameter("@CDate_9", items[WarrantyCallColumns.CDate_9]));
                                cmd.Parameters.Add(new SqlParameter("@City", items[WarrantyCallColumns.City]));
                                cmd.Parameters.Add(new SqlParameter("@CityCode", items[WarrantyCallColumns.CityCode]));
                                cmd.Parameters.Add(new SqlParameter("@CityCode_Short", items[WarrantyCallColumns.CityCode_Short]));
                                cmd.Parameters.Add(new SqlParameter("@Close_Date", items[WarrantyCallColumns.Close_Date]));
                                cmd.Parameters.Add(new SqlParameter("@CommNum", items[WarrantyCallColumns.CommNum]));
                                cmd.Parameters.Add(new SqlParameter("@CommNum_Long", items[WarrantyCallColumns.CommNum_Long]));
                                cmd.Parameters.Add(new SqlParameter("@Community", items[WarrantyCallColumns.Community]));
                                cmd.Parameters.Add(new SqlParameter("@Comp_Date", items[WarrantyCallColumns.Comp_Date]));
                                cmd.Parameters.Add(new SqlParameter("@Contact", items[WarrantyCallColumns.Contact]));
                                cmd.Parameters.Add(new SqlParameter("@Date_C", items[WarrantyCallColumns.Date_C]));
                                cmd.Parameters.Add(new SqlParameter("@Date_Conv", items[WarrantyCallColumns.Date_Conv]));
                                cmd.Parameters.Add(new SqlParameter("@Date_Conv_1", items[WarrantyCallColumns.Date_Conv_1]));
                                cmd.Parameters.Add(new SqlParameter("@Date_O", items[WarrantyCallColumns.Date_O]));
                                cmd.Parameters.Add(new SqlParameter("@Date_Open", items[WarrantyCallColumns.Date_Open]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_1", items[WarrantyCallColumns.Descript_1]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_10", items[WarrantyCallColumns.Descript_10]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_2", items[WarrantyCallColumns.Descript_2]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_3", items[WarrantyCallColumns.Descript_3]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_4", items[WarrantyCallColumns.Descript_4]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_5", items[WarrantyCallColumns.Descript_5]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_6", items[WarrantyCallColumns.Descript_6]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_7", items[WarrantyCallColumns.Descript_7]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_8", items[WarrantyCallColumns.Descript_8]));
                                cmd.Parameters.Add(new SqlParameter("@Descript_9", items[WarrantyCallColumns.Descript_9]));
                                cmd.Parameters.Add(new SqlParameter("@DisplayAuthor", items[WarrantyCallColumns.DisplayAuthor]));
                                cmd.Parameters.Add(new SqlParameter("@DivCode", items[WarrantyCallColumns.DivCode]));
                                cmd.Parameters.Add(new SqlParameter("@Division", items[WarrantyCallColumns.Division]));
                                cmd.Parameters.Add(new SqlParameter("@DocAuthor", items[WarrantyCallColumns.DocAuthor]));
                                cmd.Parameters.Add(new SqlParameter("@Duration", items[WarrantyCallColumns.Duration]));
                                cmd.Parameters.Add(new SqlParameter("@Duration_1", items[WarrantyCallColumns.Duration_1]));
                                cmd.Parameters.Add(new SqlParameter("@Duration_Pending", items[WarrantyCallColumns.Duration_Pending]));
                                cmd.Parameters.Add(new SqlParameter("@Duration_Pending_1", items[WarrantyCallColumns.Duration_Pending_1]));
                                cmd.Parameters.Add(new SqlParameter("@DurUnder7", items[WarrantyCallColumns.DurUnder7]));
                                cmd.Parameters.Add(new SqlParameter("@Elevation", items[WarrantyCallColumns.Elevation]));
                                cmd.Parameters.Add(new SqlParameter("@EmailContact", items[WarrantyCallColumns.EmailContact]));
                                cmd.Parameters.Add(new SqlParameter("@Fax_Num", items[WarrantyCallColumns.Fax_Num]));
                                cmd.Parameters.Add(new SqlParameter("@GetCloseDate", items[WarrantyCallColumns.GetCloseDate]));
                                cmd.Parameters.Add(new SqlParameter("@Hbr5Close", items[WarrantyCallColumns.Hbr5Close]));
                                cmd.Parameters.Add(new SqlParameter("@Homeowner", items[WarrantyCallColumns.Homeowner]));
                                cmd.Parameters.Add(new SqlParameter("@Home_Phone", items[WarrantyCallColumns.Home_Phone]));
                                cmd.Parameters.Add(new SqlParameter("@HOSig", items[WarrantyCallColumns.HOSig]));
                                cmd.Parameters.Add(new SqlParameter("@Item1", items[WarrantyCallColumns.Item1]));
                                cmd.Parameters.Add(new SqlParameter("@Item10", items[WarrantyCallColumns.Item10]));
                                cmd.Parameters.Add(new SqlParameter("@Item2", items[WarrantyCallColumns.Item2]));
                                cmd.Parameters.Add(new SqlParameter("@Item3", items[WarrantyCallColumns.Item3]));
                                cmd.Parameters.Add(new SqlParameter("@Item4", items[WarrantyCallColumns.Item4]));
                                cmd.Parameters.Add(new SqlParameter("@Item5", items[WarrantyCallColumns.Item5]));
                                cmd.Parameters.Add(new SqlParameter("@Item6", items[WarrantyCallColumns.Item6]));
                                cmd.Parameters.Add(new SqlParameter("@Item7", items[WarrantyCallColumns.Item7]));
                                cmd.Parameters.Add(new SqlParameter("@Item8", items[WarrantyCallColumns.Item8]));
                                cmd.Parameters.Add(new SqlParameter("@Item9", items[WarrantyCallColumns.Item9]));
                                cmd.Parameters.Add(new SqlParameter("@Items", items[WarrantyCallColumns.Items]));
                                cmd.Parameters.Add(new SqlParameter("@Items_Complete", items[WarrantyCallColumns.Items_Complete]));
                                cmd.Parameters.Add(new SqlParameter("@Job_Num", items[WarrantyCallColumns.Job_Num]));
                                cmd.Parameters.Add(new SqlParameter("@Last_Modified", items[WarrantyCallColumns.Last_Modified]));
                                cmd.Parameters.Add(new SqlParameter("@LegalDesc", items[WarrantyCallColumns.LegalDesc]));
                                cmd.Parameters.Add(new SqlParameter("@lookupserver", items[WarrantyCallColumns.lookupserver]));
                                cmd.Parameters.Add(new SqlParameter("@ModifiedBy", items[WarrantyCallColumns.ModifiedBy]));
                                cmd.Parameters.Add(new SqlParameter("@MonthComp", items[WarrantyCallColumns.MonthComp]));
                                cmd.Parameters.Add(new SqlParameter("@MonthQtr", items[WarrantyCallColumns.MonthQtr]));
                                cmd.Parameters.Add(new SqlParameter("@MonthSort", items[WarrantyCallColumns.MonthSort]));
                                cmd.Parameters.Add(new SqlParameter("@Notification_0", items[WarrantyCallColumns.Notification]));
                                cmd.Parameters.Add(new SqlParameter("@Notification_1", items[WarrantyCallColumns.Notification_1]));
                                cmd.Parameters.Add(new SqlParameter("@Notification_2", items[WarrantyCallColumns.Notification_2]));
                                cmd.Parameters.Add(new SqlParameter("@Notification_Call_Items", items[WarrantyCallColumns.Notification_Call_Items]));
                                cmd.Parameters.Add(new SqlParameter("@Notification_Homeowner", items[WarrantyCallColumns.Notification_Homeowner]));
                                cmd.Parameters.Add(new SqlParameter("@NumCalls", items[WarrantyCallColumns.NumCalls]));
                                cmd.Parameters.Add(new SqlParameter("@Orig_CompDate", items[WarrantyCallColumns.Orig_CompDate]));
                                cmd.Parameters.Add(new SqlParameter("@Other_Phone", items[WarrantyCallColumns.Other_Phone]));
                                cmd.Parameters.Add(new SqlParameter("@OwnNum", items[WarrantyCallColumns.OwnNum]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_1", items[WarrantyCallColumns.PCode_1]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_10", items[WarrantyCallColumns.PCode_10]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_2", items[WarrantyCallColumns.PCode_2]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_3", items[WarrantyCallColumns.PCode_3]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_4", items[WarrantyCallColumns.PCode_4]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_5", items[WarrantyCallColumns.PCode_5]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_6", items[WarrantyCallColumns.PCode_6]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_7", items[WarrantyCallColumns.PCode_7]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_8", items[WarrantyCallColumns.PCode_8]));
                                cmd.Parameters.Add(new SqlParameter("@PCode_9", items[WarrantyCallColumns.PCode_9]));
                                cmd.Parameters.Add(new SqlParameter("@JobPlan", items[WarrantyCallColumns.Plan]));
                                cmd.Parameters.Add(new SqlParameter("@PlanType", items[WarrantyCallColumns.PlanType]));
                                cmd.Parameters.Add(new SqlParameter("@PlanTypeDesc", items[WarrantyCallColumns.PlanTypeDesc]));
                                cmd.Parameters.Add(new SqlParameter("@Plan_Name", items[WarrantyCallColumns.Plan_Name]));
                                cmd.Parameters.Add(new SqlParameter("@Printed_Name", items[WarrantyCallColumns.Printed_Name]));
                                cmd.Parameters.Add(new SqlParameter("@Project", items[WarrantyCallColumns.Project]));
                                cmd.Parameters.Add(new SqlParameter("@ProjectCode", items[WarrantyCallColumns.ProjectCode]));
                                cmd.Parameters.Add(new SqlParameter("@RequestCalc", items[WarrantyCallColumns.RequestCalc]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_1", items[WarrantyCallColumns.ResCode_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_10", items[WarrantyCallColumns.ResCode_10]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_10_1", items[WarrantyCallColumns.ResCode_10_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_1_1", items[WarrantyCallColumns.ResCode_1_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_2", items[WarrantyCallColumns.ResCode_2]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_2_1", items[WarrantyCallColumns.ResCode_2_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_3", items[WarrantyCallColumns.ResCode_3]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_3_1", items[WarrantyCallColumns.ResCode_3_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_4", items[WarrantyCallColumns.ResCode_4]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_4_1", items[WarrantyCallColumns.ResCode_4_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_5", items[WarrantyCallColumns.ResCode_5]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_5_1", items[WarrantyCallColumns.ResCode_5_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_6", items[WarrantyCallColumns.ResCode_6]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_6_1", items[WarrantyCallColumns.ResCode_6_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_7", items[WarrantyCallColumns.ResCode_7]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_7_1", items[WarrantyCallColumns.ResCode_7_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_8", items[WarrantyCallColumns.ResCode_8]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_8_1", items[WarrantyCallColumns.ResCode_8_1]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_9", items[WarrantyCallColumns.ResCode_9]));
                                cmd.Parameters.Add(new SqlParameter("@ResCode_9_1", items[WarrantyCallColumns.ResCode_9_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_1", items[WarrantyCallColumns.Root_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_10", items[WarrantyCallColumns.Root_10]));
                                cmd.Parameters.Add(new SqlParameter("@Root_10_1", items[WarrantyCallColumns.Root_10_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_1_1", items[WarrantyCallColumns.Root_1_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_2", items[WarrantyCallColumns.Root_2]));
                                cmd.Parameters.Add(new SqlParameter("@Root_2_1", items[WarrantyCallColumns.Root_2_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_3", items[WarrantyCallColumns.Root_3]));
                                cmd.Parameters.Add(new SqlParameter("@Root_3_1", items[WarrantyCallColumns.Root_3_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_4", items[WarrantyCallColumns.Root_4]));
                                cmd.Parameters.Add(new SqlParameter("@Root_4_1", items[WarrantyCallColumns.Root_4_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_5", items[WarrantyCallColumns.Root_5]));
                                cmd.Parameters.Add(new SqlParameter("@Root_5_1", items[WarrantyCallColumns.Root_5_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_6", items[WarrantyCallColumns.Root_6]));
                                cmd.Parameters.Add(new SqlParameter("@Root_6_1", items[WarrantyCallColumns.Root_6_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_7", items[WarrantyCallColumns.Root_7]));
                                cmd.Parameters.Add(new SqlParameter("@Root_7_1", items[WarrantyCallColumns.Root_7_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_8", items[WarrantyCallColumns.Root_8]));
                                cmd.Parameters.Add(new SqlParameter("@Root_8_1", items[WarrantyCallColumns.Root_8_1]));
                                cmd.Parameters.Add(new SqlParameter("@Root_9", items[WarrantyCallColumns.Root_9]));
                                cmd.Parameters.Add(new SqlParameter("@Root_9_1", items[WarrantyCallColumns.Root_9_1]));
                                cmd.Parameters.Add(new SqlParameter("@SalesEmp_Num", items[WarrantyCallColumns.SalesEmp_Num]));
                                cmd.Parameters.Add(new SqlParameter("@Sales_Consultant", items[WarrantyCallColumns.Sales_Consultant]));
                                cmd.Parameters.Add(new SqlParameter("@SateliteCode", items[WarrantyCallColumns.SateliteCode]));
                                cmd.Parameters.Add(new SqlParameter("@Select2YearComp", items[WarrantyCallColumns.Select2YearComp]));
                                cmd.Parameters.Add(new SqlParameter("@Server_Name", items[WarrantyCallColumns.Server_Name]));
                                cmd.Parameters.Add(new SqlParameter("@StateCode", items[WarrantyCallColumns.State]));
                                cmd.Parameters.Add(new SqlParameter("@Swing", items[WarrantyCallColumns.Swing]));
                                cmd.Parameters.Add(new SqlParameter("@TypeOfSomething", items[WarrantyCallColumns.Type]));
                                cmd.Parameters.Add(new SqlParameter("@unid", items[WarrantyCallColumns.unid]));
                                cmd.Parameters.Add(new SqlParameter("@UnitNo", items[WarrantyCallColumns.UnitNo]));
                                cmd.Parameters.Add(new SqlParameter("@User_Defined", items[WarrantyCallColumns.User_Defined]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_1", items[WarrantyCallColumns.VendorCode_1]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_10", items[WarrantyCallColumns.VendorCode_10]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_2", items[WarrantyCallColumns.VendorCode_2]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_3", items[WarrantyCallColumns.VendorCode_3]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_4", items[WarrantyCallColumns.VendorCode_4]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_5", items[WarrantyCallColumns.VendorCode_5]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_6", items[WarrantyCallColumns.VendorCode_6]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_7", items[WarrantyCallColumns.VendorCode_7]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_8", items[WarrantyCallColumns.VendorCode_8]));
                                cmd.Parameters.Add(new SqlParameter("@VendorCode_9", items[WarrantyCallColumns.VendorCode_9]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_1", items[WarrantyCallColumns.VendorName_1]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_10", items[WarrantyCallColumns.VendorName_10]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_2", items[WarrantyCallColumns.VendorName_2]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_3", items[WarrantyCallColumns.VendorName_3]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_4", items[WarrantyCallColumns.VendorName_4]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_5", items[WarrantyCallColumns.VendorName_5]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_6", items[WarrantyCallColumns.VendorName_6]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_7", items[WarrantyCallColumns.VendorName_7]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_8", items[WarrantyCallColumns.VendorName_8]));
                                cmd.Parameters.Add(new SqlParameter("@VendorName_9", items[WarrantyCallColumns.VendorName_9]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_1", items[WarrantyCallColumns.VendorPhoneNum_1]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_10", items[WarrantyCallColumns.VendorPhoneNum_10]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_2", items[WarrantyCallColumns.VendorPhoneNum_2]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_3", items[WarrantyCallColumns.VendorPhoneNum_3]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_4", items[WarrantyCallColumns.VendorPhoneNum_4]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_5", items[WarrantyCallColumns.VendorPhoneNum_5]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_6", items[WarrantyCallColumns.VendorPhoneNum_6]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_7", items[WarrantyCallColumns.VendorPhoneNum_7]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_8", items[WarrantyCallColumns.VendorPhoneNum_8]));
                                cmd.Parameters.Add(new SqlParameter("@VendorPhoneNum_9", items[WarrantyCallColumns.VendorPhoneNum_9]));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone_1", items[WarrantyCallColumns.WorkPhone_1]));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone_2", items[WarrantyCallColumns.WorkPhone_2]));
                                cmd.Parameters.Add(new SqlParameter("@WSFU", items[WarrantyCallColumns.WSFU]));
                                cmd.Parameters.Add(new SqlParameter("@WSFU_1", items[WarrantyCallColumns.WSFU_1]));
                                cmd.Parameters.Add(new SqlParameter("@WsrEmp_Num", items[WarrantyCallColumns.WsrEmp_Num]));
                                cmd.Parameters.Add(new SqlParameter("@YearComp", items[WarrantyCallColumns.YearComp]));
                                cmd.Parameters.Add(new SqlParameter("@ZipCode", items[WarrantyCallColumns.ZipCode]));
                                cmd.Parameters.Add(new SqlParameter("@ReturnValue", items[WarrantyCallColumns.Return]));

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
