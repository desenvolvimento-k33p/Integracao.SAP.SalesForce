
--	select

--	(isnull(SUM((100 - (OSTA.Rate * (OSTA.U_Base/100)))),0)
	
--	- 
	
--		((select (select

--	isnull(SUM(isnull(((100 - (OSTA.Rate * (OSTA.U_Base/100)))),0) /100),0) as ICMS	
	

--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN ODRF ON ODRF.DocEntry = DRF1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--WHERE 
--	DRF1.DocEntry = {1}
--	AND OSTT.Name = 'ICMS'

--	) *

--		(select

--		isnull(SUM(isnull((((OSTA.Rate * (OSTA.U_Base/100)))),0) /100),0) as ICMS	
--	--,DRF4.TaxRate

--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN ODRF ON ODRF.DocEntry = DRF1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--WHERE 
--	DRF1.DocEntry = {1}
--	--AND OSTT.Name IN ('PIS','COFINS')))*100))/100	as Coeficiente	
--	AND (OSTT.Name like '%COF%' OR OSTT.Name LIKE '%PIS%')))*100))/100	as Coeficiente	

--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN ODRF ON ODRF.DocEntry = DRF1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--WHERE 
--	DRF1.DocEntry = {1}
--	AND OSTT.Name = 'ICMS'

--	GROUP BY DRF4.TaxRate;

--select

--	(isnull(SUM((100 - (OSTA.Rate * (OSTA.U_Base/100)))),0)
	
--	- 
	
--		((select (select

--	isnull(SUM(isnull(((100 - (OSTA.Rate * (OSTA.U_Base/100)))),0) /100),0) as ICMS	
	

--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN OQUT ON OQUT.DocEntry = QUT1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--WHERE 
--	DRF1.DocEntry = {1}
--	AND OSTT.Name = 'ICMS'

--	) *

--		(select
--	--OSTA.Rate,OSTA.U_Base, 
--	--STA1.U_Base
--	ISNULL(SUM((((OSTA.Rate * (STA1.U_Base/100)))) /100),0) as ICMS	
--	--,QUT4.TaxRate
--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN OQUT ON OQUT.DocEntry = QUT1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--	Inner Join [{0}].dbo.STA1 on STA1.StaCode = OSTA.Code
--	Inner Join [{0}].dbo.STC1 on STC1.STACode = OSTA.Code and STC1.STAType = STA1.SttType
--WHERE 
--	DRF1.DocEntry = {1}
--	AND (OSTT.Name like '%COF%' OR OSTT.Name LIKE '%PIS%')
--	And STC1.STCCode = DRF1.TaxCode))*100))/100	
	
	
	
--	as Coeficiente	
	

--	FROM 	
--	[{0}].dbo.DRF1
--	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
--	--INNER JOIN OQUT ON OQUT.DocEntry = QUT1.DocEntry
--	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type =DRF4.staType
--	INNER JOIN [{0}].dbo.OSTT ON OSTT.AbsId = DRF4.staType
--WHERE 
--	DRF1.DocEntry = {1}
--	AND OSTT.Name = 'ICMS'

--	GROUP BY DRF4.TaxRate;

Select isnull(U_Perc_Imp,-1) from [{0}].dbo.OSTC where Code = '{1}'