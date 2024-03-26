
--select CostCenter FROM [{0}].dbo.OHEM where U_CFS_OCRCODE2 = '{1}'

SELECT T0.[PrcCode] FROM OPRC T0 WHERE T0.[DimCode] = 2 and U_BU = '{1}'