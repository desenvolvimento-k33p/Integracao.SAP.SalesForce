--select CostCenter FROm [{0}].dbo.OHEM WHERE Email = '{1}';
--Select CostCenter from {0}.dbo.OHEM where salesPrson = (Select SlpCode from {0}.dbo.OSLP where U_IdSF = '{1}');

Select T0.DocDueDate 


FROm [{0}].dbo.OQUT T0 
WHERE T0.U_k33p_SFID = '{1}';
