--dbo.Config----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Clave],[Valor],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Sync.Receiver.URL','http://localhost:5000','URL de acceso a la API REST del Sincronizador'),
('WebApp.Admin.URL','http://localhost:4202','URL de acceso a la aplicación de Administradores'),
('WebApp.Clientes.URL','http://localhost:4201','URL de acceso a la aplicación de Clientes')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Clave],[Valor],[Description]))
merge	[dbo].[Config] as t
using	cte_data as s
on		1=1 and t.[Clave] = s.[Clave]
when matched then
	update set
	[Valor] = s.[Valor],[Description] = s.[Description]
when not matched by target then
	insert([Clave],[Valor],[Description])
	values(s.[Clave],s.[Valor],s.[Description])
when not matched by source then
	delete;