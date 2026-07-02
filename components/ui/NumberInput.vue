<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue?: number | string | null
  decimals?:   number
  min?:        number
  max?:        number
  step?:       number
  placeholder?: string
  disabled?:   boolean
  align?:      'left' | 'right' | 'center'
  prefix?:     string
  suffix?:     string
}>(), {
  decimals:    2,
  min:         undefined,
  max:         undefined,
  step:        undefined,
  disabled:    false,
  align:       'right',
})

const emit = defineEmits<{
  'update:modelValue': [val: number | null]
  change: [val: number | null]
  blur: [val: number | null]
}>()

const focused = ref(false)
const rawInput = ref('')

const numVal = computed((): number | null => {
  const n = parseFloat(String(props.modelValue ?? ''))
  return isNaN(n) ? null : n
})

function formatDisplay(n: number | null): string {
  if (n === null || n === undefined) return ''
  return n.toLocaleString('th-TH', {
    minimumFractionDigits: props.decimals,
    maximumFractionDigits: props.decimals,
  })
}

const displayVal = computed(() =>
  focused.value
    ? (rawInput.value !== '' ? rawInput.value : (numVal.value !== null ? String(numVal.value) : ''))
    : formatDisplay(numVal.value)
)

function onFocus() {
  focused.value = true
  rawInput.value = numVal.value !== null ? String(numVal.value) : ''
}

function onBlur() {
  focused.value = false
  const parsed = rawInput.value !== '' ? parseFloat(rawInput.value) : null
  const clamped =
    parsed !== null && !isNaN(parsed)
      ? (props.max !== undefined ? Math.min(parsed, props.max) : parsed)
      : null
  const final =
    clamped !== null && props.min !== undefined ? Math.max(clamped, props.min) : clamped
  emit('update:modelValue', final)
  emit('blur', final)
  emit('change', final)
}

function onInput(e: Event) {
  const v = (e.target as HTMLInputElement).value
  // Allow digits, dot, minus
  rawInput.value = v.replace(/[^0-9.\-]/g, '')
}

function onKeypress(e: KeyboardEvent) {
  const allowed = /[0-9.\-]/
  if (!allowed.test(e.key) && !['Backspace','Delete','ArrowLeft','ArrowRight','Tab'].includes(e.key)) {
    e.preventDefault()
  }
}

const alignClass: Record<string, string> = {
  left:   'text-left',
  center: 'text-center',
  right:  'text-right',
}
</script>

<template>
  <div class="relative flex items-center">
    <span v-if="prefix" class="absolute left-3 text-[12.5px] text-[var(--text-3)] pointer-events-none">{{ prefix }}</span>
    <input
      :value="displayVal"
      :disabled="disabled"
      :placeholder="placeholder ?? (decimals === 0 ? '0' : `0.${'0'.repeat(decimals)}`)"
      inputmode="decimal"
      :class="[
        'w-full px-3 py-[7px] text-[13px] border border-[var(--border)] rounded-[var(--r)] bg-white text-[var(--text)] focus:outline-none focus:border-[var(--accent)] transition-colors disabled:opacity-50 disabled:cursor-not-allowed font-[var(--font-m)]',
        alignClass[align],
        prefix ? 'pl-7' : '',
        suffix ? 'pr-10' : '',
      ]"
      @focus="onFocus"
      @blur="onBlur"
      @input="onInput"
      @keypress="onKeypress"
    />
    <span v-if="suffix" class="absolute right-3 text-[12.5px] text-[var(--text-3)] pointer-events-none">{{ suffix }}</span>
  </div>
</template>
