	


	
select T7.TaxId0 FROm [{0}].dbo.CRD7 T7 INNER JOIN [{0}].dbo.OCRD T0 ON T7.CardCode = T0.CardCode WHERE ISNULL(T0."U_k33p_SFID", '') = '{1}'