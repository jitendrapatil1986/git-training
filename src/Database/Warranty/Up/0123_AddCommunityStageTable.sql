CREATE TABLE imports.CommunityStage(
    RowId UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_CommunityStage_RowId PRIMARY KEY
        CONSTRAINT DF_CommunityStage_RowId DEFAULT NEWSEQUENTIALID(),
    City VARCHAR(4000),
    Division VARCHAR(4000),
    Project VARCHAR(4000),
    Comm_Num VARCHAR(4000),
    Community VARCHAR(4000),
    Satelite VARCHAR(4000),
    SateliteCode VARCHAR(4000),
    AreaPresCode VARCHAR(4000),
    StatusCode VARCHAR(4000),
    StatusDescription VARCHAR(4000),
    TypeCC VARCHAR(4000),
    AreaPres VARCHAR(4000),
    BOYL VARCHAR(4000),
    DivPresEmpID VARCHAR(4000),
    PMEmpID VARCHAR(4000),
    PlantypeDesc VARCHAR(4000),
    DivCode VARCHAR(4000),
    ProjectCode VARCHAR(4000),
    CityCode VARCHAR(4000)
)