<script setup lang="ts">
definePageMeta({ layout: false });

const { public: { apiBase } } = useRuntimeConfig();
const { login } = useAuth();
const router = useRouter();

interface Company { mainCode: string; mainName: string; defaultLogin: string | null; }

const companies    = ref<Company[]>([]);
const form         = ref({ userId: '', userPass: '', mainCode: '' });
const loading      = ref(false);
const error        = ref('');
const showPass     = ref(false);
const showDropdown = ref(false);
const searchQuery  = ref('');
const canvasRef    = ref<HTMLCanvasElement | null>(null);
let   animId       = 0;

const selectedCompany = computed(() =>
  companies.value.find(c => c.mainCode === form.value.mainCode)
);

const filteredCompanies = computed(() => {
  const q = searchQuery.value.toLowerCase();
  return q
    ? companies.value.filter(c => c.mainName?.toLowerCase().includes(q) || c.mainCode?.toLowerCase().includes(q))
    : companies.value;
});

onMounted(async () => {
  try {
    const res = await $fetch<{ success: boolean; data: Company[] }>(`${apiBase}/api/public/login-companies`);
    companies.value = res.data ?? [];
    const def = companies.value.find(c => c.defaultLogin === 'Y') ?? companies.value[0];
    if (def) form.value.mainCode = def.mainCode;
  } catch { }

  initCanvas();
  document.addEventListener('click', closeDropdown);
});

onUnmounted(() => {
  cancelAnimationFrame(animId);
  document.removeEventListener('click', closeDropdown);
});

const closeDropdown = (e: MouseEvent) => {
  if (!(e.target as HTMLElement)?.closest?.('.company-wrap')) showDropdown.value = false;
};

const selectCompany = (code: string) => {
  form.value.mainCode = code;
  showDropdown.value  = false;
  searchQuery.value   = '';
};

const handleLogin = async () => {
  if (!form.value.userId || !form.value.userPass) { error.value = 'Please enter username and password.'; return; }
  loading.value = true;
  error.value   = '';
  try {
    await login(form.value.userId, form.value.userPass, form.value.mainCode || undefined);
    await router.push('/dashboard');
  } catch (e: any) {
    if (e?.data?.error) error.value = e.data.error;
    else if (e?.message?.toLowerCase().includes('fetch') || e?.status === undefined)
      error.value = 'Cannot connect to backend — ensure dotnet run is running';
    else error.value = `Error (${e?.status ?? e?.message ?? 'unknown'})`;
  } finally {
    loading.value = false;
  }
};

function initCanvas() {
  const canvas = canvasRef.value;
  if (!canvas) return;
  const ctx = canvas.getContext('2d')!;

  const resize = () => { canvas.width = canvas.offsetWidth; canvas.height = canvas.offsetHeight; };
  resize();
  window.addEventListener('resize', resize);

  type P = { x: number; y: number; vx: number; vy: number; r: number; opacity: number };
  const pts: P[] = Array.from({ length: 55 }, () => ({
    x: Math.random() * canvas.width,
    y: Math.random() * canvas.height,
    vx: (Math.random() - 0.5) * 0.35,
    vy: (Math.random() - 0.5) * 0.35,
    r: Math.random() * 1.8 + 0.8,
    opacity: Math.random() * 0.5 + 0.3,
  }));

  const tick = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    for (let i = 0; i < pts.length; i++) {
      for (let j = i + 1; j < pts.length; j++) {
        const dx = pts[i].x - pts[j].x, dy = pts[i].y - pts[j].y;
        const d = Math.sqrt(dx * dx + dy * dy);
        if (d < 130) {
          ctx.beginPath();
          ctx.strokeStyle = `rgba(130,180,255,${(1 - d / 130) * 0.25})`;
          ctx.lineWidth = 0.6;
          ctx.moveTo(pts[i].x, pts[i].y);
          ctx.lineTo(pts[j].x, pts[j].y);
          ctx.stroke();
        }
      }
    }
    pts.forEach(p => {
      ctx.beginPath();
      ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
      ctx.fillStyle = `rgba(160,200,255,${p.opacity})`;
      ctx.fill();
      p.x += p.vx; p.y += p.vy;
      if (p.x < 0 || p.x > canvas.width)  p.vx *= -1;
      if (p.y < 0 || p.y > canvas.height) p.vy *= -1;
    });
    animId = requestAnimationFrame(tick);
  };
  tick();
}
</script>

