UPDATE ServiceCalls SET ServiceCallStatusId = S.ServiceCallStatusId
FROM lookups.ServiceCallStatuses S
WHERE
    (
        CASE WHEN CompletionDate = '' THEN NULL ELSE CompletionDate END IS NULL
        AND S.ServiceCallStatus = 'Open'
    )
    OR
    (
        CASE WHEN CompletionDate = '' THEN NULL ELSE CompletionDate END IS NOT NULL
        AND S.ServiceCallStatus = 'Closed'
    )
