<script setup lang="ts">
const props = withDefaults(defineProps<{
  title?:    string
  size?:     'sm' | 'md' | 'lg' | 'xl'
  hideFooter?: boolean
  closable?: boolean
}>(), {
  size:     'md',
  closable: true,
})

const emit = defineEmits<{
  close: []
}>()

const widthClass: Record<string, string> = {
  sm: 'max-w-sm',
  md: 'max-w-lg',
  lg: 'max-w-2xl',
  xl: 'max-w-4xl',
}

function onBackdrop(e: MouseEvent) {
  if (e.target === e.currentTarget && props.closable) emit('close')
}

function onKeydown(e: KeyboardEvent) {
  if (e.key === 'Escape' && props.closable) emit('close')
}

onMounted(() => {
  document.addEventListener('keydown', onKeydown)
  document.body.style.overflow = 'hidden'
})
onUnmounted(() => {
  document.removeEventListener('keydown', onKeydown)
  document.body.style.overflow = ''
})
</script>

<template>
  <Teleport to="body">
    <!-- Backdrop -->
    <div
      class="fixed inset-0 z-[500] flex items-center justify-center p-4"
      style="background: rgba(0,0,0,.45); backdrop-filter: blur(2px);"
      @click="onBackdrop"
    >
      <!-- Panel -->
      <div
        class="relative w-full bg-white rounded-xl shadow-2xl flex flex-col max-h-[90vh] animate-[modalIn_.22s_cubic-bezier(.16,1,.3,1)]"
        :class="widthClass[size]"
        @click.stop
      >
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-4 border-b border-[var(--border)] shrink-0">
          <h3 class="text-[15px] font-700 text-[var(--text)] tracking-[-0.3px]">
            <slot name="title">{{ title }}</slot>
          </h3>
          <button
            v-if="closable"
            @click="emit('close')"
            class="w-7 h-7 flex items-center justify-center rounded-md text-[var(--text-3)] hover:text-[var(--text)] hover:bg-[var(--surface2)] transition-colors border-0 bg-transparent cursor-pointer"
          >
            <svg viewBox="0 0 16 16" fill="currentColor" width="14" height="14">
              <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z"/>
            </svg>
          </button>
        </div>

        <!-- Body -->
        <div class="flex-1 overflow-y-auto px-6 py-5">
          <slot />
        </div>

        <!-- Footer -->
        <div
          v-if="!hideFooter && $slots.footer"
          class="flex items-center justify-end gap-2 px-6 py-4 border-t border-[var(--border)] shrink-0"
        >
          <slot name="footer" />
        </div>
      </div>
    </div>
  </Teleport>
</template>

<style>
@keyframes modalIn {
  from { opacity: 0; transform: scale(.96) translateY(10px); }
  to   { opacity: 1; transform: scale(1) translateY(0); }
}
</style>
