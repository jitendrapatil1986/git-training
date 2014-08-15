DELETE rowsToDelete FROM (
  SELECT
    *
    , ROW_NUMBER() OVER (PARTITION BY JdeIdentifier ORDER BY RequestedDate) AS rowNum
  FROM Payments 
) AS rowsToDelete
WHERE rowNum > 1;
