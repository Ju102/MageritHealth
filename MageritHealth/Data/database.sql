--------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------CREACIÓN DE TABLAS-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------

drop table MEDICIONES
drop table TIPOS_MEDICIONES
drop table ANALITICAS
drop table PRESCRIPCIONES
drop table CITAS
drop table DOCTORES_PACIENTES
drop table MEDICAMENTOS
drop table CREDENCIALES
drop table ANTECEDENTES_MEDICOS
drop table INFO_CLINICA_PACIENTES
drop table USUARIOS
drop table ESPECIALIDADES

create table ESPECIALIDADES
(
	IdEspecialidad int primary key not null,
	NombreEspecialidad nvarchar(50) not null
)

create table USUARIOS
(
	-- Datos personales
	IdUsuario int primary key not null,
	Nombre nvarchar (50) not null,
	Apellido1 nvarchar(50) not null,
	Apellido2 nvarchar(50) null,
	Dni nvarchar(20) not null unique, -- admite uso de nº pasaporte
	FechaNacimiento date not null,
	Telefono nvarchar(12) not null,
	Genero nvarchar(20) not null, -- Masculino, Femenino, Otro
	Direccion nvarchar(100) not null,


	-- Credenciales
	Email nvarchar(100) not null unique,
	Pass nvarchar(100) not null, -- Provisional para password de desarrollo (12345), luego este campo se retirara de la BD.
	Rol nvarchar(10) not null,

	-- Usuario Doctor
	IdEspecialidad int null,
	NumeroColegiado nvarchar(20) null,

	-- Usuario Paciente
	NumeroAsegurado nvarchar(20) null,

	-- 
	Activo bit not null default 1,
	FechaCreacion datetime default getdate(),

	-- Constraints
	constraint CK_USUARIOS_ROLES check (Rol in ('doctor', 'paciente', 'admin')),
	constraint CK_USUARIOS_GENEROS check (Genero in ('masculino','femenino','otro')),
	constraint CK_USUARIOS_DOCTORES check (Rol != 'doctor' or (IdEspecialidad is not null and NumeroColegiado is not null)),
	constraint CK_USUARIOS_PACIENTES check (Rol != 'paciente' or (NumeroAsegurado) is not null),
	constraint FK_USUARIOS_ESPECIALIDADES foreign key (IdEspecialidad) references ESPECIALIDADES(IdEspecialidad)
)

create table CREDENCIALES
(
	IdCredencial int primary key not null,
	IdUsuario int not null,
	PasswordHash varbinary(max) not null,
	Salt nvarchar(50) not null,

	constraint FK_CREDENCIALES_USUARIOS foreign key (IdUsuario) references USUARIOS(IdUsuario)
)

create table DOCTORES_PACIENTES
(
	IdAsignacion int primary key not null,
	IdPaciente int not null,
	IdDoctor int not null,
	FechaAsignacion datetime default getdate(),

	constraint FK_DOCPAC_PACIENTE foreign key (IdPaciente) references USUARIOS(IdUsuario),
	constraint FK_DOCPAC_DOCTOR foreign key (IdDoctor) references USUARIOS(IdUsuario)
)

create table CITAS
(
	IdCita int primary key not null,
	IdPaciente int not null,
	IdDoctor int not null,

	Motivo nvarchar(max) not null,
	FechaHora datetime not null,
	Notas nvarchar(max) null,
	Estado nvarchar(20), -- Programada, Progreso, Completada, Cancelada

	FechaCreacion datetime default getdate(),
	Activa bit default 1, -- Para poder filtrar por citas activas

	constraint CK_CITAS_ESTADOS check (Estado in ('programada', 'progreso', 'completada', 'cancelada')),
	constraint FK_CITAS_PACIENTES foreign key (IdPaciente) references USUARIOS(IdUsuario),
	constraint FK_CITAS_DOCTORES foreign key (IdDoctor) references USUARIOS(IdUsuario),
)

