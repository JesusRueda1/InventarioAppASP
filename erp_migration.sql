-- ==============================================================
-- erp_migration.sql
-- MIGRACIÓN DE SISTEMA BÁSICO A ERP (IMPUESTOS, KARDEX, PAGOS)
-- ==============================================================
-- ADVERTENCIA: Haz un respaldo de tu base de datos antes de 
--              ejecutar este script.
-- ==============================================================

USE `inventario`;

-- 1. TABLA IMPUESTOS Y DATOS SEMILLA
CREATE TABLE IF NOT EXISTS `impuestos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `porcentaje` decimal(5,2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

INSERT IGNORE INTO `impuestos` (`id`, `nombre`, `porcentaje`) VALUES 
(1, 'IVA 19%', 19.00),
(2, 'Exento 0%', 0.00),
(3, 'Excluido 0%', 0.00);

-- 2. MODIFICACION A PRODUCTOS (Cuentan con impuesto asignado)
ALTER TABLE `productos`
  ADD COLUMN `impuesto_id` int NULL AFTER `categoria_id`;

ALTER TABLE `productos`
  ADD CONSTRAINT `fk_producto_impuesto` FOREIGN KEY (`impuesto_id`) REFERENCES `impuestos` (`id`) ON DELETE SET NULL;

-- Asignar el IVA 19% por defecto a todos los productos actuales
UPDATE `productos` SET `impuesto_id` = 1 WHERE `impuesto_id` IS NULL;

-- 3. MODIFICACIONES A TRANSACCIONES (Subtotales, Estado Pago, Saldos)
ALTER TABLE `transacciones`
  ADD COLUMN `subtotal` decimal(12,2) NOT NULL DEFAULT 0 AFTER `fecha`,
  ADD COLUMN `total_impuesto` decimal(12,2) NOT NULL DEFAULT 0 AFTER `subtotal`,
  ADD COLUMN `estado_pago` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Pagado' AFTER `total`,
  ADD COLUMN `saldo_pendiente` decimal(12,2) NOT NULL DEFAULT 0 AFTER `estado_pago`;

-- Migrar data actual (todo lo viejo no tenía impuestos y se considera Pagado)
UPDATE `transacciones` SET `subtotal` = `total`, `estado_pago` = 'Pagado', `saldo_pendiente` = 0;

-- 4. MODIFICACIONES A DETALLES DE COMPRAS/VENTAS (Historizar impuestos)
ALTER TABLE `detalle_compras`
  ADD COLUMN `porcentaje_impuesto` decimal(5,2) NOT NULL DEFAULT 0 AFTER `precio_costo`,
  ADD COLUMN `monto_impuesto` decimal(10,2) NOT NULL DEFAULT 0 AFTER `porcentaje_impuesto`;

ALTER TABLE `detalle_ventas`
  ADD COLUMN `porcentaje_impuesto` decimal(5,2) NOT NULL DEFAULT 0 AFTER `precio_venta`,
  ADD COLUMN `monto_impuesto` decimal(10,2) NOT NULL DEFAULT 0 AFTER `porcentaje_impuesto`;

-- 5. TABLA MOVIMIENTOS KARDEX
CREATE TABLE IF NOT EXISTS `movimientos_kardex` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `fecha` datetime(6) NOT NULL,
  `tipo` varchar(15) NOT NULL,
  `cantidad` int NOT NULL,
  `saldo` int NOT NULL,
  `motivo` varchar(200) NOT NULL,
  `transaccion_id` int NULL,
  `usuario_id` int NULL,
  PRIMARY KEY (`id`),
  KEY `ix_kardex_producto` (`producto_id`),
  KEY `ix_kardex_transaccion` (`transaccion_id`),
  KEY `ix_kardex_usuario` (`usuario_id`),
  CONSTRAINT `fk_kardex_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kardex_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_kardex_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 6. TABLA PAGOS
CREATE TABLE IF NOT EXISTS `pagos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transaccion_id` int NOT NULL,
  `fecha` datetime(6) NOT NULL,
  `monto` decimal(12,2) NOT NULL,
  `metodo_pago` varchar(50) NOT NULL,
  `referencia` varchar(100) NULL,
  `usuario_id` int NULL,
  PRIMARY KEY (`id`),
  KEY `ix_pago_transaccion` (`transaccion_id`),
  KEY `ix_pago_usuario` (`usuario_id`),
  CONSTRAINT `fk_pago_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_pago_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 7. TABLA AUDITORIA LOGS
CREATE TABLE IF NOT EXISTS `auditoria_logs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime(6) NOT NULL,
  `usuario_id` int NULL,
  `modulo` varchar(100) NOT NULL,
  `accion` varchar(100) NOT NULL,
  `detalles` text NOT NULL,
  `direccion_ip` varchar(50) NULL,
  PRIMARY KEY (`id`),
  KEY `ix_auditoria_usuario` (`usuario_id`),
  CONSTRAINT `fk_auditoria_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 8. COMPATIBILIDAD FINAL Y PERMISOS NUEVOS
-- Como somos admin, nos inyectaremos los permisos base de estas cosas.
-- Asumimos que el ROL Administrador es id=1
INSERT IGNORE INTO `permisos` (`id`, `nombre`, `descripcion`) VALUES
(20, 'auditoria.ver', 'Auditoría Global'),
(21, 'pagos.registrar', 'Registrar Pagos'),
(22, 'pagos.ver', 'Ver Pagos'),
(23, 'kardex.ver', 'Ver Inventario'),
(24, 'kardex.ajustar', 'Ajustar Inventario'),
(25, 'impuestos.gestionar', 'Configuración de Impuestos');

INSERT IGNORE INTO `rol_permisos` (`rol_id`, `permiso_id`) VALUES
(1, 20), (1, 21), (1, 22), (1, 23), (1, 24), (1, 25);
