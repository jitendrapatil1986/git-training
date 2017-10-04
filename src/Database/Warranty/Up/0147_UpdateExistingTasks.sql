UPDATE Tasks set Description = REPLACE(Description, '244 walk', 'warranty walk')
where Description LIKE '%244 walk%'