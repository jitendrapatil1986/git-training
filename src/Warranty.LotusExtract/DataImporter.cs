using System;
using System.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Impl;
using Warranty.Core.DataAccess;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Warranty.Core.Security;

namespace Warranty.LotusExtract
{
    public class DataImporter
    {
        private const char _fieldDelimiter = '╫';

        protected static Dictionary<string, int> ColumnIndexLookup = new Dictionary<string, int>();

        private ISession _session;

        public DataImporter()
        {
            var sessionFactory = new ConfigurationFactory().CreateConfigurationWithAuditing(new UserSession()).BuildSessionFactory();
            _session = sessionFactory.OpenSession();
        }

        public void Import(string fileName, string marketList)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var lotusNotesAttachmentsPath = appSettings["LotusAttachmentLocation"];

            var markets = marketList.Split(',').ToList();

            //clear out any existing documents in collection
            WipeCollection<Home>(markets);
            WipeCollection<Community>(markets);
            WipeCollection<CustomerServiceLineItem>(markets);
            WipeCollection<CustomerServiceRequest>(markets);
            WipeCollection<Division>(markets);
            WipeCollection<HomeOwner>(markets);
            WipeCollection<Project>(markets);
            WipeCollection<TeamMember>(markets);

            var file = new StreamReader(fileName);

            var headerItems = GetColumnIndexes(file).ToList();

            for (int i = 0; i < headerItems.Count; i++)
            {
                ColumnIndexLookup[headerItems[i]] = i;
            }

            string row = "";
            string line;
            while ((line = file.ReadLine()) != null)
            {
                row += line;
                if (row.Split(_fieldDelimiter).Count() == headerItems.Count())
                {
                    //TODO: Write logic to process data into entities
                }
                else
                {
                    row += "\r\n";
                }
            }

            file.Close();
        }

        private static string[] GetColumnIndexes(StreamReader file)
        {
            var line = file.ReadLine();
            var rowUnquoted = line
            .Replace("\"" + _fieldDelimiter + "\"", _fieldDelimiter.ToString())
            .TrimStart('"')
            .TrimEnd('"');

            var headerItems = rowUnquoted.Split(_fieldDelimiter);
            return headerItems;
        }

        public static void WipeCollection<T>(IList<string> markets)
        {
            //TODO: write method to wipe market data for all tables
        }
    }
}
