-- ============================================================
-- SQL Script para actualizar la tabla usuarios
-- Agrega la columna username y restringe para que no se repitan
-- ============================================================

-- 1. Agregar la columna username
ALTER TABLE `usuarios` 
ADD COLUMN `username` varchar(50) NOT NULL AFTER `nombre`;

-- 2. (Opcional) Asignar un username a los usuarios existentes basándose en su correo para evitar duplicados en el paso 3
UPDATE `usuarios` 
SET `username` = SUBSTRING_INDEX(`correo`, '@', 1) 
WHERE `id` > 0;

-- 3. Crear el índice único para que no se repitan los usernames
ALTER TABLE `usuarios` 
ADD UNIQUE INDEX `uk_usuarios_username` (`username`);

-- Nota: Si usas la cuenta demo, el username ahora es 'admin'
-- y la contraseña sigue siendo 'Admin123'
