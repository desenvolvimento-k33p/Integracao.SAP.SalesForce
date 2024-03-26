                                    
                                    DELETE FROM {0}.dbo.[CRD11] WHERE CardCode = '{1}';
                                    INSERT INTO {0}.[dbo].[CRD11]
                                           ([CardCode]
                                           ,[Address]
                                           ,[TributID]
                                           ,[TributType]
                                           ,[TTStartDat]
                                           ,[TTEndDate]
                                           ,[TribRegCod]
                                           ,[TRCStartD]
                                           ,[TRCEndDate]
                                           ,[LogInstanc])
                                     VALUES
                                           ('{1}'
                                           ,''
                                           ,(select (max(TributID) + 1) FROM CRD11)
                                           ,{2}
                                           ,'{3}'
                                           ,'{4}'
                                           ,{5}
                                           ,'{6}'
                                           ,'{7}'
                                            ,0);