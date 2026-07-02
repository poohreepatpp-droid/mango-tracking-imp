<script setup lang="ts">
const { apiBase } = useApi();
const { authHeaders } = useAuth();
const { data: stats, pending } = await useFetch(`${apiBase}/api/stats`, { headers: authHeaders.value });

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
</script>

<template>
  <div>
    <div class="app-header">
      <span class="page-crumb">Home</span>
      <span class="crumb-sep">/</span>
      <span class="page-title-h">HR Dashboard</span>
    </div>

    <div class="app-main page-enter">
      <!-- Stat cards -->
      <div class="stat-grid">
        <div class="stat-card c-amber">
          <div class="stat-ico">👥</div>
          <div class="stat-label">Total Employees</div>
          <div class="stat-val">{{ stats?.total ?? '—' }}</div>
          <div class="stat-sub">{{ stats?.deptCount ?? 0 }} departments</div>
        </div>
        <div class="stat-card c-green">
          <div class="stat-ico">✅</div>
          <div class="stat-label">Active</div>
          <div class="stat-val">{{ stats?.active ?? '—' }}</div>
          <div class="stat-sub">currently working</div>
        </div>
        <div class="stat-card c-amber" style="--c: var(--accent);">
          <div class="stat-ico">🏖️</div>
          <div class="stat-label">On Leave</div>
          <div class="stat-val">{{ stats?.onLeave ?? '—' }}</div>
          <div class="stat-sub">temporary absence</div>
        </div>
        <div class="stat-card c-blue">
          <div class="stat-ico">💵</div>
          <div class="stat-label">Avg. Salary</div>
          <div class="stat-val">
            {{ stats?.avgSalary ? stats.avgSalary.toLocaleString('th-TH', { maximumFractionDigits: 0 }) : '—' }}
          </div>
          <div class="stat-sub">baht / month</div>
        </div>
      </div>

      <div class="panel-grid">
        <!-- Department breakdown -->
        <div class="card">
          <div class="card-head">
            <span class="card-title">By Department</span>
            <span class="badge badge-muted">{{ stats?.deptCount ?? 0 }} depts</span>
          </div>
          <div v-if="!stats?.depts?.length" class="empty-state">
            <div class="empty-state-icon">🏢</div>
            <div class="empty-state-title">No departments yet</div>
          </div>
          <div v-else class="card-body" style="padding:0;">
            <div
              v-for="d in stats.depts" :key="d.id"
              style="display:flex; align-items:center; justify-content:space-between; padding:11px 18px; border-bottom:1px solid var(--border);"
            >
              <span style="color:var(--text); font-size:13px;">{{ d.name }}</span>
              <span class="badge badge-muted">{{ d._count.employees }} people</span>
            </div>
          </div>
        </div>

        <!-- Recent hires -->
        <div class="card">
          <div class="card-head">
            <span class="card-title">Recent Hires</span>
            <NuxtLink to="/employees" style="font-size:11px; color:var(--accent); text-decoration:none;">View all →</NuxtLink>
          </div>
          <div v-if="pending" class="card-body" style="text-align:center; color:var(--text-3); padding:32px;">Loading...</div>
          <div v-else-if="!stats?.recentHires?.length" class="empty-state">
            <div class="empty-state-icon">👤</div>
            <div class="empty-state-title">No employees yet</div>
          </div>
          <table v-else class="data-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Position</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="e in stats.recentHires" :key="e.id">
                <td class="td-bold">{{ e.firstName }} {{ e.lastName }}</td>
                <td style="color:var(--text-2); font-size:12px;">{{ e.position }}</td>
                <td>
                  <span :class="`badge ${statusBadge(e.status)}`">{{ statusLabel(e.status) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>
