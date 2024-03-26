select distinct

T0.U_k33p_SFID as Id
,'Cotação: ' + cast(T0.DocNum as nvarchar) as [Name]  --T0.Comments as [Name]
,'Integrado' as [Status]
,T0.DocDueDate as ExpirationDate
,T0.DocNum as NumeroCotacaoSAP__c
,T0.DocDate as DataCotacaoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM QUT4 WHERE staType in(27,10,16,26)) as Tax
,T0.DocTotalFC as ValorCotacao__c
,'Brasil' as BillingCoutry
,T0.DocRate as TaxaR__c
,(SELECT SUM(TaxRate)		FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 0 and DocEntry = T0.DocEntry) as COFINS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 0 and DocEntry = T0.DocEntry) as Valor_Cofins__c 
,(SELECT SUM(TaxRate)		FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 1 and DocEntry = T0.DocEntry) as ICMS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 1 and DocEntry = T0.DocEntry) as Valor_ICMS__c 
,(SELECT SUM(TaxRate)		FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 2 and DocEntry = T0.DocEntry) as IPI__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 2 and DocEntry = T0.DocEntry) as Valor_PI__c 
,(SELECT SUM(TaxRate)		FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 3 and DocEntry = T0.DocEntry) as PIS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [SBOPRODBR].dbo.QUT4 WHERE LineSeq = 3 and DocEntry = T0.DocEntry) as Valor_PIS__c 
,T0.DocEntry as numeroDraft
,'Brasil' as pais
,(select CardName from OCRD WHERE CardCode = T.Carrier) as transp
,T.Carrier
,(SELECT SUM(VatPrcnt)	FROM  [SBOPRODBR].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVA 
,(SELECT SUM(VatSumFrgn)	FROM  [SBOPRODBR].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVAV 
FROM



[SBOPRODBR].dbo.OQUT T0 --INNER JOIN [INTEGRACAOBR].dbo.QUT4 T4 ON T0.DocEntry = T4.DocEntry
--INNER JOIN [SBOPRODBR].dbo.OQUT TF ON TF.DocNum = T0.DocNum
INNER JOIN [SBOPRODBR].dbo.QUT12 T ON T.DocEntry = T0.DocEntry

WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [SBOPRODBR].dbo.QUT4 WHERE DocEntry = T0.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''

UNION ALL

select

U_k33p_SFID as Id
,Comments as [Name]
,'Integrado' as [Status]
,DocDueDate as ExpirationDate
,DocNum as NumeroCotacaoSAP__c
,DocDate as DataCotacaoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM QUT4 WHERE staType in(27,10,16,26)) as Tax
,DocTotalFC as ValorCotacao__c
,'Argentina' as BillingCoutry
,DocRate as TaxaR__c
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 27 and DocEntry = T0.DocEntry),0) as COFINS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 27 and DocEntry = T0.DocEntry),0) as Valor_Cofins__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 10 and DocEntry = T0.DocEntry),0) as ICMS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 10 and DocEntry = T0.DocEntry),0) as Valor_ICMS__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 16 and DocEntry = T0.DocEntry),0) as IPI__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 16 and DocEntry = T0.DocEntry) ,0)as Valor_PI__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 26 and DocEntry = T0.DocEntry),0) as PIS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODAR].dbo.QUT4 WHERE staType = 26 and DocEntry = T0.DocEntry),0) as Valor_PIS__c 
,T0.DocEntry as numeroDraft
,'Argentina' as pais
,(select CardName from OCRD WHERE CardCode = T.Carrier) as transp
,T.Carrier
,(SELECT SUM(VatPrcnt)	FROM  [SBOPRODAR].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVA 
,(SELECT SUM(VatSumFrgn)	FROM  [SBOPRODAR].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVAV 
FROM

[SBOPRODAR].dbo.OQUT T0 --INNER JOIN [INTEGRACAOBR].dbo.QUT4 T4 ON T0.DocEntry = T4.DocEntry
INNER JOIN [SBOPRODAR].dbo.QUT12 T ON T.DocEntry = T0.DocEntry
WHERE 

U_k33p_SFSend = 'Q'-- A Integrar
AND T0.CANCELED = 'N'


UNION ALL

select

U_k33p_SFID as Id
,Comments as [Name]
,'Integrado' as [Status]
,DocDueDate as ExpirationDate
,DocNum as NumeroCotacaoSAP__c
,DocDate as DataCotacaoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM QUT4 WHERE staType in(27,10,16,26)) as Tax
,DocTotalFC as ValorCotacao__c
,'Chile' as BillingCoutry
,DocRate as TaxaR__c
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 27 and DocEntry = T0.DocEntry),0) as COFINS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 27 and DocEntry = T0.DocEntry),0) as Valor_Cofins__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 10 and DocEntry = T0.DocEntry),0) as ICMS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM[SBOPRODCH].dbo. QUT4 WHERE staType = 10 and DocEntry = T0.DocEntry),0) as Valor_ICMS__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 16 and DocEntry = T0.DocEntry),0) as IPI__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 16 and DocEntry = T0.DocEntry),0) as Valor_PI__c 
,isnull((SELECT SUM(TaxRate) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 26 and DocEntry = T0.DocEntry),0) as PIS__c 
,isnull((SELECT SUM(TaxSumFrgn) FROM [SBOPRODCH].dbo.QUT4 WHERE staType = 26 and DocEntry = T0.DocEntry),0) as Valor_PIS__c 
,T0.DocEntry as numeroDraft
,'Chile' as pais
,(select CardName from OCRD WHERE CardCode = T.Carrier) as transp
,T.Carrier
,(SELECT SUM(VatPrcnt)	FROM  [SBOPRODCH].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVA 
,(SELECT SUM(VatSumFrgn)	FROM  [SBOPRODCH].dbo.QUT1 WHERE DocEntry = T0.DocEntry) as IVAV 
FROM

[SBOPRODCH].dbo.OQUT T0 --INNER JOIN [INTEGRACAOBR].dbo.QUT4 T4 ON T0.DocEntry = T4.DocEntry
INNER JOIN [SBOPRODCH].dbo.QUT12 T ON T.DocEntry = T0.DocEntry
WHERE 

U_k33p_SFSend = 'Q'
AND T0.CANCELED = 'N'