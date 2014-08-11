namespace Warranty.LotusExtract
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using Core.Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var connectionString = ConfigurationManager.ConnectionStrings["WarrantyDB"].ConnectionString;
            var response = "";
            var markets = "";

            do
            {
                Console.WriteLine("Lotus Data Importer");
                Console.WriteLine("Which markets are you importing (comma separated list, leave empty for all markets)?");
                markets = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("Are you sure you want to run the importer for the following markets:");
                Console.WriteLine("{0}", (markets.IsNullOrEmpty()) ? "All markets" : markets);

                Console.Write("If yes(Y) all data will be removed from the database for the listed markets:");
                response = Console.ReadLine();
            } while (response != "Y");

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("imports.DeleteByCity", sc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CityCodeList", markets));
                    cmd.ExecuteNonQuery();
                }
            }

            var warrantyCallFilePath = appSettings["ServiceCallFlatFileLocation"];
            new ServiceCallDataImporter().Import(warrantyCallFilePath, null);

            var masterCommunityFilePath = appSettings["MasterCommunityFlatFileLocation"];
            new MasterCommunityDataImporter().Import(masterCommunityFilePath, null);

            var customerFilePath = appSettings["CustomerFlatFileLocation"];
            new CustomerDataImporter().Import(customerFilePath, null);

            var jobOptionFilePath = appSettings["JobOptionFlatFileLocation"];
            new JobOptionDataImporter().Import(jobOptionFilePath, null);

            using (var sc = new SqlConnection(connectionString))
            {
                sc.Open();

                using (var cmd = new SqlCommand("imports.ImportData", sc))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
