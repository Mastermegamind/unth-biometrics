-- Migration for regno/section/year, attendance session metadata, and lookup tables.

-- Attendance updates
ALTER TABLE `attendance`
  CHANGE `matricno` `regno` varchar(50) NOT NULL,
  CHANGE `day` `day_name` varchar(20) NOT NULL;

ALTER TABLE `attendance`
  ADD COLUMN `session_name` varchar(100) NOT NULL DEFAULT '' AFTER `day_name`,
  ADD COLUMN `course_code` varchar(50) NOT NULL DEFAULT '' AFTER `session_name`,
  ADD COLUMN `course_name` text NULL AFTER `course_code`;

-- Drop old unique key if it exists (run manually if needed):
-- DROP INDEX matricno_date ON attendance;
ALTER TABLE `attendance`
  ADD UNIQUE KEY `regno_date_course_session` (`regno`, `date`, `course_code`, `session_name`);

-- Enrollment updates moved to migration_v4.sql (normalized enrollment storage)

-- Student updates
ALTER TABLE `students`
  CHANGE `matricno` `regno` varchar(100) NOT NULL,
  DROP COLUMN `faculty`,
  DROP COLUMN `department`,
  DROP COLUMN `bloodgroup`,
  DROP COLUMN `gradyear`,
  ADD COLUMN `section_id` int NULL,
  ADD COLUMN `year_id` int NULL,
  MODIFY `passport` varchar(255) NULL;

-- Lookup tables
CREATE TABLE IF NOT EXISTS `sections` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `section_name_unique` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS `years` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `year_name_unique` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `sections` (`name`) VALUES ('Science'), ('Arts'), ('Commercial')
ON DUPLICATE KEY UPDATE `name` = VALUES(`name`);

INSERT INTO `years` (`name`) VALUES ('Year 1'), ('Year 2'), ('Year 3'), ('Year 4')
ON DUPLICATE KEY UPDATE `name` = VALUES(`name`);

-- Session settings (single active row)
CREATE TABLE IF NOT EXISTS `session_settings` (
  `id` int(4) NOT NULL,
  `session_date` date NOT NULL,
  `day_name` varchar(20) NOT NULL,
  `session_name` varchar(100) NOT NULL,
  `course_code` varchar(50) NOT NULL,
  `course_name` text NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `session_settings` (`id`, `session_date`, `day_name`, `session_name`, `course_code`, `course_name`)
VALUES (1, CURDATE(), DATE_FORMAT(CURDATE(), '%W'), '', '', NULL)
ON DUPLICATE KEY UPDATE `session_date` = VALUES(`session_date`);

-- Ensure unique regno
ALTER TABLE `students`
  ADD UNIQUE KEY `regno_unique` (`regno`);
