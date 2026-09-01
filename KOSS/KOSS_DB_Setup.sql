-- ============================================================
--  KOSS ERP - SQL Server Database Setup Script
--  قاعدة بيانات نظام KOSS - شركة بن سوما للمطابخ
--  تشغيل هذا الملف في SQL Server Management Studio
-- ============================================================

-- 1. إنشاء قاعدة البيانات
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KOSS_DB')
BEGIN
    CREATE DATABASE KOSS_DB
    COLLATE Arabic_CI_AS;
    PRINT 'تم إنشاء قاعدة البيانات KOSS_DB بنجاح.';
END
GO

USE KOSS_DB;
GO

-- ============================================================
--  ملاحظة: تشغيل Entity Framework Code First Migrations
--  سيُنشئ الجداول تلقائياً عند أول تشغيل للتطبيق.
--  هذا الملف للإعداد الأولي والبيانات الأساسية فقط.
-- ============================================================

PRINT 'قاعدة البيانات KOSS_DB جاهزة.';
PRINT 'تأكد من تحديث Web.config بـ: Data Source=YOUR_SERVER_NAME;Initial Catalog=KOSS_DB';
GO