<template>
  <div class="root">

    <!-- ══ LEFT PANEL ══ -->
    <div class="left">
      <canvas ref="canvasRef" class="particles"></canvas>

      <div class="left-content">
        <div class="glow-orb"></div>

        <div class="left-body">
          <div class="brand-mark">
            <span class="brand-square"></span>
            HR System
          </div>

          <h1 class="left-title">Manage<br>Your Workforce<br>with Precision.</h1>
          <div class="pill-row">
            <span class="pill">EMPLOYEES</span>
            <span class="pill">DEPARTMENTS</span>
            <span class="pill">ANALYTICS</span>
          </div>
        </div>

        <div class="left-footer">v1.0 &middot; HR System &middot; .NET Core 10</div>
      </div>
    </div>

    <!-- ══ RIGHT PANEL ══ -->
    <div class="right">

      <!-- Circuit decoration -->
      <div class="circuit-wrap">
        <svg class="circuit-svg" viewBox="0 0 280 260" fill="none" xmlns="http://www.w3.org/2000/svg">
          <!-- Board base -->
          <rect x="30" y="60" width="180" height="160" rx="8"
                fill="#e8e0d4" stroke="#d0c4b4" stroke-width="1.5"/>
          <!-- Inner board -->
          <rect x="44" y="74" width="152" height="132" rx="4" fill="#dfd6c8"/>

          <!-- Horizontal traces -->
          <line x1="60" y1="104" x2="180" y2="104" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="60" y1="130" x2="120" y2="130" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="140" y1="130" x2="180" y2="130" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="60" y1="158" x2="100" y2="158" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="140" y1="158" x2="180" y2="158" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="60" y1="184" x2="180" y2="184" stroke="#c4b4a4" stroke-width="1.5"/>

          <!-- Vertical traces -->
          <line x1="80"  y1="104" x2="80"  y2="184" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="120" y1="104" x2="120" y2="184" stroke="#c4b4a4" stroke-width="1.5"/>
          <line x1="160" y1="104" x2="160" y2="184" stroke="#c4b4a4" stroke-width="1.5"/>

          <!-- IC Chip (center) -->
          <rect x="96" y="118" width="48" height="36" rx="3"
                fill="#ccc0b0" stroke="#b8a898" stroke-width="1.2"/>
          <rect x="102" y="124" width="36" height="24" rx="2" fill="#b8a898"/>
          <!-- Chip pins left -->
          <line x1="96" y1="123" x2="89" y2="123" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="96" y1="130" x2="89" y2="130" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="96" y1="137" x2="89" y2="137" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="96" y1="144" x2="89" y2="144" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <!-- Chip pins right -->
          <line x1="144" y1="123" x2="151" y2="123" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="144" y1="130" x2="151" y2="130" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="144" y1="137" x2="151" y2="137" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>
          <line x1="144" y1="144" x2="151" y2="144" stroke="#b0a090" stroke-width="2" stroke-linecap="round"/>

          <!-- Pads -->
          <circle cx="60"  cy="104" r="4.5" fill="#b8a898" stroke="#a89080" stroke-width="1"/>
          <circle cx="180" cy="104" r="4.5" fill="#b8a898" stroke="#a89080" stroke-width="1"/>
          <circle cx="60"  cy="184" r="4.5" fill="#e8905a" stroke="#d07040" stroke-width="1"/>
          <circle cx="180" cy="184" r="4.5" fill="#b8a898" stroke="#a89080" stroke-width="1"/>
          <circle cx="80"  cy="158" r="3.5" fill="#e8905a" stroke="#d07040" stroke-width="1"/>
          <circle cx="160" cy="130" r="3.5" fill="#b8a898" stroke="#a89080" stroke-width="1"/>
          <circle cx="120" cy="184" r="3.5" fill="#b8a898" stroke="#a89080" stroke-width="1"/>

          <!-- Small components -->
          <rect x="155" y="96" width="14" height="7" rx="1.5" fill="#c4b4a4" stroke="#b0a090" stroke-width="1"/>
          <rect x="71"  y="96" width="14" height="7" rx="1.5" fill="#c4b4a4" stroke="#b0a090" stroke-width="1"/>

          <!-- Magnifying glass -->
          <circle cx="210" cy="45" r="34" fill="rgba(225,215,200,0.45)"
                  stroke="#c8b8a4" stroke-width="2.5"/>
          <circle cx="210" cy="45" r="22" fill="rgba(240,233,220,0.6)"
                  stroke="#c0b098" stroke-width="1.8"/>
          <!-- Glass inner detail -->
          <circle cx="203" cy="38" r="5" fill="rgba(255,255,255,0.4)"/>
          <!-- Handle -->
          <line x1="228" y1="63" x2="252" y2="87"
                stroke="#c0b098" stroke-width="5" stroke-linecap="round"/>
          <line x1="228" y1="63" x2="252" y2="87"
                stroke="#d4c4b0" stroke-width="2.5" stroke-linecap="round"/>
        </svg>
      </div>

      <div class="form-wrap">
        <div class="form-header">
          <h2 class="form-title">Welcome back</h2>
          <p class="form-sub">Sign in to continue</p>
        </div>

        <!-- Error -->
        <Transition name="err">
          <div v-if="error" class="err-box">
            <svg width="13" height="13" viewBox="0 0 16 16" fill="currentColor">
              <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
              <path d="M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 4.995z"/>
            </svg>
            {{ error }}
          </div>
        </Transition>

        <form @submit.prevent="handleLogin" class="form">

          <!-- Company -->
          <div v-if="companies.length > 0" class="field">
            <label class="label">Company</label>
            <div class="company-wrap">
              <div class="select-btn" @click.stop="showDropdown = !showDropdown">
                <svg class="f-icon" viewBox="0 0 16 16" fill="currentColor" width="13" height="13">
                  <path d="M1 2.5A1.5 1.5 0 0 1 2.5 1h3A1.5 1.5 0 0 1 7 2.5v3A1.5 1.5 0 0 1 5.5 7h-3A1.5 1.5 0 0 1 1 5.5v-3zm8 0A1.5 1.5 0 0 1 10.5 1h3A1.5 1.5 0 0 1 15 2.5v3A1.5 1.5 0 0 1 13.5 7h-3A1.5 1.5 0 0 1 9 5.5v-3zm-8 8A1.5 1.5 0 0 1 2.5 9h3A1.5 1.5 0 0 1 7 10.5v3A1.5 1.5 0 0 1 5.5 15h-3A1.5 1.5 0 0 1 1 13.5v-3zm8 0A1.5 1.5 0 0 1 10.5 9h3a1.5 1.5 0 0 1 1.5 1.5v3a1.5 1.5 0 0 1-1.5 1.5h-3A1.5 1.5 0 0 1 9 13.5v-3z"/>
                </svg>
                <span class="select-text">{{ selectedCompany?.mainName ?? 'Select company…' }}</span>
                <svg class="chevron" :class="{ open: showDropdown }" viewBox="0 0 16 16" fill="currentColor" width="11">
                  <path d="M7.247 11.14 2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 0 1 .753 1.659l-4.796 5.48a1 1 0 0 1-1.506 0z"/>
                </svg>
              </div>

              <Transition name="drop">
                <div v-if="showDropdown" class="dropdown">
                  <div class="search-wrap">
                    <svg viewBox="0 0 16 16" fill="currentColor" width="12" height="12" class="search-icon">
                      <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.099zm-5.242 1.656a5.5 5.5 0 1 1 0-11 5.5 5.5 0 0 1 0 11z"/>
                    </svg>
                    <input v-model="searchQuery" class="search-input" placeholder="Search…" @click.stop />
                  </div>
                  <div class="drop-list">
                    <div
                      v-for="c in filteredCompanies" :key="c.mainCode"
                      class="drop-item" :class="{ active: c.mainCode === form.mainCode }"
                      @click="selectCompany(c.mainCode)"
                    >{{ c.mainName }}</div>
                    <div v-if="!filteredCompanies.length" class="drop-empty">No results</div>
                  </div>
                </div>
              </Transition>
            </div>
          </div>

          <!-- Username -->
          <div class="field">
            <label class="label">Username</label>
            <div class="input-wrap">
              <svg class="f-icon" viewBox="0 0 16 16" fill="currentColor">
                <path d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6zm2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0zm4 8c0 1-1 1-1 1H3s-1 0-1-1 1-4 6-4 6 3 6 4zm-1-.004c-.001-.246-.154-.986-.832-1.664C11.516 10.68 10.029 10 8 10s-3.516.68-4.168 1.332c-.678.678-.83 1.418-.832 1.664h10z"/>
              </svg>
              <input v-model="form.userId" type="text" class="f-input" placeholder="admin"
                     autocomplete="username" autofocus />
            </div>
          </div>

          <!-- Password -->
          <div class="field">
            <label class="label">Password</label>
            <div class="input-wrap">
              <svg class="f-icon" viewBox="0 0 16 16" fill="currentColor">
                <path d="M8 1a2 2 0 0 1 2 2v4H6V3a2 2 0 0 1 2-2zm3 6V3a3 3 0 0 0-6 0v4a2 2 0 0 0-2 2v5a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2z"/>
              </svg>
              <input v-model="form.userPass" :type="showPass ? 'text' : 'password'"
                     class="f-input" placeholder="••••••••" autocomplete="current-password" />
              <button type="button" class="eye-btn" @click="showPass = !showPass" tabindex="-1">
                <svg v-if="!showPass" viewBox="0 0 16 16" fill="currentColor" width="13" height="13">
                  <path d="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8zM1.173 8a13.133 13.133 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.133 13.133 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5c-2.12 0-3.879-1.168-5.168-2.457A13.134 13.134 0 0 1 1.172 8z"/>
                  <path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5zM4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0z"/>
                </svg>
                <svg v-else viewBox="0 0 16 16" fill="currentColor" width="13" height="13">
                  <path d="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7.028 7.028 0 0 0-2.79.588l.77.771A5.944 5.944 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.134 13.134 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755-.165.165-.337.328-.517.486l.708.709z"/>
                  <path d="M11.297 9.176a3.5 3.5 0 0 0-4.474-4.474l.823.823a2.5 2.5 0 0 1 2.829 2.829l.822.822zm-2.943 1.299.822.822a3.5 3.5 0 0 1-4.474-4.474l.823.823a2.5 2.5 0 0 0 2.829 2.829z"/>
                  <path d="M3.35 5.47c-.18.16-.353.322-.518.487A13.134 13.134 0 0 0 1.172 8l.195.288c.335.48.83 1.12 1.465 1.755C4.121 11.332 5.881 12.5 8 12.5c.716 0 1.39-.133 2.02-.36l.77.772A7.029 7.029 0 0 1 8 13.5C3 13.5 0 8 0 8s.939-1.721 2.641-3.238l.708.709zm10.296 8.884-12-12 .708-.708 12 12-.708.708z"/>
                </svg>
              </button>
            </div>
          </div>

          <!-- Submit -->
          <button type="submit" class="submit-btn" :disabled="loading">
            <span v-if="!loading">Sign in</span>
            <span v-else class="spinner"></span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&family=DM+Sans:wght@400;500&display=swap');

