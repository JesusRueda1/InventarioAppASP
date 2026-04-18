-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         8.0.30 - MySQL Community Server - GPL
-- SO del servidor:              Win64
-- HeidiSQL Versión:             12.4.0.6659
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Volcando estructura para tabla inventario.auditoria_logs
CREATE TABLE IF NOT EXISTS `auditoria_logs` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime(6) NOT NULL,
  `usuario_id` int DEFAULT NULL,
  `modulo` varchar(100) NOT NULL,
  `accion` varchar(100) NOT NULL,
  `detalles` text NOT NULL,
  `direccion_ip` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_auditoria_usuario` (`usuario_id`),
  CONSTRAINT `fk_auditoria_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.auditoria_logs: ~9 rows (aproximadamente)
INSERT INTO `auditoria_logs` (`id`, `fecha`, `usuario_id`, `modulo`, `accion`, `detalles`, `direccion_ip`) VALUES
	(1, '2026-04-16 17:23:46.126651', 2, 'Categorias', 'POST - Guardar', 'El usuario interactuó con el módulo Categorias ejecutando la acción Guardar.', '::1'),
	(2, '2026-04-16 17:24:14.992123', 2, 'Categorias', 'POST - Guardar', 'El usuario interactuó con el módulo Categorias ejecutando la acción Guardar.', '::1'),
	(3, '2026-04-16 17:25:21.469047', 2, 'Productos', 'POST - Guardar', 'El usuario interactuó con el módulo Productos ejecutando la acción Guardar.', '::1'),
	(4, '2026-04-16 17:26:57.979987', 2, 'Ventas', 'POST - Registrar', 'El usuario interactuó con el módulo Ventas ejecutando la acción Registrar.', '::1'),
	(5, '2026-04-16 17:28:19.331500', 2, 'Roles', 'POST - Edit', 'El usuario interactuó con el módulo Roles ejecutando la acción Edit.', '::1'),
	(6, '2026-04-16 17:37:04.182667', 2, 'Categorias', 'POST - Guardar', 'El usuario interactuó con el módulo Categorias ejecutando la acción Guardar.', '::1'),
	(7, '2026-04-16 17:37:51.895858', 2, 'Productos', 'POST - Guardar', 'El usuario interactuó con el módulo Productos ejecutando la acción Guardar.', '::1'),
	(8, '2026-04-16 17:38:44.143179', 2, 'Compras', 'POST - Registrar', 'El usuario interactuó con el módulo Compras ejecutando la acción Registrar.', '::1'),
	(9, '2026-04-17 09:56:02.108581', 2, 'Ventas', 'POST - Registrar', 'El usuario interactuó con el módulo Ventas ejecutando la acción Registrar.', '::1'),
	(10, '2026-04-18 12:24:07.846134', 2, 'Usuarios', 'POST - Edit', 'El usuario interactuó con el módulo Usuarios ejecutando la acción Edit.', '::1'),
	(11, '2026-04-18 12:24:55.626482', 2, 'Usuarios', 'POST - Create', 'El usuario interactuó con el módulo Usuarios ejecutando la acción Create.', '::1'),
	(12, '2026-04-18 12:29:37.529176', 2, 'Usuarios', 'POST - Edit', 'El usuario interactuó con el módulo Usuarios ejecutando la acción Edit.', '::1');

-- Volcando estructura para tabla inventario.categorias
CREATE TABLE IF NOT EXISTS `categorias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.categorias: ~8 rows (aproximadamente)
INSERT INTO `categorias` (`id`, `nombre`) VALUES
	(1, 'Electrónica'),
	(2, 'Ropa'),
	(3, 'Alimentos'),
	(4, 'Herramientas'),
	(5, 'Bebidas'),
	(6, 'Bebidas'),
	(7, 'ca'),
	(8, 'Test Categor'),
	(9, 'Test Category ERP'),
	(10, 'Test ERP Cat');

-- Volcando estructura para tabla inventario.compras_bak
CREATE TABLE IF NOT EXISTS `compras_bak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `proveedor` varchar(150) DEFAULT NULL,
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.compras_bak: ~3 rows (aproximadamente)
INSERT INTO `compras_bak` (`id`, `fecha`, `proveedor`, `total`) VALUES
	(1, '2026-03-16 16:16:07', 'JUANITO', 765.50),
	(2, '2026-04-07 12:56:26', 'Distribuidora ABC', 385.28),
	(3, '2026-04-07 13:02:16', 'Distribuidora ABC', 750.00),
	(4, '2026-04-07 13:35:30', 'Proveedor Test', 750.01);

