
UPDATE Tasks set Description = REPLACE(Description, 'warranty introduction', 'Quality Introduction of WSR')
 where Description Like '%warranty introduction%' and TaskType='8'