export default defineNuxtConfig({
  compatibilityDate: "2024-11-01",
  devtools: { enabled: true },
  modules: ["@nuxtjs/tailwindcss"],
  css: ["~/assets/css/app.css"],
  tailwindcss: {
    cssPath: "~/assets/css/tailwind.css",
    configPath: "tailwind.config.js",
  },
  runtimeConfig: {
    // Server-side only (SSR → backend via Docker internal network)
    apiBaseInternal: process.env.API_BASE_INTERNAL ?? "",
    public: {
      // Client-side (browser → backend via public host/port)
      apiBase: process.env.API_BASE_URL ?? "http://localhost:5187",
    },
  },
});
