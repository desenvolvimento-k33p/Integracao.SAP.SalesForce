select distinct


T0.U_k33p_SFID as Id
,'Pedido: ' + cast(T0.DocNum as nvarchar) as [Name]--T0.Comments as [Name]
,'Integrado' as [Status]
,T0.DocDueDate as ExpirationDate
,T0.DocNum as NumeroPedidoSAP__c
,T0.DocDate as DataPedidoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM QUT4 WHERE staType in(27,10,16,26)) as Tax
,T0.DocTotalFC as ValorCotacao__c
,'Brasil' as BillingCoutry
,T0.DocRate as TaxaR__c
,(SELECT SUM(TaxRate)		FROM  [{0}].dbo.RDR4 WHERE LineSeq = 0 and DocEntry = T0.DocEntry) as COFINS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [{0}].dbo.RDR4 WHERE LineSeq = 0 and DocEntry = T0.DocEntry) as Valor_Cofins__c 
,(SELECT SUM(TaxRate)		FROM  [{0}].dbo.RDR4 WHERE LineSeq = 1 and DocEntry = T0.DocEntry) as ICMS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [{0}].dbo.RDR4 WHERE LineSeq = 1 and DocEntry = T0.DocEntry) as Valor_ICMS__c 
,(SELECT SUM(TaxRate)		FROM  [{0}].dbo.RDR4 WHERE LineSeq = 2 and DocEntry = T0.DocEntry) as IPI__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [{0}].dbo.RDR4 WHERE LineSeq = 2 and DocEntry = T0.DocEntry) as Valor_PI__c 
,(SELECT SUM(TaxRate)		FROM  [{0}].dbo.RDR4 WHERE LineSeq = 3 and DocEntry = T0.DocEntry) as PIS__c 
,(SELECT SUM(TaxSumFrgn)	FROM  [{0}].dbo.RDR4 WHERE LineSeq = 3 and DocEntry = T0.DocEntry) as Valor_PIS__c 
,T0.DocEntry as numeroDraft
,'Brasil' as pais
,T0.BPLId as filial
,(select CardName from OCRD WHERE CardCode = TF.Carrier) as transp
,T0.GroupNum as CondicaoPagamento__c

FROM

[{0}].dbo.ORDR T0
INNER JOIN [{0}].dbo.RDR12 TF ON TF.DocEntry = T0.DocEntry


WHERE 

T0.U_k33p_SFSend = 'Q'-- A Integrar
AND (SELECT SUM(TaxRate) FROM  [{0}].dbo.RDR4 WHERE DocEntry = T0.DocEntry) IS NOT NULL
AND ISNULL(T0.U_k33p_SFID,'') <> ''
AND CANCELED = 'N'

/*UNION ALL

select

U_k33p_SFID as Id
,Comments as [Name]
,'Aprovado' as [Status]
,DocDueDate as ExpirationDate
,DocNum as NumeroCotacaoSAP__c
,DocDate as DataCotacaoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType in(27,10,16,26)) as Tax
,DocTotalFC as ValorCotacao__c
,'Brasil' as BillingCoutry
,DocRate as TaxaR__c
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 27 and DocEntry = T0.DocEntry) as COFINS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 27 and DocEntry = T0.DocEntry) as Valor_Cofins__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 10 and DocEntry = T0.DocEntry) as ICMS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 10 and DocEntry = T0.DocEntry) as Valor_ICMS__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 16 and DocEntry = T0.DocEntry) as IPI__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 16 and DocEntry = T0.DocEntry) as Valor_PI__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 26 and DocEntry = T0.DocEntry) as PIS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 26 and DocEntry = T0.DocEntry) as Valor_PIS__c 

FROM

[{1}].dbo.OQUT T0 --INNER JOIN [INTEGRACAOBR].dbo.RDR4 T4 ON T0.DocEntry = T4.DocEntry

WHERE 

U_k33p_SFSend = 'Q'-- A Integrar

UNION ALL

select

U_k33p_SFID as Id
,Comments as [Name]
,'Aprovado' as [Status]
,DocDueDate as ExpirationDate
,DocNum as NumeroCotacaoSAP__c
,DocDate as DataCotacaoSAP__c
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType in(27,10,16,26)) as Tax
,DocTotalFC as ValorCotacao__c
,'Brasil' as BillingCoutry
,DocRate as TaxaR__c
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 27 and DocEntry = T0.DocEntry) as COFINS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 27 and DocEntry = T0.DocEntry) as Valor_Cofins__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 10 and DocEntry = T0.DocEntry) as ICMS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 10 and DocEntry = T0.DocEntry) as Valor_ICMS__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 16 and DocEntry = T0.DocEntry) as IPI__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 16 and DocEntry = T0.DocEntry) as Valor_PI__c 
,(SELECT SUM(TaxRate) FROM RDR4 WHERE staType = 26 and DocEntry = T0.DocEntry) as PIS__c 
,(SELECT SUM(TaxSumFrgn) FROM RDR4 WHERE staType = 26 and DocEntry = T0.DocEntry) as Valor_PIS__c 

FROM

[{2}].dbo.OQUT T0 --INNER JOIN [INTEGRACAOBR].dbo.RDR4 T4 ON T0.DocEntry = T4.DocEntry

WHERE 

U_k33p_SFSend = 'Q'-- A Integrar
*/