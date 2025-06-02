USE master
GO
DROP DATABASE IF EXISTS Gara_Auto
GO
CREATE DATABASE Gara_Auto
GO
USE Gara_Auto
GO

--========================Crearea Tabelelor==============================

--Tabelul Localitate
DROP TABLE IF EXISTS dbo.Localitate;
GO
CREATE TABLE Localitate(
	IDLocalitate INT PRIMARY KEY IDENTITY(1,1),
	Denumire NVARCHAR(100) NOT NULL UNIQUE
)
GO

--Tabelul Statie
DROP TABLE IF EXISTS dbo.Statie;
GO
CREATE TABLE Statie(
	IDStatie INT PRIMARY KEY IDENTITY(1,1),
	IDLocalitate INT FOREIGN KEY REFERENCES Localitate(IDLocalitate),
	Adresa NVARCHAR(200) NOT NULL,
	Latitudine DECIMAL(9,6) NOT NULL, 
	Longitudine DECIMAL(9,6) NOT NULL
	
)
GO

--Tabelul Traseu
DROP TABLE IF EXISTS dbo.Traseu;
GO
CREATE TABLE Traseu(
	IDTraseu INT PRIMARY KEY IDENTITY(1,1),
	PunctPornireID INT  FOREIGN KEY (PunctPornireID) REFERENCES Statie(IDStatie),
	Km FLOAT,
    DestinatieID INT FOREIGN KEY (DestinatieID) REFERENCES Statie(IDStatie),
    CHECK (PunctPornireID <> DestinatieID)
)
GO

--Tabelul TipTransport
DROP TABLE IF EXISTS dbo.TipTransport;
GO
CREATE TABLE TipTransport(
	IDTipTransport INT PRIMARY KEY IDENTITY(1,1),
	Denumire NVARCHAR(100) NOT NULL,
	Nr_locuri INT NOT NULL
)
GO



--Tabelul Post
DROP TABLE IF EXISTS dbo.Post;
GO
CREATE TABLE Post(
	IDPost INT PRIMARY KEY IDENTITY(1,1),
	Denumire NVARCHAR(100) NOT NULL UNIQUE
)
GO

--Tabelul Angajat
DROP TABLE IF EXISTS dbo.Angajat;
GO
CREATE TABLE Angajat(
	IDAngajat INT PRIMARY KEY IDENTITY(1,1),
	CNP VARCHAR(13) CHECK(LEN(CNP)=13) UNIQUE,
	Nume NVARCHAR(100) NOT NULL,
	Prenume NVARCHAR(100) NOT NULL,
	Gen VARCHAR(10)  CHECK (Gen IN ('Masculin', 'Feminin')),
	Data_nastere DATE CHECK(DATEDIFF(YY,Data_nastere, GETDATE())>=18),
	Nr_telefon VARCHAR(9) CHECK(LEN(Nr_telefon)=9) UNIQUE,
	IDLocalitate INT FOREIGN KEY REFERENCES Localitate(IDLocalitate),
	IDPost INT FOREIGN KEY REFERENCES Post(IDPost)
)
GO



--Tabelul Cursa
DROP TABLE IF EXISTS dbo.Cursa;
GO
CREATE TABLE Cursa(
	IDCursa INT PRIMARY KEY IDENTITY(1,1),
	IDTraseu INT FOREIGN KEY REFERENCES Traseu(IDTraseu),
	IDTipTransport INT FOREIGN KEY REFERENCES TipTransport(IDTipTransport),
	IDAngajat INT FOREIGN KEY REFERENCES Angajat(IDAngajat) NOT NULL,
    DataPlecare DATE NOT NULL,
    OraPlecare TIME NOT NULL,
)
GO

--Tabelul Pasager
DROP TABLE IF EXISTS dbo.Pasager;
GO
CREATE TABLE Pasager(
	IDPasager INT PRIMARY KEY IDENTITY(1,1),
	Nume NVARCHAR(100) NOT NULL,
	Prenume NVARCHAR(100) NOT NULL,
	Nr_telefon VARCHAR(9) CHECK(LEN(Nr_telefon)=9) UNIQUE
)
GO

