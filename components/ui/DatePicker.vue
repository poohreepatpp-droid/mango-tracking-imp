<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue?: string | null
  type?:       'date' | 'month' | 'year'
  disabled?:   boolean
  min?:        string
  max?:        string
  placeholder?: string
}>(), {
  type:        'date',
  disabled:    false,
  placeholder: 'DD/MM/YYYY',
})

const emit = defineEmits<{
  'update:modelValue': [val: string]
  change: [val: string]
}>()

// Convert DD/MM/YYYY ↔ YYYY-MM-DD (HTML input works with ISO)
function toIso(val: string | null | undefined): string {
  if (!val) return ''
  if (/^\d{4}-\d{2}-\d{2}/.test(val)) return val
  const [d, m, y] = val.split('/')
  return `${y}-${m?.padStart(2,'0')}-${d?.padStart(2,'0')}`
}

function fromIso(val: string): string {
  if (!val) return ''
  if (props.type === 'month') {
    const [y, m] = val.split('-')
    return `${m}/${y}`
  }
  const [y, m, d] = val.split('-')
  return `${d}/${m}/${y}`
}

const inputVal = computed({
  get: () => toIso(props.modelValue),
  set: (v: string) => {
    const formatted = fromIso(v)
    emit('update:modelValue', formatted)
    emit('change', formatted)
  },
})

function setToday() {
  const now = new Date()
  const d = String(now.getDate()).padStart(2, '0')
  const m = String(now.getMonth() + 1).padStart(2, '0')
  const y = now.getFullYear()
  const iso = `${y}-${m}-${d}`
  inputVal.value = iso
}
</script>

<template>
  <div class="relative flex items-center">
    <input
      :type="type === 'month' ? 'month' : 'date'"
      :value="inputVal"
      :disabled="disabled"
      :min="min"
      :max="max"
      @change="(e) => { inputVal = (e.target as HTMLInputElement).value }"
      class="w-full px-3 py-[7px] pr-16 text-[13px] border border-[var(--border)] rounded-[var(--r)] bg-white text-[var(--text)] focus:outline-none focus:border-[var(--accent)] transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    />
    <button
      type="button"
      @click="setToday"
      :disabled="disabled"
      class="absolute right-1 text-[10.5px] font-600 text-[var(--accent)] px-1.5 py-0.5 rounded hover:bg-[var(--accent-dim)] transition-colors border-0 bg-transparent cursor-pointer disabled:opacity-40"
    >
      Today
    </button>
  </div>
</template>
