# دليل تشغيل نظام KOSS
## خطوات الإعداد من الصفر

---

### المتطلبات الأساسية
| الأداة | الإصدار |
| :--- | :--- |
| Visual Studio 2022 | Community أو أعلى |
| .NET Framework | 4.8 |
| SQL Server | 2019/2022 أو Express |
| SQL Server Management Studio (SSMS) | أي إصدار |

---

### الخطوة 1: إنشاء قاعدة البيانات

1. افتح **SQL Server Management Studio (SSMS)**
2. اتصل بخادم SQL Server
3. افتح الملف: `c:\BenGsomaSystem\KOSS\KOSS_DB_Setup.sql`
4. اضغط **Execute (F5)** — ستُنشأ قاعدة `KOSS_DB`

---

### الخطوة 2: تحديث سلسلة الاتصال

افتح: `KOSS.Web\Web.config`

ابحث عن:
```xml
<add name="KossDbContext"
     connectionString="Data Source=.;Initial Catalog=KOSS_DB;..."
```

غيّر `Data Source=.` باسم خادم SQL Server لديك.
مثال: `Data Source=DESKTOP-ABC\SQLEXPRESS`

---

### الخطوة 3: فتح المشروع في Visual Studio

1. افتح Visual Studio 2022
2. `File → Open → Project/Solution`
3. اختر: `c:\BenGsomaSystem\KOSS\KOSS.sln`
4. في قائمة `Tools → NuGet Package Manager → Package Manager Console` شغّل:
   ```
   Update-Package -reinstall
   ```
5. أو `Build → Rebuild Solution` لتنزيل الحزم تلقائياً

---

### الخطوة 4: تشغيل الـ Migrations

في Package Manager Console:
```
Enable-Migrations
Add-Migration InitialCreate
Update-Database
```

هذا سينشئ جميع الجداول تلقائياً في `KOSS_DB`.

---

### الخطوة 5: تشغيل التطبيق

1. اضغط **F5** أو **Ctrl+F5** في Visual Studio
2. سيفتح المتصفح على `http://localhost:XXXX`
3. صفحة تسجيل الدخول ستظهر

---

### بيانات الدخول الافتراضية

| الحقل | القيمة |
| :--- | :--- |
| **البريد الإلكتروني** | `admin@koss.ly` |
| **كلمة المرور** | `Admin@123` |
| **الدور** | مدير تنفيذي (Executive) |

> ⚠️ **قم بتغيير كلمة المرور فوراً بعد أول دخول!**

---

### نشر النظام على خادم LAN

1. في Visual Studio: `Build → Publish → Folder`
2. انشر الملفات على IIS في الخادم
3. في **IIS Manager**:
   - أنشئ موقع جديد يشير لمجلد النشر
   - تأكد من تثبيت **ASP.NET 4.8** على IIS
4. افتح المتصفح على أجهزة الشبكة: `http://IP_ADDRESS_OF_SERVER/`

---

### هيكل الأدوار والصلاحيات

| الدور | الوصول |
| :--- | :--- |
| `Executive` | كامل النظام |
| `Finance` | المالية + الرواتب + كشف الحسابات |
| `SalesStaff` | تسجيل العملاء + العقود |
| `Designer` | وحدة التصميم + BOM |
| `FieldSurveyor` | إدخال القياسات |
| `FactoryManager` | لوحة المصنع + BOM |

---

### للمساعدة: اتصل بـ Malik (مطور النظام) 📞
