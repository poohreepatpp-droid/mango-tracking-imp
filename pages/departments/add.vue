<script setup lang="ts">
const router = useRouter();
const { apiBase } = useApi();
const { authHeaders } = useAuth();
const name = ref('');
const saving = ref(false);
const error = ref('');
const handleSubmit = async () => {
  saving.value = true; error.value = '';
  const { error: e } = await useFetch(`${apiBase}/api/departments`, { method: 'POST', body: { name: name.value }, headers: authHeaders.value });
  saving.value = false;
  if (!e.value) await router.push('/departments');
  else error.value = 'เกิดข้อผิดพลาด กรุณาลองใหม่';
};
</script>

<template>
  <div>
    <div class="app-header">
      <NuxtLink to="/departments" class="page-crumb">Departments</NuxtLink>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">Add Department</span>
    </div>
    <div class="app-main page-enter" style="max-width:480px;">
      <div v-if="error" class="alert alert-error">{{ error }}</div>
      <form @submit.prevent="handleSubmit">
        <div class="form-sec">
          <div class="form-sec-head"><div class="form-sec-title">Department Details</div></div>
          <div class="form-sec-body">
            <div class="form-group">
              <label class="form-label">Department Name <span class="req">*</span></label>
              <input v-model="name" class="form-input" placeholder="e.g. Engineering, HR, Finance" required />
            </div>
          </div>
        </div>
        <div style="display:flex; gap:10px;">
          <button type="submit" class="btn btn-accent" :disabled="saving">
            {{ saving ? 'Saving...' : 'Save Department' }}
          </button>
          <NuxtLink to="/departments" class="btn btn-ghost">Cancel</NuxtLink>
        </div>
      </form>
    </div>
  </div>
</template>
