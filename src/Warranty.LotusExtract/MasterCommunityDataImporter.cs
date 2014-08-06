using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Warranty.LotusExtract.Columns;

namespace Warranty.LotusExtract
{
    public class MasterCommunityDataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        public void Import(string fileName, string marketList)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("TRUNCATE TABLE imports.MasterCommunityImports", sc))
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

                            var sql = @"INSERT INTO imports.MasterCommunityImports (City, Division, Project, Community, WarrantyServiceRepresentative, AreaPresident, AreaPresidentCode, BuildOnYourLot, CityCode, CommunityId, CommunityNumber, CommunityNumberOld, DivisionCode, DivisionPresidentEmployeeID, DocumentAuthor, DocumentAuthorEmployeeId, DocumentAuthorFax, PlanTypeDesc, ProjectManagerEmployeeID, ProjectCode, Satelite, SateliteCode, CommunityStatus, StatusDescription, TypeCC)
                                        SELECT @City, @Division, @Project, @Community, @WarrantyServiceRepresentative, @AreaPresident, @AreaPresidentCode, @BuildOnYourLot, @CityCode, @CommunityId, @CommunityNumber, @CommunityNumberOld, @DivisionCode, @DivisionPresidentEmployeeID, @DocumentAuthor, @DocumentAuthorEmployeeId, @DocumentAuthorFax, @PlanTypeDesc, @ProjectManagerEmployeeID, @ProjectCode, @Satelite, @SateliteCode, @CommunityStatus, @StatusDescription, @TypeCC";

                            using (var cmd = new SqlCommand(sql, sc))
                            {
                                cmd.Parameters.Add(new SqlParameter("@City", items[MasterCommunityColumns.City]));
                                cmd.Parameters.Add(new SqlParameter("@Division", items[MasterCommunityColumns.Division]));
                                cmd.Parameters.Add(new SqlParameter("@Project", items[MasterCommunityColumns.Project]));
                                cmd.Parameters.Add(new SqlParameter("@Community", items[MasterCommunityColumns.Community]));
                                cmd.Parameters.Add(new SqlParameter("@WarrantyServiceRepresentative", items[MasterCommunityColumns.WarrantyServiceRepresentative]));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresident", items[MasterCommunityColumns.AreaPresident]));
                                cmd.Parameters.Add(new SqlParameter("@AreaPresidentCode", items[MasterCommunityColumns.AreaPresidentCode]));
                                cmd.Parameters.Add(new SqlParameter("@BuildOnYourLot", items[MasterCommunityColumns.BuildOnYourLot]));
                                cmd.Parameters.Add(new SqlParameter("@CityCode", items[MasterCommunityColumns.CityCode]));
                                cmd.Parameters.Add(new SqlParameter("@CommunityId", items[MasterCommunityColumns.CommunityId]));
                                cmd.Parameters.Add(new SqlParameter("@CommunityNumber", items[MasterCommunityColumns.CommunityNumber]));
                                cmd.Parameters.Add(new SqlParameter("@CommunityNumberOld", items[MasterCommunityColumns.CommunityNumberOld]));
                                cmd.Parameters.Add(new SqlParameter("@DivisionCode", items[MasterCommunityColumns.DivisionCode]));
                                cmd.Parameters.Add(new SqlParameter("@DivisionPresidentEmployeeID", items[MasterCommunityColumns.DivisionPresidentEmployeeID]));
                                cmd.Parameters.Add(new SqlParameter("@DocumentAuthor", items[MasterCommunityColumns.DocumentAuthor]));
                                cmd.Parameters.Add(new SqlParameter("@DocumentAuthorEmployeeId", items[MasterCommunityColumns.DocumentAuthorEmployeeId]));
                                cmd.Parameters.Add(new SqlParameter("@DocumentAuthorFax", items[MasterCommunityColumns.DocumentAuthorFax]));
                                cmd.Parameters.Add(new SqlParameter("@PlanTypeDesc", items[MasterCommunityColumns.PlanTypeDesc]));
                                cmd.Parameters.Add(new SqlParameter("@ProjectManagerEmployeeID", items[MasterCommunityColumns.ProjectManagerEmployeeID]));
                                cmd.Parameters.Add(new SqlParameter("@ProjectCode", items[MasterCommunityColumns.ProjectCode]));
                                cmd.Parameters.Add(new SqlParameter("@Satelite", items[MasterCommunityColumns.Satelite]));
                                cmd.Parameters.Add(new SqlParameter("@SateliteCode", items[MasterCommunityColumns.SateliteCode]));
                                cmd.Parameters.Add(new SqlParameter("@CommunityStatus", items[MasterCommunityColumns.Status]));
                                cmd.Parameters.Add(new SqlParameter("@StatusDescription", items[MasterCommunityColumns.StatusDescription]));
                                cmd.Parameters.Add(new SqlParameter("@TypeCC", items[MasterCommunityColumns.TypeCC]));
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