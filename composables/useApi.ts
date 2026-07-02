export const useApi = () => {
  const config = useRuntimeConfig();
  const { authHeaders } = useAuth();

  // import.meta.server is a compile-time constant — Nitro tree-shakes the
  // unused branch, so the result is always a plain string (no .value needed).
  const apiBase: string =
    import.meta.server && config.apiBaseInternal
      ? config.apiBaseInternal          // SSR inside Docker → http://backend:5000
      : config.public.apiBase;          // Browser → http://host:5000

  const apiFetch = <T>(path: string, options?: Parameters<typeof $fetch>[1]) =>
    $fetch<T>(`${apiBase}${path}`, {
      ...options,
      headers: { ...authHeaders.value, ...(options?.headers ?? {}) },
    });

  return { apiFetch, apiBase };
};
