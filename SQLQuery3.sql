USE SIMA_SOFTWARE_DB
GO

-- =========================
-- TIPOS DE CLIENTE
-- =========================
INSERT INTO TipoClientes (Categoria)
VALUES ('Minorista'), ('Mayorista');

-- =========================
-- DIRECCIONES
-- =========================
INSERT INTO Direcciones (Barrio, Calle, NroPuerta)
VALUES
('Centro', 'San Martin', '123'),
('Norte', 'Belgrano', '456');

-- =========================
-- CUENTAS
-- =========================
INSERT INTO Cuentas (Saldo, CondicionesPago)
VALUES
(1000, 'Contado'),
(2000, '30 días');

-- =========================
-- CLIENTES
-- =========================
INSERT INTO Clientes (Nombre, Email, Telefono, DireccionId, TipoClienteId, CuentaId, Activo)
VALUES
('Juan Perez', 'juan@gmail.com', '111111', 1, 1, 1, 1),
('Maria Lopez', 'maria@gmail.com', '222222', 2, 2, 2, 1);




SELECT * FROM Clientes

INSERT INTO Productos (Nombre, Precio)
VALUES
('Zapatillas Nike', 20000),
('Remera Adidas', 8000),
('Pantalón Puma', 12000);

INSERT INTO Inventarios (IdProducto, Stock)
VALUES
(1, 10),
(2, 20),
(3, 15);