--SELECT OcrCode  FROm [{0}].dbo.OOCR  WHERE OCrName = (select SlpName from [{0}].dbo.OSLP WHERE SlpCode = (select SlpCode from [{0}].dbo.OCRD WHERE CardCode = '{1}'));

Select CostCenter from {0}.dbo.OHEM where salesPrson = (Select SlpCode from OSLP where U_IdSF = '{1}');