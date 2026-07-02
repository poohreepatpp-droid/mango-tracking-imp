<template>
  <div>
    <div class="app-header">
      <span class="page-title-h">Overview</span>
      <div class="header-actions">
        <NuxtLink to="/dashboard" class="btn btn-ghost btn-sm">Dashboard →</NuxtLink>
        <NuxtLink to="/employees/add" class="btn btn-accent btn-sm">+ Add Employee</NuxtLink>
      </div>
    </div>

    <div class="app-main page-enter">
      <!-- Hero -->
      <div style="margin-bottom:28px; padding:28px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r-lg); position:relative; overflow:hidden;">
        <div style="position:absolute; top:-40px; right:-40px; width:200px; height:200px; background:radial-gradient(circle, var(--accent-glow) 0%, transparent 70%); pointer-events:none;"></div>
        <div style="font-size:10px; text-transform:uppercase; letter-spacing:.12em; color:var(--text-3); margin-bottom:10px;">Nuxt 3 · TypeScript · Prisma · SQLite / PostgreSQL</div>
        <h1 style="font-family:var(--font-d); font-size:28px; font-weight:800; color:var(--text); letter-spacing:-1px; line-height:1.1; margin-bottom:10px;">
          HR<span style="color:var(--accent);">System</span>
        </h1>
        <p style="color:var(--text-2); max-width:420px; line-height:1.6; font-size:13px;">
          จัดการข้อมูลพนักงานและแผนก ด้วย full CRUD API, Prisma ORM และ dark design system พร้อม deploy บน Vercel
        </p>
        <div style="display:flex; gap:10px; margin-top:18px; flex-wrap:wrap;">
          <NuxtLink to="/dashboard" class="btn btn-accent">Open Dashboard</NuxtLink>
          <NuxtLink to="/employees" class="btn btn-ghost">Browse Employees</NuxtLink>
        </div>
      </div>

      <!-- API reference -->
      <div class="card">
        <div class="card-head">
          <span class="card-title">REST API Reference</span>
          <span class="badge badge-green">● live</span>
        </div>
        <div style="overflow-x:auto;">
          <table class="data-table">
            <thead>
              <tr>
                <th style="width:80px;">Method</th>
                <th>Endpoint</th>
                <th>Description</th>
                <th style="width:90px;">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td><span class="badge badge-green">GET</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/employees</td>
                <td>List all employees (with department)</td>
                <td><span class="badge badge-green">200 OK</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-blue">POST</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/employees</td>
                <td>Create new employee</td>
                <td><span class="badge badge-blue">201</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-amber">PUT</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/employees/:id</td>
                <td>Update employee</td>
                <td><span class="badge badge-amber">200 OK</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-red">DELETE</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/employees/:id</td>
                <td>Delete employee</td>
                <td><span class="badge badge-red">200 OK</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-green">GET</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/departments</td>
                <td>List all departments (with headcount)</td>
                <td><span class="badge badge-green">200 OK</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-blue">POST</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/departments</td>
                <td>Create new department</td>
                <td><span class="badge badge-blue">201</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-red">DELETE</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/departments/:id</td>
                <td>Delete department</td>
                <td><span class="badge badge-red">200 OK</span></td>
              </tr>
              <tr>
                <td><span class="badge badge-green">GET</span></td>
                <td style="font-family:var(--font-m); color:var(--text-2);">/api/stats</td>
                <td>HR dashboard statistics</td>
                <td><span class="badge badge-green">200 OK</span></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Quick start -->
      <div class="panel-grid" style="margin-top:16px;">
        <div class="card">
          <div class="card-head"><span class="card-title">Quick Start</span></div>
          <div class="card-body">
            <div style="font-size:12px; color:var(--text-3); margin-bottom:10px; text-transform:uppercase; letter-spacing:.06em;">Setup</div>
            <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:12px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
              <div><span style="color:var(--text-3);"># 1.</span> cp .env.example .env</div>
              <div><span style="color:var(--text-3);"># 2.</span> npx prisma migrate dev</div>
              <div><span style="color:var(--text-3);"># 3.</span> npm run dev</div>
            </div>
          </div>
        </div>
        <div class="card">
          <div class="card-head"><span class="card-title">Data Models</span></div>
          <div class="card-body">
            <div style="font-size:12px; color:var(--text-3); margin-bottom:10px; text-transform:uppercase; letter-spacing:.06em;">Schema</div>
            <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:12px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
              <div><span style="color:var(--accent);">Employee</span> → firstName, lastName, email</div>
              <div style="padding-left:14px; color:var(--text-3);">position, salary, status</div>
              <div><span style="color:var(--accent);">Department</span> → name, employees[]</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
