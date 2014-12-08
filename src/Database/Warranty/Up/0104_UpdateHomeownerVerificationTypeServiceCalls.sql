/*
	ServiceCallStatusId 1 = Requested, 2 = Open, 3 = Closed, 4 = Homeowner Signed.
	HomeownerVerificationType 1 = Signature, 2 = Email, 3 = Phone Call, 4 = No Response, 5 = Not Verified, 6 = Imported.
*/

UPDATE ServiceCalls
SET HomeownerVerificationTypeId = CASE  WHEN ServiceCallStatusId IN (1, 2) THEN 5
										WHEN ServiceCallStatusId IN (3, 4) THEN 6
										ELSE 5
										END
WHERE HomeownerVerificationTypeId = '' OR HomeownerVerificationTypeId IS NULL