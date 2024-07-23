--
-- FBGC
--

EXEC sp_addlinkedserver @server = N'FBGC',
    @srvproduct = N' ',
    @provider = N'SQLNCLI',
    @datasrc = N'localhost',			--- UPDATE HERE TO YOUR COMPUTER NAME
    @catalog = N'fbgcommon',
	@provstr='Integrated Security=SSPI;'
go

exec sp_serveroption @server='FBGC', @optname='rpc', @optvalue='true'
exec sp_serveroption @server='FBGC', @optname='rpc out', @optvalue='true'


-- TEST :
select top 10 * from FBGC.fbgcommon.dbo.BadEmails
