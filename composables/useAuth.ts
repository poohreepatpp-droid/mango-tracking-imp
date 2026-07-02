import { computed } from 'vue';
import { useState, useCookie, useRuntimeConfig, navigateTo } from 'nuxt/app';

export interface AuthUser {
  empno:           number;
  empcode:         string | null;
  userid:          string;
  maincode:        string;
  mainname:        string | null;
  is_authenticated: boolean;
  is_authen:       boolean;
  is_super_user:   boolean;
  is_admin:        boolean;
  is_admin_it:     boolean;
  session_id:      string | null;
  empname:         string | null;
  emppos:          string | null;
  emptel:          string | null;
  empmail:         string | null;
  dpt_code:        string | null;
  dpt_name:        string | null;
  dpt_name_eng:    string | null;
  sale:            string;
  lang_web:        string;
  level:           number | null;
  last_access:     string | null;
  session_expire:  string | null;
}

export const useAuth = () => {
  const config = useRuntimeConfig();
  const apiBase: string =
    import.meta.server && config.apiBaseInternal
      ? config.apiBaseInternal
      : config.public.apiBase;
  const token = useCookie<string | null>('mango_auth', { maxAge: 60 * 60 * 8, path: '/' });
  const user  = useState<AuthUser | null>('auth_user', () => null);

  const authHeaders = computed(() => ({
    'X-Mango-Auth': token.value ?? '',
  }));

  const login = async (userId: string, userPass: string, mainCode?: string) => {
    const res = await $fetch<{ success: boolean; data: string; user: { empNo: number; userId: string; fullName: string; level: number } }>(
      `${apiBase}/api/auth/login`,
      { method: 'POST', body: { userId, userPass, mainCode } }
    );
    token.value = res.data;
    // fetch full user data right after login
    await fetchMe();
    return res;
  };

  const logout = async () => {
    try {
      await $fetch(`${apiBase}/api/auth/logout`, {
        method: 'POST',
        headers: authHeaders.value,
      });
    } finally {
      token.value = null;
      user.value  = null;
      await navigateTo('/login');
    }
  };

  const fetchMe = async () => {
    if (!token.value) return null;
    try {
      const res = await $fetch<{ success: boolean; data: AuthUser }>(
        `${apiBase}/api/user/get_current_user_data`,
        { headers: authHeaders.value }
      );
      user.value = res.data;
      return res.data;
    } catch {
      token.value = null;
      user.value  = null;
      return null;
    }
  };

  const isLoggedIn = computed(() => !!token.value);

  return { token, user, isLoggedIn, authHeaders, login, logout, fetchMe };
};
