DELETE FROM [{2}].dbo.[@K33P_TOKENSF];
INSERT INTO [{2}].dbo.[@K33P_TOKENSF]
           ([Code]
           ,[Name]
           ,[U_K33P_TOKEN]
           ,[U_K33P_DATA])
     VALUES
           ((SELECT (SELECT ISNULL(MAX(ISNULL(Code,0)),0) FROM [{2}].dbo.[@K33P_TOKENSF]) + 1),CAST((SELECT (SELECT ISNULL(MAX(ISNULL(Code,0)),0) FROM [{2}].dbo.[@K33P_TOKENSF]) + 1) as nvarchar),'{0}','{1}');