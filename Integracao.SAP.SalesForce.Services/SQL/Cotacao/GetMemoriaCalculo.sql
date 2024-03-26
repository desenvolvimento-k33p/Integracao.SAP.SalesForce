select 
	 DRF1.TaxCode
	,DRF1.Quantity
	,DRF1.ItemCode
	,DRF4.StaCode
	,DRF4.staType
	,DRF4.TaxSumFrgn
	,ISNULL(OSTA.U_Base,'0') as U_Base
	,OSTA.Rate
	,(((DRF1.U_k33p_SFUPrice * DRF1.Quantity )/ (100 - OSTA.Rate)) * 100) / DRF1.Quantity as VlrUnit
	--,QUT1.U_k33p_SFUPrice + (((OSTA.U_Base / (100 - OSTA.Rate)) * 100) / QUT1.Quantity) as VlrUnit2
	
	into #tmp1
FROM 	
	[{0}].dbo.DRF1
	INNER JOIN [{0}].dbo.DRF4 ON DRF1.DocEntry = DRF4.DocEntry
	INNER JOIN [{0}].dbo.ODRF ON ODRF.DocEntry = DRF1.DocEntry
	INNER JOIN [{0}].dbo.OSTA ON OSTA.Code = DRF4.StaCode AND OSTA.Type = DRF4.staType
WHERE 
	DRF1.DocEntry = {1}
	AND DRF1.LineNum = {2};

	delete from #tmp1 where [U_Base] <= 0;
	update #tmp1 set [VlrUnit] = 0 WHERE [TaxSumFrgn] = 0;
	--select * from #tmp1;
	select SUM([VlrUnit]) from #tmp1;
	drop table #tmp1;