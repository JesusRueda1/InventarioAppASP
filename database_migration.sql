-- ============================================================
-- database_migration.sql
-- Migración: Reestructuración de BD + Módulo de Roles y Permisos
--
-- PASOS:
--   1. Crear tabla transacciones (unifica compras + ventas)
--   2. Migrar datos de compras → transacciones (tipo='Compra')
--   3. Migrar datos de ventas  → transacciones (tipo='Venta')
--   4. Actualizar detalle_compras: compra_id → transaccion_id
--   5. Actualizar detalle_ventas:  venta_id  → transaccion_id
--   6. Crear tablas roles, permisos, rol_permisos
--   7. Agregar rol_id a usuarios
--   8. Insertar roles y permisos por defecto
--   9. Renombrar tablas viejas como _bak (seguridad)
--
-- EJECUTAR en HeidiSQL / MySQL Workbench contra la BD `inventario`
-- ============================================================

SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================
-- PASO 1: Crear tabla transacciones
-- ============================================================
CREATE TABLE IF NOT EXISTS `transacciones` (
  `id`          INT           NOT NULL AUTO_INCREMENT,
  `tipo`        VARCHAR(10)   NOT NULL COMMENT 'Compra | Venta',
  `fecha`       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total`       DECIMAL(12,2) NOT NULL DEFAULT '0.00',
  `proveedor`   VARCHAR(150)  DEFAULT NULL COMMENT 'Solo para tipo Compra',
  `usuario_id`  INT           DEFAULT NULL,
  `_old_id`     INT           DEFAULT NULL COMMENT 'ID original (temporal, se elimina al final)',
  `_old_tipo`   VARCHAR(10)   DEFAULT NULL COMMENT 'Tipo original (temporal)',
  PRIMARY KEY (`id`),
  KEY `idx_tipo`       (`tipo`),
  KEY `idx_usuario_id` (`usuario_id`),
  CONSTRAINT `fk_trans_usuario` FOREIGN KEY (`usuario_id`)
    REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- PASO 2: Migrar compras → transacciones (tipo = 'Compra')
-- ============================================================
INSERT INTO `transacciones` (`tipo`, `fecha`, `proveedor`, `total`, `_old_id`, `_old_tipo`)
SELECT 'Compra', `fecha`, `proveedor`, `total`, `id`, 'Compra'
FROM `compras`
ORDER BY `id`;

-- ============================================================
-- PASO 3: Migrar ventas → transacciones (tipo = 'Venta')
-- ============================================================
INSERT INTO `transacciones` (`tipo`, `fecha`, `proveedor`, `total`, `_old_id`, `_old_tipo`)
SELECT 'Venta', `fecha`, NULL, `total`, `id`, 'Venta'
FROM `ventas`
ORDER BY `id`;

-- ============================================================
-- PASO 4: Actualizar detalle_compras
--   • Agregar columna transaccion_id
--   • Poblarla usando el mapeo _old_id / _old_tipo
--   • Eliminar columna compra_id y su FK
-- ============================================================

-- 4a. Agregar nueva columna
ALTER TABLE `detalle_compras`
  ADD COLUMN `transaccion_id` INT NULL AFTER `id`;

-- 4b. Poblar transaccion_id a partir del mapeo
UPDATE `detalle_compras` dc
  JOIN `transacciones` t
    ON t.`_old_id` = dc.`compra_id`
   AND t.`_old_tipo` = 'Compra'
SET dc.`transaccion_id` = t.`id`;

-- 4c. Hacer la columna NOT NULL y agregar FK
ALTER TABLE `detalle_compras`
  MODIFY COLUMN `transaccion_id` INT NOT NULL,
  ADD CONSTRAINT `fk_dc_transaccion`
    FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE;

-- 4d. Eliminar la FK vieja y la columna compra_id
ALTER TABLE `detalle_compras`
  DROP FOREIGN KEY `fk_dc_compra`,
  DROP COLUMN `compra_id`;

-- ============================================================
-- PASO 5: Actualizar detalle_ventas
--   • Agregar columna transaccion_id
--   • Poblarla usando el mapeo _old_id / _old_tipo
--   • Eliminar columna venta_id y su FK
-- ============================================================

-- 5a. Agregar nueva columna
ALTER TABLE `detalle_ventas`
  ADD COLUMN `transaccion_id` INT NULL AFTER `id`;

-- 5b. Poblar transaccion_id a partir del mapeo
UPDATE `detalle_ventas` dv
  JOIN `transacciones` t
    ON t.`_old_id` = dv.`venta_id`
   AND t.`_old_tipo` = 'Venta'
SET dv.`transaccion_id` = t.`id`;

-- 5c. Hacer la columna NOT NULL y agregar FK
ALTER TABLE `detalle_ventas`
  MODIFY COLUMN `transaccion_id` INT NOT NULL,
  ADD CONSTRAINT `fk_dv_transaccion`
    FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE;

-- 5d. Eliminar la FK vieja y la columna venta_id
ALTER TABLE `detalle_ventas`
  DROP FOREIGN KEY `fk_dv_venta`,
  DROP COLUMN `venta_id`;

-- ============================================================
-- PASO 6: Limpiar columnas temporales de transacciones
-- ============================================================
ALTER TABLE `transacciones`
  DROP COLUMN `_old_id`,
  DROP COLUMN `_old_tipo`;

-- ============================================================
-- PASO 7: Crear tabla roles
-- ============================================================
CREATE TABLE IF NOT EXISTS `roles` (
  `id`          INT          NOT NULL AUTO_INCREMENT,
  `nombre`      VARCHAR(80)  NOT NULL,
  `descripcion` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- PASO 8: Crear tabla permisos
-- ============================================================
CREATE TABLE IF NOT EXISTS `permisos` (
  `id`          INT          NOT NULL AUTO_INCREMENT,
  `nombre`      VARCHAR(100) NOT NULL,
  `descripcion` VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_permiso_nombre` (`nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- PASO 9: Crear tabla pivote rol_permisos
-- ============================================================
CREATE TABLE IF NOT EXISTS `rol_permisos` (
  `rol_id`     INT NOT NULL,
  `permiso_id` INT NOT NULL,
  PRIMARY KEY (`rol_id`, `permiso_id`),
  CONSTRAINT `fk_rp_rol`     FOREIGN KEY (`rol_id`)     REFERENCES `roles`   (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_rp_permiso` FOREIGN KEY (`permiso_id`) REFERENCES `permisos` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================
-- PASO 10: Agregar rol_id a usuarios
-- ============================================================
ALTER TABLE `usuarios`
  ADD COLUMN `rol_id` INT DEFAULT NULL AFTER `password`,
  ADD CONSTRAINT `fk_usuario_rol`
    FOREIGN KEY (`rol_id`) REFERENCES `roles` (`id`) ON DELETE SET NULL;

-- ============================================================
-- PASO 11: Insertar roles predeterminados
-- ============================================================
INSERT INTO `roles` (`id`, `nombre`, `descripcion`) VALUES
  (1, 'Administrador', 'Acceso total al sistema'),
  (2, 'Vendedor',      'Puede registrar ventas y consultar productos'),
  (3, 'Bodeguero',     'Puede registrar compras y gestionar inventario');

-- ============================================================
-- PASO 12: Insertar permisos del sistema
-- ============================================================
INSERT INTO `permisos` (`nombre`, `descripcion`) VALUES
  ('productos.ver',       'Ver lista de productos'),
  ('productos.crear',     'Crear nuevos productos'),
  ('productos.editar',    'Editar productos existentes'),
  ('productos.eliminar',  'Eliminar productos'),
  ('categorias.ver',      'Ver lista de categorías'),
  ('categorias.crear',    'Crear nuevas categorías'),
  ('categorias.editar',   'Editar categorías existentes'),
  ('categorias.eliminar', 'Eliminar categorías'),
  ('compras.ver',         'Ver historial de compras'),
  ('compras.registrar',   'Registrar nuevas compras'),
  ('ventas.ver',          'Ver historial de ventas'),
  ('ventas.registrar',    'Registrar nuevas ventas'),
  ('usuarios.ver',        'Ver lista de usuarios'),
  ('usuarios.crear',      'Crear nuevos usuarios'),
  ('usuarios.editar',     'Editar usuarios existentes'),
  ('usuarios.eliminar',   'Eliminar usuarios'),
  ('roles.ver',           'Ver roles y sus permisos'),
  ('roles.editar',        'Editar permisos de los roles');

-- ============================================================
-- PASO 13: Asignar permisos al rol Administrador (TODOS)
-- ============================================================
INSERT INTO `rol_permisos` (`rol_id`, `permiso_id`)
SELECT 1, `id` FROM `permisos`;

-- ============================================================
-- PASO 14: Asignar permisos al rol Vendedor
-- ============================================================
INSERT INTO `rol_permisos` (`rol_id`, `permiso_id`)
SELECT 2, `id` FROM `permisos`
WHERE `nombre` IN (
  'productos.ver',
  'categorias.ver',
  'ventas.ver',
  'ventas.registrar'
);

-- ============================================================
-- PASO 15: Asignar permisos al rol Bodeguero
-- ============================================================
INSERT INTO `rol_permisos` (`rol_id`, `permiso_id`)
SELECT 3, `id` FROM `permisos`
WHERE `nombre` IN (
  'productos.ver',
  'productos.editar',
  'categorias.ver',
  'compras.ver',
  'compras.registrar'
);

-- ============================================================
-- PASO 16: Asignar rol Administrador al usuario existente
-- ============================================================
UPDATE `usuarios`
SET `rol_id` = 1
WHERE `correo` = 'admin@demo.com';

-- ============================================================
-- PASO 17: Renombrar tablas viejas como _bak (respaldo)
--   Puedes eliminarlas manualmente cuando estés seguro.
-- ============================================================
RENAME TABLE `compras` TO `compras_bak`;
RENAME TABLE `ventas`  TO `ventas_bak`;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================
-- FIN DE LA MIGRACIÓN
-- Verifica con:
--   SELECT * FROM transacciones;
--   SELECT * FROM detalle_compras;
--   SELECT * FROM detalle_ventas;
--   SELECT * FROM roles;
--   SELECT * FROM permisos;
--   SELECT * FROM rol_permisos;
--   SELECT id, nombre, correo, rol_id FROM usuarios;
-- ============================================================
