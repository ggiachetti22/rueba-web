SELECT *, CONCAT('El de Usuario: (',u.UserName,') Nombre de estado: ',s.StateName) AS NEWCAMP FROM Tb_Users u INNER JOIN Tb_States s ON u.UserID = s.StateID
SELECT *, CONCAT(u.UserName,' ', s.StateName) AS NEWCAMP FROM Tb_Users u INNER JOIN Tb_States s ON u.UserID = s.StateID WHERE u.UserID = 2
SELECT * FROM Tb_Users
SELECT * FROM Tb_States
SELECT * FROM Tb_Messages
