-- Migration v4: normalize fingerprint enrollment storage for faster lookup.
-- This migrates from the old new_enrollment table (fingerdata1..10) to a row-per-finger model.
-- Run ONLY if your current new_enrollment table still has fingerdata columns.

CREATE TABLE `new_enrollment_new` (
  `id` int(6) NOT NULL AUTO_INCREMENT,
  `regno` varchar(50) NOT NULL,
  `finger_index` int NOT NULL,
  `finger_name` varchar(50) NOT NULL,
  `template` longblob NULL,
  `template_hash` varbinary(32) NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `regno_finger_unique` (`regno`, `finger_index`),
  UNIQUE KEY `template_hash_unique` (`template_hash`),
  KEY `regno_index` (`regno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `new_enrollment_new` (`regno`, `finger_index`, `finger_name`, `template`, `template_hash`)
SELECT `regno`, 1, 'Right Thumb', `fingerdata1`, UNHEX(SHA2(`fingerdata1`, 256)) FROM `new_enrollment` WHERE `fingerdata1` IS NOT NULL
UNION ALL
SELECT `regno`, 2, 'Right Index', `fingerdata2`, UNHEX(SHA2(`fingerdata2`, 256)) FROM `new_enrollment` WHERE `fingerdata2` IS NOT NULL
UNION ALL
SELECT `regno`, 3, 'Right Middle', `fingerdata3`, UNHEX(SHA2(`fingerdata3`, 256)) FROM `new_enrollment` WHERE `fingerdata3` IS NOT NULL
UNION ALL
SELECT `regno`, 4, 'Right Ring', `fingerdata4`, UNHEX(SHA2(`fingerdata4`, 256)) FROM `new_enrollment` WHERE `fingerdata4` IS NOT NULL
UNION ALL
SELECT `regno`, 5, 'Right Little', `fingerdata5`, UNHEX(SHA2(`fingerdata5`, 256)) FROM `new_enrollment` WHERE `fingerdata5` IS NOT NULL
UNION ALL
SELECT `regno`, 6, 'Left Thumb', `fingerdata6`, UNHEX(SHA2(`fingerdata6`, 256)) FROM `new_enrollment` WHERE `fingerdata6` IS NOT NULL
UNION ALL
SELECT `regno`, 7, 'Left Index', `fingerdata7`, UNHEX(SHA2(`fingerdata7`, 256)) FROM `new_enrollment` WHERE `fingerdata7` IS NOT NULL
UNION ALL
SELECT `regno`, 8, 'Left Middle', `fingerdata8`, UNHEX(SHA2(`fingerdata8`, 256)) FROM `new_enrollment` WHERE `fingerdata8` IS NOT NULL
UNION ALL
SELECT `regno`, 9, 'Left Ring', `fingerdata9`, UNHEX(SHA2(`fingerdata9`, 256)) FROM `new_enrollment` WHERE `fingerdata9` IS NOT NULL
UNION ALL
SELECT `regno`, 10, 'Left Little', `fingerdata10`, UNHEX(SHA2(`fingerdata10`, 256)) FROM `new_enrollment` WHERE `fingerdata10` IS NOT NULL;

RENAME TABLE `new_enrollment` TO `new_enrollment_old`, `new_enrollment_new` TO `new_enrollment`;

-- Optional: drop old table after verifying data
-- DROP TABLE `new_enrollment_old`;
