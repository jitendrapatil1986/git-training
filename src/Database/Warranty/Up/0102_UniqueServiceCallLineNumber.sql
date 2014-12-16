DELETE FROM ServiceCallLineItems WHERE ServiceCallLineItemId IN
(SELECT CLI1.ServiceCallLineItemId
    FROM ServiceCallLineItems CLI1
    INNER JOIN ServiceCallLineItems CLI2 ON
        CLI1.ServiceCallId = CLI2.ServiceCallId
        AND CLI1.LineNumber = CLI2.LineNumber
        AND CLI1.ServiceCallLineItemId > CLI2.ServiceCallLineItemId)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM Payments WHERE ServiceCallLineItemId IS NOT NULL)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM ServiceCallNotes WHERE ServiceCallLineItemId IS NOT NULL)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM PurchaseOrders WHERE ServiceCallLineItemId IS NOT NULL);

DELETE FROM ServiceCallLineItems WHERE ServiceCallLineItemId IN
(SELECT CLI1.ServiceCallLineItemId
    FROM ServiceCallLineItems CLI1
    INNER JOIN ServiceCallLineItems CLI2 ON
        CLI1.ServiceCallId = CLI2.ServiceCallId
        AND CLI1.LineNumber = CLI2.LineNumber
        AND CLI1.ServiceCallLineItemId < CLI2.ServiceCallLineItemId)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM Payments WHERE ServiceCallLineItemId IS NOT NULL)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM ServiceCallNotes WHERE ServiceCallLineItemId IS NOT NULL)
AND ServiceCallLineItemId NOT IN (SELECT ServiceCallLineItemId FROM PurchaseOrders WHERE ServiceCallLineItemId IS NOT NULL);

ALTER TABLE ServiceCallLineItems ADD
CONSTRAINT UQ_ServiceCallLineItems_Id_Number UNIQUE (ServiceCallId, LineNumber);