create table MEDICAMENTOS
(
	IdMedicamento int primary key not null,
	NombreComercial nvarchar(100) not null, -- Gelocatil
	PrincipioActivo nvarchar(100) not null, -- Ej: Ibuprofeno, Amoxicilina
	Concentracion nvarchar(50) not null, -- Ej: 500mg, 1g, 50ml
	Formato nvarchar(50) not null, -- Ej: Comprimidos, Jarabe, Sobres, Inyectable
	Fabricante nvarchar(100) null,
	
	Activo bit not null default 1, -- Para descatalogar medicamentos sin borrar historial
	FechaCreacion datetime default getdate(),

	constraint CK_MEDICAMENTOS_FORMATO check (Formato in ('comprimidos', 'jarabe', 'sobres', 'inyectable', 'crema', 'gotas', 'otro'))
)

create table PRESCRIPCIONES
(
	IdPrescripcion int primary key not null,
	IdCita int not null,
	IdMedicamento int not null,

	Instrucciones nvarchar(100) not null,
	FechaInicio date not null default getdate(),
	FechaFin date not null,

	FechaCreacion datetime default getdate(),

	constraint FK_PRESCRIPCIONES_CITAS foreign key (IdCita) references CITAS(IdCita),
	constraint FK_PRESCRIPCIONES_MEDICAMENTOS foreign key (IdMedicamento) references MEDICAMENTOS(IdMedicamento),
)

create table ANALITICAS
(
	IdAnalitica int primary key not null,
	IdCita int not null,
	FechaAnalitica datetime not null,
	Estado nvarchar(50) not null, -- Programada, Progreso, Completada, Cancelada
	Notas nvarchar(max) null,

	constraint FK_ANALITICAS_CITAS foreign key (IdCita) references CITAS(IdCita),
	constraint CK_ANALITICAS_ESTADOS check (Estado in ('programada', 'progreso', 'completada', 'cancelada'))
)

create table TIPOS_MEDICIONES
(
	IdTipoMedicion int primary key not null,
	NombreMedicion nvarchar(50) not null, -- Peso, Glucosa, Presión, Colesterol, ...
	UnidadMedicion nvarchar(10) not null,
	ValorMaximo decimal(10,2) not null,
	ValorMinimo decimal(10,2) not null,
)

create table MEDICIONES
(
	IdMedicion int primary key not null,
	IdTipoMedicion int not null,
	ValorMedicion decimal(10,2) not null,
	IdAnalitica int null,
	IdCita int null,

	constraint FK_MEDICIONES_ANALISIS foreign key (IdAnalitica) references ANALITICAS(IdAnalitica),
	constraint FK_MEDICIONES_TIPOSMEDICIONES foreign key (IdTipoMedicion) references TIPOS_MEDICIONES(IdTipoMedicion)
)

