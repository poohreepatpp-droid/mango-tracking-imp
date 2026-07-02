<script setup lang="ts">
const route = useRoute();
const { user, logout, fetchMe } = useAuth();
const { public: { apiBase } } = useRuntimeConfig();
const scalarUrl = computed(() => `${apiBase}/scalar`);
const isActive = (path: string) => path === '/' ? route.path === '/' : route.path.startsWith(path);

const sidebarCollapsed = ref(true);
const sidebarOpen = ref(false);   // mobile overlay
const userMenuOpen = ref(false);

watch(() => route.path, () => { sidebarOpen.value = false; });

const userInitial = computed(() =>
  (user.value?.empname ?? user.value?.userid ?? '?').charAt(0).toUpperCase()
);

const userRole = computed(() =>
  user.value?.is_admin ? 'Admin' : user.value?.is_super_user ? 'Super User' : 'User'
);

function toggleSidebar() {
  if (window.innerWidth < 768) {
    sidebarOpen.value = !sidebarOpen.value;
  } else {
    sidebarCollapsed.value = !sidebarCollapsed.value;
  }
}

const userMenuEl = ref<HTMLElement | null>(null);
function onDocClick(e: MouseEvent) {
  if (userMenuEl.value && !userMenuEl.value.contains(e.target as Node)) {
    userMenuOpen.value = false;
  }
}

let _pollTimer: ReturnType<typeof setInterval> | null = null;
onMounted(() => {
  document.addEventListener('click', onDocClick, true);
  _pollTimer = setInterval(async () => {
    const me = await fetchMe();
    if (!me) await logout();
  }, 10_000);
});
onUnmounted(() => {
  if (_pollTimer) clearInterval(_pollTimer);
  document.removeEventListener('click', onDocClick, true);
});
</script>