--Tabelul Bilet
DROP TABLE IF EXISTS dbo.Bilet;
GO
CREATE TABLE Bilet(
	IDBilet INT PRIMARY KEY IDENTITY(1,1),
	IDPasager INT FOREIGN KEY REFERENCES Pasager(IDPasager),
	IDCursa INT FOREIGN KEY REFERENCES Cursa(IDCursa),
	DataRezervare DATE NOT NULL,
	Nr_loc INT NOT NULL,
	Pret SMALLMONEY 
)
GO

--Tabelul Utilizator
DROP TABLE IF EXISTS dbo.Utilizator;
GO
CREATE TABLE Utilizator (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Email VARCHAR(100) UNIQUE NOT NULL,
    ParolaHash VARCHAR(200) NOT NULL,
    Rol VARCHAR(20) CHECK (Rol IN ('Admin', 'Angajat', 'Pasager')) NOT NULL,
    IDLegat INT NOT NULL
)
GO

--========================Înserarea datelor==============================

INSERT INTO Post VALUES
('Manager'),
('Casier'),
('Sofer'),
('Controlor'),
('Dispecer')
GO


INSERT INTO Localitate VALUES
('Chișinău'),
('Orhei'),
('Ungheni'),
('Soroca'),
('Cahul'),
('Cimislia'),
('Hincesti'),
('Edinet'),
('Telenesti'),
('Soldanesti'),
('Ialoveni'),
('Calarasi'),
('Causeni'),
('Singerei'),
('Balti'),
('Briceni')
GO

INSERT INTO TipTransport (Denumire, Nr_locuri)
VALUES
('Microbuz', 18),
('Autocar', 50),
('Midibus', 30),
('Autobuz Electric', 40),
('Minivan', 8),
('Autobuz Mare', 60),
('Autobuz Urban', 45),
('Autobuz Interurban', 55)
GO

INSERT INTO Statie (IDLocalitate, Adresa, Latitudine, Longitudine)
VALUES
(1, 'Aleea Gării 1', 47.010447, 28.857207),
(2, 'Strada Vasile Lupu 94', 47.386340, 28.823118),
(15, 'Strada Decebal 131', 47.762022, 27.918236),
(5, 'Strada Ștefan cel Mare 2', 45.899751, 28.193800),
(7, 'Strada Mihalcea Hîncu 65', 46.825356, 28.594075),
(3, 'Strada Națională 21', 47.206619, 27.800141),
(4, 'Strada Independenței 5', 48.160120, 28.294214),
(16, 'Strada Ștefan cel Mare 10', 48.365145, 27.080519),
(1, 'Strada Gheorghiu 82', 47.003768, 28.861389),
(2, 'Strada Aanei 73', 47.380255, 28.817320),
(2, 'Strada Pușcașu 84', 47.392427, 28.819394),
(3, 'Strada Florea 96', 47.216500, 27.801580),
(4, 'Strada Nistor 52', 48.156892, 28.295156),
(4, 'Strada Popescu 18', 48.152440, 28.297147),
(5, 'Strada Florea 21', 45.911222, 28.187057),
(5, 'Strada Popa 9', 45.909842, 28.192443),
(7, 'Strada Oprea 69', 46.836206, 28.581591),
(7, 'Strada Ababei 44', 46.837618, 28.594672),
(15, 'Strada Cristea 59', 47.755331, 27.927995),
(15, 'Strada Ionescu 93', 47.753165, 27.933742),
(16, 'Strada Ioniță 21', 48.361378, 27.075378),
(6, 'Strada Ștefan cel Mare 35', 46.522700, 28.771389),
(8, 'Strada Independenței 10', 48.172256, 27.308745),
(9, 'Strada Mihai Eminescu 3', 47.504200, 28.364525),
(10, 'Strada 31 August 15', 47.816500, 28.805800),
(11, 'Strada Alexandru cel Bun 7', 46.943330, 28.780321),
(12, 'Strada Mihai Viteazul 12', 47.256700, 28.305900),
(13, 'Strada Ștefan cel Mare 49', 46.642410, 29.408520),
(14, 'Strada 31 August 21', 47.634180, 28.149400)
GO



