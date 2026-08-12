-- Executer dans SSMS apres connexion a YOSRI\SQLEXPRESS
USE VectorysDB;
GO

SELECT @@SERVERNAME AS ServeurSql, DB_NAME() AS BaseCourante, SUSER_SNAME() AS UtilisateurWindows;

SELECT name AS NomTable
FROM sys.tables
ORDER BY name;

SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;

SELECT Id, Email, CodePersonnel, Role FROM Utilisateurs;
SELECT TOP 10 Id, Reference, Intitule, Statut FROM Inventaires ORDER BY Id DESC;
