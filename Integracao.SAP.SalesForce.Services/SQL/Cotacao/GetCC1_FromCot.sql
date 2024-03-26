--select CostCenter FROm [{0}].dbo.OHEM WHERE Email = '{1}';
--Select CostCenter from {0}.dbo.OHEM where salesPrson = (Select SlpCode from {0}.dbo.OSLP where U_IdSF = '{1}');

Select T1.OcrCode 


FROm [{0}].dbo.QUT1 T1 INNER JOIN [{0}].dbo.OQUT T0 ON T1.DocEntry = T0.DocEntry
WHERE T0.U_k33p_SFID = '{1}'
AND LineNum = 0;