<script setup lang="ts">
const { apiBase } = useApi();
const { authHeaders } = useAuth();
interface Department { id: number; name: string; employeeCount: number; createdAt: string; }
const { data: departments, refresh } = await useFetch<Department[]>(`${apiBase}/api/departments`, { headers: authHeaders.value });
const alertMsg = ref(''); const alertType = ref<'success'|'error'>('success');
const showAlert = (msg: string, type: 'success'|'error') => { alertMsg.value = msg; alertType.value = type; setTimeout(() => { alertMsg.value = ''; }, 3000); };
const handleDelete = async (id: number) => {
  if (!confirm('ลบแผนกนี้? พนักงานในแผนกจะต้องถูกย้ายก่อน')) return;
  const { error } = await useFetch(`${apiBase}/api/departments/${id}`, { method: 'DELETE', headers: authHeaders.value });
  if (!error.value) { showAlert('ลบสำเร็จ', 'success'); await refresh(); }
  else showAlert('ลบไม่สำเร็จ', 'error');
};
</script>

<template>
  <div>
    <div class="app-header">
      <span class="page-crumb">Home</span><span class="crumb-sep">/</span>
      <span class="page-title-h">Departments</span>
      <div class="header-actions">
        <NuxtLink to="/departments/add" class="btn btn-accent">
          <svg width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
            <line x1="8" y1="2" x2="8" y2="14"/><line x1="2" y1="8" x2="14" y2="8"/>
          </svg>
          Add Department
        </NuxtLink>
      </div>
    </div>
    <div class="app-main page-enter">
      <div v-if="alertMsg" :class="`alert alert-${alertType}`">{{ alertMsg }}</div>
      <div class="card">
        <div class="card-head">
          <span class="card-title">All Departments</span>
          <span class="badge badge-muted">{{ departments?.length ?? 0 }} depts</span>
        </div>
        <div v-if="!departments?.length" class="empty-state">
          <div class="empty-state-icon">🏢</div>
          <div class="empty-state-title">No departments yet</div>
          <p style="font-size:12px; margin-top:8px;"><NuxtLink to="/departments/add" style="color:var(--accent); text-decoration:none;">Add first department →</NuxtLink></p>
        </div>
        <div v-else style="overflow-x:auto;">
          <table class="data-table">
            <thead><tr><th style="width:40px;">#</th><th>Department Name</th><th style="text-align:center;">Headcount</th><th style="text-align:center; width:100px;">Actions</th></tr></thead>
            <tbody>
              <tr v-for="d in departments" :key="d.id">
                <td style="color:var(--text-3);">{{ d.id }}</td>
                <td class="td-bold">{{ d.name }}</td>
                <td style="text-align:center;"><span class="badge badge-blue">{{ d.employeeCount }}</span></td>
                <td style="text-align:center;">
                  <button class="btn btn-danger" @click="handleDelete(d.id)">Del</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
