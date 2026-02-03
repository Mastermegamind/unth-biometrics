CREATE TABLE IF NOT EXISTS `student_fields` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `field_key` varchar(50) NOT NULL,
  `field_label` varchar(100) NOT NULL,
  `field_type` varchar(20) NOT NULL DEFAULT 'text',
  `options` text NULL,
  `is_required` tinyint(1) NOT NULL DEFAULT 0,
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `display_order` int NOT NULL DEFAULT 0,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `field_key_unique` (`field_key`),
  UNIQUE KEY `field_label_unique` (`field_label`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS `student_field_values` (
  `id` int(8) NOT NULL AUTO_INCREMENT,
  `regno` varchar(100) NOT NULL,
  `field_id` int NOT NULL,
  `field_value` text NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `regno_field_unique` (`regno`, `field_id`),
  KEY `field_id_index` (`field_id`),
  KEY `regno_index` (`regno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT IGNORE INTO `student_fields` (`field_key`, `field_label`, `field_type`, `options`, `is_required`, `is_active`, `display_order`) VALUES
('class', 'Class', 'text', NULL, 0, 1, 10),
('department', 'Department', 'text', NULL, 0, 1, 20),
('faculty', 'Faculty', 'text', NULL, 0, 1, 30),
('blood_group', 'Blood Group', 'dropdown', 'A,B,AB,O', 0, 1, 40),
('blood_genotype', 'Blood Genotype', 'dropdown', 'AA,AS,SS,AC', 0, 1, 50),
('phone', 'Phone', 'text', NULL, 0, 1, 60),
('email', 'Email', 'text', NULL, 0, 1, 70),
('address', 'Address', 'multiline', NULL, 0, 1, 80),
('date_of_birth', 'Date of Birth', 'date', NULL, 0, 1, 90);
