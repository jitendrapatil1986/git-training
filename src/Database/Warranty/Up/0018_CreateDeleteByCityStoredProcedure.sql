CREATE PROCEDURE imports.DeleteByCity @CityCodeList VARCHAR(4000) AS

SELECT CommunityId INTO #CommunitiesToDelete
    FROM Communities 
    WHERE CityId IN (SELECT CityId FROM Cities WHERE ',' + @CityCodeList + ',' LIKE '%,' + CityCode + ',%' OR @CityCodeList IS NULL);

UPDATE Jobs SET CurrentHomeOwnerId = null WHERE
    CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete);

DELETE FROM HomeOwners WHERE
    JobId IN (SELECT JobId FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete));

DELETE FROM JobOptions WHERE
    JobId IN (SELECT JobId FROM Jobs
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete));

DELETE FROM ServiceCallComments WHERE
    ServiceCallId IN (SELECT ServiceCallId 
                            FROM ServiceCalls
                            WHERE JobId IN (SELECT JobId FROM Jobs 
                                                WHERE CommunityId IN (SELECT CommunityId 
                                                                        FROM #CommunitiesToDelete)));

DELETE FROM ServiceCallLineItems WHERE
    ServiceCallId IN (SELECT ServiceCallId 
                            FROM ServiceCalls
                            WHERE JobId IN (SELECT JobId FROM Jobs 
                                                WHERE CommunityId IN (SELECT CommunityId 
                                                                        FROM #CommunitiesToDelete)));

DELETE FROM ServiceCalls WHERE
    JobId IN (SELECT JobId FROM Jobs 
                WHERE CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete));

DELETE FROM Jobs WHERE
    CommunityId IN (SELECT CommunityId FROM #CommunitiesToDelete);