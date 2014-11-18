UPDATE C SET CityId = C2.CityId
FROM Communities C
INNER JOIN Cities C1 ON
    C.CityId = C1.CityId
    AND C1.CityCode = ''
INNER JOIN Cities C2 ON
    C1.CityName = C2.CityName
    and C2.CityCode != ''

UPDATE C SET SateliteCityId = C2.CityId
FROM Communities C
INNER JOIN Cities C1 ON
    C.SateliteCityId = C1.CityId
    AND C1.CityCode = ''
INNER JOIN Cities C2 ON
    C1.CityName = C2.CityName
    and C2.CityCode != ''

DELETE FROM Cities WHERE CityId NOT IN (SELECT CityId FROM Communities) 
AND CityId NOT IN (SELECT SateliteCityId FROM Communities);
