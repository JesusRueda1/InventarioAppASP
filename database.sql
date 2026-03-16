-- ============================================================
-- Sistema de Inventario Simple
-- Script SQL para MySQL / MariaDB
-- Usar con HeidiSQL: abrir Query, pegar y ejecutar (F9)
-- ============================================================

CREATE DATABASE IF NOT EXISTS inventario
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE inventario;

-- ------------------------------------------------------------
-- Tabla: usuarios
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS usuarios (
    id       INT AUTO_INCREMENT PRIMARY KEY,
    nombre   VARCHAR(100) NOT NULL,
    correo   VARCHAR(150) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL          -- se guarda el hash BCrypt
);

-- Usuario de prueba: correo=admin@demo.com  password=Admin123
INSERT INTO usuarios (nombre, correo, password) VALUES
('Administrador',
 'admin@demo.com',
 '$2a$11$K7xWqzFJV6NiGiKHJhZ8/.r.5JlVFlXPSLzRFn.aDAzXm3WvVsmTm');

-- ------------------------------------------------------------
-- Tabla: categorias
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS categorias (
    id     INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

INSERT INTO categorias (nombre) VALUES
('Electrónica'),
('Ropa'),
('Alimentos'),
('Herramientas');

-- ------------------------------------------------------------
-- Tabla: productos
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS productos (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    nombre       VARCHAR(150) NOT NULL,
    descripcion  TEXT,
    precio       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    stock        INT           NOT NULL DEFAULT 0,
    categoria_id INT NOT NULL,
    CONSTRAINT fk_producto_categoria
        FOREIGN KEY (categoria_id) REFERENCES categorias(id)
        ON UPDATE CASCADE ON DELETE RESTRICT
);

INSERT INTO productos (nombre, descripcion, precio, stock, categoria_id) VALUES
('Laptop HP 15"',    'Procesador i5, 8 GB RAM, 256 GB SSD', 750.00, 12, 1),
('Audífonos Sony',   'Inalámbricos, cancelación de ruido',   89.99,  3,  1),
('Camiseta Polo',    'Algodón 100%, talla M',                15.50,  40, 2),
('Arroz Diana 1kg',  'Arroz blanco de primera calidad',       2.50,  2,  3),
('Martillo 16oz',    'Mango de fibra de vidrio',             12.00,  8,  4);

-- ------------------------------------------------------------
-- Tabla: compras (entrada de mercancía)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS compras (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    fecha        DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    proveedor    VARCHAR(150),
    total        DECIMAL(12,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE IF NOT EXISTS detalle_compras (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    compra_id   INT           NOT NULL,
    producto_id INT           NOT NULL,
    cantidad    INT           NOT NULL,
    precio_costo DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_dc_compra   FOREIGN KEY (compra_id)   REFERENCES compras(id)   ON DELETE CASCADE,
    CONSTRAINT fk_dc_producto FOREIGN KEY (producto_id) REFERENCES productos(id) ON DELETE RESTRICT
);

-- ------------------------------------------------------------
-- Tabla: ventas (POS)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ventas (
    id      INT AUTO_INCREMENT PRIMARY KEY,
    fecha   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    total   DECIMAL(12,2) NOT NULL DEFAULT 0.00
);

CREATE TABLE IF NOT EXISTS detalle_ventas (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    venta_id    INT           NOT NULL,
    producto_id INT           NOT NULL,
    cantidad    INT           NOT NULL,
    precio_venta DECIMAL(10,2) NOT NULL,
    CONSTRAINT fk_dv_venta    FOREIGN KEY (venta_id)    REFERENCES ventas(id)    ON DELETE CASCADE,
    CONSTRAINT fk_dv_producto FOREIGN KEY (producto_id) REFERENCES productos(id) ON DELETE RESTRICT
);
