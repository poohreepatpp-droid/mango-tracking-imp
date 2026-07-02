<!--
  ┌─────────────────────────────────────────────┐
  │  EMPTY PAGE INDEX                           │
  │  หน้ารวม template ทั้งหมดใน /empty          │
  │  ใช้เป็น catalog เลือก template ที่ต้องการ  │
  └─────────────────────────────────────────────┘
-->
<template>
  <div>
    <!-- ── Header bar ─────────────────────────────────────────────
         app-header   = แถบบนสุด (sticky)
         page-crumb   = breadcrumb ก่อนหน้า (คลิกได้)
         crumb-sep    = ตัวคั่น " / "
         page-title-h = ชื่อหน้าปัจจุบัน
         header-actions = ปุ่มด้านขวา
    ──────────────────────────────────────────────────────────────── -->
    <div class="app-header">
      <span class="page-title-h">Empty Templates</span>
      <div class="header-actions">
        <NuxtLink to="/" class="btn btn-ghost btn-sm">← Home</NuxtLink>
      </div>
    </div>

    <!-- ── Main content ───────────────────────────────────────────
         app-main   = wrapper padding กลาง
         page-enter = animation fade-in ตอนโหลดหน้า
    ──────────────────────────────────────────────────────────────── -->
    <div class="app-main page-enter">

      <!-- Info banner -->
      <div style="margin-bottom:24px; padding:20px 24px; background:var(--surface); border:1px solid var(--border); border-radius:var(--r-lg);">
        <div style="font-size:11px; text-transform:uppercase; letter-spacing:.1em; color:var(--text-3); margin-bottom:6px;">Starter Templates</div>
        <h2 style="font-family:var(--font-d); font-size:20px; font-weight:800; color:var(--text); margin-bottom:8px; letter-spacing:-.5px;">
          Empty Page Templates
        </h2>
        <p style="font-size:13px; color:var(--text-2); line-height:1.7; max-width:560px;">
          เลือก template ที่ตรงกับประเภทหน้าที่ต้องการสร้าง แล้วคัดลอกไปวางใน
          <code style="font-family:var(--font-m); background:var(--surface2); padding:1px 5px; border-radius:3px; font-size:11px;">pages/</code>
          ทำตาม comment ที่มีอยู่ในไฟล์เพื่อปรับแต่งให้เหมาะกับงาน
        </p>
      </div>

      <!-- Template cards -->
      <div class="panel-grid">
        <NuxtLink
          v-for="t in templates"
          :key="t.to"
          :to="t.to"
          style="text-decoration:none;"
        >
          <div class="card" style="cursor:pointer; transition:border-color .15s;" @mouseenter="e => e.currentTarget.style.borderColor='var(--accent)'" @mouseleave="e => e.currentTarget.style.borderColor='var(--border)'">
            <div class="card-head">
              <span class="card-title">{{ t.title }}</span>
              <span :class="`badge ${t.badge}`">{{ t.type }}</span>
            </div>
            <div class="card-body">
              <p style="font-size:12px; color:var(--text-2); line-height:1.7; margin-bottom:12px;">{{ t.desc }}</p>
              <div style="font-size:11px; color:var(--text-3);">
                <span v-for="tag in t.tags" :key="tag"
                  style="display:inline-block; margin-right:6px; padding:2px 7px; background:var(--surface2); border-radius:20px; border:1px solid var(--border);">
                  {{ tag }}
                </span>
              </div>
            </div>
            <div style="padding:10px 14px; border-top:1px solid var(--border); font-size:11px; color:var(--accent);">
              ดู template →
            </div>
          </div>
        </NuxtLink>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
// ── Meta ───────────────────────────────────────────────────────────
// useHead ใช้ตั้งค่า <title> และ <meta> ของหน้า
useHead({ title: 'Empty Templates · HR System' })

// ── Data ───────────────────────────────────────────────────────────
// ข้อมูล static ไม่ต้อง useFetch เพราะไม่มี API
const templates = [
  {
    to: '/empty/list-page',
    title: 'List Page',
    type: 'Table',
    badge: 'badge-green',
    desc: 'หน้าแสดงรายการข้อมูลแบบตาราง พร้อม fetch API, delete, empty-state และ alert',
    tags: ['useFetch', 'table', 'delete', 'alert'],
  },
  {
    to: '/empty/form-page',
    title: 'Form Page',
    type: 'Form',
    badge: 'badge-blue',
    desc: 'หน้าฟอร์มกรอกข้อมูล พร้อม validation, loading state และ submit ไป API',
    tags: ['form', 'POST', 'validation', 'router'],
  },
  {
    to: '/empty/detail-page',
    title: 'Detail Page',
    type: 'Detail',
    badge: 'badge-amber',
    desc: 'หน้าแสดงรายละเอียด record เดียว พร้อม dynamic route และ back navigation',
    tags: ['useRoute', 'GET :id', 'breadcrumb'],
  },
  {
    to: '/empty/blank-page',
    title: 'Blank Page',
    type: 'Blank',
    badge: 'badge-muted',
    desc: 'หน้าเปล่าสุด โครงสร้างขั้นต่ำที่สุด — เหมาะสำหรับเริ่มต้นหน้าใหม่ตั้งแต่ศูนย์',
    tags: ['minimal', 'starter'],
  },
]
</script>
