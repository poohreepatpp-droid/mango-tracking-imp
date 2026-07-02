<script setup lang="ts" generic="T extends Record<string, any>">
export interface ColDef<T> {
  key:       keyof T | string
  label:     string
  sortable?: boolean
  width?:    string
  align?:    'left' | 'center' | 'right'
  format?:   (val: any, row: T) => string
  class?:    string | ((row: T) => string)
}

const props = withDefaults(defineProps<{
  cols:        ColDef<T>[]
  rows:        T[]
  loading?:    boolean
  searchable?: boolean
  pageSize?:   number
  emptyText?:  string
  rowKey?:     keyof T | string
}>(), {
  loading:    false,
  searchable: false,
  pageSize:   0,
  emptyText:  'No data',
  rowKey:     'id',
})

const emit = defineEmits<{
  rowClick: [row: T]
}>()

// ── Search ────────────────────────────────
const search = ref('')

// ── Sort ─────────────────────────────────
const sortKey = ref<string>('')
const sortDir = ref<'asc' | 'desc'>('asc')

function setSort(key: string) {
  if (sortKey.value === key) {
    sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = key
    sortDir.value = 'asc'
  }
  page.value = 1
}

// ── Pagination ────────────────────────────
const page = ref(1)

const filtered = computed(() => {
  let rows = props.rows
  if (search.value.trim()) {
    const q = search.value.toLowerCase()
    rows = rows.filter(r =>
      props.cols.some(c => String(r[c.key] ?? '').toLowerCase().includes(q))
    )
  }
  if (sortKey.value) {
    rows = [...rows].sort((a, b) => {
      const va = a[sortKey.value] ?? ''
      const vb = b[sortKey.value] ?? ''
      const cmp = va < vb ? -1 : va > vb ? 1 : 0
      return sortDir.value === 'asc' ? cmp : -cmp
    })
  }
  return rows
})

const pageCount = computed(() =>
  props.pageSize > 0 ? Math.ceil(filtered.value.length / props.pageSize) : 1
)

const paginated = computed(() => {
  if (props.pageSize <= 0) return filtered.value
  const start = (page.value - 1) * props.pageSize
  return filtered.value.slice(start, start + props.pageSize)
})

watch(search, () => { page.value = 1 })

function cellVal(row: T, col: ColDef<T>): string {
  const val = row[col.key as keyof T]
  return col.format ? col.format(val, row) : (val ?? '') as string
}

function cellClass(row: T, col: ColDef<T>): string {
  if (typeof col.class === 'function') return col.class(row)
  return col.class ?? ''
}

const alignClass: Record<string, string> = {
  left:   'text-left',
  center: 'text-center',
  right:  'text-right',
}
</script>

<template>
  <div class="flex flex-col gap-3">
    <!-- Toolbar -->
    <div v-if="searchable || $slots.toolbar" class="flex items-center gap-2">
      <slot name="toolbar" />
      <div v-if="searchable" class="relative ml-auto">
        <svg class="absolute left-2.5 top-1/2 -translate-y-1/2 text-[var(--text-3)]" viewBox="0 0 16 16" fill="currentColor" width="13" height="13">
          <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
        </svg>
        <input
          v-model="search"
          type="text"
          placeholder="Search..."
          class="pl-8 pr-3 py-1.5 text-[12.5px] border border-[var(--border)] rounded-[var(--r)] bg-white text-[var(--text)] placeholder:text-[var(--text-3)] focus:outline-none focus:border-[var(--accent)] transition-colors w-52"
        />
      </div>
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-[var(--r-lg)] border border-[var(--border)]">
      <table class="w-full border-collapse text-[13px]">
        <thead>
          <tr class="bg-[var(--surface2)] border-b border-[var(--border)]">
            <th
              v-for="col in cols"
              :key="String(col.key)"
              :style="col.width ? `width:${col.width}` : ''"
              :class="[
                'px-4 py-2.5 font-600 text-[var(--text-2)] text-[11.5px] uppercase tracking-[.06em]',
                alignClass[col.align ?? 'left'],
                col.sortable ? 'cursor-pointer select-none hover:text-[var(--text)] transition-colors' : '',
              ]"
              @click="col.sortable ? setSort(String(col.key)) : undefined"
            >
              <span class="inline-flex items-center gap-1">
                {{ col.label }}
                <template v-if="col.sortable">
                  <svg
                    viewBox="0 0 16 16" fill="currentColor" width="10" height="10"
                    :class="[
                      'transition-opacity',
                      sortKey === String(col.key) ? 'opacity-100 text-[var(--accent)]' : 'opacity-30',
                      sortKey === String(col.key) && sortDir === 'desc' ? 'rotate-180' : '',
                    ]"
                  >
                    <path fill-rule="evenodd" d="M8 1a.5.5 0 0 1 .5.5v11.793l3.146-3.147a.5.5 0 0 1 .708.708l-4 4a.5.5 0 0 1-.708 0l-4-4a.5.5 0 0 1 .708-.708L7.5 13.293V1.5A.5.5 0 0 1 8 1z"/>
                  </svg>
                </template>
              </span>
            </th>
          </tr>
        </thead>

        <tbody>
          <!-- Loading skeleton -->
          <template v-if="loading">
            <tr v-for="n in 5" :key="n" class="border-b border-[var(--border)] last:border-0">
              <td v-for="col in cols" :key="String(col.key)" class="px-4 py-3">
                <div class="h-4 bg-[var(--surface3)] rounded animate-pulse" :style="`width:${60 + (n * 13) % 35}%`"></div>
              </td>
            </tr>
          </template>

          <!-- Empty -->
          <tr v-else-if="paginated.length === 0">
            <td :colspan="cols.length" class="px-4 py-12 text-center text-[var(--text-3)] text-[13px]">
              <div class="flex flex-col items-center gap-2">
                <svg viewBox="0 0 48 48" fill="none" stroke="currentColor" stroke-width="1.5" width="36" height="36" class="text-[var(--border2)]">
                  <rect x="6" y="10" width="36" height="28" rx="3"/><line x1="6" y1="18" x2="42" y2="18"/>
                  <line x1="16" y1="26" x2="26" y2="26"/><line x1="16" y1="30" x2="22" y2="30"/>
                </svg>
                {{ emptyText }}
              </div>
            </td>
          </tr>

          <!-- Data rows -->
          <tr
            v-else
            v-for="row in paginated"
            :key="String(row[rowKey])"
            class="border-b border-[var(--border)] last:border-0 hover:bg-[var(--surface2)] transition-colors cursor-default"
            @click="emit('rowClick', row)"
          >
            <td
              v-for="col in cols"
              :key="String(col.key)"
              :class="['px-4 py-3 text-[var(--text)]', alignClass[col.align ?? 'left'], cellClass(row, col)]"
            >
              <slot :name="`cell-${String(col.key)}`" :row="row" :val="cellVal(row, col)">
                {{ cellVal(row, col) }}
              </slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination + count -->
    <div v-if="pageSize > 0 && pageCount > 1" class="flex items-center justify-between text-[12px] text-[var(--text-3)]">
      <span>{{ filtered.length }} records</span>
      <div class="flex items-center gap-1">
        <button
          v-for="p in pageCount"
          :key="p"
          @click="page = p"
          :class="[
            'w-7 h-7 rounded flex items-center justify-center font-500 border-0 cursor-pointer transition-colors',
            page === p
              ? 'bg-[var(--accent)] text-white'
              : 'bg-transparent text-[var(--text-2)] hover:bg-[var(--surface2)]',
          ]"
        >
          {{ p }}
        </button>
      </div>
    </div>
  </div>
</template>