INSERT INTO Traseu (PunctPornireID, DestinatieID, Km)
VALUES
(29, 2, 41.87),
(1, 3, 109.58),
(2, 28, 79.87),
(2, 4, 172.13),
(3, 4, 208.09),
(27, 5, 116.0),
(4, 5, 107.4),
(4, 6, 148.38),
(5, 6, 73.76),
(5, 26, 150.11),
(6, 7, 112.32),
(6, 8, 139.64),
(22, 8, 92.95),
(7, 9, 135.46),
(8, 9, 201.95),
(20, 10, 200.91),
(9, 10, 1.19),
(9, 11, 1.63),
(10, 11, 0.91),
(10, 12, 41.98),
(11, 23, 41.69),
(11, 13, 40.54),
(12, 13, 1.52),
(25, 14, 0.75),
(13, 14, 1.36),
(21, 15, 78.57),
(14, 24, 79.05),
(14, 16, 79.98),
(15, 16, 0.94),
(15, 17, 0.76),
(16, 17, 0.76),
(16, 23, 111.67),
(17, 18, 110.93),
(17, 19, 110.51),
(18, 19, 0.52),
(18, 20, 1.21),
(19, 20, 1.39)
GO


INSERT INTO Angajat (CNP, Nume, Prenume, Gen, Data_nastere, Nr_telefon, IDLocalitate, IDPost)
VALUES 
('6776790190029', 'Nistor', 'Antonia','Feminin', '1981-12-27', '593824219', 13, 4),
('5455239660329', 'Diaconescu', 'Cătălina','Feminin', '1986-11-03', '659387784', 2, 3),
('1640737815720', 'Nistor', 'Severin','Masculin', '1964-06-12', '139332871', 16, 4),
('5630673011870', 'Preda', 'Florentin','Masculin', '1971-10-16', '858398947', 10, 2),
('5576520688037', 'Stoica', 'Marina','Feminin', '1984-02-16', '947112201', 12, 5),
('6883473211403', 'Pușcașu', 'Octaviana','Feminin', '1999-11-17', '947751591', 7, 3),
('4274310130446', 'Ababei', 'Noemi', 'Masculin','1972-05-04', '601230989', 5, 3),
('2186253015528', 'Toma', 'Iulia', 'Feminin','2005-08-18', '109032173', 5, 1),
('9573579953203', 'Niță', 'Sandu','Masculin', '1971-04-11', '456208709', 9, 3),
('3506373832008', 'Georgescu', 'Vasilica','Feminin', '1996-05-14', '258419720', 5, 3),
('3504373832008', 'Popescu', 'Marin','Masculin', '1986-02-11', '258409720', 3, 3)
GO


INSERT INTO Cursa (IDTraseu, IDTipTransport, IDAngajat, DataPlecare, OraPlecare)
VALUES
(4, 1, 2, '2025-06-01', '15:45'), 
(5, 2, 6, '2025-06-05', '20:30'), 
(2, 3, 7, '2025-06-04', '10:00'), 
(7, 4, 9, '2025-06-05', '10:00'), 
(9, 5, 10, '2025-06-02', '19:15'), 
(10, 6, 11, '2025-06-07', '10:15'), 
(14, 7, 2, '2025-06-08', '08:30'), 
(1, 8, 6, '2025-06-02', '13:30'), 
(16, 1, 7, '2025-06-03', '14:15'), 
(12, 2, 9, '2025-06-03', '06:30') 
GO



INSERT INTO Pasager (Nume, Prenume, Nr_telefon)
VALUES 
('Voinea', 'Aurelian', '280715084'),
('Grigore', 'Valeria', '309077952'),
('Stănescu', 'Codruț', '775187094'),
('Petrescu', 'Silvia', '360419236'),
('Neagu', 'Eduard', '265397309'),
('Dumitrache', 'Andrada', '813944387'),
('Tudor', 'Denisa', '773055808'),
('Matei', 'Florin', '690674530'),
('Mihăescu', 'Paul', '218450632'),
('Ionescu', 'Nicoleta', '289431927'),
('Oprea', 'Vlad', '871987928'),
('Barbu', 'Cristina', '771234568'),
('Georgescu', 'Alin', '504667831'),
('Vlad', 'Daniel', '208394583'),
('Moldovan', 'Simona', '667320905')
GO


