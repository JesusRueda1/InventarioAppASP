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

-- Volcando estructura para tabla inventario.categorias
CREATE TABLE IF NOT EXISTS `categorias` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.categorias: ~6 rows (aproximadamente)
INSERT INTO `categorias` (`id`, `nombre`) VALUES
	(1, 'Electrónica'),
	(2, 'Ropa'),
	(3, 'Alimentos'),
	(4, 'Herramientas'),
	(5, 'Bebidas'),
	(6, 'Bebidas');

-- Volcando estructura para tabla inventario.compras
CREATE TABLE IF NOT EXISTS `compras` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `proveedor` varchar(150) DEFAULT NULL,
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.compras: ~3 rows (aproximadamente)
INSERT INTO `compras` (`id`, `fecha`, `proveedor`, `total`) VALUES
	(1, '2026-03-16 16:16:07', 'JUANITO', 765.50),
	(2, '2026-04-07 12:56:26', 'Distribuidora ABC', 385.28),
	(3, '2026-04-07 13:02:16', 'Distribuidora ABC', 750.00),
	(4, '2026-04-07 13:35:30', 'Proveedor Test', 750.01);

-- Volcando estructura para tabla inventario.detalle_compras
CREATE TABLE IF NOT EXISTS `detalle_compras` (
  `id` int NOT NULL AUTO_INCREMENT,
  `compra_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_costo` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_dc_compra` (`compra_id`),
  KEY `fk_dc_producto` (`producto_id`),
  CONSTRAINT `fk_dc_compra` FOREIGN KEY (`compra_id`) REFERENCES `compras` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_dc_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.detalle_compras: ~0 rows (aproximadamente)
INSERT INTO `detalle_compras` (`id`, `compra_id`, `producto_id`, `cantidad`, `precio_costo`) VALUES
	(1, 4, 1, 1, 750.01);

-- Volcando estructura para tabla inventario.detalle_ventas
CREATE TABLE IF NOT EXISTS `detalle_ventas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `venta_id` int NOT NULL,
  `producto_id` int NOT NULL,
  `cantidad` int NOT NULL,
  `precio_venta` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_dv_venta` (`venta_id`),
  KEY `fk_dv_producto` (`producto_id`),
  CONSTRAINT `fk_dv_producto` FOREIGN KEY (`producto_id`) REFERENCES `productos` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_dv_venta` FOREIGN KEY (`venta_id`) REFERENCES `ventas` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.detalle_ventas: ~0 rows (aproximadamente)
INSERT INTO `detalle_ventas` (`id`, `venta_id`, `producto_id`, `cantidad`, `precio_venta`) VALUES
	(1, 4, 1, 1, 750.00);

-- Volcando estructura para tabla inventario.productos
CREATE TABLE IF NOT EXISTS `productos` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) NOT NULL,
  `descripcion` text,
  `precio` decimal(10,2) NOT NULL DEFAULT '0.00',
  `stock` int NOT NULL DEFAULT '0',
  `categoria_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_producto_categoria` (`categoria_id`),
  CONSTRAINT `fk_producto_categoria` FOREIGN KEY (`categoria_id`) REFERENCES `categorias` (`id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.productos: ~7 rows (aproximadamente)
INSERT INTO `productos` (`id`, `nombre`, `descripcion`, `precio`, `stock`, `categoria_id`) VALUES
	(1, 'Laptop HP 15"', 'Procesador i5, 8 GB RAM, 256 GB SSD', 750.00, 12, 1),
	(2, 'Audífonos Sony', 'Inalámbricos, cancelación de ruido', 89.99, 3, 1),
	(3, 'Camiseta Polo', 'Algodón 100%, talla M', 15.50, 40, 2),
	(4, 'Arroz Diana 1kg', 'Arroz blanco de primera calidad', 2.50, 2, 3),
	(5, 'Martillo 16oz', 'Mango de fibra de vidrio', 12.00, 8, 4),
	(8, 'Jugo de Naranja', 'Jugo natural 500ml', 3.50, 20, 5),
	(9, 'Leche Entera', 'Caja 1L Natural', 1.20, 50, 1);

-- Volcando estructura para tabla inventario.usuarios
CREATE TABLE IF NOT EXISTS `usuarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `correo` varchar(150) NOT NULL,
  `password` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `correo` (`correo`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.usuarios: ~1 rows (aproximadamente)
INSERT INTO `usuarios` (`id`, `nombre`, `correo`, `password`) VALUES
	(2, 'Administrador', 'admin@demo.com', '$2a$11$OhHdSApn1LnQ.SpHeBYPweEyvw2niIjDle2JJnIasohhnkXbxAVJa');

-- Volcando estructura para tabla inventario.ventas
CREATE TABLE IF NOT EXISTS `ventas` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total` decimal(12,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Volcando datos para la tabla inventario.ventas: ~4 rows (aproximadamente)
INSERT INTO `ventas` (`id`, `fecha`, `total`) VALUES
	(1, '2026-03-16 14:22:02', 15.50),
	(2, '2026-03-16 14:48:23', 15.50),
	(3, '2026-04-07 13:01:13', 750.00),
	(4, '2026-04-07 13:36:34', 750.00);

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
