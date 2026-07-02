<script setup lang="ts">
const { user } = useAuth()
const { apiFetch } = useApi()
const router = useRouter()

onMounted(() => {
  if (!user.value?.is_admin && !user.value?.is_super_user) router.replace('/')
})

// ── Types ──────────────────────────────────────────────────────────────
interface SqlFile {
  filename: string
  size:     number
  modified: string
  selected: boolean
  status:   'idle' | 'running' | 'ok' | 'error'
}
interface ResultItem {
  success: boolean
  command: string
  error:   string | null
  version: string
}

// ── State ──────────────────────────────────────────────────────────────
const files          = ref<SqlFile[]>([])
const loading        = ref(true)
const isSelectedAll  = ref(false)
const isProcess      = ref(false)

const currentVersion  = ref('')
const resultItems     = ref<ResultItem[]>([])
const completedCount  = ref(0)
const totalCount      = ref(0)

// ── File list ──────────────────────────────────────────────────────────
async function loadFiles() {
  loading.value = true
  try {
    const res = await apiFetch<{ data: { filename: string; size: number; modified: string }[] }>('/api/sql-update')
    files.value = (res.data ?? []).map(f => ({
      ...f, selected: false, status: 'idle' as const,
    }))
  } catch { files.value = [] }
  finally  { loading.value = false }
}

function selectAll()  { files.value.forEach(f => (f.selected = isSelectedAll.value)) }
function onRowCheck() { isSelectedAll.value = files.value.length > 0 && files.value.every(f => f.selected) }

// Create Bundle = select all idle files
function createBundle() {
  files.value.forEach(f => { if (f.status === 'idle') f.selected = true })
  isSelectedAll.value = files.value.every(f => f.selected)
}

// ── Run ────────────────────────────────────────────────────────────────
async function updateStructure() {
  if (isProcess.value) return
  const queue = files.value.filter(f => f.selected)
  if (!queue.length) {
    alert('กรุณาเลือก Version ที่ต้องการอัพเดทอย่างน้อย 1 รายการ')
    return
  }

  isProcess.value   = true
  resultItems.value = []
  completedCount.value = 0
  totalCount.value     = queue.length
  currentVersion.value = ''

  for (const file of queue) {
    if (!isProcess.value) break

    file.status          = 'running'
    currentVersion.value = displayName(file.filename)

    try {
      const res = await apiFetch<{ data: { all_ok: boolean; results: { sql: string; status: string; message?: string }[] } }>(
        `/api/sql-update/run/${encodeURIComponent(file.filename)}`,
        { method: 'POST' },
      )
      const data  = res.data
      file.status = data?.all_ok ? 'ok' : 'error'

      for (const b of data?.results ?? []) {
        resultItems.value.push({
          success: b.status === 'ok',
          command: b.sql,
          error:   b.message ?? null,
          version: file.filename,
        })
      }
    } catch (e: any) {
      file.status = 'error'
      resultItems.value.push({
        success: false,
        command: file.filename,
        error:   e?.message ?? 'Request failed',
        version: file.filename,
      })
    }

    completedCount.value++
  }

  isProcess.value = false
}

function cancel() { isProcess.value = false }

// ── Helpers ────────────────────────────────────────────────────────────
const displayName = (f: string) => f.replace(/\.sql$/i, '')

const rowClass = (f: SqlFile) => ({
  'row-running': f.status === 'running',
  'row-ok':      f.status === 'ok',
  'row-error':   f.status === 'error',
})

onMounted(loadFiles)
</script>

