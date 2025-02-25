# NZWalks API - Discover New Zealand's Stunning Walking Tracks

## 🌍 Overview
NZWalks API is a **secure and scalable RESTful API** designed to provide details about **New Zealand's walking tracks**. It allows users to explore different walks based on **region, difficulty level, and length**, making it a valuable resource for hikers, travelers, and adventure seekers.

---

## 🚀 Features

- ✅ **Full CRUD Operations** – Manage walks & Regions with create, read, update, and delete functionality.
- ✅ **Role-Based Access Control (RBAC)**:
  - **Reader Role** – Can view walk & Region details.
  - **Writer Role** – Can create, update, and delete walks & Regions.
- ✅ **JWT Authentication & Authorization** – Secure access to API endpoints.
- ✅ **AutoMapper** – Efficient DTO-to-model mapping.
- ✅ **Model Validation (`ValidateModel`)** – Ensures valid data submission.
- ✅ **Image Uploading** – Supports walk image uploads.
- ✅ **Filtering & Searching & Pagination** – Retrieve walks based on **name, and length**.

---

## 🏗️ Tech Stack

- **ASP.NET Core Web API** – High-performance and scalable backend.
- **Entity Framework Core** – Used with **SQL Server** for data storage.
- **JWT Token-Based Authentication** – Secure user authentication and role management.
- **AutoMapper** – Simplifies object mapping between models and DTOs.
- **File Uploads** – Enables storing and retrieving images for walks.
- **Fluent Validation** – Ensures data integrity and structured validation.

---


## 🔑 Authentication & Roles

1. **Register a New User:**
   - Send a `POST` request to `/api/auth/register` with username and password.
2. **Login & Get JWT Token:**
   - Authenticate via `/api/auth/login` to receive a **JWT token**.
3. **Assign Roles:**
   - Admins can assign either `Reader` or `Writer` roles to users.

---

## 📷 Image Uploading

- Upload walk images via the `/api/walks/upload` endpoint.
- Images are stored securely on the server.

---


## 🛠️ Author
Developed by **Eslam Ayman** 👨‍💻

---

