﻿select Code from [{0}].dbo.OBNI where CAST(IndexType as nvarchar) + cast(Code as nvarchar) = '{1}' and IndexType = 19;