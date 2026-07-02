<!--
  ┌──────────────────────────────────────────────────────────────────┐
  │  TEMPLATE: DETAIL PAGE                                           │
  │  ใช้สำหรับ: หน้าดูรายละเอียด record เดียว (dynamic route)      │
  │  วิธีใช้: คัดลอกไปที่ pages/<entity>/[id].vue                  │
  │           Nuxt อ่าน [id] จาก URL อัตโนมัติ เช่น /items/42     │
  └──────────────────────────────────────────────────────────────────┘
-->
<script setup lang="ts">
// ── 1. อ่าน dynamic :id จาก URL ───────────────────────────────────
// useRoute = Nuxt composable อ่าน route params / query
const route = useRoute()
const id    = route.params.id  // ตรงกับชื่อไฟล์ [id].vue

// ── 2. Type ของข้อมูล ──────────────────────────────────────────────
// TODO: แทนที่ด้วย fields จริง
interface Item {
  id:          number
  name:        string
  description: string | null
  createdAt:   string
  // TODO: เพิ่ม field เพิ่มเติม
}

// ── 3. Fetch ข้อมูล record เดียว ──────────────────────────────────
// TODO: เปลี่ยน '/api/items/' เป็น endpoint จริง
const { data: item, error: fetchError } = await useFetch<Item>(`/api/items/${id}`)

// ── 4. Format helpers ──────────────────────────────────────────────
const fmtDate = (d: string) =>
  new Date(d).toLocaleDateString('th-TH', { year: 'numeric', month: 'long', day: 'numeric' })
</script>

<template>
  <div>
    <!-- ── Header ──────────────────────────────────────────────────
         TODO: เปลี่ยน breadcrumb, title
    ───────────────────────────────────────────────────────────────── -->
    <div class="app-header">
      <NuxtLink to="/items" class="page-crumb">Items</NuxtLink><!-- TODO -->
      <span class="crumb-sep">/</span>
      <span class="page-title-h">{{ item?.name ?? `Item #${id}` }}</span>
      <div class="header-actions">
        <!-- TODO: เปลี่ยน path ปุ่ม Edit -->
        <NuxtLink v-if="item" :to="`/items/edit/${id}`" class="btn btn-ghost btn-sm">Edit</NuxtLink>
        <NuxtLink to="/items" class="btn btn-accent btn-sm">← Back</NuxtLink>
      </div>
    </div>

    <div class="app-main page-enter" style="max-width:720px;">

      <!-- ── Not found / error ────────────────────────────────────── -->
      <div v-if="fetchError || !item" class="empty-state">
        <div class="empty-state-icon">🔍</div>
        <div class="empty-state-title">ไม่พบข้อมูล</div>
        <p style="font-size:12px; margin-top:8px;">
          <NuxtLink to="/items" style="color:var(--accent); text-decoration:none;">← กลับไปรายการ</NuxtLink>
        </p>
      </div>

      <!-- ── Detail card ─────────────────────────────────────────────
           แสดงเมื่อโหลดข้อมูลสำเร็จ
      ──────────────────────────────────────────────────────────────── -->
      <template v-else>
        <div class="card">
          <div class="card-head">
            <span class="card-title">{{ item.name }}</span>
            <!-- TODO: เพิ่ม badge status ถ้ามี เช่น -->
            <!-- <span class="badge badge-green">Active</span> -->
          </div>
          <div class="card-body">

            <!-- ── ตาราง key-value แสดง fields ──────────────────────
                 รูปแบบ 2 column label / value ที่อ่านง่าย
                 TODO: เพิ่ม row ตาม fields จริง
            ──────────────────────────────────────────────────────── -->
            <dl style="display:grid; grid-template-columns:140px 1fr; gap:10px 16px; font-size:13px;">

              <dt style="color:var(--text-3); padding-top:1px;">ID</dt>
              <dd style="color:var(--text-2); margin:0;">{{ item.id }}</dd>

              <dt style="color:var(--text-3); padding-top:1px;">Name</dt><!-- TODO -->
              <dd style="color:var(--text); margin:0; font-weight:600;">{{ item.name }}</dd>

              <dt style="color:var(--text-3); padding-top:1px;">Description</dt><!-- TODO -->
              <dd style="color:var(--text-2); margin:0; line-height:1.6;">
                {{ item.description ?? '—' }}
              </dd>

              <dt style="color:var(--text-3); padding-top:1px;">Created</dt>
              <dd style="color:var(--text-2); margin:0;">{{ fmtDate(item.createdAt) }}</dd>

              <!-- TODO: เพิ่ม row เพิ่มเติม -->

            </dl>
          </div>
        </div>

        <!-- TODO: เพิ่ม card เพิ่มเติม เช่น related items, activity log -->

      </template>
    </div>
  </div>
</template>
