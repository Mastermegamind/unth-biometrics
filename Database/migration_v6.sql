-- Migration v6: attendance session_id, indexes, and admin action logging.

ALTER TABLE `attendance`
  ADD COLUMN `session_id` int NULL AFTER `regno`,
  ADD KEY `attendance_date_index` (`date`),
  ADD KEY `attendance_regno_index` (`regno`),
  ADD KEY `attendance_course_index` (`course_code`),
  ADD KEY `attendance_session_index` (`session_id`);

CREATE TABLE IF NOT EXISTS `admin_logs` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `action_name` varchar(100) NOT NULL,
  `details` text NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `admin_logs_username_index` (`username`),
  KEY `admin_logs_created_index` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