INSERT INTO Bilet (IDPasager, IDCursa,  DataRezervare, Nr_loc, Pret)
VALUES 
(5, 6, '2024-05-15', 2, 64.20),
(3, 8,  '2024-05-17', 1, 37.75),
(14, 10,  '2024-05-19', 3, 90.50),
(8, 2,  '2024-05-20', 1, 22.00),
(9, 5, '2024-05-16', 2, 74.90),
(2, 7,  '2024-05-19', 1, 44.30),
(6, 9,  '2024-05-18', 1, 35.80),
(11, 9,  '2024-05-22', 4, 99.10),
(1, 10,  '2024-05-17', 2, 52.45),
(13, 8,  '2024-05-25', 2, 88.00),
(4, 4,  '2024-05-14', 3, 78.60),
(12, 3,  '2024-05-13', 1, 49.90),
(10, 7,  '2024-05-15', 1, 27.10),
(7, 1,  '2024-05-16', 2, 62.30),
(15, 1,  '2024-05-18', 1, 39.00),
(6, 10,  '2024-05-19', 1, 59.70),
(5, 2,  '2024-05-20', 2, 66.25),
(3, 9, '2024-05-21', 1, 48.80),
(2, 4, '2024-05-23', 1, 33.40),
(8, 6, '2024-05-24', 2, 73.10)
GO



INSERT INTO Utilizator (Email, ParolaHash, Rol, IDLegat)
VALUES (
    'admin@gara.md',
    '$2a$11$AttbvKBlwuPHWveKwBrjD.jn/ffa9sDOurO6UI6a0GN3Fa6yRZozO', 
    'Admin',
    0
)
GO

--========================Crearea Vederilor==============================


CREATE OR ALTER VIEW Angajat_Complet AS
SELECT 
    a.IDAngajat,
    a.CNP, 
    a.Nume, 
    a.Prenume, 
    a.Gen,
    a.Data_nastere, 
    a.Nr_telefon, 
    loc.Denumire AS Localitate, 
    p.Denumire   AS Post,
    CASE 
        WHEN u.ID IS NOT NULL THEN 'Da' 
        ELSE 'Nu' 
    END AS AreCont
FROM Angajat a
JOIN Post        p   ON a.IDPost        = p.IDPost
JOIN Localitate  loc ON a.IDLocalitate  = loc.IDLocalitate
LEFT JOIN Utilizator u 
    ON u.IDLegat = a.IDAngajat
   AND u.Rol     = 'Angajat';
GO


CREATE OR ALTER VIEW Conturi_Angajat
AS
SELECT * FROM Angajat_Complet
WHERE AreCont='Da'
GO



CREATE OR ALTER VIEW Utilizator_angajat
AS
SELECT Email, ParolaHash, Rol, IDLegat, Nume, Prenume
FROM Utilizator JOIN Angajat  ON IDLegat=IDAngajat AND Rol='Angajat'
GO

CREATE OR ALTER VIEW Pasageri_DataRezervare
AS
SELECT DISTINCT Pasager.IDPasager, Nume, Prenume, DataRezervare
FROM Pasager JOIN Bilet ON Pasager.IDPasager=Bilet.IDPasager
GO

CREATE OR ALTER VIEW Trasee_Pornire
AS
SELECT IDTraseu, Denumire as Localitate_Pornire, Adresa as Adresa_Pornire, km, DestinatieID
FROM Traseu JOIN Statie s ON PunctPornireID=IDStatie
JOIN Localitate l ON s.IDLocalitate=l.IDLocalitate
GO


CREATE OR ALTER VIEW Trasee
AS
SELECT IDTraseu, Localitate_Pornire, Adresa_Pornire, Denumire as Localitate_Destinatie, Adresa as Adresa_Destinatie, km
FROM Trasee_Pornire JOIN Statie s ON DestinatieID=IDStatie
JOIN Localitate l ON s.IDLocalitate=l.IDLocalitate
GO

CREATE OR ALTER VIEW Curse_Complet
AS
SELECT IDCursa, CAST(Cursa.IDTraseu AS NVARCHAR(10))+': '+Localitate_Pornire+' - '+Localitate_Destinatie as Traseu,Denumire TipTransport, DataPlecare, OraPlecare, Nume+' '+Prenume as Sofer, km
FROM Cursa JOIN TipTransport ON Cursa.IDTipTransport=TipTransport.IDTipTransport
JOIN Trasee ON Cursa.IDTraseu=Trasee.IDTraseu
JOIN Angajat ON Angajat.IDAngajat=Cursa.IDAngajat
GO

