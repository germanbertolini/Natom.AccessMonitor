CREATE TABLE ConfigTolerancia
(
	ConfigToleranciaId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ClienteId INT NOT NULL,
	PlaceId INT,
	IngresoToleranciaMins INT,
	EgresoToleranciaMins INT,
	AlmuerzoHorarioDesde INT,
	AlmuerzoHorarioHasta INT,
	AlmuerzoTiempoLimiteMins INT,
	ConfiguroUsuarioId INT,
	ConfiguroFechaHora DATETIME,
	AplicaDesde DATE NOT NULL,
	AplicaHasta DATE
)