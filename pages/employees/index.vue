<script setup lang="ts">
interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  position: string;
  department: { id: number; name: string };
  salary: number;
  startDate: string;
  status: string;
}

const { apiFetch, apiBase } = useApi();
const { authHeaders } = useAuth();

const { data: employees, refresh } = await useFetch<Employee[]>(`${apiBase}/api/employees`, { headers: authHeaders.value });

const alertMsg = ref('');
const alertType = ref<'success' | 'error'>('success');

const showAlert = (msg: string, type: 'success' | 'error') => {
  alertMsg.value = msg;
  alertType.value = type;
  setTimeout(() => { alertMsg.value = ''; }, 3000);
};

const handleDelete = async (id: number) => {
  if (!confirm('ต้องการลบพนักงานคนนี้?')) return;
  const { error } = await useFetch(`${apiBase}/api/employees/${id}`, { method: 'DELETE', headers: authHeaders.value });
  if (!error.value) {
    showAlert('ลบข้อมูลสำเร็จ', 'success');
    await refresh();
  } else {
    showAlert('ลบไม่สำเร็จ', 'error');
  }
};

const statusBadge = (s: string) => {
  if (s === 'active')   return 'badge-green';
  if (s === 'on-leave') return 'badge-amber';
  return 'badge-red';
};

const statusLabel = (s: string) => {
  if (s === 'active')   return 'Active';
  if (s === 'on-leave') return 'On Leave';
  return 'Resigned';
};

const fmtDate = (d: string) => new Date(d).toLocaleDateString('th-TH', { year: 'numeric', month: 'short', day: 'numeric' });
</script>

<template>
  <div>
    <div class="app-header">
      <span class="page-crumb">Home</span>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Employees</span>
      <div class="header-actions">
        <NuxtLink to="/employees/add" class="btn btn-accent">
          <svg width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
            <line x1="8" y1="2" x2="8" y2="14"/><line x1="2" y1="8" x2="14" y2="8"/>
          </svg>
          Add Employee
        </NuxtLink>
      </div>
    </div>

    <div class="app-main page-enter">
      <div v-if="alertMsg" :class="`alert alert-${alertType}`">{{ alertMsg }}</div>

      <div class="card">
        <div class="card-head">
          <span class="card-title">All Employees</span>
          <span class="badge badge-muted">{{ employees?.length ?? 0 }} records</span>
        </div>

        <div v-if="!employees?.length" class="empty-state">
          <div class="empty-state-icon">👤</div>
          <div class="empty-state-title">No employees yet</div>
          <p style="font-size:12px; margin-top:8px;">
            <NuxtLink to="/employees/add" style="color:var(--accent); text-decoration:none;">Add your first employee →</NuxtLink>
          </p>
        </div>

        <div v-else style="overflow-x:auto;">
          <table class="data-table">
            <thead>
              <tr>
                <th style="width:40px;">#</th>
                <th>Name</th>
                <th>Position</th>
                <th>Department</th>
                <th style="text-align:right;">Salary</th>
                <th>Start Date</th>
                <th style="text-align:center;">Status</th>
                <th style="text-align:center; width:120px;">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="e in employees" :key="e.id">
                <td style="color:var(--text-3);">{{ e.id }}</td>
                <td>
                  <div class="td-bold">{{ e.firstName }} {{ e.lastName }}</div>
                  <div style="font-size:11px; color:var(--text-3);">{{ e.email }}</div>
                </td>
                <td style="color:var(--text-2);">{{ e.position }}</td>
                <td>
                  <span class="badge badge-blue">{{ e.department.name }}</span>
                </td>
                <td class="td-num" style="text-align:right;">
                  {{ e.salary.toLocaleString('th-TH') }}
                </td>
                <td style="color:var(--text-2); font-size:12px;">{{ fmtDate(e.startDate) }}</td>
                <td style="text-align:center;">
                  <span :class="`badge ${statusBadge(e.status)}`">{{ statusLabel(e.status) }}</span>
                </td>
                <td style="text-align:center;">
                  <div style="display:flex; gap:6px; justify-content:center;">
                    <NuxtLink :to="`/employees/edit/${e.id}`" class="btn btn-edit">Edit</NuxtLink>
                    <button class="btn btn-danger" @click="handleDelete(e.id)">Del</button>
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