CREATE OR ALTER VIEW TraseeTotal AS
SELECT 
  t.IDTraseu,
  lp.Denumire   AS Localitate_Pornire,
  sp.Adresa     AS Adresa_Pornire,
  sp.Latitudine AS StartLat,
  sp.Longitudine AS StartLon,
  ld.Denumire   AS Localitate_Destinatie,
  sd.Adresa     AS Adresa_Destinatie,
  sd.Latitudine AS EndLat,
  sd.Longitudine AS EndLon,
  t.Km
FROM Traseu t
  JOIN Statie sp ON t.PunctPornireID   = sp.IDStatie
  JOIN Localitate lp ON sp.IDLocalitate = lp.IDLocalitate
  JOIN Statie sd ON t.DestinatieID      = sd.IDStatie
  JOIN Localitate ld ON sd.IDLocalitate = ld.IDLocalitate;
GO

CREATE OR ALTER VIEW Bilete_Total
AS
SELECT IDBilet,Bilet.IDPasager, Nume as NumePasager, Prenume as PrenumePasager, 
		CAST(Bilet.IDCursa AS NVARCHAR(10))+': '
			+Localitate_Pornire+' - '
				+Localitate_Destinatie+' '
					+CAST(DataPlecare AS NVARCHAR(10))
						+' '+CAST(OraPlecare AS NVARCHAR(10))  as Cursa,
										DataRezervare, Nr_loc
FROM Bilet JOIN Pasager ON Bilet.IDPasager=Pasager.IDPasager
JOIN Cursa ON Bilet.IDCursa=Cursa.IDCursa JOIN Trasee ON Trasee.IDTraseu=Cursa.IDTraseu
GO

CREATE OR ALTER VIEW Cursa_Locuri_Stat
AS
SELECT
    c.IDCursa,
    t.Localitate_Pornire + ' – ' + t.Localitate_Destinatie AS Traseu,
    c.DataPlecare,
    c.OraPlecare,
    tt.Denumire        AS TipTransport,
	a.Nume+' '+a.Prenume   AS Sofer,
    tt.Nr_locuri       AS CapacitateTotala,
    ISNULL(b.Ocupate, 0) AS LocuriOcupate,
    tt.Nr_locuri - ISNULL(b.Ocupate, 0) AS LocuriDisponibile
FROM 
    Cursa c
    JOIN TipTransport tt
      ON c.IDTipTransport = tt.IDTipTransport
    JOIN Trasee t
      ON c.IDTraseu = t.IDTraseu
	JOIN Angajat a
	  ON a.IDAngajat=c.IDAngajat
    LEFT JOIN (
        SELECT 
            IDCursa,
            COUNT(*) AS Ocupate
        FROM Bilet
        GROUP BY IDCursa
    ) b
      ON c.IDCursa = b.IDCursa;
GO

--========================Creare Triggeri==============================

CREATE OR ALTER TRIGGER trg_CalculeazaPretBilet
ON Bilet
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Inserează biletul cu preț calculat
    INSERT INTO Bilet (IDPasager, IDCursa, Nr_loc,DataRezervare, Pret)
    SELECT
        i.IDPasager,
        i.IDCursa,
        i.Nr_loc,
		i.DataRezervare,
        t.Km * 0.35 AS PretCalculat
    FROM inserted i
    JOIN Cursa c ON c.IDCursa = i.IDCursa
    JOIN Traseu t ON t.IDTraseu = c.IDTraseu
END
GO

--========================Crearea Procedurilor==============================

