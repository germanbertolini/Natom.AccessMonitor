--dbo.Permiso---------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([PermisoId],[Scope],[Descripcion])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('abm_usuarios','WebApp.Admin','ABM SysAdmins'),
('abm_clientes','WebApp.Admin','ABM Clientes'),
('abm_clientes_usuarios', 'WebApp.Admin', 'ABM Usuarios de clientes'),

('*','WebApp.Clientes','Permiso total'),
('abm_usuarios','WebApp.Clientes','ABM Usuarios')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([PermisoId],[Scope],[Descripcion]))
merge	[dbo].[Permiso] as t
using	cte_data as s
on		1=1 and t.[PermisoId] = s.[PermisoId] and t.[Scope] = s.[Scope]
when matched then
	update set
	[Descripcion] = s.[Descripcion]
when not matched by target then
	insert([PermisoId],[Scope],[Descripcion])
	values(s.[PermisoId],s.[Scope],s.[Descripcion])
when not matched by source then
	delete;