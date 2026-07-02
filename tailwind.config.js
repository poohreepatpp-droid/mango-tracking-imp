/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./pages/**/*.{vue,js,ts}",
    "./layouts/**/*.{vue,js,ts}",
    "./components/**/*.{vue,js,ts}",
    "./composables/**/*.{ts,js}",
  ],
  corePlugins: {
    // Disable preflight — app.css already has its own base reset
    preflight: false,
  },
  theme: {
    extend: {
      colors: {
        // Main surface
        bg:       "var(--bg)",
        surface:  "var(--surface)",
        surface2: "var(--surface2)",
        surface3: "var(--surface3)",
        border:   "var(--border)",
        border2:  "var(--border2)",

        // Accent
        accent:   "var(--accent)",

        // Semantic status
        "status-green": "var(--green)",
        "status-red":   "var(--red)",
        "status-blue":  "var(--blue)",
        "status-amber": "var(--amber)",

        // Text
        text:    "var(--text)",
        text2:   "var(--text-2)",
        text3:   "var(--text-3)",

        // Sidebar
        "sb-bg":      "var(--sb-bg)",
        "sb-border":  "var(--sb-border)",
        "sb-sect":    "var(--sb-sect)",
        "sb-link":    "var(--sb-link)",
        "sb-link-hi": "var(--sb-link-hi)",
        "sb-accent":  "var(--sb-accent)",
      },
      fontFamily: {
        display: ["Plus Jakarta Sans", "sans-serif"],
        mono:    ["JetBrains Mono", "monospace"],
      },
      borderRadius: {
        sm:  "var(--r)",
        md:  "var(--r-lg)",
        lg:  "var(--r-xl)",
      },
      spacing: {
        sidebar: "var(--sidebar-w)",
        header:  "var(--header-h)",
      },
      boxShadow: {
        xs: "var(--shadow-xs)",
        sm: "var(--shadow-sm)",
      },
    },
  },
  plugins: [],
}
