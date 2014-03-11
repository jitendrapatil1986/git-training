namespace LotusExtract
{
    public static class WarrantyCallColumns
    {
        public static int Address = 0; //Text field for street number/name of address
        public static int Area = 1; //db lookup for area based on community. @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum";@Left(Job_Num;4); 9)
        public static int AreaPresCode = 2; //The code for area president based off of community. @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum"; @Left(Job_Num;4); 13)
        public static int Assigned_By = 3; //Username of the team member who assigned the ticket to the currently assigned team member. @Name([CN]; @V3UserName)
        public static int Assigned_To = 4; //The team member currently assigned this case. @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum";@Left(Job_Num;4);6)
        public static int BldNo = 5; //One of two fields (UnitNo is other) that shows up based on PlanType field. PlanType = "" | PlanType = "SNG"
        public static int Breaks = 6; //A text box that has no condition that I can tell
        public static int Builder = 7; //The builder who built the home
        public static int BuilderEmp_Num = 8; //The employee number of the builder who built the home
        public static int CallCompnotification = 9; //Unknown
        public static int Call_Comments = 10; //Has a label of "Work Summary:" and is a text box

        /*TimeNow := @Now;
        MonthString := @Select(@Month(TimeNow); "01"; "02"; "03"; "04"; "05"; "06"; "07"; "08"; "09"; "10"; "11"; "12");
        DayNumber := @Day(TimeNow);
        DayString := @Select(DayNumber; "01"; "02"; "03"; "04"; "05"; "06"; "07"; "08"; "09"; @Text(DayNumber));
        MinuteNumber := @Minute(TimeNow);
        MinuteString := @If(MinuteNumber = 0; "00"; @Select(MinuteNumber; "01"; "02"; "03"; "04"; "05"; "06"; "07"; "08"; "09"; @Text(MinuteNumber)));
        SecondNumber := @Second(TimeNow);
        SecondString := @If(SecondNumber = 0; "00"; @Select(SecondNumber; "01"; "02"; "03"; "04"; "05"; "06"; "07"; "08"; "09"; @Text(SecondNumber)));
        @TextToNumber(MonthString + DayString + MinuteString + SecondString)*/
        public static int Call_Num = 11; //The id of the call, the number is generated via the above code

        public static int Cause_1 = 12; //Cause_# 1-10 are on the item and hidden if this evaluates to true: !@IsDocBeingEdited & PCode_1 = ""
        public static int Cause_10 = 13;
        public static int Cause_2 = 14;
        public static int Cause_3 = 15;
        public static int Cause_4 = 16;
        public static int Cause_5 = 17;
        public static int Cause_6 = 18;
        public static int Cause_7 = 19;
        public static int Cause_8 = 20;
        public static int Cause_9 = 21;
        
        public static int CDate_1 = 22; //CDate_# 1-10: Yes/No field to mark the line item complete
        public static int CDate_10 = 23;
        public static int CDate_2 = 24;
        public static int CDate_3 = 25;
        public static int CDate_4 = 26;
        public static int CDate_5 = 27;
        public static int CDate_6 = 28;
        public static int CDate_7 = 29;
        public static int CDate_8 = 30;
        public static int CDate_9 = 31;
        
        public static int City = 32; //Part of the address, text box
        public static int CityCode = 33; //Value is looked up based on this query: @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum";@Left(Job_Num;4); 3)
        public static int CityCode_Short = 34; //Value is looked up based on this query: @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum";@Left(Job_Num;4); 14)
        public static int Close_Date = 35; //DateTime field that the user can set
        public static int CommNum = 36; //simply the value of Comm_Num which I think is from the db
        public static int CommNum_Long = 37; //CommNum plus zeros: CommNum + "0000"
        public static int Community = 38; //Community queried from db: @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum"; @Left(Job_Num;4); 2)
        public static int Comp_Date = 39; //This gets date time stamped when the last item is marked complete
        public static int Contact = 40; //A text box that appears with the homeowner and address field (assumed to be a contact name besides homeowner name)
        public static int Date_C = 41; //A field at the top of the form, text box that appears editable
        public static int Date_Conv = 42; //simply the value of Close_Date; Looks like a date conversion. Only month, day and year and always show a 4 digit year
        public static int Date_Conv_1 = 43; //another field for Close_Date; Looks like a date conversion. Only month, day and year and always show a 4 digit year
        public static int Date_O = 44; //Top of the screen but no hint on value yet
        public static int Date_Open = 45; //Date call was created; @Created formatted with Only month, day and year and always show a 4 digit year
        
        public static int Descript_1 = 46; //Descript_# 1-10 use this validation rule: @If(Descript_1 = "" & PCode_1 != "";@Failure("Item # 1 must have a Description.");@Success)
        public static int Descript_10 = 47;
        public static int Descript_2 = 48;
        public static int Descript_3 = 49;
        public static int Descript_4 = 50;
        public static int Descript_5 = 51;
        public static int Descript_6 = 52;
        public static int Descript_7 = 53;
        public static int Descript_8 = 54;
        public static int Descript_9 = 55;

        public static int DisplayAuthor = 56; //If it's a new doc use the current user as value otherwise DocAuthor: @If(@IsNewDoc; @Name([CN]; @UserName); DocAuthor)
        public static int DivCode = 57; //Lookup from the db based on community: @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum"; @Left(Job_Num;4); 11)
        public static int Division = 58; //Lookup from the db based on the community: @DbLookup("" : "NoCache"; "862564C7:0083BED0"; "CommNum"; @Left(Job_Num;4); 4)
        public static int DocAuthor = 59; //Looks like it's set as the current username when a new call is created: @Name([CN]; @V3UserName)
        public static int Duration = 60; //Calculated  based off of: temp:= ((Comp_Date - Date_Open)/86400); @If(@IsError(temp);0;temp)
        public static int Duration_1 = 61; //Calculated based off of: temp:= ((Comp_Date - Date_Conv)/86400); @If(@IsError(temp);0;temp) (doesn't appear to be different than Duration)
        public static int Duration_Pending = 62; //Duration_Pending:= ((@Today - Date_Open)/86400); @If(@IsError(Duration_Pending);0;Duration_Pending)
        public static int Duration_Pending_1 = 63; //@Text(((@Today - Date_Conv)/86400))
        public static int DurUnder7 = 64; //Looks to pull from data: DurUnder7
        public static int Elevation = 65; //Looks to pull from data: Elevation
        public static int EmailContact = 66; //Looks to pull from data: EmailContact
        public static int Fax_Num = 67; //@DbLookup("" : "NoCache"; "862564C7:0083BED0"; "WSR"; Assigned_To;2)

        /*getclosedate:=@DbLookup("" : "NoCache"; "862564B1:00565238"; "CloseDates"; Job_Num;2);
        @If(@IsError(getclosedate); "Error";getclosedate)*/
        public static int GetCloseDate = 68; //Looks up close date
        
        public static int Hbr5Close = 69; //Text area that has no default value
        public static int Homeowner = 70; //text box for Homeowner value
        public static int Home_Phone = 71; //text box for Home_Phone
        public static int HOSig = 72; //It propercases the home owner sig: @ProperCase(HOSig)
        
        public static int Item1 = 73; //Items 1 - 10 are just the field to hold the item # on the call
        public static int Item10 = 74;
        public static int Item2 = 75;
        public static int Item3 = 76;
        public static int Item4 = 77;
        public static int Item5 = 78;
        public static int Item6 = 79;
        public static int Item7 = 80;
        public static int Item8 = 81;
        public static int Item9 = 82;
        public static int Items = 83;
        
        /*@If(Items_Complete = "Yes" & Descript_1 != "" & CDate_1 = "No" ;@Failure("Item Number 1 is not complete.");
        @If(Items_Complete = "Yes" & Descript_2 != "" & CDate_2 = "No" ;@Failure("Item Number 2 is not complete.");
        @If(Items_Complete = "Yes" & Descript_3 != "" & CDate_3 = "No" ;@Failure("Item Number 3 is not complete.");
        @If(Items_Complete = "Yes" & Descript_4 != "" &CDate_4 = "No" ;@Failure("Item Number 4 is not complete.");
        @If(Items_Complete = "Yes" & Descript_5 != "" & CDate_5 = "No" ;@Failure("Item Number 5 is not complete.");
        @If(Items_Complete = "Yes" & Descript_6 != "" &  CDate_6 = "No" ;@Failure("Item Number 6 is not complete.");
        @If(Items_Complete = "Yes" & Descript_7 != "" &  CDate_7 = "No" ;@Failure("Item Number 7 is not complete.");
        @If(Items_Complete = "Yes" & Descript_8 != "" &  CDate_8 = "No" ;@Failure("Item Number 8 is not complete.");
        @If(Items_Complete = "Yes" & Descript_9 != "" &  CDate_9 = "No" ;@Failure("Item Number 9 is not complete.");
        @If(Items_Complete = "Yes" & Descript_10 != "" & CDate_10 = "No" ;@Failure("Item Number 10 is not complete.");@Success))))))))))*/
        public static int Items_Complete = 84; //If all items are marked complete, have a description and a CDate then it lets you choose this (see above)
        
        public static int Job_Num = 85; //Shows the Job_Num value, assuming this is the job number of this home
        public static int Last_Modified = 86; //If a new doc empty, then when saving stamp it with Last_Modified: @If(@IsNewDoc; ""; @If(@IsDocBeingSaved;@Now;Last_Modified))
        public static int LegalDesc = 87; //Shows the LegalDesc value which is the legal description of the job
        public static int lookupserver = 88; //It appears to do some sort of split of the DB based on city: thisserver:=@UpperCase(@Name([CN]; @Subset(@DbName; 1))); @If(thisserver="DALLAS1";"Dallas1";"Houston1")
        public static int ModifiedBy = 89; //If it's a new doc empty, else if being saved set username - @If(@IsNewDoc; ""; @If(@IsDocBeingSaved;@Name([CN]; @V3UserName);ModifiedBy))
        public static int MonthComp = 90; //Shows the value MonthComp, could be calculated off of other duration fields since those divide by # of seconds in a month
        public static int MonthQtr = 91; //Shows the value MonthQtr
        public static int MonthSort = 92; //Shows the value MonthSort
        public static int Notification = 93; //If it's a new document show Assigned_To else empty? Seems backwards: @If(@IsNewDoc;Assigned_To;"")
        public static int Notification_1 = 94; //Conditional field: @If(@IsNewDoc & Assigned_To !="No Wsr Assigned";WsrEmp_Num;"")
        public static int Notification_2 = 95; //Conditioanl with db lookup: @If(@IsNewDoc;@DbLookup("" : "NoCache"; "862564C7:0083BED0"; "WSRLU"; Notification_1;4);"")
        
        /*@If(Descript_1 != "";Descript_1;"") +@NewLine+
        @If(Descript_2 !="";Descript_2;"") +@NewLine+
        @If(Descript_3 !="";Descript_3;"")+@NewLine+
        @If(Descript_4 != "";Descript_4;"")+@NewLine+
        @If(Descript_5 !="";Descript_5;"") +@NewLine+
        @If(Descript_6 !="";Descript_6;"")+@NewLine+
        @If(Descript_7 != "";Descript_7;"") +@NewLine+
        @If(Descript_8 !="";Descript_8;"") +@NewLine+
        @If(Descript_9 !="";Descript_9;"") +@NewLine+
        @If(Descript_10 !="";Descript_10;"")*/
        public static int Notification_Call_Items = 96; //Looks to be a field that simply concatenates all of the line item descriptions

        public static int Notification_Homeowner = 97; //Shows the homeowner contact info: Homeowner + "   # " + Home_Phone + "  " + Community+ " -" + Address
        public static int NumCalls = 98; //Calculated based on # of Descript_# fields have a non empty string value
        public static int Orig_CompDate = 99;
        public static int Other_Phone = 100;
        public static int OwnNum = 101;
        
        public static int PCode_1 = 102; //PCode_1-10 go with the Item_# fields, queries the db for @Unique(@DbColumn("" : "NoCache"; ""; "PROB"; 1)) drop down
        public static int PCode_10 = 103;
        public static int PCode_2 = 104;
        public static int PCode_3 = 105;
        public static int PCode_4 = 106;
        public static int PCode_5 = 107;
        public static int PCode_6 = 108;
        public static int PCode_7 = 109;
        public static int PCode_8 = 110;
        public static int PCode_9 = 111;
        
        public static int Plan = 112;
        public static int PlanType = 113;
        public static int PlanTypeDesc = 114;
        public static int Plan_Name = 115;
        public static int Printed_Name = 116;
        public static int Project = 117;
        public static int ProjectCode = 118;
        public static int RequestCalc = 119;
        
        public static int ResCode_1 = 120; //ResCode_# 1-10 validate with: @If(Descript_1 != "" & CDate_1 = "Yes" & ResCode_1 = "";@Failure("Item 1 is missing a Classification.");@Success)
        public static int ResCode_10 = 121;
        public static int ResCode_10_1 = 122;
        public static int ResCode_1_1 = 123;
        public static int ResCode_2 = 124;
        public static int ResCode_2_1 = 125;
        public static int ResCode_3 = 126;
        public static int ResCode_3_1 = 127;
        public static int ResCode_4 = 128;
        public static int ResCode_4_1 = 129;
        public static int ResCode_5 = 130;
        public static int ResCode_5_1 = 131;
        public static int ResCode_6 = 132;
        public static int ResCode_6_1 = 133;
        public static int ResCode_7 = 134;
        public static int ResCode_7_1 = 135;
        public static int ResCode_8 = 136;
        public static int ResCode_8_1 = 137;
        public static int ResCode_9 = 138;
        public static int ResCode_9_1 = 139;

        public static int Root_1 = 140; //Root_# 1-10 are hidden if this evaluates true: !@IsDocBeingEdited & PCode_1 = ""
        public static int Root_10 = 141;
        public static int Root_10_1 = 142;
        public static int Root_1_1 = 143; //Root_#_# 1-1 - 10-1 are a look up based on: "o "+@If(PCode_1 !="" & Root_1 ="";@DbLookup("" : "NoCache"; ""; "PROB_1"; PCode_1; 3);Root_1)
        public static int Root_2 = 144;
        public static int Root_2_1 = 145;
        public static int Root_3 = 146;
        public static int Root_3_1 = 147;
        public static int Root_4 = 148;
        public static int Root_4_1 = 149;
        public static int Root_5 = 150;
        public static int Root_5_1 = 151;
        public static int Root_6 = 152;
        public static int Root_6_1 = 153;
        public static int Root_7 = 154;
        public static int Root_7_1 = 155;
        public static int Root_8 = 156;
        public static int Root_8_1 = 157;
        public static int Root_9 = 158;
        public static int Root_9_1 = 159;
        
        public static int SalesEmp_Num = 160;
        public static int Sales_Consultant = 161;
        public static int SateliteCode = 162;
        public static int Select2YearComp = 163;
        public static int Server_Name = 164;
        public static int State = 165;
        public static int Swing = 166;
        public static int Type = 167;
        public static int unid = 168;
        public static int UnitNo = 169; //One of two fields (BldNo is other) that shows up based on PlanType field. PlanType = "" | PlanType = "SNG"
        public static int User_Defined = 170;
        public static int VendorCode_1 = 171; //VendorCode_# are calculated by:
        public static int VendorCode_10 = 172; //@If(PCode_1="";@Return("");""); 
        public static int VendorCode_2 = 173; //vendor:=@Unique(@DbLookup("" : "NoCache"; lookupserver : "Purchasing\\Vendors.nsf"; "VC"; PCode_1 + Job_Num;3)); 
        public static int VendorCode_3 = 174; //@If(@IsError(vendor); " "; @NewLine + vendor + @NewLine)
        public static int VendorCode_4 = 175;
        public static int VendorCode_5 = 176;
        public static int VendorCode_6 = 177;
        public static int VendorCode_7 = 178;
        public static int VendorCode_8 = 179;
        public static int VendorCode_9 = 180;
        public static int VendorName_1 = 181; //VendorName_# are calculated by:
        public static int VendorName_10 = 182; //@If(PCode_1="";@Return("");"");
        public static int VendorName_2 = 183; //vendor:=@Unique(@DbLookup("" : "NoCache"; lookupserver : "Purchasing\\Vendors.nsf"; "VC"; PCode_1 + Job_Num;3));
        public static int VendorName_3 = 184; //@If(@IsError(vendor); " Vendor Not Found  "; vendor)
        public static int VendorName_4 = 185;
        public static int VendorName_5 = 186;
        public static int VendorName_6 = 187;
        public static int VendorName_7 = 188;
        public static int VendorName_8 = 189;
        public static int VendorName_9 = 190;
        public static int VendorPhoneNum_1 = 191; //VendorPhoneNum_# are calculated by:
        public static int VendorPhoneNum_10 = 192; //@If(PCode_1="";@Return("");"");
        public static int VendorPhoneNum_2 = 193; //vendor:=@Unique(@DbLookup("" : "NoCache"; lookupserver : "Purchasing\\Vendors.nsf"; "VC"; PCode_1 + Job_Num;5));
        public static int VendorPhoneNum_3 = 194; //@If(@IsError(vendor); " "; @NewLine + vendor)
        public static int VendorPhoneNum_4 = 195;
        public static int VendorPhoneNum_5 = 196;
        public static int VendorPhoneNum_6 = 197;
        public static int VendorPhoneNum_7 = 198;
        public static int VendorPhoneNum_8 = 199;
        public static int VendorPhoneNum_9 = 200;
        public static int WorkPhone_1 = 201;
        public static int WorkPhone_2 = 202;
        public static int WSFU = 203;
        public static int WSFU_1 = 204;
        public static int WsrEmp_Num = 205;
        public static int YearComp = 206;
        public static int ZipCode = 207;
        public static int Return = 208;
    }
}