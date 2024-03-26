select


T1.U_k33p_SFID as Id,
T1.ItemCode

FROM

[{0}].dbo.ORDR T0 INNER JOIN [{0}].dbo.RDR1 T1 ON T0.DocEntry = T1.DocEntry
INNER JOIN [{0}].dbo.ORDR TF ON TF.DocNum = T0.DocNum

WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [{0}].dbo.RDR4 WHERE DocEntry = TF.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''

