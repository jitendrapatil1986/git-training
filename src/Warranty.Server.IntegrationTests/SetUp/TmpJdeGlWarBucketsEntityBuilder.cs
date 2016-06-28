using System;
using NPoco;
using Warranty.Server.IntegrationTests.Handlers.Reports;

namespace Warranty.Server.IntegrationTests.SetUp
{
    public class TmpJdeGlWarBucketsEntityBuilder : EntityBuilder<Tmp_Jde_Gl_War_Buckets>
    {
        public TmpJdeGlWarBucketsEntityBuilder(IDatabase database) : base(database)
        {
        }

        public override Tmp_Jde_Gl_War_Buckets GetSaved(Action<Tmp_Jde_Gl_War_Buckets> action)
        {
            var entity = new Tmp_Jde_Gl_War_Buckets
            {
                JAN = 200,
                FEB = 200,
                MAR = 200,
                APR = 200,
                MAY = 200,
                JUN = 200,
                JUL = 200,
                AUG = 200,
                SEP = 200,
                OCT = 200,
                NOV = 200,
                DEC = 200,
            };

            return Saved(entity, action);
        }
    }
}