<template>
  <div>
    <!-- Mobile overlay backdrop -->
    <div class="sb-overlay" :class="{ open: sidebarOpen }" @click="sidebarOpen = false"></div>

    <!-- ─── Sidebar ─────────────────────────────────────────── -->
    <aside class="sidebar" :class="{ open: sidebarOpen, collapsed: sidebarCollapsed }">
      <div class="sb-brand">
        <NuxtLink to="/" class="sb-logo" title="HR System">
          <span class="sb-dot"></span>
          <span class="sb-brand-text">HR System</span>
        </NuxtLink>
        <div class="sb-sub">v1.0 · .NET Core 10</div>
      </div>

      <nav class="sb-nav">
        <div class="sb-section">Main</div>
        <NuxtLink to="/" class="sb-link" :class="{ active: route.path === '/' }" data-label="Overview">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
            <path d="M6.5 14.5v-3.505c0-.245.25-.495.5-.495h2c.25 0 .5.25.5.5v3.5a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5v-7a.5.5 0 0 0-.146-.354L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.354 1.146a.5.5 0 0 0-.708 0l-6 6A.5.5 0 0 0 1.5 7.5v7a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5z"/>
          </svg>
          <span class="sb-link-text">Overview</span>
        </NuxtLink>
        <NuxtLink to="/dashboard" class="sb-link" :class="{ active: isActive('/dashboard') }" data-label="Dashboard">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
            <path d="M1 2.5A1.5 1.5 0 0 1 2.5 1h3A1.5 1.5 0 0 1 7 2.5v3A1.5 1.5 0 0 1 5.5 7h-3A1.5 1.5 0 0 1 1 5.5v-3zm8 0A1.5 1.5 0 0 1 10.5 1h3A1.5 1.5 0 0 1 15 2.5v3A1.5 1.5 0 0 1 13.5 7h-3A1.5 1.5 0 0 1 9 5.5v-3zm-8 8A1.5 1.5 0 0 1 2.5 9h3A1.5 1.5 0 0 1 7 10.5v3A1.5 1.5 0 0 1 5.5 15h-3A1.5 1.5 0 0 1 1 13.5v-3zm8 0A1.5 1.5 0 0 1 10.5 9h3a1.5 1.5 0 0 1 1.5 1.5v3a1.5 1.5 0 0 1-1.5 1.5h-3A1.5 1.5 0 0 1 9 13.5v-3z"/>
          </svg>
          <span class="sb-link-text">Dashboard</span>
        </NuxtLink>

        <div class="sb-section">Employees</div>
        <NuxtLink to="/employees" class="sb-link" :class="{ active: route.path === '/employees' }" data-label="All Employees">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
            <path d="M7 14s-1 0-1-1 1-4 5-4 5 3 5 4-1 1-1 1H7zm4-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>
            <path fill-rule="evenodd" d="M5.216 14A2.238 2.238 0 0 1 5 13c0-1.355.68-2.75 1.936-3.72A6.325 6.325 0 0 0 5 9c-4 0-5 3-5 4s1 1 1 1h4.216z"/>
            <path d="M4.5 8a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5z"/>
          </svg>
          <span class="sb-link-text">All Employees</span>
        </NuxtLink>
        <NuxtLink to="/employees/add" class="sb-link" :class="{ active: isActive('/employees/add') }" data-label="Add Employee">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round">
            <line x1="8" y1="2" x2="8" y2="14"/><line x1="2" y1="8" x2="14" y2="8"/>
          </svg>
          <span class="sb-link-text">Add Employee</span>
        </NuxtLink>

        <div class="sb-section">Organization</div>
        <NuxtLink to="/departments" class="sb-link" :class="{ active: route.path === '/departments' }" data-label="Departments">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
            <path d="M14.5 3a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-13a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h13zm-13-1A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13z"/>
            <path d="M7 5.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 1 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0zM7 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 0 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0z"/>
          </svg>
          <span class="sb-link-text">Departments</span>
        </NuxtLink>
        <NuxtLink to="/departments/add" class="sb-link" :class="{ active: isActive('/departments/add') }" data-label="Add Department">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round">
            <line x1="8" y1="2" x2="8" y2="14"/><line x1="2" y1="8" x2="14" y2="8"/>
          </svg>
          <span class="sb-link-text">Add Department</span>
        </NuxtLink>

        <div class="sb-section">Templates</div>
        <NuxtLink to="/empty" class="sb-link" :class="{ active: isActive('/empty') }" data-label="Empty Pages">
          <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
            <path d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5L14 4.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5h-2z"/>
          </svg>
          <span class="sb-link-text">Empty Pages</span>
        </NuxtLink>

        <template v-if="user?.is_admin || user?.is_super_user">
          <div class="sb-section">System</div>
          <NuxtLink to="/sql-update" class="sb-link" :class="{ active: isActive('/sql-update') }" data-label="SQL Update">
            <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
              <path d="M1 2.5A1.5 1.5 0 0 1 2.5 1h11A1.5 1.5 0 0 1 15 2.5v11a1.5 1.5 0 0 1-1.5 1.5h-11A1.5 1.5 0 0 1 1 13.5v-11zM2.5 2a.5.5 0 0 0-.5.5V8h12V2.5a.5.5 0 0 0-.5-.5h-11zM14 9H2v4.5a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5V9zM3 3h2v2H3V3zm4 0h2v2H7V3z"/>
            </svg>
            <span class="sb-link-text">SQL Update</span>
          </NuxtLink>
          <a :href="scalarUrl" target="_blank" rel="noopener" class="sb-link sb-link-ext" data-label="API Docs">
            <svg class="sb-icon" viewBox="0 0 16 16" fill="currentColor">
              <path d="M14 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h12zM2 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H2z"/>
              <path d="M6.854 4.646a.5.5 0 0 1 0 .708L4.207 8l2.647 2.646a.5.5 0 0 1-.708.708l-3-3a.5.5 0 0 1 0-.708l3-3a.5.5 0 0 1 .708 0zm2.292 0a.5.5 0 0 0 0 .708L11.793 8l-2.647 2.646a.5.5 0 0 0 .708.708l3-3a.5.5 0 0 0 0-.708l-3-3a.5.5 0 0 0-.708 0z"/>
            </svg>
            <span class="sb-link-text">
              API Docs
              <svg viewBox="0 0 12 12" fill="currentColor" width="10" height="10" style="opacity:.5;margin-left:3px;flex-shrink:0">
                <path d="M3.5 1h7a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0V2.707L3.354 9.354a.5.5 0 1 1-.708-.708L9.293 2H3.5a.5.5 0 0 1 0-1z"/>
              </svg>
            </span>
          </a>
        </template>
      </nav>
    </aside>

    <!-- ─── Main area ─────────────────────────────────────────── -->
    <div class="app-wrap" :class="{ 'sidebar-collapsed': sidebarCollapsed }">

      <!-- Layout top bar (sticky) -->
      <div class="layout-bar">
        <!-- Sidebar toggle -->
        <button class="layout-toggle" @click="toggleSidebar" :title="sidebarCollapsed ? 'Expand sidebar' : 'Collapse sidebar'">
          <svg viewBox="0 0 20 20" fill="currentColor" width="17" height="17">
            <path fill-rule="evenodd" d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd"/>
          </svg>
        </button>

        <div style="flex: 1;"></div>

        <!-- User menu -->
        <div class="user-menu-wrap" ref="userMenuEl">
          <button class="user-menu-btn" @click.stop="userMenuOpen = !userMenuOpen">
            <div class="user-avatar-sm">{{ userInitial }}</div>
            <div class="user-name-group">
              <span class="user-display-name">{{ user?.empname ?? user?.userid ?? '—' }}</span>
              <span class="user-display-role">{{ userRole }}</span>
            </div>
            <svg class="chevron-icon" :class="{ 'chevron-up': userMenuOpen }" viewBox="0 0 16 16" fill="currentColor" width="11" height="11">
              <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>
            </svg>
          </button>

          <Transition name="ud-anim">
            <div v-if="userMenuOpen" class="user-dropdown">
              <!-- Header -->
              <div class="ud-header">
                <div class="ud-avatar">{{ userInitial }}</div>
                <div class="ud-info">
                  <div class="ud-name">{{ user?.empname ?? user?.userid }}</div>
                  <div class="ud-email">{{ user?.empmail ?? (user?.empcode ? `#${user.empcode}` : '') }}</div>
                </div>
              </div>
              <div class="ud-divider"></div>
              <!-- Details -->
              <div class="ud-body">
                <div class="ud-row">
                  <span class="ud-label">Company</span>
                  <span class="ud-val">{{ user?.mainname ?? user?.maincode ?? '—' }}</span>
                </div>
                <div class="ud-row">
                  <span class="ud-label">Department</span>
                  <span class="ud-val">{{ user?.dpt_name ?? '—' }}</span>
                </div>
                <div class="ud-row">
                  <span class="ud-label">Position</span>
                  <span class="ud-val">{{ user?.emppos ?? '—' }}</span>
                </div>
              </div>
              <div class="ud-divider"></div>
              <!-- Logout -->
              <div class="ud-footer">
                <button class="ud-logout" @click="logout">
                  <svg viewBox="0 0 16 16" fill="currentColor" width="13" height="13">
                    <path fill-rule="evenodd" d="M10 12.5a.5.5 0 0 1-.5.5h-8a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h8a.5.5 0 0 1 .5.5v2a.5.5 0 0 0 1 0v-2A1.5 1.5 0 0 0 9.5 2h-8A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h8a1.5 1.5 0 0 0 1.5-1.5v-2a.5.5 0 0 0-1 0v2z"/>
                    <path fill-rule="evenodd" d="M15.854 8.354a.5.5 0 0 0 0-.708l-3-3a.5.5 0 0 0-.708.708L14.293 7.5H5.5a.5.5 0 0 0 0 1h8.793l-2.147 2.146a.5.5 0 0 0 .708.708l3-3z"/>
                  </svg>
                  Sign out
                </button>
              </div>
            </div>
          </Transition>
        </div>
      </div>

      <!-- Page content from each page -->
      <slot />
    </div>
  </div>
</template>
