<!--
  ┌──────────────────────────────────────────────────────────────────┐
  │  TEMPLATE: BLANK PAGE                                            │
  │  ใช้สำหรับ: จุดเริ่มต้นเปล่าสุด — โครงสร้างขั้นต่ำที่สุด     │
  │  วิธีใช้: คัดลอกไฟล์นี้ไปที่ pages/<ชื่อหน้า>.vue             │
  │           ตั้งชื่อไฟล์ = URL path เช่น pages/about.vue → /about│
  └──────────────────────────────────────────────────────────────────┘
-->
<script setup lang="ts">
// TODO: เพิ่ม composables, imports, และ logic ที่ต้องการ
// ตัวอย่างที่ใช้บ่อย:
//
//   const router = useRouter()             — navigate programmatically
//   const route  = useRoute()              — อ่าน params / query
//   const { data } = await useFetch(...)   — fetch API
//   useHead({ title: 'Page Title' })       — ตั้งค่า <title>
</script>

<template>
  <div>
    <!-- ── Header ──────────────────────────────────────────────────
         TODO: เปลี่ยน breadcrumb และ title ให้ตรงกับหน้า
    ───────────────────────────────────────────────────────────────── -->
    <div class="app-header">
      <NuxtLink to="/" class="page-crumb">Home</NuxtLink>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Page Title</span><!-- TODO -->
      <div class="header-actions">
        <!-- TODO: ใส่ปุ่ม action ที่นี่ หรือลบ div นี้ออก -->
      </div>
    </div>

    <!-- ── Content area ────────────────────────────────────────────
         app-main   = wrapper padding กลาง
         page-enter = fade-in animation
         max-width สามารถกำหนดได้ เช่น 720px สำหรับฟอร์ม, ไม่ต้องใส่สำหรับตาราง
    ──────────────────────────────────────────────────────────────── -->
    <div class="app-main page-enter">

      <!-- TODO: ใส่ content ที่นี่ -->
      <!-- ── ตัวอย่าง building blocks ที่ใช้บ่อย ──────────────────

        Card พื้นฐาน:
        ─────────────
        <div class="card">
          <div class="card-head">
            <span class="card-title">Title</span>
            <span class="badge badge-green">Status</span>
          </div>
          <div class="card-body">
            content here
          </div>
        </div>

        2-column grid:
        ──────────────
        <div class="panel-grid">
          <div class="card">Left card</div>
          <div class="card">Right card</div>
        </div>

        Alert:
        ──────
        <div class="alert alert-success">สำเร็จ!</div>
        <div class="alert alert-error">เกิดข้อผิดพลาด</div>

        Empty state:
        ────────────
        <div class="empty-state">
          <div class="empty-state-icon">📋</div>
          <div class="empty-state-title">No data yet</div>
        </div>

        Badge สี:
        ─────────
        <span class="badge badge-green">Active</span>
        <span class="badge badge-red">Inactive</span>
        <span class="badge badge-amber">Pending</span>
        <span class="badge badge-blue">Info</span>
        <span class="badge badge-muted">Default</span>

        Buttons:
        ────────
        <button class="btn btn-accent">Primary</button>
        <button class="btn btn-ghost">Secondary</button>
        <button class="btn btn-danger">Delete</button>
        <button class="btn btn-edit">Edit</button>

      ──────────────────────────────────────────────────────────── -->

    </div>
  </div>
</template>
