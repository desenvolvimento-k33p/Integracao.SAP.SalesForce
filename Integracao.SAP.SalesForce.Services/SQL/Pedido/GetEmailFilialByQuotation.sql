select 

BPLId as filial,
(select Email from OSLP WHERE SlpCode = T0.SlpCode) as Email


FROm [{0}].dbo.OQUT T0

WHERE U_k33p_SFID = '{1}' ;