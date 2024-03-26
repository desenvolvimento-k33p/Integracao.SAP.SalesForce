Select T1.OcrCode2 


FROm [{0}].dbo.QUT1 T1 INNER JOIN [{0}].dbo.OQUT T0 ON T1.DocEntry = T0.DocEntry
WHERE T0.U_k33p_SFID = '{1}'
AND LineNum = 0;