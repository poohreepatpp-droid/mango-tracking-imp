<!--
  ┌──────────────────────────────────────────────────────────────────┐
  │  TEMPLATE: LIST PAGE                                             │
  │  ใช้สำหรับ: หน้าแสดงรายการข้อมูลแบบตาราง                       │
  │  วิธีใช้: คัดลอกไฟล์นี้ไปที่ pages/<ชื่อหน้า>/index.vue        │
  │           แล้วแทนที่ TODO ทุกจุด                                │
  └──────────────────────────────────────────────────────────────────┘
-->
<script setup lang="ts">
// ── 1. กำหนด Type ของข้อมูล ────────────────────────────────────────
// TODO: แทนที่ด้วย fields จริงจาก Prisma schema
interface Item {
  id: number
  name: string       // TODO: เพิ่ม/เปลี่ยน fields
  createdAt: string
}

// ── 2. Fetch ข้อมูลจาก API ─────────────────────────────────────────
// useFetch = built-in Nuxt, SSR-safe, auto-refresh ได้
// TODO: เปลี่ยน '/api/items' เป็น endpoint จริง
const { data: items, refresh } = await useFetch<Item[]>('/api/items')

// ── 3. Alert state ─────────────────────────────────────────────────
// ใช้แสดง success/error message หลัง action
const alertMsg  = ref('')
const alertType = ref<'success' | 'error'>('success')

const showAlert = (msg: string, type: 'success' | 'error') => {
  alertMsg.value  = msg
  alertType.value = type
  setTimeout(() => { alertMsg.value = '' }, 3000) // ซ่อนหลัง 3 วิ
}

// ── 4. Delete handler ──────────────────────────────────────────────
// TODO: เปลี่ยน endpoint และ confirm message ให้ตรงกับ entity
const handleDelete = async (id: number) => {
  if (!confirm('ต้องการลบรายการนี้?')) return
  const { error } = await useFetch(`/api/items/${id}`, { method: 'DELETE' })
  if (!error.value) {
    showAlert('ลบข้อมูลสำเร็จ', 'success')
    await refresh() // โหลดข้อมูลใหม่หลังลบ
  } else {
    showAlert('ลบไม่สำเร็จ', 'error')
  }
}
</script>

<template>
  <div>
    <!-- ── Header ──────────────────────────────────────────────────
         TODO: เปลี่ยน breadcrumb, title และ link ปุ่ม
    ───────────────────────────────────────────────────────────────── -->
    <div class="app-header">
      <NuxtLink to="/" class="page-crumb">Home</NuxtLink>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Items</span><!-- TODO: เปลี่ยนชื่อ -->
      <div class="header-actions">
        <NuxtLink to="/items/add" class="btn btn-accent">
          <!-- Plus icon -->
          <svg width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
            <line x1="8" y1="2" x2="8" y2="14"/><line x1="2" y1="8" x2="14" y2="8"/>
          </svg>
          Add Item<!-- TODO: เปลี่ยนชื่อปุ่ม -->
        </NuxtLink>
      </div>
    </div>

    <div class="app-main page-enter">

      <!-- ── Alert ──────────────────────────────────────────────────
           แสดงเมื่อ alertMsg ไม่ว่าง — ซ่อนอัตโนมัติ 3 วิ
      ──────────────────────────────────────────────────────────────── -->
      <div v-if="alertMsg" :class="`alert alert-${alertType}`">{{ alertMsg }}</div>

      <div class="card">
        <div class="card-head">
          <span class="card-title">All Items</span><!-- TODO: เปลี่ยนชื่อ -->
          <!-- badge แสดงจำนวน records -->
          <span class="badge badge-muted">{{ items?.length ?? 0 }} records</span>
        </div>

        <!-- ── Empty state ─────────────────────────────────────────
             แสดงเมื่อไม่มีข้อมูล — TODO: แก้ icon, ข้อความ, ลิงก์
        ──────────────────────────────────────────────────────────── -->
        <div v-if="!items?.length" class="empty-state">
          <div class="empty-state-icon">📋</div><!-- TODO: เปลี่ยน emoji -->
          <div class="empty-state-title">ยังไม่มีข้อมูล</div>
          <p style="font-size:12px; margin-top:8px;">
            <NuxtLink to="/items/add" style="color:var(--accent); text-decoration:none;">
              เพิ่มรายการแรก →<!-- TODO: เปลี่ยนข้อความ -->
            </NuxtLink>
          </p>
        </div>

        <!-- ── Data table ──────────────────────────────────────────
             overflow-x:auto รองรับตารางกว้างบนมือถือ
             TODO: เพิ่ม/ลด <th> และ <td> ให้ตรงกับ fields
        ──────────────────────────────────────────────────────────── -->
        <div v-else style="overflow-x:auto;">
          <table class="data-table">
            <thead>
              <tr>
                <th style="width:40px;">#</th>
                <th>Name</th><!-- TODO: เพิ่ม column -->
                <th>Created</th>
                <th style="text-align:center; width:120px;">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in items" :key="item.id">
                <td style="color:var(--text-3);">{{ item.id }}</td>
                <td class="td-bold">{{ item.name }}</td><!-- TODO: เพิ่ม column -->
                <td style="font-size:12px; color:var(--text-2);">
                  {{ new Date(item.createdAt).toLocaleDateString('th-TH') }}
                </td>
                <td style="text-align:center;">
                  <div style="display:flex; gap:6px; justify-content:center;">
                    <!-- TODO: เปลี่ยน path edit ให้ตรง -->
                    <NuxtLink :to="`/items/edit/${item.id}`" class="btn btn-edit">Edit</NuxtLink>
                    <button class="btn btn-danger" @click="handleDelete(item.id)">Del</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

    </div>
  </div>
</template>
