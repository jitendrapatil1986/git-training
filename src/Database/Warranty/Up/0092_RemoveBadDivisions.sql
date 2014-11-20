UPDATE C SET DivisionId = C2.DivisionId
FROM Communities C
INNER JOIN Divisions C1 ON
    C.DivisionId = C1.DivisionId
    AND (C1.DivisionCode = '' OR C1.DivisionCode IS NULL)
LEFT JOIN Divisions C2 ON
    C1.DivisionName = C2.DivisionName
    and C2.DivisionCode != ''

DELETE FROM Divisions WHERE Divisions.DivisionId NOT IN (SELECT C.DivisionId FROM Communities C WHERE C.DivisionId IS NOT NULL);

UPDATE C SET DivisionId = C2.DivisionId
FROM Communities C
INNER JOIN Divisions C1 ON
    C.DivisionId = C1.DivisionId
INNER JOIN Divisions C2 ON
    C1.DivisionCode = C2.DivisionCode
    and C1.DivisionId > C2.DivisionId

DELETE FROM Divisions WHERE Divisions.DivisionId NOT IN (SELECT C.DivisionId FROM Communities C WHERE C.DivisionId IS NOT NULL);
