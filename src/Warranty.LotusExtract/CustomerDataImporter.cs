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

    public class CustomerDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        public void Import(string fileName, string marketList, string archivedFile = null)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.CustomerImports", sc))
                    cmd.ExecuteNonQuery();

                using (var file = new StreamReader(fileName))
                {
                    var row = "";
                    string line;

                    var headerColumnCount = GetColumnIndexes(file);

                    while ((line = file.ReadLine()) != null)
                    {
                        line = line
                            .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString(CultureInfo.InvariantCulture))
                            .TrimStart('"')
                            .TrimEnd('"');

                        row += line;
                        
                        if (row.Split(_fieldDelimiter).Count() >= headerColumnCount)
                        {
                            var items = row.Split(_fieldDelimiter);

                            var sql = @"INSERT INTO imports.CustomerImports (JobAddress, Area, AreaPresidentCode, Attachments, BuildNumber, Breaks, Builder, BuilderEmployeeNumber, City, CityCode, CityShortCode, CloseDate, Community, CommunityNumber, Coversheets, DateConverted, DivisionCode, Division, DocumentAuthor, Elevation, EmailContact, HomeOwner, HomeOwnerMoved, HomePhone, JobNumber, LastModified, LegalDescription, ModifiedBy, Options, OtherPhone, OwnerNumber, JobPlan, PlanType, PlanTypeDescription, PlanName, PreviousOwner1, PreviousOwner2, PreviousOwner3, PrintedName, Processed, Project, ProjectCode, SalesConsultantNumber, SalesConsultant, SateliteCode, SelectionSheets, StateCode, Swing, TotalSalesPrice, UnitNumber, WarrantyExpirationDate, WorkPhone1, WorkPhone2, ZipCode, FileNames)
                                        SELECT @JobAddress, @Area, @AreaPresidentCode, @Attachments, @BuildNumber, @Breaks, @Builder, @BuilderEmployeeNumber, @City, @CityCode, @CityShortCode, @CloseDate, @Community, @CommunityNumber, @Coversheets, @DateConverted, @DivisionCode, @Division, @DocumentAuthor, @Elevation, @EmailContact, @HomeOwner, @HomeOwnerMoved, @HomePhone, @JobNumber, @LastModified, @LegalDescription, @ModifiedBy, @Options, @OtherPhone, @OwnerNumber, @JobPlan, @PlanType, @PlanTypeDescription, @PlanName, @PreviousOwner1, @PreviousOwner2, @PreviousOwner3, @PrintedName, @Processed, @Project, @ProjectCode, @SalesConsultantNumber, @SalesConsultant, @SateliteCode, @SelectionSheets, @StateCode, @Swing, @TotalSalesPrice, @UnitNumber, @WarrantyExpirationDate, @WorkPhone1, @WorkPhone2, @ZipCode, @FileNames";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@JobAddress", items[CustomerColumns.Address].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Area", items[CustomerColumns.Area].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresidentCode", items[CustomerColumns.AreaPresidentCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Attachments", items[CustomerColumns.Attachments].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@BuildNumber", items[CustomerColumns.BuildNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Breaks", items[CustomerColumns.Breaks].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Builder", items[CustomerColumns.Builder].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@BuilderEmployeeNumber", items[CustomerColumns.BuilderEmployeeNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@City", items[CustomerColumns.City].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CityCode", items[CustomerColumns.CityCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CityShortCode", items[CustomerColumns.CityShortCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CloseDate", items[CustomerColumns.CloseDate].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Community", items[CustomerColumns.Community].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@CommunityNumber", items[CustomerColumns.CommunityNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Coversheets", items[CustomerColumns.Coversheets].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DateConverted", items[CustomerColumns.DateConverted].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DivisionCode", items[CustomerColumns.DivisionCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Division", items[CustomerColumns.Division].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@DocumentAuthor", items[CustomerColumns.DocumentAuthor].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Elevation", items[CustomerColumns.Elevation].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@EmailContact", items[CustomerColumns.EmailContact].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@HomeOwner", items[CustomerColumns.HomeOwner].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@HomeOwnerMoved", items[CustomerColumns.HomeOwnerMoved].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@HomePhone", items[CustomerColumns.HomePhone].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@JobNumber", items[CustomerColumns.JobNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@LastModified", items[CustomerColumns.LastModified].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@LegalDescription", items[CustomerColumns.LegalDescription].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ModifiedBy", items[CustomerColumns.ModifiedBy].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Options", items[CustomerColumns.Options].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@OtherPhone", items[CustomerColumns.OtherPhone].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@OwnerNumber", items[CustomerColumns.OwnerNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@JobPlan", items[CustomerColumns.Plan].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PlanType", items[CustomerColumns.PlanType].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PlanTypeDescription", items[CustomerColumns.PlanTypeDescription].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PlanName", items[CustomerColumns.PlanName].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner1", items[CustomerColumns.PreviousOwner1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner2", items[CustomerColumns.PreviousOwner2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner3", items[CustomerColumns.PreviousOwner3].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@PrintedName", items[CustomerColumns.PrintedName].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Processed", items[CustomerColumns.Processed].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Project", items[CustomerColumns.Project].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ProjectCode", items[CustomerColumns.ProjectCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SalesConsultantNumber", items[CustomerColumns.SalesConsultantNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SalesConsultant", items[CustomerColumns.SalesConsultant].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SateliteCode", items[CustomerColumns.SateliteCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@SelectionSheets", items[CustomerColumns.SelectionSheets].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@StateCode", items[CustomerColumns.State].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@Swing", items[CustomerColumns.Swing].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@TotalSalesPrice", items[CustomerColumns.TotalSalesPrice].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@UnitNumber", items[CustomerColumns.UnitNumber].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WarrantyExpirationDate", items[CustomerColumns.WarrantyExpirationDate].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone1", items[CustomerColumns.WorkPhone1].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone2", items[CustomerColumns.WorkPhone2].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@ZipCode", items[CustomerColumns.ZipCode].MaxLength(4000)));
                                cmd.Parameters.Add(new SqlParameter("@FileNames", items[CustomerColumns.FileNames].MaxLength(4000)));
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

                if (!archivedFile.IsNullOrEmpty() && File.Exists(archivedFile))
                {
                    using (var file = new StreamReader(archivedFile))
                    {
                        var row = "";
                        string line;

                        var headerColumnCount = GetColumnIndexes(file);

                        while ((line = file.ReadLine()) != null)
                        {
                            line = line
                                .Replace("\"" + _fieldDelimiter + "\"",
                                    _fieldDelimiter.ToString(CultureInfo.InvariantCulture))
                                .TrimStart('"')
                                .TrimEnd('"');

                            row += line;

                            if (row.Split(_fieldDelimiter).Count() >= headerColumnCount)
                            {
                                var items = row.Split(_fieldDelimiter);

                                var sql =
                                    @"INSERT INTO imports.CustomerImports (JobAddress, Area, AreaPresidentCode, Attachments, BuildNumber, Breaks, Builder, BuilderEmployeeNumber, City, CityCode, CityShortCode, CloseDate, Community, CommunityNumber, Coversheets, DateConverted, DivisionCode, Division, DocumentAuthor, Elevation, EmailContact, HomeOwner, HomeOwnerMoved, HomePhone, JobNumber, LastModified, LegalDescription, ModifiedBy, Options, OtherPhone, OwnerNumber, JobPlan, PlanType, PlanTypeDescription, PlanName, PreviousOwner1, PreviousOwner2, PreviousOwner3, PrintedName, Processed, Project, ProjectCode, SalesConsultantNumber, SalesConsultant, SateliteCode, SelectionSheets, StateCode, Swing, TotalSalesPrice, UnitNumber, WarrantyExpirationDate, WorkPhone1, WorkPhone2, ZipCode, FileNames)
                                        SELECT @JobAddress, @Area, @AreaPresidentCode, @Attachments, @BuildNumber, @Breaks, @Builder, @BuilderEmployeeNumber, @City, @CityCode, @CityShortCode, @CloseDate, @Community, @CommunityNumber, @Coversheets, @DateConverted, @DivisionCode, @Division, @DocumentAuthor, @Elevation, @EmailContact, @HomeOwner, @HomeOwnerMoved, @HomePhone, @JobNumber, @LastModified, @LegalDescription, @ModifiedBy, @Options, @OtherPhone, @OwnerNumber, @JobPlan, @PlanType, @PlanTypeDescription, @PlanName, @PreviousOwner1, @PreviousOwner2, @PreviousOwner3, @PrintedName, @Processed, @Project, @ProjectCode, @SalesConsultantNumber, @SalesConsultant, @SateliteCode, @SelectionSheets, @StateCode, @Swing, @TotalSalesPrice, @UnitNumber, @WarrantyExpirationDate, @WorkPhone1, @WorkPhone2, @ZipCode, @FileNames";

                                using (var cmd = new SqlCommand(sql, sc))
                                {
                                    cmd.Parameters.Add(new SqlParameter("@JobAddress", items[ArchivedCustomerColumns.Address].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Area", items[ArchivedCustomerColumns.Area].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@AreaPresidentCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Attachments", items[ArchivedCustomerColumns.Attachments].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@BuildNumber", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Breaks", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Builder", items[ArchivedCustomerColumns.Builder].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@BuilderEmployeeNumber", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@City", items[ArchivedCustomerColumns.City].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CityCode", items[ArchivedCustomerColumns.CityCode].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CityShortCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@CloseDate", items[ArchivedCustomerColumns.Close_Date].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Community", items[ArchivedCustomerColumns.Community].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@CommunityNumber", items[ArchivedCustomerColumns.Comm_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Coversheets", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@DateConverted", items[ArchivedCustomerColumns.Date_Conv].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DivisionCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@Division", items[ArchivedCustomerColumns.Division].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@DocumentAuthor", items[ArchivedCustomerColumns.DocAuthor].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Elevation", items[ArchivedCustomerColumns.Elevation].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@EmailContact", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@HomeOwner", items[ArchivedCustomerColumns.Homeowner].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@HomeOwnerMoved", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@HomePhone", items[ArchivedCustomerColumns.Home_Phone].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@JobNumber", items[ArchivedCustomerColumns.Job_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@LastModified", items[ArchivedCustomerColumns.Last_Modified].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@LegalDescription", items[ArchivedCustomerColumns.LegalDesc].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ModifiedBy", items[ArchivedCustomerColumns.ModifiedBy].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Options", items[ArchivedCustomerColumns.Options].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@OtherPhone", items[ArchivedCustomerColumns.Other_Phone].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@OwnerNumber", items[ArchivedCustomerColumns.OwnerNo].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@JobPlan", items[ArchivedCustomerColumns.Plan].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PlanType", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@PlanTypeDescription", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@PlanName", items[ArchivedCustomerColumns.Plan_Name].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PreviousOwner1", items[ArchivedCustomerColumns.Previous1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PreviousOwner2", items[ArchivedCustomerColumns.Previous2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PreviousOwner3", items[ArchivedCustomerColumns.Previous3].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@PrintedName", items[ArchivedCustomerColumns.Printed_Name].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Processed", items[ArchivedCustomerColumns.Processed].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Project", items[ArchivedCustomerColumns.Project].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ProjectCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@SalesConsultantNumber", items[ArchivedCustomerColumns.SalesEmp_Num].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@SalesConsultant", items[ArchivedCustomerColumns.Sales_Consultant].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@SateliteCode", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@SelectionSheets", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@StateCode", items[ArchivedCustomerColumns.State].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@Swing", items[ArchivedCustomerColumns.Swing].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@TotalSalesPrice", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@UnitNumber", DBNull.Value));
                                    cmd.Parameters.Add(new SqlParameter("@WarrantyExpirationDate", items[ArchivedCustomerColumns.WarrExp].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WorkPhone1", items[ArchivedCustomerColumns.Work_1].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@WorkPhone2", items[ArchivedCustomerColumns.Work_2].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@ZipCode", items[ArchivedCustomerColumns.ZipCode].MaxLength(4000)));
                                    cmd.Parameters.Add(new SqlParameter("@FileNames", items[ArchivedCustomerColumns.Attachment_Names].MaxLength(4000)));
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