CREATE OR ALTER PROCEDURE sp_AdaugaTraseuCuKm
    @PunctPornireID INT,
    @DestinatieID INT,
    @IDTraseuNou INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificare ca punctele să fie diferite
    IF @PunctPornireID = @DestinatieID
    BEGIN
        RAISERROR('Punctul de pornire și destinația trebuie să fie diferite.', 16, 1)
        RETURN
    END

    -- Obține coordonatele
    DECLARE @Lat1 FLOAT, @Lon1 FLOAT, @Lat2 FLOAT, @Lon2 FLOAT
    SELECT @Lat1 = Latitudine, @Lon1 = Longitudine FROM Statie WHERE IDStatie = @PunctPornireID
    SELECT @Lat2 = Latitudine, @Lon2 = Longitudine FROM Statie WHERE IDStatie = @DestinatieID

    -- Calculează kilometrajul
    DECLARE @Km FLOAT = dbo.CalculKmHaversine(@Lat1, @Lon1, @Lat2, @Lon2)

    -- Inserează în Traseu
    INSERT INTO Traseu (PunctPornireID, DestinatieID, Km)
    VALUES (@PunctPornireID, @DestinatieID, @Km)

    -- Returnează ID-ul nou
    SET @IDTraseuNou = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE sp_InregistrarePasager
    @Nume VARCHAR(100),
    @Prenume VARCHAR(100),
    @NrTelefon VARCHAR(9),
    @Email VARCHAR(100),
    @ParolaHash VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Verificăm dacă emailul e deja folosit
    IF EXISTS (SELECT 1 FROM Utilizator WHERE Email = @Email)
    BEGIN
        RAISERROR('Email deja folosit', 16, 1);
        RETURN;
    END

    DECLARE @IDPasager INT;

    -- 2. Căutăm pasager cu acest număr de telefon
    SELECT @IDPasager = IDPasager
    FROM Pasager
    WHERE Nr_telefon = @NrTelefon;

    -- 3. Dacă există deja nr. de telefon
    IF @IDPasager IS NOT NULL
    BEGIN
        -- Verificăm dacă numele și prenumele corespund
        IF NOT EXISTS (
            SELECT 1 FROM Pasager
            WHERE IDPasager = @IDPasager AND Nume = @Nume AND Prenume = @Prenume
        )
        BEGIN
            RAISERROR('Numărul de telefon aparține altui pasager.', 16, 1);
            RETURN;
        END

        -- Verificăm dacă deja are cont
        IF EXISTS (
            SELECT 1 FROM Utilizator
            WHERE IDLegat = @IDPasager AND Rol = 'Pasager'
        )
        BEGIN
            RAISERROR('Pasagerul are deja un cont.', 16, 1);
            RETURN;
        END
    END
    ELSE
    BEGIN
        -- 4. Dacă nu există, creăm pasagerul
        INSERT INTO Pasager (Nume, Prenume, Nr_telefon)
        VALUES (@Nume, @Prenume, @NrTelefon);

        SET @IDPasager = SCOPE_IDENTITY();
    END

    -- 5. Inserăm contul legat de pasager
    INSERT INTO Utilizator (Email, ParolaHash, Rol, IDLegat)
    VALUES (@Email, @ParolaHash, 'Pasager', @IDPasager);
END
GO


CREATE OR ALTER PROCEDURE sp_AngajatNou
	@Nume VARCHAR(100),
    @Prenume VARCHAR(100),
    @NrTelefon VARCHAR(9),
	@Post VARCHAR(100),
	@Localitate VARCHAR(100),
	@Data_Nastere DATE,
	@CNP VARCHAR(13),
	@Gen VARCHAR(10)
AS 
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdPost INT;

	SELECT @IdPost=IDPost
	FROM Post
	WHERE Denumire=@Post;

	DECLARE @IdLocalitate INT;
	SELECT @IdLocalitate=IDLocalitate
	FROM Localitate
	WHERE Denumire=@Localitate;

	IF EXISTS (SELECT 1 FROM Angajat WHERE Nr_telefon = @NrTelefon)
	BEGIN
		RAISERROR('Numărul de telefon aparține altui angajat.', 16, 1);
		RETURN;
	END

	IF EXISTS (SELECT 1 FROM Angajat WHERE CNP = @CNP)
	BEGIN
		RAISERROR('CNP-ul aparține altui angajat.', 16, 1);
		RETURN;
	END

	IF DATEDIFF(YEAR, @Data_Nastere, GETDATE()) < 18
	BEGIN
		RAISERROR('Angajatul nu poate avea vârsta mai mică de 18 ani.', 16, 1);
		RETURN;
	END

	INSERT INTO Angajat (CNP, Nume, Prenume, Data_nastere, Gen, Nr_telefon, IDLocalitate, IDPost)VALUES
	(@CNP,@Nume,@Prenume,@Data_Nastere,@Gen,@NrTelefon,@IdLocalitate,@IdPost);

	
END
GO



CREATE OR ALTER PROCEDURE sp_UserAngajatNou
 @Email VARCHAR(100),
 @ParolaHash VARCHAR(200),
 @IdLegat INT
AS
BEGIN
SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM Utilizator WHERE IDLegat = @IdLegat AND Rol='Angajat')
    BEGIN
        RAISERROR('Acest angajat are deja cont', 16, 1);
        RETURN;
    END

 IF EXISTS (SELECT 1 FROM Utilizator WHERE Email = @Email)
    BEGIN
        RAISERROR('Email deja folosit', 16, 1);
        RETURN;
    END

