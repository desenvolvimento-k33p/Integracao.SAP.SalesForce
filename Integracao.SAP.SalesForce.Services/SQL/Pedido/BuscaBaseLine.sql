Select T1.LineNum 


FROm [{0}].dbo.OQUT T0 INNER JOIN [{0}].dbo.QUT1 T1 On T0.DocEntry = T1.DocEntry
WHERE T0.U_k33p_SFID = '{1}' AND T1.U_k33p_SFID = '{2}';