# Metafar-Challenge
Challenge de Metafar - Entrevista

# Tecnologías usadas
  - Base de datos [MSSQL Server 2022]
  - [Minimal API with .NET 8](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0) and [Carter](https://github.com/CarterCommunity/Carter)
  - [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
  - CQRS with [MediatR](https://github.com/jbogard/MediatR)
  - [FluentValidation](https://fluentvalidation.net/)
  - [Entity Framework Core 8](https://docs.microsoft.com/en-us/ef/core/)
  - Swagger with Code generation using [NSwag](https://github.com/RicoSuter/NSwag)

# DER
![image](https://github.com/AndreVillalta/Metafar-Challenge/assets/152721275/725e1d1c-5f33-4513-95fe-c72687c4dee1)


# Antes de iniciar la API  
## Ejecutar las siguientes instrucciones en tu motor de base de datos.

CREATE DATABASE MetafarDB
GO

USE MetafarDB
GO

CREATE TABLE Users
(
 UserID INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
 UserName NVARCHAR(100) NOT NULL,
 AccountNumber NVARCHAR(50) NOT NULL,
 CurrentBalance DECIMAL(18, 2) NOT NULL
);

CREATE TABLE Cards
(
 CardID INT IDENTITY(1,1) CONSTRAINT PK_Cards PRIMARY KEY,
 CardNumber NVARCHAR(50) NOT NULL,
 PIN INT CHECK (PIN >= 1000 AND PIN <= 9999),
 FailedAttempts INT NOT NULL DEFAULT 0,
 IsBlocked BIT NOT NULL DEFAULT 0,
 UserID INT NOT NULL,
 CONSTRAINT FK_Cards_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Transactions
(
 TransactionID INT IDENTITY(1,1) CONSTRAINT PK_Transactions PRIMARY KEY,
 TransactionDate DATETIME NOT NULL,
 TransactionType NVARCHAR(50) NOT NULL,
 Amount DECIMAL(18, 2) NOT NULL,
 CardID INT NOT NULL,
 CONSTRAINT FK_Transactions_Cards FOREIGN KEY (CardID) REFERENCES Cards(CardID)
);

INSERT INTO Users (UserName, AccountNumber, CurrentBalance)
VALUES ('Juan Perez', '1234567890', 10000.00),
('Maria Rodriguez', '2345678901', 20000.00),
('Carlos Sanchez', '3456789012', 15000.00),
('Leonel Mesii', '1056789010', 995000.00);

-- Insertar Tarjetas
INSERT INTO Cards (CardNumber, PIN, UserID)
VALUES ('1111222233334444', 1234, 1),
       ('5555666677778888', 2345, 2),
       ('9999000011112222', 3456, 3),
	   ('8889000012112222', 5151, 3),
	   ('1089000012102210', 1010, 4);

-- Insertar Transacciones
-- Depositos para los usuarios
INSERT INTO Transactions (TransactionDate, TransactionType, Amount, CardID)
VALUES (GETDATE(), 'Deposito', 10000.00, 1),
       (GETDATE(), 'Deposito', 20000.00, 2),
       (GETDATE(), 'Deposito', 15000.00, 3),
	   (GETDATE(), 'Deposito', 5000.00, 3);

-- Retiros para los usuarios
INSERT INTO Transactions (TransactionDate, TransactionType, Amount, CardID)
VALUES (GETDATE(), 'Retiro', 5000.00, 1),
       (GETDATE(), 'Retiro', 7000.00, 2),
       (GETDATE(), 'Retiro', 6000.00, 3);

### Cambiar el valor del connectionString desde el appsettings.Development.json
![image](https://github.com/AndreVillalta/Metafar-Challenge/assets/152721275/4b87720f-61ac-4965-ac2f-18f8a2cb1a24)


## Listo, puedes probarlo! 
