IF NOT EXISTS (SELECT 1 FROM sysdatabases WHERE ('[' + name + ']' = '{{DatabaseName}}_NServicebus' OR name = '{{DatabaseName}}_NServicebus'))
CREATE DATABASE {{DatabaseName}}_NServicebus