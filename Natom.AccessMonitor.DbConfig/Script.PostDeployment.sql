--dbo.Config----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Clave],[Valor],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('Sync.Receiver.URL','http://localhost:5000','URL de acceso a la API REST del Sincronizador'),
('Logging.Discord.WebhookInvoker.CancellationTokenDurationMS', '5000', 'Duración del CancellationToken del llamado Http a Discord.'),
('Logging.Discord.EnableLog', 'True', 'Habilita / Deshabilita el Log en el servicio de Discord.'),
('Logging.Discord.Info.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''Info'' en el servicio de Discord.'),
('Logging.Discord.Info.WebhookUrl', 'https://discordapp.com/api/webhooks/886359578906877983/FnWSQ-AtwkAqQXoQyN63bpIwta-ZOvufydAs2PEofx4q0fLjXvQxxqa2wc9HBzDSV-Ft', 'URL del Webhook de Discord para avisos ''Info''.'),
('Logging.Discord.ServiceStatus.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''ServiceStatus'' en el servicio de Discord.'),
('Logging.Discord.ServiceStatus.WebhookUrl', 'https://discordapp.com/api/webhooks/886360349010440252/KVBRPuO3cG2Jo_PP-efaD_xKIbTQIXE0Usmd9mIa9nOdNBqGRdDiNfyDGVJEXWzzgwCh', 'URL del Webhook de Discord para avisos ''ServiceStatus''.'),
('Logging.Discord.Exception.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''Exception'' en el servicio de Discord.'),
('Logging.Discord.Exception.WebhookUrl', 'https://discordapp.com/api/webhooks/886361496127410236/ejrf8oEsGk8ehv18nLQChhkgmUHGrqvGbZN7ZbELk7O-FxQgsEkNMUFJBEso9crLTJW3', 'URL del Webhook de Discord para avisos ''Exception''.'),
('WebApp.Admin.URL','http://localhost:4202','URL de acceso a la aplicación de Administradores'),
('WebApp.Clientes.URL','http://localhost:4201','URL de acceso a la aplicación de Clientes'),
('ConnectionStrings.DbLogs', 'Data Source=localhost; Initial Catalog=AccessMonitor_Logs; Integrated Security=SSPI;', 'ConnectionString de la base de datos de Logs'),
('Cache.RedisServer.IP','localhost','IP del servidor Redis'),
('Cache.RedisServer.Port','6379','Puerto del servidor Redis'),
('Cache.RedisServer.Ssl','False','Si utilizar o no SSL en la conexión a Redis'),
('Cache.RedisServer.KeepAlive','30','Configuración de Redis'),
('Cache.RedisServer.ConnectTimeout','15000','Configuración de Redis'),
('Cache.RedisServer.SyncTimeout','15000','Configuración de Redis')
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