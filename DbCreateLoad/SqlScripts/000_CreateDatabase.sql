IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UserDB')
BEGIN
    CREATE DATABASE UserDB;
END