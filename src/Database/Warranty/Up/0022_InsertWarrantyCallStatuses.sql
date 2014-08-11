INSERT INTO lookups.WarrantyCallStatuses
    SELECT 1, 'Requested'
    UNION ALL SELECT 2, 'Open'
    UNION ALL SELECT 3, 'Special Project'
    UNION ALL SELECT 4, 'Closed'