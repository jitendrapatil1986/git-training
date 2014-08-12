ALTER TABLE ServiceCalls ADD SpecialProject BIT CONSTRAINT 
    DF_ServiceCalls_SpecialProject DEFAULT 0;
