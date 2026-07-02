<template>
  <div>
    <div class="app-header">
      <span class="page-title-h">Getting Started</span>
      <div class="header-actions">
        <NuxtLink to="/" class="btn btn-ghost btn-sm">← Overview</NuxtLink>
        <NuxtLink to="/dashboard" class="btn btn-accent btn-sm">Open Dashboard →</NuxtLink>
      </div>
    </div>

    <div class="app-main page-enter">

      <!-- Hero banner -->
      <div style="margin-bottom:24px; padding:28px 28px 24px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r-lg); position:relative; overflow:hidden;">
        <div style="position:absolute; top:-60px; right:-60px; width:260px; height:260px; background:radial-gradient(circle, var(--accent-glow) 0%, transparent 65%); pointer-events:none;"></div>
        <div style="font-size:10px; text-transform:uppercase; letter-spacing:.12em; color:var(--text-3); margin-bottom:10px;">Template · Nuxt 3 · Prisma · Bootstrap 5</div>
        <h1 style="font-family:var(--font-d); font-size:26px; font-weight:800; color:var(--text); letter-spacing:-1px; line-height:1.1; margin-bottom:10px;">
          เริ่มต้นใช้งาน<span style="color:var(--accent);"> Template</span>
        </h1>
        <p style="color:var(--text-2); max-width:520px; line-height:1.7; font-size:13px;">
          Template นี้พร้อมใช้งานสำหรับ Nuxt 3 พร้อม TypeScript, Prisma ORM, SQLite/PostgreSQL และ design system สีเข้ม
          เพียงทำตามขั้นตอนด้านล่างก็พร้อม develop ได้ทันที
        </p>
      </div>

      <!-- Steps -->
      <div style="margin-bottom:24px;">
        <div style="font-size:11px; text-transform:uppercase; letter-spacing:.1em; color:var(--text-3); margin-bottom:14px;">ขั้นตอนการตั้งค่า</div>
        <div style="display:flex; flex-direction:column; gap:12px;">

          <div style="display:flex; gap:14px; align-items:flex-start; padding:18px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r);">
            <div style="flex-shrink:0; width:28px; height:28px; border-radius:50%; background:var(--accent); display:flex; align-items:center; justify-content:center; font-size:12px; font-weight:700; color:#000;">1</div>
            <div>
              <div style="font-size:13px; font-weight:600; color:var(--text); margin-bottom:4px;">Clone & ติดตั้ง dependencies</div>
              <div style="font-size:12px; color:var(--text-3); margin-bottom:10px;">ดึง repository และติดตั้ง packages ทั้งหมด</div>
              <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:10px 14px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
                <div>git clone &lt;repo-url&gt;</div>
                <div>cd nuxt3-template</div>
                <div>npm install</div>
              </div>
            </div>
          </div>

          <div style="display:flex; gap:14px; align-items:flex-start; padding:18px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r);">
            <div style="flex-shrink:0; width:28px; height:28px; border-radius:50%; background:var(--accent); display:flex; align-items:center; justify-content:center; font-size:12px; font-weight:700; color:#000;">2</div>
            <div>
              <div style="font-size:13px; font-weight:600; color:var(--text); margin-bottom:4px;">ตั้งค่า Environment</div>
              <div style="font-size:12px; color:var(--text-3); margin-bottom:10px;">คัดลอกไฟล์ .env และกำหนด database connection</div>
              <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:10px 14px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
                <div>cp .env.example .env</div>
                <div style="color:var(--text-3);"># แก้ไข DATABASE_URL ใน .env</div>
              </div>
            </div>
          </div>

          <div style="display:flex; gap:14px; align-items:flex-start; padding:18px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r);">
            <div style="flex-shrink:0; width:28px; height:28px; border-radius:50%; background:var(--accent); display:flex; align-items:center; justify-content:center; font-size:12px; font-weight:700; color:#000;">3</div>
            <div>
              <div style="font-size:13px; font-weight:600; color:var(--text); margin-bottom:4px;">สร้าง Database</div>
              <div style="font-size:12px; color:var(--text-3); margin-bottom:10px;">รัน Prisma migration เพื่อสร้าง schema ใน database</div>
              <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:10px 14px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
                <div>npx prisma migrate dev</div>
                <div>npx prisma generate</div>
              </div>
            </div>
          </div>

          <div style="display:flex; gap:14px; align-items:flex-start; padding:18px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r);">
            <div style="flex-shrink:0; width:28px; height:28px; border-radius:50%; background:var(--accent); display:flex; align-items:center; justify-content:center; font-size:12px; font-weight:700; color:#000;">4</div>
            <div>
              <div style="font-size:13px; font-weight:600; color:var(--text); margin-bottom:4px;">เริ่ม Dev Server</div>
              <div style="font-size:12px; color:var(--text-3); margin-bottom:10px;">รัน Nuxt development server บนพอร์ต 3000</div>
              <div style="background:var(--surface2); border:1px solid var(--border); border-radius:var(--r); padding:10px 14px; font-family:var(--font-m); font-size:12px; color:var(--text-2); line-height:2;">
                <div>npm run dev</div>
                <div style="color:var(--text-3);"># เปิด http://localhost:3000</div>
              </div>
            </div>
          </div>

        </div>
      </div>

      <!-- Tech stack & empty page note -->
      <div class="panel-grid">
        <div class="card">
          <div class="card-head"><span class="card-title">Tech Stack</span></div>
          <div class="card-body">
            <div style="display:flex; flex-direction:column; gap:8px;">
              <div v-for="tech in techStack" :key="tech.name" style="display:flex; align-items:center; justify-content:space-between; padding:8px 0; border-bottom:1px solid var(--border);">
                <div>
                  <div style="font-size:12px; font-weight:600; color:var(--text);">{{ tech.name }}</div>
                  <div style="font-size:11px; color:var(--text-3);">{{ tech.desc }}</div>
                </div>
                <span class="badge" :class="tech.badge">{{ tech.version }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="card">
          <div class="card-head"><span class="card-title">หน้านี้คืออะไร?</span></div>
          <div class="card-body">
            <p style="font-size:13px; color:var(--text-2); line-height:1.7; margin-bottom:14px;">
              หน้านี้เป็น <strong style="color:var(--accent);">empty starter page</strong> ตัวอย่าง — ใช้เป็นจุดเริ่มต้นเมื่อต้องการเพิ่มหน้าใหม่เข้าไปใน project
            </p>
            <p style="font-size:13px; color:var(--text-2); line-height:1.7; margin-bottom:14px;">
              โครงสร้างหน้าใช้ layout เดียวกันทั้งหมด ผ่าน <code style="font-family:var(--font-m); background:var(--surface2); padding:1px 5px; border-radius:3px; font-size:11px;">layouts/default.vue</code>
            </p>
            <div style="background:var(--surface2); border:1px dashed var(--border); border-radius:var(--r); padding:14px; font-size:12px; color:var(--text-3); line-height:1.8;">
              <div style="color:var(--text-2); font-weight:600; margin-bottom:6px;">เพิ่มหน้าใหม่:</div>
              <div>1. สร้างไฟล์ใน <code style="font-family:var(--font-m);">pages/</code></div>
              <div>2. เพิ่มลิงก์ใน sidebar (<code style="font-family:var(--font-m);">layouts/default.vue</code>)</div>
              <div>3. Nuxt จัดการ routing ให้อัตโนมัติ</div>
            </div>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
const techStack = [
  { name: 'Nuxt 3', desc: 'Vue 3 framework + SSR', version: 'v3.x', badge: 'badge-green' },
  { name: 'TypeScript', desc: 'Type-safe JavaScript', version: 'v5.x', badge: 'badge-blue' },
  { name: 'Prisma ORM', desc: 'Database toolkit', version: 'v5.x', badge: 'badge-accent' },
  { name: 'Bootstrap 5', desc: 'UI component library', version: 'v5.3', badge: 'badge-amber' },
  { name: 'SQLite', desc: 'Local database (dev)', version: 'default', badge: 'badge-green' },
]
</script>
