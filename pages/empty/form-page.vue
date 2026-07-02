<!--
  ┌──────────────────────────────────────────────────────────────────┐
  │  TEMPLATE: FORM PAGE                                             │
  │  ใช้สำหรับ: หน้าสร้าง / แก้ไขข้อมูล (Add / Edit)              │
  │  วิธีใช้: คัดลอกไปที่ pages/<entity>/add.vue                   │
  │           สำหรับ Edit ให้เพิ่ม useFetch GET :id เพื่อ prefill  │
  └──────────────────────────────────────────────────────────────────┘
-->
<script setup lang="ts">
// ── 1. Router — ใช้ redirect หลัง submit สำเร็จ ────────────────────
const router = useRouter()

// ── 2. Form state ──────────────────────────────────────────────────
// reactive object รวม field ทั้งหมดของฟอร์ม
// TODO: เพิ่ม/ลด fields ให้ตรงกับ Prisma model
const form = ref({
  name:        '',
  description: '',
  // เพิ่ม field เพิ่มเติมที่นี่ เช่น:
  // status: 'active',
  // categoryId: '',
})

// ── 3. UI state ────────────────────────────────────────────────────
const saving = ref(false)   // ป้องกัน double-submit + แสดง loading
const error  = ref('')      // error message จาก API

// ── 4. Submit handler ──────────────────────────────────────────────
// TODO: เปลี่ยน '/api/items' เป็น endpoint จริง
// TODO: เปลี่ยน redirect path หลัง save สำเร็จ
const handleSubmit = async () => {
  saving.value = true
  error.value  = ''

  const { error: fetchError } = await useFetch('/api/items', {
    method: 'POST',
    body: { ...form.value },
  })

  saving.value = false

  if (!fetchError.value) {
    await router.push('/items') // TODO: เปลี่ยน path
  } else {
    // ดึง error message จาก API response (Nitro format)
    error.value = (fetchError.value as any)?.data?.message || 'เกิดข้อผิดพลาด กรุณาลองใหม่'
  }
}
</script>

<template>
  <div>
    <!-- ── Header ──────────────────────────────────────────────────
         TODO: เปลี่ยน breadcrumb, title
    ───────────────────────────────────────────────────────────────── -->
    <div class="app-header">
      <NuxtLink to="/items" class="page-crumb">Items</NuxtLink><!-- TODO -->
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Add Item</span><!-- TODO -->
    </div>

    <!-- max-width:720px เหมาะกับฟอร์ม 2 column -->
    <div class="app-main page-enter" style="max-width:720px;">

      <!-- ── Error banner ────────────────────────────────────────── -->
      <div v-if="error" class="alert alert-error">{{ error }}</div>

      <!-- ── Form ────────────────────────────────────────────────────
           @submit.prevent ป้องกัน browser default submit
      ──────────────────────────────────────────────────────────────── -->
      <form @submit.prevent="handleSubmit">

        <!-- ── Section 1: ข้อมูลหลัก ──────────────────────────────
             form-sec     = กล่อง section มี border
             form-sec-head/title = header ของ section
             form-sec-body = area ฟอร์มด้านใน
             form-row     = 2-column grid (responsive)
             form-group   = wrapper ของ label + input คู่เดียว
             form-label   = label บน input
             form-input   = input / select / textarea
             req          = เครื่องหมาย * สีแดง (required)
             form-hint    = ข้อความช่วยเหลือใต้ input
        ──────────────────────────────────────────────────────────── -->
        <div class="form-sec">
          <div class="form-sec-head">
            <div class="form-sec-title">Basic Information</div><!-- TODO: เปลี่ยนชื่อ section -->
          </div>
          <div class="form-sec-body">

            <!-- 2 column row -->
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Name <span class="req">*</span></label>
                <input v-model="form.name" class="form-input" placeholder="e.g. My Item" required />
              </div>
              <!-- TODO: เพิ่ม field ที่ 2 ในแถวเดียวกัน หรือลบ form-row wrapper ออกถ้ามีแค่ 1 field -->
            </div>

            <!-- 1 column (textarea) -->
            <div class="form-group">
              <label class="form-label">Description</label>
              <textarea v-model="form.description" class="form-input" rows="3"
                placeholder="รายละเอียด..." style="resize:vertical;"></textarea>
              <div class="form-hint">ไม่บังคับ — อธิบายรายละเอียดเพิ่มเติม</div>
            </div>

            <!-- TODO: เพิ่ม section / field เพิ่มเติมตามต้องการ -->

          </div>
        </div>

        <!-- ── Actions bar ─────────────────────────────────────────
             :disabled ป้องกัน submit ซ้ำระหว่าง loading
        ──────────────────────────────────────────────────────────── -->
        <div style="display:flex; gap:10px; align-items:center;">
          <button type="submit" class="btn btn-accent" :disabled="saving">
            <!-- checkmark icon — ซ่อนระหว่าง loading -->
            <svg v-if="!saving" width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
              <polyline points="2,8 6,12 14,4"/>
            </svg>
            {{ saving ? 'Saving...' : 'Save' }}<!-- TODO: เปลี่ยนข้อความปุ่ม -->
          </button>
          <!-- TODO: เปลี่ยน Cancel path ให้ตรง -->
          <NuxtLink to="/items" class="btn btn-ghost">Cancel</NuxtLink>
          <span style="font-size:11px; color:var(--text-3); margin-left:auto;">
            Fields marked <span style="color:var(--red);">*</span> are required
          </span>
        </div>

      </form>
    </div>
  </div>
</template>
