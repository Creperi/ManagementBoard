CREATE DATABASE IF NOT EXISTS managementboard;
USE managementboard;

CREATE TABLE IF NOT EXISTS PROJECT(
    ID INT UNIQUE PRIMARY KEY,
    TITLE VARCHAR(100),
    DESCRIPTION VARCHAR(100),
    NAME VARCHAR(80),
    PROGRESS INT,
    STARTDATE DATE,
    FINISHDATE DATE
);

CREATE TABLE IF NOT EXISTS TASK(
    ID INT PRIMARY KEY,
    PROJECTID INT,
    DESCRIPTION VARCHAR(100),
    PROGRESS INT,
    STATUS VARCHAR(20),
    STARTDATE DATE,
    FINISHDATE DATE,
    FOREIGN KEY (PROJECTID) REFERENCES PROJECT(ID)
);

INSERT INTO PROJECT (ID, TITLE, DESCRIPTION, NAME, PROGRESS, STARTDATE, FINISHDATE)
VALUES (1, 'Cleaning my bedroom', 'Because I want to spot my things easier', 'Elias', 1, '2024-01-29', '2024-01-31');
INSERT INTO TASK (ID, PROJECTID, DESCRIPTION, PROGRESS, STATUS, STARTDATE, FINISHDATE)
VALUES (1, 1, 'Tidying my clothes in my closet', 25, "PROGRESS" ,'2024-01-29', '2024-01-31');
