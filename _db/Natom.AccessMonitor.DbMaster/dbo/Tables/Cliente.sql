CREATE TABLE [dbo].[Cliente]
(
	ClienteId INT NOT NULL IDENTITY(1,1),
	RazonSocial NVARCHAR(50) NOT NULL,
	NombreFantasia NVARCHAR(50) NOT NULL,
	CUIT NVARCHAR(20) NOT NULL,
	Domicilio NVARCHAR(50) NOT NULL,
	Localidad NVARCHAR(50) NOT NULL,
	ContactoTelefono1 NVARCHAR(30),
	ContactoTelefono2 NVARCHAR(30),
	ContactoEmail1 NVARCHAR(50),
	ContactoEmail2 NVARCHAR(50),
	ContactoObservaciones NVARCHAR(200),
	Activo BIT NOT NULL,
	Observaciones NVARCHAR(200),
	PRIMARY KEY (ClienteId)
)
