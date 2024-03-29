﻿--dbo.Permiso---------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([PermisoId],[Scope],[Descripcion])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('abm_usuarios','WebApp.Admin','ABM SysAdmins'),
('abm_clientes','WebApp.Admin','ABM Clientes'),
('abm_clientes_usuarios', 'WebApp.Admin', 'ABM Usuarios de clientes'),
('abm_clientes_dispositivos','WebApp.Admin','ABM sincronizadores de clientes'),
('abm_clientes_places_goals', 'WebApp.Admin', 'ABM Plantas / Oficinas / Porterías / Accesos de clientes'),
('abm_clientes_places_horarios', 'WebApp.Admin', 'Administrar Horarios / Tolerancias'),

('*','WebApp.Clientes','Permiso total'),
('abm_usuarios','WebApp.Clientes','ABM Usuarios'),
('abm_titles', 'WebApp.Clientes', 'ABM Cargos'),
('abm_dockets', 'WebApp.Clientes', 'ABM Legajos'),
('admin_sync', 'WebApp.Clientes', 'Administrar sincronizador / Ver dispositivos'),
('abm_places_goals', 'WebApp.Clientes', 'Administrar Plantas / Porterías'),
('abm_places_horarios', 'WebApp.Clientes', 'Administrar Horarios / Tolerancias'),
('reportes_ver_horas_trabajadas_mes', 'WebApp.Clientes', 'Reportes: Ver horas trabajadas mensuales'),
('reportes_estadistica_asistencia', 'WebApp.Clientes', 'Reportes: Estadística de asistencia'),
('reportes_ver_asistencia_mensual', 'WebApp.Clientes', 'Reportes: Asistencia mensual')
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