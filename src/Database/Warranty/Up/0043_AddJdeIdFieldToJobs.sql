ALTER TABLE Jobs ADD JdeIdentifier VARCHAR(255) NULL;

CREATE NONCLUSTERED INDEX IDX_Jobs_JdeId ON Jobs(JdeIdentifier);