<template>
  <!-- ── top bar ─────────────────────────────────────────────────────── -->
  <div class="us-bar">
    <span class="us-bar-title">Select Version SQL</span>

    <div class="us-bar-btns">
      <button class="us-btn primary" @click="createBundle" :disabled="isProcess">
        Create Bundle
      </button>
      <button class="us-btn dark" @click="updateStructure" :disabled="isProcess">
        Update Structure
      </button>
      <button class="us-btn danger" @click="cancel" :disabled="!isProcess">
        Cancel
      </button>
    </div>

    <div class="us-bar-right">
      <span class="us-version">Version : <strong>{{ currentVersion }}</strong></span>
      <span class="us-progress">{{ completedCount }} of {{ totalCount }} completed!</span>
    </div>
  </div>

  <!-- ── 2-column body ─────────────────────────────────────────────── -->
  <div class="us-body">

    <!-- LEFT: file list ─────────────────────────────────────── -->
    <div class="us-left">
      <!-- tab bar -->
      <div class="us-tabs">
        <span class="us-tab active">Version</span>
      </div>

      <!-- table -->
      <div class="us-table-wrap">
        <table class="us-table">
          <thead>
            <tr>
              <th class="col-cb">
                <input type="checkbox" v-model="isSelectedAll" @change="selectAll" :disabled="isProcess" />
              </th>
              <th class="col-no">No.</th>
              <th>Version Update</th>
            </tr>
          </thead>
          <tbody>
            <!-- skeleton -->
            <template v-if="loading">
              <tr v-for="i in 3" :key="i">
                <td><div class="skel" style="width:14px;height:14px;border-radius:2px"></div></td>
                <td><div class="skel" style="width:20px"></div></td>
                <td><div class="skel" style="width:160px"></div></td>
              </tr>
            </template>

            <!-- empty -->
            <tr v-else-if="files.length === 0">
              <td colspan="3" class="us-empty">No Data</td>
            </tr>

            <!-- rows -->
            <tr v-else v-for="(f, idx) in files" :key="f.filename" :class="rowClass(f)">
              <td class="col-cb">
                <input type="checkbox" v-model="f.selected" @change="onRowCheck" :disabled="isProcess" />
              </td>
              <td class="col-no">{{ idx + 1 }}</td>
              <td class="col-name">
                <span class="row-status-dot" v-if="f.status !== 'idle'">
                  <span v-if="f.status === 'running'" class="dot-spin"></span>
                  <span v-else-if="f.status === 'ok'"    class="dot dot-ok">✓</span>
                  <span v-else-if="f.status === 'error'" class="dot dot-err">✕</span>
                </span>
                {{ displayName(f.filename) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- RIGHT: results ──────────────────────────────────────── -->
    <div class="us-right">
      <div class="us-right-inner">
        <div v-if="resultItems.length === 0 && !isProcess" class="us-no-result">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.4" width="40" height="40">
            <path stroke-linecap="round" stroke-linejoin="round" d="M8.25 3v1.5M4.5 8.25H3m18 0h-1.5M4.5 12H3m18 0h-1.5m-15 3.75H3m18 0h-1.5M8.25 19.5V21M12 3v1.5m0 15V21m3.75-18v1.5m0 15V21m-9-1.5h10.5a2.25 2.25 0 002.25-2.25V6.75a2.25 2.25 0 00-2.25-2.25H6.75A2.25 2.25 0 004.5 6.75v10.5a2.25 2.25 0 002.25 2.25zm.75-12h9v9h-9v-9z"/>
          </svg>
          <p>Select versions and click<br><strong>Update Structure</strong> to begin</p>
        </div>

        <div v-if="isProcess && resultItems.length === 0" class="us-running-msg">
          <div class="spin-lg"></div>
          <span>Running {{ currentVersion }}…</span>
        </div>

        <div v-for="(item, i) in resultItems" :key="i"
             class="result-card" :class="item.success ? 'card-ok' : 'card-err'">
          <div class="result-card-head">
            <strong>{{ item.success ? '✓ Success' : '✕ Failed' }}</strong>
            <span class="result-ver">{{ displayName(item.version) }}</span>
          </div>
          <div class="result-field">
            <span class="result-label">Command :</span>
            <code class="result-cmd">{{ item.command }}</code>
          </div>
          <div v-if="item.error" class="result-field">
            <span class="result-label">Error :</span>
            <span class="result-err-msg">{{ item.error }}</span>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
/* ── Top bar ─────────────────────────────────────────────────────────── */
.us-bar {
  display: flex; align-items: center; gap: 12px; flex-wrap: wrap;
  padding: 10px 20px; background: #fff;
  border-bottom: 1px solid var(--border);
  position: sticky; top: 52px; z-index: 50;
}
.us-bar-title {
  font-size: 14px; font-weight: 600; color: var(--text); white-space: nowrap;
}
.us-bar-btns { display: flex; gap: 6px; }
.us-btn {
  padding: 5px 14px; font-size: 12.5px; font-weight: 500;
  border: none; border-radius: 4px; cursor: pointer; transition: filter .15s;
  white-space: nowrap;
}
.us-btn:disabled { opacity: .45; cursor: not-allowed; }
.us-btn.primary { background: #0d6efd; color: #fff; }
.us-btn.dark    { background: #343a40; color: #fff; }
.us-btn.danger  { background: #dc3545; color: #fff; }
.us-btn:not(:disabled):hover { filter: brightness(1.12); }

.us-bar-right   { display: flex; align-items: center; gap: 20px; margin-left: auto; }
.us-version     { font-size: 13px; color: var(--text2); white-space: nowrap; }
.us-version strong { color: var(--text); }
.us-progress    { font-size: 13px; color: var(--text2); white-space: nowrap; font-weight: 500; }

/* ── Body ────────────────────────────────────────────────────────────── */
.us-body {
  display: flex; flex: 1;
  min-height: calc(100vh - 52px - 49px);
}

/* ── Left ────────────────────────────────────────────────────────────── */
.us-left {
  flex: 0 0 50%; max-width: 50%;
  border-right: 1px solid var(--border);
  padding: 16px;
}

/* Tabs */
.us-tabs { border-bottom: 1px solid var(--border); margin-bottom: 14px; }
.us-tab {
  display: inline-block; padding: 6px 16px; font-size: 13px; font-weight: 600;
  cursor: pointer; border-bottom: 2px solid transparent; margin-bottom: -1px;
  color: var(--text-3);
}
.us-tab.active { border-bottom-color: var(--accent); color: var(--accent); }

/* Table */
.us-table-wrap { overflow-x: auto; }
.us-table { width: 100%; border-collapse: collapse; font-size: 13px; }
.us-table th {
  text-align: left; padding: 8px 10px; font-size: 12px; font-weight: 700;
  color: var(--text-3); background: var(--surface);
  border: 1px solid var(--border);
}
.us-table td {
  padding: 7px 10px; border: 1px solid var(--border);
  color: var(--text); vertical-align: middle;
}
.col-cb { width: 36px; text-align: center !important; }
.col-no { width: 48px; text-align: center; color: var(--text-3); }
.col-name { display: flex; align-items: center; gap: 7px; }

/* Row states */
.us-table tbody tr.row-running td { background: #fffbeb; }
.us-table tbody tr.row-ok      td { background: #f0fdf4; }
.us-table tbody tr.row-error   td { background: #fef2f2; }
.us-table tbody tr:hover td  { filter: brightness(.98); }

/* Status dots */
.dot { font-size: 11px; font-weight: 700; }
.dot-ok  { color: #16a34a; }
.dot-err { color: #dc2626; }
.dot-spin {
  display: inline-block; width: 10px; height: 10px; border-radius: 50%;
  border: 2px solid #d97706; border-top-color: transparent;
  animation: spin .7s linear infinite; vertical-align: middle;
}

.us-empty { text-align: center; padding: 32px; color: var(--text-3); font-size: 13px; }

/* ── Right ───────────────────────────────────────────────────────────── */
.us-right { flex: 1; padding: 16px; overflow: hidden; }
.us-right-inner {
  height: calc(100vh - 52px - 49px - 32px);
  overflow-y: auto; display: flex; flex-direction: column; gap: 8px;
}

.us-no-result {
  flex: 1; display: flex; flex-direction: column; align-items: center;
  justify-content: center; gap: 12px; color: var(--text-3); text-align: center;
}
.us-no-result p { font-size: 13px; line-height: 1.6; margin: 0; }

.us-running-msg {
  display: flex; align-items: center; gap: 12px;
  padding: 16px; color: var(--text-3); font-size: 13px;
}

/* Result cards */
.result-card {
  border-radius: 5px; padding: 10px 12px; font-size: 12.5px;
  border: 1px solid transparent;
}
.result-card.card-ok  { background: #f0fdf4; border-color: #bbf7d0; }
.result-card.card-err { background: #fef2f2; border-color: #fecaca; }

.result-card-head {
  display: flex; align-items: center; justify-content: space-between;
  margin-bottom: 6px;
}
.result-card-head strong { font-size: 12.5px; }
.card-ok  .result-card-head strong { color: #166534; }
.card-err .result-card-head strong { color: #991b1b; }
.result-ver { font-size: 11px; color: var(--text-3); font-family: var(--font-m); }

.result-field { display: flex; gap: 6px; margin-top: 3px; align-items: flex-start; }
.result-label { font-weight: 600; color: var(--text2); white-space: nowrap; font-size: 12px; }
.result-cmd {
  font-family: var(--font-m); font-size: 11.5px; color: var(--text);
  word-break: break-all; line-height: 1.5;
}
.result-err-msg { font-size: 12px; color: #991b1b; line-height: 1.5; word-break: break-all; }

/* ── Shared ──────────────────────────────────────────────────────────── */
.spin-lg {
  width: 22px; height: 22px; border-radius: 50%;
  border: 3px solid var(--border); border-top-color: var(--accent);
  animation: spin .8s linear infinite; flex-shrink: 0;
}
.skel { height: 13px; background: var(--surface2); border-radius: 3px; animation: pulse 1.4s ease infinite; }

@keyframes spin  { to { transform: rotate(360deg); } }
@keyframes pulse { 0%,100%{opacity:1} 50%{opacity:.4} }
</style>