INSERT INTO Utilizator (Email, ParolaHash, Rol, IDLegat)
    VALUES (@Email, @ParolaHash, 'Angajat', @IdLegat)

END
GO

CREATE OR ALTER PROCEDURE sp_DeleteUser
	@IdUser INT
AS
BEGIN
SET NOCOUNT ON;

        DELETE FROM Utilizator
		WHERE @IdUser=IDLegat AND Rol='Angajat'
	

END
GO

CREATE OR ALTER PROCEDURE dbo.sp_UpdateAngajat
	@IDAngajat INT,
	@Nume VARCHAR(100),
    @Prenume VARCHAR(100),
    @NrTelefon VARCHAR(9), 
	@Localitate  VARCHAR(100),
	@Post  VARCHAR(100)
AS
BEGIN

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM Angajat WHERE Nr_telefon = @NrTelefon AND IDAngajat<>@IDAngajat)
	BEGIN
		RAISERROR('Numărul de telefon aparține altui angajat.', 16, 1);
		RETURN;
END

DECLARE @IDLocalitate INT;

SELECT @IDLocalitate=IDLocalitate
FROM Localitate
WHERE Denumire= @Localitate

DECLARE @IDPost INT;

SELECT @IDPost=IDPost
FROM Post
WHERE Denumire=@Post;

IF EXISTS (
        SELECT 1
        FROM dbo.Angajat
        WHERE IDAngajat    = @IDAngajat
          AND Nume         = @Nume
          AND Prenume      = @Prenume
          AND Nr_telefon   = @NrTelefon
          AND IDLocalitate = @IDLocalitate
          AND IDPost       = @IDPost
    )
        RAISERROR('Nu s-au făcut modificări (datele erau identice).', 16, 1);


UPDATE Angajat
SET Nume=@Nume,
	Prenume=@Prenume,
	Nr_telefon=@NrTelefon,
	IDLocalitate=@IDLocalitate,
	IDPost=@IDPost
WHERE IDAngajat=@IDAngajat


END
GO


CREATE OR ALTER PROCEDURE sp_UpdateUserAngajat
	@IdLegat INT,
	@ParolaHashNoua VARCHAR(200)
AS
BEGIN
	

	UPDATE Utilizator
	SET ParolaHash=@ParolaHashNoua
	WHERE IDLegat=@IdLegat AND Rol='Angajat'

END
GO

SELECT *FROM Utilizator
GO

CREATE OR ALTER PROCEDURE sp_InsertBilet
	@IdCursa INT,
	@IDPasager INT,
	@Nr_loc INT,
	@Data_Rezervare DATE
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO Bilet(IDPasager, IDCursa,DataRezervare,Nr_loc) VALUES 
	(@IDPasager,@IdCursa,@Data_Rezervare,@Nr_loc)
END
GO

CREATE OR ALTER PROCEDURE InsertCursa
    @IDTraseu INT,
    @IDTipTransport INT,
    @IDAngajat INT,
    @DataPlecare DATE,
    @OraPlecare TIME
AS
BEGIN
    INSERT INTO Cursa
      (IDTraseu, IDTipTransport, IDAngajat, DataPlecare, OraPlecare)
    VALUES
      (@IDTraseu, @IDTipTransport, @IDAngajat, @DataPlecare, @OraPlecare);
    SELECT SCOPE_IDENTITY();
END
GO


CREATE OR ALTER PROCEDURE UpdateCursa
    @IDCursa INT,
    @IDTraseu INT,
    @IDTipTransport INT,
    @IDAngajat INT,
    @DataPlecare DATE,
    @OraPlecare TIME
AS
BEGIN
    UPDATE Cursa
    SET
      IDTraseu       = @IDTraseu,
      IDTipTransport = @IDTipTransport,
      IDAngajat      = @IDAngajat,
      DataPlecare    = @DataPlecare,
      OraPlecare     = @OraPlecare
    WHERE IDCursa = @IDCursa;
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteCursa
	@IDCursa INT
AS
BEGIN

        DELETE FROM Cursa
		WHERE @IdCursa=IDCursa 
	
END
GO

SELECT *FROM Utilizator
