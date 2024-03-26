select


T1.U_k33p_SFID as Id,
T1.ItemCode

FROM

[{0}].dbo.OQUT T0 INNER JOIN [{0}].dbo.QUT1 T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [{0}].dbo.OQUT TF ON TF.DocNum = T0.DocNum

WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [{0}].dbo.QUT4 WHERE DocEntry = TF.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''
AND T0.CANCELED = 'N'

UNION ALL

select


T1.U_k33p_SFID as Id,
T1.ItemCode

FROM

[{1}].dbo.OQUT T0 INNER JOIN [{1}].dbo.QUT1 T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [{1}].dbo.OQUT TF ON TF.DocNum = T0.DocNum

WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [{1}].dbo.QUT4 WHERE DocEntry = TF.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''
AND T0.CANCELED = 'N'

UNION ALL

select


T1.U_k33p_SFID as Id,
T1.ItemCode

FROM

[{2}].dbo.OQUT T0 INNER JOIN [{2}].dbo.QUT1 T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [{2}].dbo.OQUT TF ON TF.DocNum = T0.DocNum

WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [{2}].dbo.QUT4 WHERE DocEntry = TF.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''
AND T0.CANCELED = 'N'

