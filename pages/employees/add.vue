<script setup lang="ts">
const router = useRouter();
const { apiBase } = useApi();
const { authHeaders } = useAuth();

interface Department { id: number; name: string; }

const { data: departments } = await useFetch<Department[]>(`${apiBase}/api/departments`, { headers: authHeaders.value });

const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  position: '',
  departmentId: '',
  salary: '',
  startDate: new Date().toISOString().split('T')[0],
  status: 'active',
});

const saving = ref(false);
const error = ref('');

const handleSubmit = async () => {
  saving.value = true;
  error.value = '';
  const { error: fetchError } = await useFetch(`${apiBase}/api/employees`, {
    method: 'POST',
    body: { ...form.value },
    headers: authHeaders.value,
  });
  saving.value = false;
  if (!fetchError.value) {
    await router.push('/employees');
  } else {
    error.value = (fetchError.value as any)?.data?.message || 'เกิดข้อผิดพลาด กรุณาลองใหม่';
  }
};
</script>

<template>
  <div>
    <div class="app-header">
      <NuxtLink to="/employees" class="page-crumb">Employees</NuxtLink>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Add Employee</span>
    </div>

    <div class="app-main page-enter" style="max-width:720px;">
      <div v-if="error" class="alert alert-error">{{ error }}</div>

      <form @submit.prevent="handleSubmit">
        <!-- Personal Info -->
        <div class="form-sec">
          <div class="form-sec-head">
            <div class="form-sec-title">Personal Information</div>
          </div>
          <div class="form-sec-body">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">First Name <span class="req">*</span></label>
                <input v-model="form.firstName" class="form-input" placeholder="e.g. Somchai" required />
              </div>
              <div class="form-group">
                <label class="form-label">Last Name <span class="req">*</span></label>
                <input v-model="form.lastName" class="form-input" placeholder="e.g. Rakdee" required />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Email <span class="req">*</span></label>
                <input v-model="form.email" class="form-input" type="email" placeholder="somchai@company.com" required />
              </div>
              <div class="form-group">
                <label class="form-label">Phone</label>
                <input v-model="form.phone" class="form-input" placeholder="08x-xxx-xxxx" />
              </div>
            </div>
          </div>
        </div>

        <!-- Job Info -->
        <div class="form-sec">
          <div class="form-sec-head">
            <div class="form-sec-title">Job Information</div>
          </div>
          <div class="form-sec-body">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Position <span class="req">*</span></label>
                <input v-model="form.position" class="form-input" placeholder="e.g. Software Engineer" required />
              </div>
              <div class="form-group">
                <label class="form-label">Department <span class="req">*</span></label>
                <select v-model="form.departmentId" class="form-input" required>
                  <option value="" disabled>Select department</option>
                  <option v-for="d in departments" :key="d.id" :value="d.id">{{ d.name }}</option>
                </select>
                <div class="form-hint">
                  No departments?
                  <NuxtLink to="/departments/add" style="color:var(--accent); text-decoration:none;">Add one first →</NuxtLink>
                </div>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Salary (THB/month) <span class="req">*</span></label>
                <input v-model="form.salary" class="form-input" type="number" min="0" placeholder="30000" required />
              </div>
              <div class="form-group">
                <label class="form-label">Start Date <span class="req">*</span></label>
                <input v-model="form.startDate" class="form-input" type="date" required />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Status</label>
              <div style="display:flex; gap:10px; flex-wrap:wrap; margin-top:2px;">
                <label v-for="s in [{v:'active',l:'Active'},{v:'on-leave',l:'On Leave'},{v:'resigned',l:'Resigned'}]" :key="s.v"
                  style="display:flex; align-items:center; gap:6px; cursor:pointer; padding:6px 12px; border-radius:var(--r); border:1px solid var(--border); font-size:12px;"
                  :style="form.status === s.v ? 'border-color:var(--accent); background:var(--accent-dim); color:var(--accent)' : 'color:var(--text-2)'"
                >
                  <input type="radio" v-model="form.status" :value="s.v" style="display:none;" />
                  {{ s.l }}
                </label>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions -->
        <div style="display:flex; gap:10px; align-items:center;">
          <button type="submit" class="btn btn-accent" :disabled="saving">
            <svg v-if="!saving" width="12" height="12" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
              <polyline points="2,8 6,12 14,4"/>
            </svg>
            {{ saving ? 'Saving...' : 'Save Employee' }}
          </button>
          <NuxtLink to="/employees" class="btn btn-ghost">Cancel</NuxtLink>
          <span style="font-size:11px; color:var(--text-3); margin-left:auto;">
            Fields marked <span style="color:var(--red);">*</span> are required
          </span>
        </div>
      </form>
    </div>
  </div>
</template>
