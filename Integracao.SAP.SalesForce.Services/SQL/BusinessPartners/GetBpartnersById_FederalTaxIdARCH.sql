SELECT DISTINCT
	T0."CardCode",T0."U_k33p_SFID"
FROM [{1}].dbo.OCRD T0
	--INNER JOIN [{1}].dbo.CRD7 T1 ON T1."CardCode" = T0."CardCode"
WHERE 
ISNULL(T0.LicTradNum, '') = '{0}'
	--ISNULL(T0.U_Rut, '') = '{0}'
	--AND T1.Address = ''
	--AND T1."AddrType" = 'S'
	--AND T0."CardType" = 'C'
