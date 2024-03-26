select 
			--(SELECT [Name] from OSTT WHERE AbsId = T4.staType) as "Imposto"
			T0.[Name] as "Imposto"
			,TaxRate
			,TaxSum
			,TaxSumFrgn
			
			 from [{0}].dbo.DRF4 T4 INNER JOIN [{0}].dbo.OSTT T0 ON T0.AbsId = T4.staType
			 WHERE T4.DocEntry = {1}

			 group by 
			 T0.[Name]
			,TaxRate
			,TaxSum
			,TaxSumFrgn