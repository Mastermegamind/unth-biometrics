-- Migration v5: add sessions list + active session pointer and reset default date.

CREATE TABLE IF NOT EXISTS `sessions` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `session_date` date NOT NULL,
  `day_name` varchar(20) NOT NULL,
  `session_name` varchar(100) NOT NULL,
  `course_code` varchar(50) NOT NULL,
  `course_name` text NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `session_date_index` (`session_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

ALTER TABLE `session_settings`
  ADD COLUMN `active_session_id` int NULL;

INSERT INTO `session_settings` (`session_date`, `day_name`, `session_name`, `course_code`, `course_name`)
SELECT CURDATE(), DAYNAME(CURDATE()), 'Session 1', '', NULL
WHERE NOT EXISTS (SELECT 1 FROM `session_settings`);

UPDATE `session_settings`
SET `session_date` = CURDATE(),
    `day_name` = DAYNAME(CURDATE())
WHERE `session_date` = '2023-11-05';

INSERT INTO `sessions` (`session_date`, `day_name`, `session_name`, `course_code`, `course_name`)
SELECT `session_date`, `day_name`, `session_name`, `course_code`, `course_name`
FROM `session_settings`
WHERE NOT EXISTS (SELECT 1 FROM `sessions`)
ORDER BY `id`
LIMIT 1;

UPDATE `session_settings`
SET `active_session_id` = (SELECT `id` FROM `sessions` ORDER BY `session_date` DESC, `id` DESC LIMIT 1)
WHERE `active_session_id` IS NULL OR `active_session_id` = 0;
