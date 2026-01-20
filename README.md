# Youtube Learning Assistant - Data Management Service (C# Backend)

## Tổng quan học thuật (Academic Overview)

Dự án này đóng vai trò là **Persistence Layer** (Lớp lưu trữ dữ liệu) và **Business Logic Provider** cho toàn bộ hệ thống. Repo sử dụng framework ASP.NET Core để xây dựng một hệ thống Web API mạnh mẽ, chịu trách nhiệm quản lý vòng đời dữ liệu (Data Lifecycle) từ lúc tiếp nhận kết quả phân tích AI đến khi phục vụ người dùng trên Dashboard.

Trọng tâm của repo này là triển khai mô hình **Repository Pattern** và **Entity Framework Core** để đảm bảo tính toàn vẹn dữ liệu (Data Integrity) và tối ưu hóa các truy vấn vào cơ sở dữ liệu quan hệ (RDBMS).

---

## Kiến trúc kết nối (System Integration)

C# Backend hoạt động như một trung tâm quản lý dữ liệu tĩnh trong mô hình Microservices:

1. **Giao tiếp với Chrome Extension (Frontend):**
* Cung cấp Endpoint `POST /api/video/save` để tiếp nhận dữ liệu đã được AI xử lý từ Extension.
* Cung cấp Endpoint `GET /api/video/history` để trả về danh sách lịch sử học tập cho Dashboard.


2. **Tương tác với Cơ sở dữ liệu (MySQL):**
* Chịu trách nhiệm thực hiện các thao tác CRUD (Create, Read, Update, Delete) trên bảng `videos`.
* Đảm bảo kiểu dữ liệu `longtext` được xử lý đúng cách để lưu trữ các bản tóm tắt Markdown dài từ AI.



---

## Công nghệ sử dụng (Tech Stack)

* **Framework:** ASP.NET Core 9.0 Web API.
* **ORM:** Entity Framework Core (EF Core).
* **Database:** MySQL / MariaDB.
* **Documentation:** OpenAPI (Swagger).
* **Design Pattern:** Singleton, Dependency Injection (DI).

---

## Hướng dẫn cài đặt chi tiết (Setup Guide)

### 1. Chuẩn bị môi trường

* Cài đặt **.NET 9.0 SDK**.
* Cài đặt **MySQL Server**.

### 2. Cấu hình Cơ sở dữ liệu

1. Tạo một Database mới trong MySQL (Ví dụ: `yt_learning_db`).
2. Mở file `appsettings.json` trong thư mục gốc dự án.
3. Cập nhật dòng `DefaultConnection` với thông tin MySQL của bạn:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;database=yt_learning_db;user=root;password=your_password"
}

```



### 3. Khởi tạo Database (Migrations)

Mở terminal (hoặc Package Manager Console) và chạy lệnh:

```bash
dotnet ef database update

```

### 4. Khởi chạy Server

Chạy lệnh sau để server bắt đầu hoạt động:

```bash
dotnet run

```

* Server sẽ chạy mặc định tại: `http://localhost:5104`.
* Dashboard cá nhân: `http://localhost:5104/dashboard.html`.

---

## Các Endpoint quan trọng

* `POST /api/video/save`: Tiếp nhận JSON (VideoId, Title, Summary) và lưu vào MySQL.
* `GET /api/video/history`: Trả về toàn bộ lịch sử video đã phân tích.
* `DELETE /api/video/clear-all`: Xóa sạch kho dữ liệu kiến thức.

---

### Tại sao C# là lựa chọn tối ưu cho phần này?

Trong kiến trúc 3 phần của bạn, C# được chọn để quản lý dữ liệu vì:

* **Tính bảo mật:** ASP.NET Core có cơ chế Middleware mạnh mẽ để quản lý CORS và xác thực.
* **Hiệu năng lưu trữ:** Entity Framework giúp quản lý các mối quan hệ dữ liệu phức tạp hiệu quả hơn Python trong môi trường Production.
* **Tính ổn định:** Hệ thống có thể chạy 24/7 với độ tin cậy cực cao.
