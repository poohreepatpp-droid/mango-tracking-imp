# HR System — Nuxt 3 Template

Web template สำหรับระบบ HR จัดการข้อมูลพนักงานและแผนก พร้อม Dashboard, REST API, และ Responsive UI

![Nuxt 3](https://img.shields.io/badge/Nuxt-3.17-00DC82?logo=nuxt.js&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?logo=typescript&logoColor=white)
![Prisma](https://img.shields.io/badge/Prisma-5-2D3748?logo=prisma&logoColor=white)

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | Nuxt 3.17 (Vue 3, SSR, Auto-imports) |
| Language | TypeScript |
| Styling | Custom CSS Design System (Plus Jakarta Sans + JetBrains Mono) |
| ORM | Prisma 5 |
| Database (dev) | SQLite (`prisma/dev.db`) |
| Database (prod) | PostgreSQL |
| Deploy | Vercel |

---

## เริ่มต้นใช้งาน

### 1. ติดตั้ง dependencies
```bash
npm install
```

### 2. ตั้งค่า environment variables
```bash
cp .env.example .env
```
สำหรับ local dev ใช้ SQLite ไม่ต้องแก้ไขอะไร

### 3. สร้าง database และ generate Prisma client
```bash
npx prisma migrate dev --name init
```

### 4. รัน development server
```bash
npm run dev
```
เปิด [http://localhost:3000](http://localhost:3000)

---

## โครงสร้างโปรเจกต์

```
nuxt3-template/
├── assets/
│   └── css/
│       └── app.css              # Design system (CSS variables, components)
├── layouts/
│   └── default.vue              # Sidebar layout + responsive hamburger menu
├── pages/
│   ├── index.vue                # Overview + API reference
│   ├── dashboard.vue            # HR Dashboard (stats + charts)
│   ├── employees/
│   │   ├── index.vue            # รายการพนักงานทั้งหมด
│   │   └── add.vue              # เพิ่มพนักงานใหม่
│   └── departments/
│       ├── index.vue            # รายการแผนกทั้งหมด
│       └── add.vue              # เพิ่มแผนกใหม่
├── server/
│   ├── api/
│   │   ├── employees/
│   │   │   ├── index.get.ts     # GET    /api/employees
│   │   │   ├── index.post.ts    # POST   /api/employees
│   │   │   ├── [id].get.ts      # GET    /api/employees/:id
│   │   │   ├── [id].put.ts      # PUT    /api/employees/:id
│   │   │   └── [id].delete.ts   # DELETE /api/employees/:id
│   │   ├── departments/
│   │   │   ├── index.get.ts     # GET    /api/departments
│   │   │   ├── index.post.ts    # POST   /api/departments
│   │   │   └── [id].delete.ts   # DELETE /api/departments/:id
│   │   └── stats.get.ts         # GET    /api/stats
│   └── utils/
│       └── prisma.ts            # Prisma Client singleton (auto-imported)
├── prisma/
│   ├── schema.prisma            # Database schema
│   └── migrations/              # Migration history
├── nuxt.config.ts
├── .env.example
└── README.md
```

---

## Data Models

```prisma
model Employee {
  id           Int        @id @default(autoincrement())
  firstName    String
  lastName     String
  email        String     @unique
  phone        String?
  position     String
  department   Department @relation(fields: [departmentId], references: [id])
  departmentId Int
  salary       Float
  startDate    DateTime
  status       String     @default("active")  // active | on-leave | resigned
  createdAt    DateTime   @default(now())
  updatedAt    DateTime   @updatedAt
}

model Department {
  id        Int        @id @default(autoincrement())
  name      String     @unique
  employees Employee[]
  createdAt DateTime   @default(now())
}
```

---

## API Endpoints

### Employees

| Method | Endpoint | Description |
|--------|---------|-------------|
| `GET` | `/api/employees` | ดึงพนักงานทั้งหมด (รวม department) |
| `POST` | `/api/employees` | เพิ่มพนักงานใหม่ |
| `GET` | `/api/employees/:id` | ดึงพนักงานตาม id |
| `PUT` | `/api/employees/:id` | แก้ไขข้อมูลพนักงาน |
| `DELETE` | `/api/employees/:id` | ลบพนักงาน |

### Departments

| Method | Endpoint | Description |
|--------|---------|-------------|
| `GET` | `/api/departments` | ดึงแผนกทั้งหมด (รวม headcount) |
| `POST` | `/api/departments` | เพิ่มแผนกใหม่ |
| `DELETE` | `/api/departments/:id` | ลบแผนก (ต้องไม่มีพนักงานอยู่) |

### Stats

| Method | Endpoint | Description |
|--------|---------|-------------|
| `GET` | `/api/stats` | สถิติ Dashboard (total, active, on-leave, avg salary, depts) |

### ตัวอย่าง POST /api/employees
```json
{
  "firstName": "สมชาย",
  "lastName": "รักดี",
  "email": "somchai@company.com",
  "phone": "081-234-5678",
  "position": "Software Engineer",
  "departmentId": 1,
  "salary": 45000,
  "startDate": "2024-01-15",
  "status": "active"
}
```

---

## Deploy บน Vercel

### 1. เปลี่ยน database เป็น PostgreSQL

แก้ไข `prisma/schema.prisma`:
```prisma
datasource db {
  provider = "postgresql"
  url      = env("DATABASE_URL")
}
```

### 2. Push code ขึ้น GitHub

### 3. เชื่อมต่อ Vercel กับ GitHub repository

### 4. เพิ่ม Environment Variables ใน Vercel dashboard

```
DATABASE_URL=postgresql://user:password@host:5432/dbname
```

### 5. Deploy อัตโนมัติเมื่อ push ไป `main`

---

## เพิ่ม Model ใหม่

1. แก้ไข `prisma/schema.prisma` เพิ่ม model
2. รัน `npx prisma migrate dev --name <name>`
3. สร้าง API routes ใน `server/api/<model>/`
4. สร้าง pages ใน `pages/<model>/`

---

## Design System

Custom CSS design system ใน `assets/css/app.css`

- **Colors**: `--accent` (rose `#e11d48`), semantic status colors
- **Typography**: Plus Jakarta Sans + JetBrains Mono
- **Components**: `.card`, `.stat-card`, `.data-table`, `.badge`, `.btn`, `.form-input`
- **Layout**: `.sidebar`, `.app-wrap`, `.app-header`, `.app-main`
- **Responsive**: 768px (tablet) + 480px (mobile) breakpoints, hamburger sidebar
