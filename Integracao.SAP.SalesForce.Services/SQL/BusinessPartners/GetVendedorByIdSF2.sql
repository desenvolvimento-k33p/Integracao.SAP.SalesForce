--select ISNULL(SlpName,'') as 'codigo' from [{0}].dbo.OSLP WHERE "U_IdSF" = '{1}'


select empId from [{0}].dbo.OHEM WHERE salesPrson = {1};