create table INFO_CLINICA_PACIENTES
(
	IdInfoClinica int primary key not null,
	IdPaciente int not null unique,

	GrupoSanguineo nvarchar(5) null,
	PesoNacimiento decimal(5,2) null, -- por si el paciente es un niño
	ContactoEmergenciaNombre nvarchar(100) null,
	ContactoEmergenciaTelefono nvarchar(12) null,

	FechaActualizacion datetime default getdate(),

	constraint CK_INFOCLINICA_SANGRE check (GrupoSanguineo in ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')),
	constraint FK_INFOCLINICA_PACIENTES foreign key (IdPaciente) references USUARIOS(IdUsuario)
)

create table ANTECEDENTES_MEDICOS
(
	IdAntecedente int primary key not null,
	IdPaciente int not null,
	
	Tipo nvarchar(50) not null, -- 'alergia', 'enfermedad', 'cirugia', 'habito', 'otro'
	Nombre nvarchar(100) not null, -- Ej: 'Penicilina', 'Diabetes Tipo 2', 'Apendicectomía', 'Fumador'
	Severidad nvarchar(20) null, -- 'leve', 'moderada', 'grave', 'critica' (Muy útil para alergias)
	
	FechaDiagnostico datetime null, -- Cuándo se lo detectaron/operaron (si se sabe)
	Notas nvarchar(max) null, -- Explicación detallada del caso especial
	
	Activo bit not null default 1, -- Por si un antecedente deja de ser relevante (ej: dejó de fumar)
	FechaRegistro datetime default getdate(),

	constraint CK_ANTECEDENTES_TIPO check (Tipo in ('alergia', 'enfermedad', 'cirugia', 'habito', 'otro')),
	constraint CK_ANTECEDENTES_SEVERIDAD check (Severidad in ('leve', 'moderada', 'grave', 'critica')),
	constraint FK_ANTECEDENTES_PACIENTES foreign key (IdPaciente) references USUARIOS(IdUsuario)
)




--------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------INSERCIÓN DE DATOS-------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------

-- ==========================================
-- 1. ESPECIALIDADES
-- ==========================================
INSERT INTO ESPECIALIDADES (IdEspecialidad, NombreEspecialidad)
VALUES 
(1, 'Medicina General'),
(2, 'Pediatría'),
(3, 'Cardiología'),
(4, 'Dermatología');

-- ==========================================
-- 2. USUARIOS (2 Admins, 2 Doctores, 3 Pacientes)
-- ==========================================
INSERT INTO USUARIOS (
    IdUsuario, Nombre, Apellido1, Apellido2, Dni, FechaNacimiento, 
    Telefono, Genero, Direccion, Email, Pass, Rol, 
    IdEspecialidad, NumeroColegiado, NumeroAsegurado, Activo, FechaCreacion
) 
VALUES 
(1, 'Julio Alejandro', 'Ordoñez', 'Rimacuna', '51234567A', '2002-07-10', '612345678', 'masculino', 'Ave del Paraíso 7, 1ºC Madrid', 'julioalejandro.ordonez@magerithealth.com', 'EqfW9WfXrX', 'admin', NULL, NULL, NULL, 1, '2026-03-13 19:23:44.957'),
(2, 'Jean Carlo', 'Ordoñez', 'Rimacuna', '51234567B', '2004-01-08', '698765432', 'masculino', 'Ave del Paraíso 7, 1ºC Madrid', 'jeancarlo.ordonez@magerithealth.com', 'y1rw8j3h9Z', 'admin', NULL, NULL, NULL, 1, '2026-03-13 19:37:28.403'),
(3, 'Patricia', 'Muñoz', 'García', '59876543B', '2003-03-15', '643597452', 'femenino', 'Antonio Conde 18, 2ºA Fuenlabrada', 'patricia.munozgarcia@magerithealth.com', 'H@0wv7e-4f', 'doctor', 1, 'COL-29336', NULL, 1, '2026-03-13 20:37:24.120'),
(4, 'John', 'Smith', NULL, 'X2335624G', '1990-10-10', '697631548', 'masculino', 'Gran Vía 18, 2ªA, Madrid', 'john.smith@magerithealth.com', 'tkhGkXvtks', 'paciente', NULL, NULL, 'ASEG-068', 1, '2026-03-13 21:12:35.213'),
(5, 'Carlos', 'García', 'Ruiz', '12345678A', '2011-11-11', '697654315', 'masculino', 'Desengaño 21, 1ºDCHA, 28006 Madrid', 'carlos.garciaruiz@magerithealth.com', 'kXLX26o5GI', 'paciente', NULL, NULL, 'ASEG-999', 1, '2026-03-14 18:51:27.757'),
(6, 'Ángela', 'Fernández', 'Madrid', '59876543G', '1990-02-15', '673564215', 'femenino', 'Antigua Via de Fuenlabra 2ºC, 23044 Madrid', 'angela.fernandezmadrid@magerithealth.com', 'g3W_X*H-fy', 'doctor', 2, 'COL-29342', NULL, 1, '2026-03-15 16:53:28.263'),
(7, 'Germán', 'Palomares', 'Pizarro', '56976532A', '1970-09-06', '631697513', 'masculino', 'Ave del Paraíso 7 2ºC, 28042 Madrid', 'german.palomares@magerithealth.com', 'U71Z7RQXNP', 'paciente', NULL, NULL, 'ASEG-124', 1, '2026-03-15 18:24:44.410');

-- ==========================================
-- 3. CREDENCIALES
-- ==========================================
INSERT INTO CREDENCIALES (IdCredencial, IdUsuario, PasswordHash, Salt)
VALUES 
(1, 1, 0xED48BBDA5A90D76103FC93E874A18A718BE1A349A91BD7ED73864D2A1A1A58E63C663369F98814F9866F65CBAC0322AD32C96C95983E4152DE05E4A3373F1F08, N'·Q&''A 7¥«`òsññ—''¬aaèàîö_.W´_îVŒYõ}:aZ4cím ‘îT'),
(2, 2, 0xAEF5C184CDFE784487B7EF99D03B4CFFF1D128427282AEDF965FAB541B9B64D0907DB8359C991A174800D7EB7842CAA58F0E2217092408EA6A2E07310EBBDD28, N'#@¸ÒË®N¡¡G‘CI>ˆqÈ!œàòbÊ6û Âþ²ù»&ŠÜ #J àÀŸX'),
(3, 3, 0xE4886F235661AA314B5DEB3269467F210382ED6A395DA16626B5B60062835B599933464CAED820A7878B931467011DF30A49F106F6655F976422E7DD35498CC3, N'f€?ñYÖ´ÈÆAQÄwÛü&=ðñÕžôA¹xà}Œ|¯Û¨Ì’¸F"ùª}c>'),
(4, 4, 0xCABB6BF8B24CAA4A113DD5F3498947A07E8F0103A2E53489FB1118389903B7B91AA5EC115FB3EE4E78482EFF98DC491F1AE76A533DE4BF09914EF5798A756F7D, N'Ï?sér<_,ÅxÁi¼×ú'']—ozÔÞ*–Z˜äŽ?öšÃ: ù>EÇ§Žuå¦'),
(5, 5, 0x372989418AF49939B5721787147E8714FD3EB0BD5A7FBF5170B1150AA988F1CAE0ED88D1AF6C6EC2B6D4A091C57A5889118ADD12385B3893B0E3C11ECE450292, N'[’Ç]Í¤f¤ày aÙUš«Tc·w`0ò†RO•y? Z ;ç¿d$B,–LTRrv'),
(6, 6, 0x4F036E619B870493AEE7CB398C2F356F455E1F692A9FA40EBFB1C5AD5D487D5A50CBDC5DCBEC48F69F786282681FDA0397E6A90B2DA84901D2FC014D7C0DB456, N'Oø]iÎ¯¤×—:º˜˜ÛièÒ 4¡@¢GT}yx4Ä—öª3sQ’$•gòXU–'),
(7, 7, 0x4C0657E3523A3113832651403A2853BB0711F079A42A5AB69559AC93DC02500F49766A72A880C79940FCBE2096B2EA199BB676F2987ECFF4C02EF79AB7BD8CB4, N'±‰.Zp½¦ µ§7üN Kò†úÃ¤7eEÑN#¨&;€ÐCí>žÔîó¤›iÅiŒ"');

-- ==========================================
-- 4. DOCTORES_PACIENTES
-- ==========================================
INSERT INTO DOCTORES_PACIENTES (IdAsignacion, IdPaciente, IdDoctor)
VALUES 
(1, 4, 3), -- John (Paciente) asignado a Patricia (Medicina General)
(2, 7, 3), -- Germán (Paciente) asignado a Patricia (Medicina General)
(3, 5, 6); -- Carlos (Paciente infantil) asignado a Ángela (Pediatría)

-- ==========================================
-- 5. CITAS
-- ==========================================
INSERT INTO CITAS (IdCita, IdPaciente, IdDoctor, Motivo, FechaHora, Notas, Estado, Activa)
VALUES 
(1, 4, 3, 'Revisión anual', '2026-03-20 09:30:00', 'Paciente refiere leve dolor de cabeza esporádico.', 'completada', 1),
(2, 5, 6, 'Vacunación y control de crecimiento', '2026-03-22 11:00:00', 'Traer cartilla de vacunación.', 'completada', 1),
(3, 7, 3, 'Consulta por dolor en el pecho', '2026-03-25 16:15:00', NULL, 'programada', 1),
(4, 4, 3, 'Seguimiento de tensión arterial', '2026-04-10 10:00:00', NULL, 'programada', 1);

-- ==========================================
-- 6. MEDICAMENTOS
-- ==========================================
INSERT INTO MEDICAMENTOS (IdMedicamento, NombreComercial, PrincipioActivo, Concentracion, Formato, Fabricante)
VALUES 
(1, 'Gelocatil', 'Paracetamol', '1g', 'comprimidos', 'Ferrer'),
(2, 'Ibuprofeno Kern', 'Ibuprofeno', '600mg', 'comprimidos', 'Kern Pharma'),
(3, 'Amoxidal', 'Amoxicilina', '500mg', 'sobres', 'Bayer'),
(4, 'Dalsy', 'Ibuprofeno', '20mg/ml', 'jarabe', 'Mylan'),
(5, 'Optovite', 'Vitamina B12', '1000mcg', 'inyectable', 'Normon');

-- ==========================================
-- 7. PRESCRIPCIONES
-- (Asociadas a las citas completadas)
-- ==========================================
INSERT INTO PRESCRIPCIONES (IdPrescripcion, IdCita, IdMedicamento, Instrucciones, FechaInicio, FechaFin)
VALUES 
(1, 1, 1, 'Tomar 1 comprimido cada 8 horas si hay dolor.', '2026-03-20', '2026-03-25'),
(2, 2, 4, 'Tomar 5ml cada 8 horas en caso de fiebre post-vacuna.', '2026-03-22', '2026-03-24');

-- ==========================================
-- 8. ANALITICAS
-- ==========================================
INSERT INTO ANALITICAS (IdAnalitica, IdCita, FechaAnalitica, Estado, Notas)
VALUES 
(1, 1, '2026-03-20 09:45:00', 'completada', 'Análisis de sangre y orina rutinario.'),
(2, 3, '2026-03-25 16:30:00', 'programada', 'Electrocardiograma y perfil lipídico.');

-- ==========================================
-- 9. TIPOS_MEDICIONES
-- ==========================================
INSERT INTO TIPOS_MEDICIONES (IdTipoMedicion, NombreMedicion, UnidadMedicion, ValorMaximo, ValorMinimo)
VALUES 
(1, 'Peso', 'kg', 300.00, 1.00),
(2, 'Altura', 'cm', 250.00, 40.00),
(3, 'Presión Arterial Sistólica', 'mmHg', 200.00, 70.00),
(4, 'Presión Arterial Diastólica', 'mmHg', 130.00, 40.00),
(5, 'Colesterol Total', 'mg/dL', 300.00, 100.00);

-- ==========================================
-- 10. MEDICIONES
-- (Resultados de las analíticas completadas o en cita)
-- ==========================================
INSERT INTO MEDICIONES (IdMedicion, IdTipoMedicion, ValorMedicion, IdAnalitica, IdCita)
VALUES 
(1, 1, 78.50, NULL, 1),    -- Peso de John en su cita
(2, 3, 125.00, NULL, 1),   -- Tensión sistólica de John en cita
(3, 4, 82.00, NULL, 1),    -- Tensión diastólica de John en cita
(4, 5, 195.00, 1, NULL),   -- Colesterol de John (en analítica)
(5, 1, 45.20, NULL, 2),    -- Peso de Carlos (niño) en su cita
(6, 2, 155.00, NULL, 2);   -- Altura de Carlos en su cita

-- ==========================================
-- 11. INFO_CLINICA_PACIENTES
-- ==========================================
INSERT INTO INFO_CLINICA_PACIENTES (IdInfoClinica, IdPaciente, GrupoSanguineo, PesoNacimiento, ContactoEmergenciaNombre, ContactoEmergenciaTelefono)
VALUES 
(1, 4, 'A+', NULL, 'Mary Smith', '611223344'),
(2, 5, 'O-', 3.20, 'Luisa Ruiz', '699887766'),
(3, 7, 'B+', NULL, 'Elena Pizarro', '655443322');

-- ==========================================
-- 12. ANTECEDENTES_MEDICOS
-- ==========================================
INSERT INTO ANTECEDENTES_MEDICOS (IdAntecedente, IdPaciente, Tipo, Nombre, Severidad, FechaDiagnostico, Notas)
VALUES 
(1, 4, 'alergia', 'Penicilina', 'grave', '2005-05-12', 'Reacción anafiláctica en la infancia.'),
(2, 7, 'enfermedad', 'Hipertensión', 'moderada', '2015-10-20', 'Controlada con medicación habitual.'),
(3, 7, 'habito', 'Fumador', 'grave', '1990-01-01', 'Fuma 1 paquete al día.'),
(4, 5, 'cirugia', 'Apendicectomía', 'leve', '2023-08-15', 'Sin complicaciones postoperatorias.');

