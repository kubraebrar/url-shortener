# URL Shortener API

A high-performance and scalable URL shortening service built with **.NET**.  
This API transforms long URLs into short, unique links while tracking detailed analytics such as clicks, IP addresses, and user agents.

---

##  Overview

This project is designed to demonstrate **backend development skills**, including:
- RESTful API design
- Database operations with Entity Framework Core
- Clean architecture principles (CQRS-like approach)
- Performance optimization techniques

---

## Features

✅ **URL Shortening**  
Generate clean and unique short URLs from long links.

✅ **Fast Redirection**  
Efficient HTTP 302 redirection using optimized read queries (`AsNoTracking`).

✅ **Analytics Tracking**  
Track every click with:
- Timestamp  
- IP Address  
- User-Agent  

✅ **Collision-Free Code Generation**  
Ensures every generated short URL is unique.

✅ **Swagger Integration**  
Interactive API documentation for easy testing.

---

## 🛠️ Tech Stack

| Technology | Description |
|----------|------------|
| .NET | Backend framework |
| C# | Programming language |
| Entity Framework Core | ORM |
| In-Memory Database | Development database |
| Swagger / OpenAPI | API documentation |


---
##  Project Structure
📁 URLShortener
┣ 📁 Controllers
┣ 📁 Services
┣ 📁 Models
┣ 📁 Data
┗ 📄 Program.cs


