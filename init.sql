CREATE DATABASE IF NOT EXISTS cidades_db;
USE cidades_db;

CREATE TABLE IF NOT EXISTS `Cidades` (
  `CidadeId` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(100) NOT NULL,
  `Sigla` char(2) NOT NULL,
  `IBGEMunicipio` int NOT NULL,
  `Latitude` decimal(11,8) DEFAULT NULL,
  `Longitude` decimal(11,8) DEFAULT NULL,
  PRIMARY KEY (`CidadeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `Aluno` (
  `AlunoId` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(100) DEFAULT NULL,
  `Idade` int DEFAULT NULL,
  `CidadeId` int DEFAULT NULL,
  `Foto` longblob,
  PRIMARY KEY (`AlunoId`),
  CONSTRAINT `FK_Aluno_Cidades` FOREIGN KEY (`CidadeId`) REFERENCES `Cidades` (`CidadeId`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;