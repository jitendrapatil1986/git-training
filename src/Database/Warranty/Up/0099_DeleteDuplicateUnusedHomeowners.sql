DELETE FROM Homeowners WHERE HomeownerId NOT IN (SELECT CurrentHomeownerId FROM Jobs)
AND HomeownerId NOT IN (SELECT HomeownerId FROM HomeownerContacts)
AND EXISTS (SELECT 1 FROM Homeowners H WHERE H.JobId = Homeowners.Jobid AND LOWER(LTRIM(RTRIM(H.HomeownerName))) = LOWER(LTRIM(RTRIM(Homeowners.HomeownerName))) AND H.HomeownerId != Homeowners.HomeownerId)
