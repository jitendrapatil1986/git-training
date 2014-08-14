using System;
using System.Collections.Generic;

namespace Warranty.JdeImport.Importers
{
    internal class PaymentImporter : ImporterTemplate
    {
        public override string ItemName
        {
            get { return "Payments"; }
        }

        public override string SelectionQuery
        {
            get
            {
                return @"select 
                              T5AN8  as VendorNumber
                            , T5PAAP * .01 as Amount
                            , CASE WHEN T5FLAG=' ' THEN 'P' ELSE T5FLAG END as PaymentStatus --P = Pending Payment in Construction Portal
                            , trim(T5MCU) as JobNumber
                            , trim(T5MCU) || '/' || trim(T5$OPT) || '/' || trim(T5R006) || '/' || trim(T5SUB) || '/' || trim(T5OBJ) || '/' || trim(T5DCTO) || '/' || digits(T5DOCO) || '/' || digits(T5LNID) || '/' || trim(T5SFX) as JdeIdentifier
                            , '" + DateTime.UtcNow + @"' as CreatedDate
                            , 'Warranty Jde Import' as CreatedBy
                        from f58235 p";
            }
        }

        public override string DestinationTable
        {
            get { return "Payments"; }
        }

        public override int BatchSize
        {
            get { return 5000; }
        }

        public override int NotifyCount
        {
            get { return 5000; }
        }

        public override List<KeyValuePair<string, string>> ColumnMappings
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("VendorNumber", "VendorNumber"),
                    new KeyValuePair<string, string>("Amount", "Amount"),
                    new KeyValuePair<string, string>("PaymentStatus", "PaymentStatus"),
                    new KeyValuePair<string, string>("JobNumber", "JobNumber"),
                    new KeyValuePair<string, string>("JdeIdentifier", "JdeIdentifier"),
                    new KeyValuePair<string, string>("CreatedDate", "CreatedDate"),
                    new KeyValuePair<string, string>("CreatedBy", "CreatedBy"),
                };
            }
        }
    }
}