using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Warranty.LotusExtract
{
    using System;
    using System.Configuration;
    using Core.Extensions;

    class Program
    {
        private static string _connectionString;

        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            _connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;
            var response = "";
            var markets = "";
            var archivedWarranty = "";
            var archivedCustomer = "";

            Console.WriteLine("Do you want to attachment file names? <Y>Yes <N>No");
            if (Console.ReadLine() == "Y")
            {
                Console.WriteLine("Path to deployed attachments (include trailing \\):");
                ParseFileNames(Console.ReadLine());
                return;
            }

            do
            {
                Console.Write("Supply archived warranty file path if you want to import archived data:");
                archivedWarranty = Console.ReadLine();

                Console.Write("Supply archived customer file path if you want to import archived data:");
                archivedCustomer = Console.ReadLine();

                Console.WriteLine("Lotus Data Importer");
                Console.WriteLine("Which markets are you importing (comma separated list, leave empty for all markets)?");
                markets = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to run the importer for the following markets:");
                Console.WriteLine("{0}", (markets.IsNullOrEmpty()) ? "All markets" : markets);

                Console.Write("If yes(Y) all data for the listed markets will be updated:");
                response = Console.ReadLine();
            } while (response != "Y");

            var warrantyCallFilePath = appSettings["ServiceCallFlatFileLocation"];
            new ServiceCallDataImporter().Import(warrantyCallFilePath, null, archivedWarranty);

            var masterCommunityFilePath = appSettings["MasterCommunityFlatFileLocation"];
            new MasterCommunityDataImporter().Import(masterCommunityFilePath, null);

            var customerFilePath = appSettings["CustomerFlatFileLocation"];
            new CustomerDataImporter().Import(customerFilePath, null, archivedCustomer);

            var jobOptionFilePath = appSettings["JobOptionFlatFileLocation"];
            new JobOptionDataImporter().Import(jobOptionFilePath, null);

            using (var sc = new SqlConnection(_connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("imports.ImportData", sc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CityCodeList", markets));
                    cmd.CommandTimeout = 6000;
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand("imports.ImportHomeownerFromCall", sc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CityCodeList", markets));
                    cmd.CommandTimeout = 6000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void ParseFileNames(string sharePath)
        {
            var sql = @"INSERT INTO JobAttachments (JobId, FilePath, DisplayName, IsDeleted, CreatedDate, CreatedBy)
                        SELECT
                            ji.jobid
                            , '" + sharePath + @"' + ltrim(rtrim(split.item))
                            , ltrim(rtrim(split.item))
                            , 0
                            , getdate()
                            , 'Lotus Import'
                        FROM
                        imports.CustomerImports i
                        cross apply [dbo].[DelimitedSplit8K](filenames, ', ') split
                        inner join (SELECT jobnumber, min(JobId) as jobid FROM Jobs J group by jobnumber) AS ji on
                            Ji.JobNumber = i.JobNumber 
                        inner join homeowners c on 
                            c.JobId = ji.JobId 
                            AND (LOWER(LTRIM(RTRIM(c.HomeOwnerName))) = LOWER(LTRIM(RTRIM(i.Homeowner))) 
                                    OR c.HomeownerNumber = i.OwnerNumber)
                        where 
                            (    
                                fileNames like '%, ' +  ltrim(rtrim(split.item)) + '%'
                                OR fileNames like ltrim(rtrim(split.item)) + ',%'
                                OR fileNames like ltrim(rtrim(split.item))
                            )
                            and ltrim(rtrim(filenames)) != ''
                            and ltrim(rtrim(split.item)) like '%.%'
                            and '" + sharePath + @"' + ltrim(rtrim(split.item)) not in (select filePath from JobAttachments)";

            const string badFiles = @"SELECT
                                    'Job Number = ' + ISNULL(I.JobNumber, '') + ' File Name = ' + ISNULL(ltrim(rtrim(split.item)), '')
                                FROM
                                imports.CustomerImports i
                                cross apply [dbo].[DelimitedSplit8K](filenames, ', ') split
                                inner join (SELECT jobnumber, min(JobId) as jobid FROM Jobs J group by jobnumber) AS ji on
                                    Ji.JobNumber = i.JobNumber 
                                inner join homeowners c on 
                                    c.JobId = ji.JobId 
                                    AND (LOWER(LTRIM(RTRIM(c.HomeOwnerName))) = LOWER(LTRIM(RTRIM(i.Homeowner))) 
                                            OR c.HomeownerNumber = i.OwnerNumber)
                                where 
                                    ltrim(rtrim(split.item)) NOT like '%.%'
                                    AND ltrim(rtrim(filenames)) != ''";

            using (var sc = new SqlConnection(_connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand(sql, sc))
                    cmd.ExecuteNonQuery();
            }

            using (var sc = new SqlConnection(_connectionString))
            {
                sc.Open();
                var rpt = String.Empty;


                using (var cmd = new SqlCommand(badFiles, sc))
                    using(var reader = cmd.ExecuteReader())
                        if(reader != null)
                            while (reader.Read())
                                rpt += reader.GetString(0) + Environment.NewLine;

                File.WriteAllText("badFiles.txt", rpt);
            }
        }
    }
}
