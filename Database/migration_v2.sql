-- Migration for schema hardening and type fixes.
-- Suggested least-privilege user:
-- CREATE USER 'app_user'@'localhost' IDENTIFIED BY 'change_me';
-- GRANT SELECT, INSERT, UPDATE, DELETE ON fingerprints.* TO 'app_user'@'localhost';

UPDATE `attendance`
SET `date` = STR_TO_DATE(`date`, '%d/%m/%Y')
WHERE `date` LIKE '%/%/%';

UPDATE `attendance`
SET `timein` = STR_TO_DATE(`timein`, '%H:%i')
WHERE `timein` LIKE '%:%';

UPDATE `attendance`
SET `timeout` = STR_TO_DATE(`timeout`, '%H:%i')
WHERE `timeout` LIKE '%:%';

ALTER TABLE `attendance`
  MODIFY `date` DATE NOT NULL,
  MODIFY `day` VARCHAR(20) NOT NULL,
  MODIFY `timein` TIME NOT NULL,
  MODIFY `timeout` TIME NULL;

ALTER TABLE `registration`
  MODIFY `password` VARCHAR(255) NOT NULL,
  MODIFY `email` VARCHAR(255) NOT NULL;

ALTER TABLE `students`
  ADD UNIQUE KEY `matricno_unique` (`matricno`);

ALTER TABLE `registration`
  ADD UNIQUE KEY `username_unique` (`username`),
  ADD UNIQUE KEY `email_unique` (`email`);

ALTER TABLE `attendance`
  ADD UNIQUE KEY `matricno_date` (`matricno`, `date`);

-- Ensure IDs auto-increment
ALTER TABLE `attendance`
  MODIFY `id` INT(4) NOT NULL AUTO_INCREMENT;

ALTER TABLE `new_enrollment`
  MODIFY `id` INT(6) NOT NULL AUTO_INCREMENT;

ALTER TABLE `registration`
  MODIFY `id` INT(4) NOT NULL AUTO_INCREMENT;

ALTER TABLE `students`
  MODIFY `id` INT(8) NOT NULL AUTO_INCREMENT;
