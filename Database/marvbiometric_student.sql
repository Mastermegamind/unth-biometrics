-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 11, 2023 at 04:12 PM
-- Server version: 10.4.27-MariaDB
-- PHP Version: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `fingerprints`
--

-- --------------------------------------------------------

--
-- Table structure for table `attendance`
--

CREATE TABLE `attendance` (
  `id` int(4) NOT NULL,
  `name` text NOT NULL,
  `regno` varchar(50) NOT NULL,
  `session_id` int NULL,
  `date` date NOT NULL,
  `day_name` varchar(20) NOT NULL,
  `session_name` varchar(100) NOT NULL DEFAULT '',
  `course_code` varchar(50) NOT NULL DEFAULT '',
  `course_name` text NULL,
  `timein` time NOT NULL,
  `timeout` time DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `attendance`
--

-- --------------------------------------------------------

--
-- Table structure for table `new_enrollment`
--

CREATE TABLE `new_enrollment` (
  `id` int(6) NOT NULL,
  `regno` varchar(50) NOT NULL,
  `finger_index` int NOT NULL,
  `finger_name` varchar(50) NOT NULL,
  `template` longblob NULL,
  `template_hash` varbinary(32) NOT NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `admin_logs`
--

CREATE TABLE `admin_logs` (
  `id` int(6) NOT NULL,
  `username` varchar(50) NOT NULL,
  `action_name` varchar(100) NOT NULL,
  `details` text NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `new_enrollment`
--

-- --------------------------------------------------------

--
-- Table structure for table `registration`
--

CREATE TABLE `registration` (
  `id` int(4) NOT NULL,
  `username` varchar(50) NOT NULL,
  `usertype` text NOT NULL,
  `password` varchar(255) NOT NULL,
  `name` text NOT NULL,
  `contactno` varchar(20) NOT NULL,
  `email` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `registration`
--

INSERT INTO `registration` (`id`, `username`, `usertype`, `password`, `name`, `contactno`, `email`) VALUES
(1, 'admin', 'Administrator', 'admin123', 'Administrator', '07060722008', 'admin@gmail.com'),
(2, 'user', 'Staff', 'user123', 'Test User', '1234567890', 'testuser@email.com');

-- --------------------------------------------------------

--
-- Table structure for table `sections`
--

CREATE TABLE `sections` (
  `id` int(6) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sections`
--

INSERT INTO `sections` (`id`, `name`) VALUES
(1, 'BNSC'),
(2, 'HND'),
(3, 'RN');

-- --------------------------------------------------------

--
-- Table structure for table `years`
--

CREATE TABLE `years` (
  `id` int(6) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `years`
--

INSERT INTO `years` (`id`, `name`) VALUES
(1, '2026'),
(2, '2025'),
(3, '2024'),
(4, '2023'),
(5, '2022'),
(6, '2021'),
(7, '2020');


-- --------------------------------------------------------

--
-- Table structure for table `sessions`
--

CREATE TABLE `sessions` (
  `id` int(6) NOT NULL,
  `session_date` date NOT NULL,
  `day_name` varchar(20) NOT NULL,
  `session_name` varchar(100) NOT NULL,
  `course_code` varchar(50) NOT NULL,
  `course_name` text NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sessions`
--

INSERT INTO `sessions` (`id`, `session_date`, `day_name`, `session_name`, `course_code`, `course_name`) VALUES
(1, CURDATE(), DAYNAME(CURDATE()), 'Session 1', '', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `session_settings`
--

CREATE TABLE `session_settings` (
  `id` int(4) NOT NULL,
  `active_session_id` int NULL,
  `session_date` date NOT NULL,
  `day_name` varchar(20) NOT NULL,
  `session_name` varchar(100) NOT NULL,
  `course_code` varchar(50) NOT NULL,
  `course_name` text NULL,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `session_settings`
--

INSERT INTO `session_settings` (`id`, `active_session_id`, `session_date`, `day_name`, `session_name`, `course_code`, `course_name`) VALUES
(1, 1, CURDATE(), DAYNAME(CURDATE()), 'Session 1', '', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE `students` (
  `id` int(8) NOT NULL,
  `regno` varchar(100) NOT NULL,
  `name` text NOT NULL,
  `section_id` int NULL,
  `year_id` int NULL,
  `gender` text NOT NULL,
  `passport` varchar(255) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `students`
--




--
-- Indexes for dumped tables
--

--
-- Indexes for table `attendance`
--
ALTER TABLE `attendance`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `regno_date_course_session` (`regno`, `date`, `course_code`, `session_name`),
  ADD KEY `attendance_date_index` (`date`),
  ADD KEY `attendance_regno_index` (`regno`),
  ADD KEY `attendance_course_index` (`course_code`),
  ADD KEY `attendance_session_index` (`session_id`);

--
-- Indexes for table `new_enrollment`
--
ALTER TABLE `new_enrollment`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `regno_finger_unique` (`regno`, `finger_index`),
  ADD UNIQUE KEY `template_hash_unique` (`template_hash`),
  ADD KEY `regno_index` (`regno`);

--
-- Indexes for table `admin_logs`
--
ALTER TABLE `admin_logs`
  ADD PRIMARY KEY (`id`),
  ADD KEY `admin_logs_username_index` (`username`),
  ADD KEY `admin_logs_created_index` (`created_at`);

--
-- Indexes for table `registration`
--
ALTER TABLE `registration`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username_unique` (`username`),
  ADD UNIQUE KEY `email_unique` (`email`);

--
-- Indexes for table `sections`
--
ALTER TABLE `sections`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `section_name_unique` (`name`);

--
-- Indexes for table `years`
--
ALTER TABLE `years`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `year_name_unique` (`name`);

--
-- Indexes for table `sessions`
--
ALTER TABLE `sessions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `session_date_index` (`session_date`);

--
-- Indexes for table `session_settings`
--
ALTER TABLE `session_settings`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `regno_unique` (`regno`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `attendance`
--
ALTER TABLE `attendance`
  MODIFY `id` int(4) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `new_enrollment`
--
ALTER TABLE `new_enrollment`
  MODIFY `id` int(6) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `admin_logs`
--
ALTER TABLE `admin_logs`
  MODIFY `id` int(6) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `registration`
--
ALTER TABLE `registration`
  MODIFY `id` int(4) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `sections`
--
ALTER TABLE `sections`
  MODIFY `id` int(6) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `years`
--
ALTER TABLE `years`
  MODIFY `id` int(6) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `sessions`
--
ALTER TABLE `sessions`
  MODIFY `id` int(6) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `students`
--
ALTER TABLE `students`
  MODIFY `id` int(8) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
