select "AbsId"


 from  [{1}].dbo.OCNT WHERE --"Name" = 'Águas Lindas de Goiás'
UPPER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE("Name",'ã','a'),'á','a'),'à','a') ,'ê','e') ,'é','e') ,'í','i') ,'ó','o') ,'ô','o') ,'ú','u') ,'´',''),' ',''),'ç','c'),'Ç','C'),'â','a'),'-',''),'''',''),'Á','a'),'É','e'),'Í','i'),'Ó','o'),'Ú','u'))=
UPPER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE('{0}','ã','a'),'á','a'),'à','a') ,'ê','e') ,'é','e') ,'í','i') ,'ó','o') ,'ô','o') ,'ú','u') ,'´',''),' ',''),'ç','c'),'Ç','C'),'â','a'),'-',''),'''',''),'Á','a'),'É','e'),'Í','i'),'Ó','o'),'Ú','u'))

--select * from OCNT WHERE "Name" like '%''%'







