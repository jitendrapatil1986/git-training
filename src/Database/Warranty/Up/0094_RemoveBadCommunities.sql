DELETE FROM CommunityAssignments WHERE CommunityId in (SELECT CommunityId FROM Communities WHERE LEN(CommunityNumber) != 4 OR ISNUMERIC(CommunityNumber) = 0)
DELETE FROM Communities WHERE LEN(CommunityNumber) != 4 OR ISNUMERIC(CommunityNumber) = 0

UPDATE J SET CommunityId = C2.CommunityId
FROM Jobs J
INNER JOIN Communities C1 ON
    C1.CommunityId = J.CommunityId
INNER JOIN Communities C2 ON
    C1.CommunityNumber = C2.CommunityNumber
    AND C1.CommunityId > C2.CommunityId

DELETE FROM CommunityAssignments WHERE CommunityId NOT IN (SELECT CommunityId FROM Jobs)
AND CommunityId IN (select CommunityId from Communities where CommunityNumber IN (
                        SELECT CommunityNumber FROM Communities GROUP BY CommunityNumber HAVING COUNT(*) > 1));

DELETE FROM Communities WHERE CommunityId NOT IN (SELECT CommunityId FROM Jobs)
AND CommunityId IN (select CommunityId from Communities where CommunityNumber IN (
                        SELECT CommunityNumber FROM Communities GROUP BY CommunityNumber HAVING COUNT(*) > 1));
