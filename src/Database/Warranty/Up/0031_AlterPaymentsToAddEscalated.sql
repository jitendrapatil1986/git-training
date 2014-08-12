ALTER TABLE ServiceCalls ADD Escalated BIT CONSTRAINT
    DF_ServiceCalls_Escalated DEFAULT 0;