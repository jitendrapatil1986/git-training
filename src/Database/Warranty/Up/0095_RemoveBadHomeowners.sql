DELETE FROM Homeowners WHERE HomeownerId NOT IN (SELECT CurrentHomeownerId FROM Jobs)
AND HomeownerId NOT IN (SELECT HomeownerId FROM HomeownerContacts)
AND EXISTS (SELECT 1 FROM (SELECT JobId, HomeownerNumber
                            FROM Homeowners
                            GROUP BY JobId, HomeownerNumber
                            HAVING COUNT(*) > 1) as dup
            WHERE dup.JobId = JobId 
            AND dup.HomeownerNumber = HomeownerNumber)
