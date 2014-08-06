using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Warranty.LotusExtract.Columns;

namespace Warranty.LotusExtract
{
    public class CustomerDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        public void Import(string fileName, string marketList)
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

                            var sql = @"INSERT INTO imports.CustomerImports (JobAddress, Area, AreaPresidentCode, Attachments, BuildNumber, Breaks, Builder, BuilderEmployeeNumber, City, CityCode, CityShortCode, CloseDate, Community, CommunityNumber, Coversheets, DateConverted, DivisionCode, Division, DocumentAuthor, Elevation, EmailContact, HomeOwner, HomeOwnerMoved, HomePhone, JobNumber, LastModified, LegalDescription, ModifiedBy, Options, OtherPhone, OwnerNumber, JobPlan, PlanType, PlanTypeDescription, PlanName, PreviousOwner1, PreviousOwner2, PreviousOwner3, PrintedName, Processed, Project, ProjectCode, SalesConsultantNumber, SalesConsultant, SateliteCode, SelectionSheets, StateCode, Swing, TotalSalesPrice, UnitNumber, WarrantyExpirationDate, WorkPhone1, WorkPhone2, ZipCode)
                                        SELECT @JobAddress, @Area, @AreaPresidentCode, @Attachments, @BuildNumber, @Breaks, @Builder, @BuilderEmployeeNumber, @City, @CityCode, @CityShortCode, @CloseDate, @Community, @CommunityNumber, @Coversheets, @DateConverted, @DivisionCode, @Division, @DocumentAuthor, @Elevation, @EmailContact, @HomeOwner, @HomeOwnerMoved, @HomePhone, @JobNumber, @LastModified, @LegalDescription, @ModifiedBy, @Options, @OtherPhone, @OwnerNumber, @JobPlan, @PlanType, @PlanTypeDescription, @PlanName, @PreviousOwner1, @PreviousOwner2, @PreviousOwner3, @PrintedName, @Processed, @Project, @ProjectCode, @SalesConsultantNumber, @SalesConsultant, @SateliteCode, @SelectionSheets, @StateCode, @Swing, @TotalSalesPrice, @UnitNumber, @WarrantyExpirationDate, @WorkPhone1, @WorkPhone2, @ZipCode";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@JobAddress", items[CustomerColumns.Address]));
                                cmd.Parameters.Add(new SqlParameter("@Area", items[CustomerColumns.Area]));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresidentCode", items[CustomerColumns.AreaPresidentCode]));
                                cmd.Parameters.Add(new SqlParameter("@Attachments", items[CustomerColumns.Attachments]));
                                cmd.Parameters.Add(new SqlParameter("@BuildNumber", items[CustomerColumns.BuildNumber]));
                                cmd.Parameters.Add(new SqlParameter("@Breaks", items[CustomerColumns.Breaks]));
                                cmd.Parameters.Add(new SqlParameter("@Builder", items[CustomerColumns.Builder]));
                                cmd.Parameters.Add(new SqlParameter("@BuilderEmployeeNumber", items[CustomerColumns.BuilderEmployeeNumber]));
                                cmd.Parameters.Add(new SqlParameter("@City", items[CustomerColumns.City]));
                                cmd.Parameters.Add(new SqlParameter("@CityCode", items[CustomerColumns.CityCode]));
                                cmd.Parameters.Add(new SqlParameter("@CityShortCode", items[CustomerColumns.CityShortCode]));
                                cmd.Parameters.Add(new SqlParameter("@CloseDate", items[CustomerColumns.CloseDate]));
                                cmd.Parameters.Add(new SqlParameter("@Community", items[CustomerColumns.Community]));
                                cmd.Parameters.Add(new SqlParameter("@CommunityNumber", items[CustomerColumns.CommunityNumber]));
                                cmd.Parameters.Add(new SqlParameter("@Coversheets", items[CustomerColumns.Coversheets]));
                                cmd.Parameters.Add(new SqlParameter("@DateConverted", items[CustomerColumns.DateConverted]));
                                cmd.Parameters.Add(new SqlParameter("@DivisionCode", items[CustomerColumns.DivisionCode]));
                                cmd.Parameters.Add(new SqlParameter("@Division", items[CustomerColumns.Division]));
                                cmd.Parameters.Add(new SqlParameter("@DocumentAuthor", items[CustomerColumns.DocumentAuthor]));
                                cmd.Parameters.Add(new SqlParameter("@Elevation", items[CustomerColumns.Elevation]));
                                cmd.Parameters.Add(new SqlParameter("@EmailContact", items[CustomerColumns.EmailContact]));
                                cmd.Parameters.Add(new SqlParameter("@HomeOwner", items[CustomerColumns.HomeOwner]));
                                cmd.Parameters.Add(new SqlParameter("@HomeOwnerMoved", items[CustomerColumns.HomeOwnerMoved]));
                                cmd.Parameters.Add(new SqlParameter("@HomePhone", items[CustomerColumns.HomePhone]));
                                cmd.Parameters.Add(new SqlParameter("@JobNumber", items[CustomerColumns.JobNumber]));
                                cmd.Parameters.Add(new SqlParameter("@LastModified", items[CustomerColumns.LastModified]));
                                cmd.Parameters.Add(new SqlParameter("@LegalDescription", items[CustomerColumns.LegalDescription]));
                                cmd.Parameters.Add(new SqlParameter("@ModifiedBy", items[CustomerColumns.ModifiedBy]));
                                cmd.Parameters.Add(new SqlParameter("@Options", items[CustomerColumns.Options]));
                                cmd.Parameters.Add(new SqlParameter("@OtherPhone", items[CustomerColumns.OtherPhone]));
                                cmd.Parameters.Add(new SqlParameter("@OwnerNumber", items[CustomerColumns.OwnerNumber]));
                                cmd.Parameters.Add(new SqlParameter("@JobPlan", items[CustomerColumns.Plan]));
                                cmd.Parameters.Add(new SqlParameter("@PlanType", items[CustomerColumns.PlanType]));
                                cmd.Parameters.Add(new SqlParameter("@PlanTypeDescription", items[CustomerColumns.PlanTypeDescription]));
                                cmd.Parameters.Add(new SqlParameter("@PlanName", items[CustomerColumns.PlanName]));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner1", items[CustomerColumns.PreviousOwner1]));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner2", items[CustomerColumns.PreviousOwner2]));
                                cmd.Parameters.Add(new SqlParameter("@PreviousOwner3", items[CustomerColumns.PreviousOwner3]));
                                cmd.Parameters.Add(new SqlParameter("@PrintedName", items[CustomerColumns.PrintedName]));
                                cmd.Parameters.Add(new SqlParameter("@Processed", items[CustomerColumns.Processed]));
                                cmd.Parameters.Add(new SqlParameter("@Project", items[CustomerColumns.Project]));
                                cmd.Parameters.Add(new SqlParameter("@ProjectCode", items[CustomerColumns.ProjectCode]));
                                cmd.Parameters.Add(new SqlParameter("@SalesConsultantNumber", items[CustomerColumns.SalesConsultantNumber]));
                                cmd.Parameters.Add(new SqlParameter("@SalesConsultant", items[CustomerColumns.SalesConsultant]));
                                cmd.Parameters.Add(new SqlParameter("@SateliteCode", items[CustomerColumns.SateliteCode]));
                                cmd.Parameters.Add(new SqlParameter("@SelectionSheets", items[CustomerColumns.SelectionSheets]));
                                cmd.Parameters.Add(new SqlParameter("@StateCode", items[CustomerColumns.State]));
                                cmd.Parameters.Add(new SqlParameter("@Swing", items[CustomerColumns.Swing]));
                                cmd.Parameters.Add(new SqlParameter("@TotalSalesPrice", items[CustomerColumns.TotalSalesPrice]));
                                cmd.Parameters.Add(new SqlParameter("@UnitNumber", items[CustomerColumns.UnitNumber]));
                                cmd.Parameters.Add(new SqlParameter("@WarrantyExpirationDate", items[CustomerColumns.WarrantyExpirationDate]));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone1", items[CustomerColumns.WorkPhone1]));
                                cmd.Parameters.Add(new SqlParameter("@WorkPhone2", items[CustomerColumns.WorkPhone2]));
                                cmd.Parameters.Add(new SqlParameter("@ZipCode", items[CustomerColumns.ZipCode]));
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