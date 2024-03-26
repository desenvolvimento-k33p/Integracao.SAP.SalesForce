--select CostCenter FROm [{0}].dbo.OHEM WHERE Email = '{1}';
--Select CostCenter from {0}.dbo.OHEM where salesPrson = (Select SlpCode from {0}.dbo.OSLP where U_IdSF = '{1}');




Select ItemCode from {0}.dbo.OITM 
Where trim(ItemName) = '{1}' -- NomeDoProduto__c
And U_Lancado = 'S'
And U_Principal = 'S'
And SUBSTRING(ItemCode,10,2) = '{2}'; --TipoEmbalagem__c