*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

/* ── Root ── */
.root {
  display: flex;
  min-height: 100vh;
  font-family: 'Outfit', sans-serif;
}

/* ══════════════════════════════
   LEFT PANEL
══════════════════════════════ */
.left {
  position: relative;
  width: 42%;
  flex-shrink: 0;
  background: linear-gradient(145deg, #0b1225 0%, #0f1e3d 50%, #132040 100%);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.particles {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
}

.left-content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 48px 44px;
}

.glow-orb {
  position: absolute;
  top: 80px;
  left: 30px;
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: radial-gradient(circle, #ff7043 0%, #ff4500 40%, transparent 70%);
  box-shadow: 0 0 40px 16px rgba(255, 100, 40, 0.35), 0 0 80px 30px rgba(255, 80, 20, 0.15);
  animation: pulse-orb 3s ease-in-out infinite;
}
@keyframes pulse-orb {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.08); opacity: 0.85; }
}

.left-body {
  margin-top: 120px;
  flex: 1;
}

.brand-mark {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
  font-weight: 600;
  color: rgba(160, 200, 255, 0.7);
  letter-spacing: 1px;
  text-transform: uppercase;
  margin-bottom: 32px;
}
.brand-square {
  width: 8px;
  height: 8px;
  background: #ff7043;
  border-radius: 2px;
  box-shadow: 0 0 10px rgba(255, 112, 67, 0.6);
}

