-- ============================================================
-- erp_fix_permisos.sql
-- Inserta los permisos ERP que faltan en la tabla permisos
-- y los asigna al rol Administrador.
-- ============================================================

USE `inventario`;

-- Insertar permisos ERP (si no existen ya)
INSERT IGNORE INTO `permisos` (`nombre`, `descripcion`) VALUES
('auditoria.ver',       'Ver registros de auditoría del sistema'),
('pagos.registrar',     'Registrar pagos y abonos'),
('pagos.ver',           'Ver cuentas por cobrar y pagar'),
('kardex.ver',          'Ver movimientos de inventario (Kárdex)'),
('kardex.ajustar',      'Realizar ajustes manuales de stock'),
('impuestos.gestionar', 'Configurar impuestos del sistema');

-- Asignar todos los permisos nuevos al Administrador (rol_id = 1)
INSERT IGNORE INTO `rol_permisos` (`rol_id`, `permiso_id`)
SELECT 1, `id` FROM `permisos`
WHERE `nombre` IN (
  'auditoria.ver',
  'pagos.registrar',
  'pagos.ver',
  'kardex.ver',
  'kardex.ajustar',
  'impuestos.gestionar'
);
