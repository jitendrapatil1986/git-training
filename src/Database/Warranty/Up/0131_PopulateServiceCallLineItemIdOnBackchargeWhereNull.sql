UPDATE Backcharges SET 
	ServiceCallLineItemId = p.ServiceCallLineItemId 
FROM 
	Payments p 
WHERE 
	p.PaymentId = Backcharges.PaymentId 
AND Backcharges.ServiceCallLineItemId IS NULL;