.left-title {
  font-size: clamp(26px, 3vw, 36px);
  font-weight: 700;
  line-height: 1.2;
  color: #ffffff;
  letter-spacing: -0.5px;
  margin-bottom: 16px;
  animation: fade-up 0.8s ease both;
}
@keyframes fade-up {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}

.pill-row {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  animation: fade-up 0.8s 0.3s ease both;
}
.pill {
  font-size: 9.5px;
  font-weight: 600;
  letter-spacing: 0.8px;
  color: rgba(160, 200, 255, 0.6);
  border: 1px solid rgba(100, 150, 255, 0.2);
  border-radius: 20px;
  padding: 4px 12px;
  backdrop-filter: blur(4px);
  background: rgba(255,255,255,0.03);
}

.left-footer {
  font-size: 11px;
  color: rgba(100, 130, 180, 0.5);
  letter-spacing: 0.5px;
}

/* ══════════════════════════════
   RIGHT PANEL
══════════════════════════════ */
.right {
  flex: 1;
  position: relative;
  background: #f5f0e8;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 32px;
  overflow: hidden;
}

/* Circuit decoration */
.circuit-wrap {
  position: absolute;
  top: 0;
  right: 0;
  width: 220px;
  opacity: 0.72;
  pointer-events: none;
  animation: float-circ 7s ease-in-out infinite;
}
@keyframes float-circ {
  0%, 100% { transform: translateY(0px); }
  50%       { transform: translateY(-10px); }
}
.circuit-svg { width: 100%; height: auto; display: block; }

