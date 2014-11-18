UPDATE C SET ProjectId = C2.ProjectId
FROM Communities C
INNER JOIN Projects C1 ON
    C.ProjectId = C1.ProjectId
    AND (C1.ProjectNumber = '' OR C1.ProjectNumber IS NULL)
LEFT JOIN Projects C2 ON
    C1.ProjectName = C2.ProjectName
    and C2.ProjectNumber != ''

DELETE FROM Projects WHERE Projects.ProjectId NOT IN (SELECT C.ProjectId FROM Communities C WHERE C.ProjectId IS NOT NULL);

UPDATE C SET ProjectId = C2.ProjectId
FROM Communities C
INNER JOIN Projects C1 ON
    C.ProjectId = C1.ProjectId
INNER JOIN Projects C2 ON
    C1.ProjectNumber = C2.ProjectNumber
    and C1.ProjectId > C2.ProjectId

DELETE FROM Projects WHERE Projects.ProjectId NOT IN (SELECT C.ProjectId FROM Communities C WHERE C.ProjectId IS NOT NULL);
