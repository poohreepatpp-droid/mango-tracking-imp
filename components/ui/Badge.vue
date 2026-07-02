<script setup lang="ts">
const props = withDefaults(defineProps<{
  variant?: 'green' | 'red' | 'amber' | 'blue' | 'gray'
  dot?:     boolean
}>(), {
  variant: 'gray',
  dot:     false,
})

const styles: Record<string, string> = {
  green: 'bg-[var(--green-dim)] text-[var(--green)]',
  red:   'bg-[var(--red-dim)]   text-[var(--red)]',
  amber: 'bg-[var(--amber-dim)] text-[var(--amber)]',
  blue:  'bg-[var(--blue-dim)]  text-[var(--blue)]',
  gray:  'bg-[var(--surface3)]  text-[var(--text-2)]',
}

const dotColor: Record<string, string> = {
  green: 'bg-[var(--green)]',
  red:   'bg-[var(--red)]',
  amber: 'bg-[var(--amber)]',
  blue:  'bg-[var(--blue)]',
  gray:  'bg-[var(--text-3)]',
}
</script>

<template>
  <span
    :class="[
      'inline-flex items-center gap-1.5 px-2 py-0.5 rounded-full text-[11.5px] font-600',
      styles[variant],
    ]"
  >
    <span v-if="dot" :class="['w-1.5 h-1.5 rounded-full shrink-0', dotColor[variant]]"></span>
    <slot />
  </span>
</template>