/* Form */
.form-wrap {
  width: 100%;
  max-width: 360px;
  position: relative;
  z-index: 1;
}

.form-header { margin-bottom: 28px; }

.form-title {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a2e;
  letter-spacing: -0.5px;
  margin-bottom: 4px;
}
.form-sub {
  font-size: 13px;
  color: #8a8070;
  font-family: 'DM Sans', sans-serif;
}

/* Error */
.err-box {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(220,38,38,.07);
  border: 1px solid rgba(220,38,38,.18);
  color: #dc2626;
  border-radius: 8px;
  padding: 10px 14px;
  font-size: 12px;
  margin-bottom: 18px;
}
.err-enter-active, .err-leave-active { transition: all .2s ease; }
.err-enter-from, .err-leave-to { opacity: 0; transform: translateY(-4px); }

/* Form layout */
.form { display: flex; flex-direction: column; gap: 16px; }

.field { display: flex; flex-direction: column; gap: 6px; }

.label {
  font-size: 11.5px;
  font-weight: 600;
  color: #1a1a2e;
  letter-spacing: 0.3px;
}

/* Company select */
.company-wrap { position: relative; }

.select-btn {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
  background: #fff;
  border: 1px solid #e0d8cc;
  border-radius: 10px;
  padding: 10px 14px;
  cursor: pointer;
  transition: border-color .15s, box-shadow .15s;
  user-select: none;
}
.select-btn:hover { border-color: #c8b8a8; }
.select-btn:focus-within { border-color: #e8906a; box-shadow: 0 0 0 3px rgba(232,144,106,.12); }

.f-icon {
  color: #a89880;
  flex-shrink: 0;
  width: 14px;
  height: 14px;
}

.select-text {
  flex: 1;
  font-size: 13px;
  color: #3a3020;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.chevron {
  color: #a89880;
  transition: transform .2s;
  flex-shrink: 0;
}
.chevron.open { transform: rotate(180deg); }

/* Dropdown */
.dropdown {
  position: absolute;
  top: calc(100% + 6px);
  left: 0;
  right: 0;
  background: #fff;
  border: 1px solid #e0d8cc;
  border-radius: 10px;
  box-shadow: 0 8px 24px rgba(0,0,0,.1);
  z-index: 100;
  overflow: hidden;
}
.drop-enter-active, .drop-leave-active { transition: all .18s ease; }
.drop-enter-from, .drop-leave-to { opacity: 0; transform: translateY(-6px); }

.search-wrap {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 14px;
  border-bottom: 1px solid #f0e8dc;
}
.search-icon { color: #a89880; flex-shrink: 0; }
.search-input {
  flex: 1;
  border: none;
  outline: none;
  font-size: 12.5px;
  color: #3a3020;
  background: transparent;
  font-family: 'Outfit', sans-serif;
}
.search-input::placeholder { color: #b8a898; }

.drop-list { max-height: 180px; overflow-y: auto; }
.drop-item {
  padding: 9px 14px;
  font-size: 12.5px;
  color: #3a3020;
  cursor: pointer;
  transition: background .1s;
}
.drop-item:hover { background: #f8f2ea; }
.drop-item.active { background: #f0e8dc; color: #c06840; font-weight: 500; }
.drop-empty { padding: 12px 14px; font-size: 12px; color: #a89880; text-align: center; }

/* Input fields */
.input-wrap {
  position: relative;
  display: flex;
  align-items: center;
}
.input-wrap .f-icon {
  position: absolute;
  left: 13px;
}
.f-input {
  width: 100%;
  background: #fff;
  border: 1px solid #e0d8cc;
  border-radius: 10px;
  padding: 10px 40px 10px 38px;
  font-size: 13px;
  font-family: 'Outfit', sans-serif;
  color: #1a1a2e;
  outline: none;
  transition: border-color .15s, box-shadow .15s;
}
.f-input::placeholder { color: #c0b0a0; }
.f-input:focus {
  border-color: #e8906a;
  box-shadow: 0 0 0 3px rgba(232,144,106,.12);
}
.eye-btn {
  position: absolute;
  right: 12px;
  background: none;
  border: none;
  cursor: pointer;
  color: #a89880;
  display: flex;
  align-items: center;
  padding: 4px;
  border-radius: 4px;
  transition: color .15s;
}
.eye-btn:hover { color: #3a3020; }

/* Submit button */
.submit-btn {
  margin-top: 6px;
  width: 100%;
  background: linear-gradient(135deg, #e8706a 0%, #e8509a 100%);
  color: #fff;
  border: none;
  border-radius: 10px;
  padding: 12px;
  font-size: 14px;
  font-weight: 600;
  font-family: 'Outfit', sans-serif;
  cursor: pointer;
  transition: opacity .15s, transform .1s, box-shadow .15s;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 44px;
  box-shadow: 0 4px 16px rgba(232,80,154,.3);
  letter-spacing: 0.3px;
}
.submit-btn:hover:not(:disabled) {
  opacity: 0.92;
  box-shadow: 0 6px 20px rgba(232,80,154,.4);
}
.submit-btn:active:not(:disabled) { transform: translateY(1px); }
.submit-btn:disabled { opacity: .65; cursor: not-allowed; }

.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,.3);
  border-top-color: #fff;
  border-radius: 50%;
  animation: spin .7s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── Responsive ── */
@media (max-width: 768px) {
  .left { display: none; }
  .right { background: #f5f0e8; }
}
</style>