-- Volcando estructura para tabla inventario.detalle_compras
CREATE TABLE IF NOT EXISTS `detalle_compras` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transaccion_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_costo` decimal(10,2) NOT NULL,
  `porcentaje_impuesto` decimal(5,2) NOT NULL DEFAULT '0.00',
  `monto_impuesto` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `fk_dc_producto` (`producto_id`),
  KEY `fk_dc_transaccion` (`transaccion_id`),
  CONSTRAINT `fk_dc_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_dc_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.detalle_compras: ~1 rows (aproximadamente)
INSERT INTO `detalle_compras` (`id`, `transaccion_id`, `producto_id`, `cantidad`, `precio_costo`, `porcentaje_impuesto`, `monto_impuesto`) VALUES
	(1, 4, 1, 1, 750.01, 0.00, 0.00),
	(2, 18, 1, 5, 7501000.00, 19.00, 7125950.00);

-- Volcando estructura para tabla inventario.detalle_ventas
CREATE TABLE IF NOT EXISTS `detalle_ventas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transaccion_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_venta` decimal(10,2) NOT NULL,
  `porcentaje_impuesto` decimal(5,2) NOT NULL DEFAULT '0.00',
  `monto_impuesto` decimal(10,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`),
  KEY `fk_dv_producto` (`producto_id`),
  KEY `fk_dv_transaccion` (`transaccion_id`),
  CONSTRAINT `fk_dv_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_dv_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.detalle_ventas: ~1 rows (aproximadamente)
INSERT INTO `detalle_ventas` (`id`, `transaccion_id`, `producto_id`, `cantidad`, `precio_venta`, `porcentaje_impuesto`, `monto_impuesto`) VALUES
	(1, 11, 1, 1, 750.00, 0.00, 0.00),
	(2, 15, 3, 1, 15.50, 19.00, 2.95),
	(3, 17, 11, 1, 25000.00, 19.00, 4750.00),
	(4, 19, 3, 7, 15.50, 19.00, 20.62);

-- Volcando estructura para tabla inventario.impuestos
CREATE TABLE IF NOT EXISTS `impuestos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `porcentaje` decimal(5,2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.impuestos: ~3 rows (aproximadamente)
INSERT INTO `impuestos` (`id`, `nombre`, `porcentaje`) VALUES
	(1, 'IVA 19%', 19.00),
	(2, 'Exento 0%', 0.00),
	(3, 'Excluido 0%', 0.00);

-- Volcando estructura para tabla inventario.movimientos_kardex
CREATE TABLE IF NOT EXISTS `movimientos_kardex` (
  `id` int NOT NULL AUTO_INCREMENT,
  `producto_id` int NOT NULL,
  `fecha` datetime(6) NOT NULL,
  `tipo` varchar(15) NOT NULL,
  `cantidad` int NOT NULL,
  `saldo` int NOT NULL,
  `motivo` varchar(200) NOT NULL,
  `transaccion_id` int DEFAULT NULL,
  `usuario_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_kardex_producto` (`producto_id`),
  KEY `ix_kardex_transaccion` (`transaccion_id`),
  KEY `ix_kardex_usuario` (`usuario_id`),
  CONSTRAINT `fk_kardex_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_kardex_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE SET NULL,
  CONSTRAINT `fk_kardex_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.movimientos_kardex: ~5 rows (aproximadamente)
INSERT INTO `movimientos_kardex` (`id`, `producto_id`, `fecha`, `tipo`, `cantidad`, `saldo`, `motivo`, `transaccion_id`, `usuario_id`) VALUES
	(1, 3, '2026-04-16 17:12:38.572835', 'Egreso', 1, 39, 'Venta facturada en terminal POS', 15, 2),
	(2, 10, '2026-04-16 17:13:41.965512', 'Ingreso', 50, 50, 'Inventario inicial al crear el producto', NULL, NULL),
	(3, 11, '2026-04-16 17:25:20.983408', 'Ingreso', 100, 100, 'Inventario inicial al crear el producto', NULL, NULL),
	(4, 11, '2026-04-16 17:26:57.710830', 'Egreso', 1, 99, 'Venta facturada en terminal POS', 17, 2),
	(5, 12, '2026-04-16 17:37:51.625617', 'Ingreso', 50, 50, 'Inventario inicial al crear el producto', NULL, NULL),
	(6, 1, '2026-04-16 17:38:43.663302', 'Ingreso', 5, 127, 'Entrada por compra a proveedor Test Proveedor', 18, 2),
	(7, 3, '2026-04-17 09:56:01.563946', 'Egreso', 7, 32, 'Venta facturada en terminal POS', 19, 2);

-- Volcando estructura para tabla inventario.pagos
CREATE TABLE IF NOT EXISTS `pagos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `transaccion_id` int NOT NULL,
  `fecha` datetime(6) NOT NULL,
  `monto` decimal(12,2) NOT NULL,
  `metodo_pago` varchar(50) NOT NULL,
  `referencia` varchar(100) DEFAULT NULL,
  `usuario_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_pago_transaccion` (`transaccion_id`),
  KEY `ix_pago_usuario` (`usuario_id`),
  CONSTRAINT `fk_pago_transaccion` FOREIGN KEY (`transaccion_id`) REFERENCES `transacciones` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_pago_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.pagos: ~0 rows (aproximadamente)

-- Volcando estructura para tabla inventario.permisos
CREATE TABLE IF NOT EXISTS `permisos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_permiso_nombre` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.permisos: ~24 rows (aproximadamente)
INSERT INTO `permisos` (`id`, `nombre`, `descripcion`) VALUES
	(1, 'productos.ver', 'Ver lista de productos'),
	(2, 'productos.crear', 'Crear nuevos productos'),
	(3, 'productos.editar', 'Editar productos existentes'),
	(4, 'productos.eliminar', 'Eliminar productos'),
	(5, 'categorias.ver', 'Ver lista de categorías'),
	(6, 'categorias.crear', 'Crear nuevas categorías'),
	(7, 'categorias.editar', 'Editar categorías existentes'),
	(8, 'categorias.eliminar', 'Eliminar categorías'),
	(9, 'compras.ver', 'Ver historial de compras'),
	(10, 'compras.registrar', 'Registrar nuevas compras'),
	(11, 'ventas.ver', 'Ver historial de ventas'),
	(12, 'ventas.registrar', 'Registrar nuevas ventas'),
	(13, 'usuarios.ver', 'Ver lista de usuarios'),
	(14, 'usuarios.crear', 'Crear nuevos usuarios'),
	(15, 'usuarios.editar', 'Editar usuarios existentes'),
	(16, 'usuarios.eliminar', 'Eliminar usuarios'),
	(17, 'roles.ver', 'Ver roles y sus permisos'),
	(18, 'roles.editar', 'Editar permisos de los roles'),
	(19, 'auditoria.ver', 'Ver registros de auditoría del sistema'),
	(20, 'pagos.registrar', 'Registrar pagos y abonos'),
	(21, 'pagos.ver', 'Ver cuentas por cobrar y pagar'),
	(22, 'kardex.ver', 'Ver movimientos de inventario (Kárdex)'),
	(23, 'kardex.ajustar', 'Realizar ajustes manuales de stock'),
	(24, 'impuestos.gestionar', 'Configurar impuestos del sistema');

-- Volcando estructura para tabla inventario.productos
CREATE TABLE IF NOT EXISTS `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) NOT NULL,
  `descripcion` text,
  `precio` decimal(10,2) NOT NULL DEFAULT '0.00',
  `stock` int NOT NULL DEFAULT '0',
  `categoria_id` int NOT NULL,
  `impuesto_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_producto_categoria` (`categoria_id`),
  KEY `fk_producto_impuesto` (`impuesto_id`),
  CONSTRAINT `fk_producto_categoria` FOREIGN KEY (`categoria_id`) REFERENCES `categorias` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_producto_impuesto` FOREIGN KEY (`impuesto_id`) REFERENCES `impuestos` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.productos: ~9 rows (aproximadamente)
INSERT INTO `productos` (`id`, `nombre`, `descripcion`, `precio`, `stock`, `categoria_id`, `impuesto_id`) VALUES
	(1, 'Laptop HP 15"', 'Procesador i5, 8 GB RAM, 256 GB SSD', 750.00, 127, 1, 1),
	(2, 'Audífonos Sony', 'Inalámbricos, cancelación de ruido', 89.99, 3, 1, 1),
	(3, 'Camiseta Polo', 'Algodón 100%, talla M', 15.50, 32, 2, 1),
	(4, 'Arroz Diana 1kg', 'Arroz blanco de primera calidad', 2.50, 2, 3, 1),
	(5, 'Martillo 16oz', 'Mango de fibra de vidrio', 12.00, 8, 4, 1),
	(8, 'Jugo de Naranja', 'Jugo natural 500ml', 3.50, 20, 5, 1),
	(9, 'Leche Entera', 'Caja 1L Natural', 1.20, 50, 1, 1),
	(10, 'EBA 1', 'CA', 200.00, 50, 3, 1),
	(11, 'Producto Test ERP', '', 25000.00, 99, 1, 1),
	(12, 'Producto ERP Test', '', 5000.00, 50, 1, 1);

-- Volcando estructura para tabla inventario.roles
CREATE TABLE IF NOT EXISTS `roles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(80) NOT NULL,
  `descripcion` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.roles: ~3 rows (aproximadamente)
INSERT INTO `roles` (`id`, `nombre`, `descripcion`) VALUES
	(1, 'Administrador', 'Acceso total al sistema'),
	(2, 'Vendedor', 'Puede registrar ventas y consultar productos'),
	(3, 'Bodeguero', 'Puede registrar compras y gestionar inventario');

-- Volcando estructura para tabla inventario.rol_permisos
CREATE TABLE IF NOT EXISTS `rol_permisos` (
  `rol_id` int NOT NULL,
  `permiso_id` int NOT NULL,
  PRIMARY KEY (`rol_id`,`permiso_id`),
  KEY `fk_rp_permiso` (`permiso_id`),
  CONSTRAINT `fk_rp_permiso` FOREIGN KEY (`permiso_id`) REFERENCES `permisos` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_rp_rol` FOREIGN KEY (`rol_id`) REFERENCES `roles` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.rol_permisos: ~33 rows (aproximadamente)
INSERT INTO `rol_permisos` (`rol_id`, `permiso_id`) VALUES
	(1, 1),
	(2, 1),
	(3, 1),
	(1, 2),
	(1, 3),
	(3, 3),
	(1, 4),
	(1, 5),
	(2, 5),
	(3, 5),
	(1, 6),
	(1, 7),
	(1, 8),
	(1, 9),
	(3, 9),
	(1, 10),
	(3, 10),
	(1, 11),
	(2, 11),
	(1, 12),
	(2, 12),
	(1, 13),
	(1, 14),
	(1, 15),
	(1, 16),
	(1, 17),
	(1, 18),
	(1, 19),
	(1, 20),
	(1, 21),
	(1, 22),
	(1, 23),
	(1, 24);

-- Volcando estructura para tabla inventario.transacciones
CREATE TABLE IF NOT EXISTS `transacciones` (
  `id` int NOT NULL AUTO_INCREMENT,
  `tipo` varchar(10) NOT NULL COMMENT 'Compra | Venta',
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `subtotal` decimal(12,2) NOT NULL DEFAULT '0.00',
  `total_impuesto` decimal(12,2) NOT NULL DEFAULT '0.00',
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  `estado_pago` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Pagado',
  `saldo_pendiente` decimal(12,2) NOT NULL DEFAULT '0.00',
  `proveedor` varchar(150) DEFAULT NULL COMMENT 'Solo para tipo Compra',
  `usuario_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_tipo` (`tipo`),
  KEY `idx_usuario_id` (`usuario_id`),
  CONSTRAINT `fk_trans_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Volcando datos para la tabla inventario.transacciones: ~11 rows (aproximadamente)
INSERT INTO `transacciones` (`id`, `tipo`, `fecha`, `subtotal`, `total_impuesto`, `total`, `estado_pago`, `saldo_pendiente`, `proveedor`, `usuario_id`) VALUES
	(1, 'Compra', '2026-03-16 16:16:07', 765.50, 0.00, 765.50, 'Pagado', 0.00, 'JUANITO', NULL),
	(2, 'Compra', '2026-04-07 12:56:26', 385.28, 0.00, 385.28, 'Pagado', 0.00, 'Distribuidora ABC', NULL),
	(3, 'Compra', '2026-04-07 13:02:16', 750.00, 0.00, 750.00, 'Pagado', 0.00, 'Distribuidora ABC', NULL),
	(4, 'Compra', '2026-04-07 13:35:30', 750.01, 0.00, 750.01, 'Pagado', 0.00, 'Proveedor Test', NULL),
	(8, 'Venta', '2026-03-16 14:22:02', 15.50, 0.00, 15.50, 'Pagado', 0.00, NULL, NULL),
	(9, 'Venta', '2026-03-16 14:48:23', 15.50, 0.00, 15.50, 'Pagado', 0.00, NULL, NULL),
	(10, 'Venta', '2026-04-07 13:01:13', 750.00, 0.00, 750.00, 'Pagado', 0.00, NULL, NULL),
	(11, 'Venta', '2026-04-07 13:36:34', 750.00, 0.00, 750.00, 'Pagado', 0.00, NULL, NULL),
	(15, 'Venta', '2026-04-16 17:12:39', 15.50, 2.95, 18.45, 'Pagado', 0.00, NULL, 2),
	(16, 'Compra', '2026-04-16 17:26:09', 8251650000.00, 1567813500.00, 9819463500.00, 'Pendiente', 9819463500.00, 'Proveedor Test', 2),
	(17, 'Venta', '2026-04-16 17:26:58', 25000.00, 4750.00, 29750.00, 'Pagado', 0.00, NULL, 2),
	(18, 'Compra', '2026-04-16 17:38:44', 37505000.00, 7125950.00, 44630950.00, 'Pendiente', 44630950.00, 'Test Proveedor', 2),
	(19, 'Venta', '2026-04-17 09:56:02', 108.50, 20.62, 129.12, 'Pagado', 0.00, NULL, 2);

-- Volcando estructura para tabla inventario.usuarios
CREATE TABLE IF NOT EXISTS `usuarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `username` varchar(50) NOT NULL,
  `correo` varchar(150) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rol_id` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `correo` (`correo`),
  UNIQUE KEY `uk_usuarios_username` (`username`),
  KEY `fk_usuario_rol` (`rol_id`) USING BTREE,
  CONSTRAINT `fk_usuario_rol` FOREIGN KEY (`rol_id`) REFERENCES `roles` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.usuarios: ~2 rows (aproximadamente)
INSERT INTO `usuarios` (`id`, `nombre`, `username`, `correo`, `password`, `rol_id`) VALUES
	(2, 'Administrador', 'Admin', 'admin@demo.com', '$2a$11$OhHdSApn1LnQ.SpHeBYPweEyvw2niIjDle2JJnIasohhnkXbxAVJa', 1),
	(3, 'Pedro Navaja', 'VentasPedro', 'ventaspedro@gmail.com', '$2a$11$D7qXCkRveXHGlj1T5I56mOar95aIYISa/EvaWaaIVrFrR.2MVQWQS', 2);

-- Volcando estructura para tabla inventario.ventas_bak
CREATE TABLE IF NOT EXISTS `ventas_bak` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.ventas_bak: ~4 rows (aproximadamente)
INSERT INTO `ventas_bak` (`id`, `fecha`, `total`) VALUES
	(1, '2026-03-16 14:22:02', 15.50),
	(2, '2026-03-16 14:48:23', 15.50),
	(3, '2026-04-07 13:01:13', 750.00),
	(4, '2026-04-07 13:36:34', 750.00);

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
