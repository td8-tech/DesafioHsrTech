CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(10) NOT NULL CHECK (Role IN ('Admin', 'User'))
);

go

CREATE TABLE Produtos (
    Id INT PRIMARY KEY IDENTITY,
    Nome NVARCHAR(100) NOT NULL,
    CreatedByUserId INT FOREIGN KEY REFERENCES Users(Id),
    Discriminator NVARCHAR(20) NOT NULL,
    Autor NVARCHAR(100),
    PeriodoGarantia INT
);

go

CREATE VIEW ProdutosDetails AS
SELECT Id, Nome, CreatedByUserId, Discriminator, Autor, PeriodoGarantia
FROM Produtos;

go

CREATE PROCEDURE GetUserProdutos
    @UserId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Id = @UserId AND Role = 'Admin')
        SELECT * FROM Produtos
    ELSE
        SELECT * FROM Produtos WHERE CreatedByUserId = @UserId
END

go

ALTER TABLE Users
DROP CONSTRAINT CK__Users__Role__25869641; --  Use o nome correto

-- Adicione uma nova constraint com os valores permitidos
ALTER TABLE Users
ADD CONSTRAINT CK_Users_Role CHECK (Role IN ('Admin', 'User', 'admin', 'user'));