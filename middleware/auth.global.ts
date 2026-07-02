export default defineNuxtRouteMiddleware(async (to) => {
  if (to.path === '/login') return;

  const { token, fetchMe } = useAuth();

  if (!token.value) {
    return navigateTo('/login');
  }

  const { user } = useAuth();
  if (!user.value) {
    const me = await fetchMe();
    if (!me) return navigateTo('/login');
  }
});
