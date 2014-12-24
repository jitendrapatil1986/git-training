DELETE FROM Employees WHERE EmployeeNumber IN (SELECT EmployeeNumber FROM Employees GROUP BY EmployeeNumber HAVING COUNT(*) > 1)
AND EmployeeId NOT IN (SELECT EmployeeId FROM CommunityAssignments)
