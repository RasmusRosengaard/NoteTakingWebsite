<template>
  <nav class="navbar">
    <div class="nav-container">
      <div class="nav-brand">
        <router-link to="/" class="brand-link">
          <span class="brand-text">MyNote
          </span>
        </router-link>
      </div>

      <div class="nav-links">
        <template v-if="isLoggedIn">
          <router-link to="/canvas" class="nav-item">Canvases</router-link>
          <button @click="handleLogout" class="btn-logout">Logout</button>
        </template>
        <template v-else>
          <router-link to="/login" class="nav-item">Login</router-link>
          <router-link to="/register" class="btn-signup">Sign Up</router-link>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const isLoggedIn = ref(false);

const checkAuth = () => {
  isLoggedIn.value = !!localStorage.getItem('token');
};

watch(() => route.path, () => checkAuth());
onMounted(() => checkAuth());

function handleLogout() {
  localStorage.removeItem('token');
  checkAuth();
  router.push('/login');
}
</script>

<style scoped>
.navbar {
  background-color: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid var(--border);
  height: 64px;
  display: flex;
  align-items: center;
  position: sticky;
  top: 0;
  z-index: 1000;
  width: 100%;
}

.nav-container {
  width: 100%;
  max-width: none; /* Removed the 1400px limit */
  margin: 0;       /* Removed the centering */
  padding: 0 2rem; /* Keeps a small 32px gap from the edge */
  display: flex;
  justify-content: space-between; 
  align-items: center;
  box-sizing: border-box;
}

.brand-link {
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.brand-dot {
  width: 12px;
  height: 12px;
  background: linear-gradient(135deg, var(--primary), #a855f7);
  border-radius: 3px;
  transform: rotate(45deg);
}

.brand-text {
  font-weight: 800;
  font-size: 1.25rem;
  letter-spacing: -0.03em;
  color: var(--text-main);
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.nav-item {
  text-decoration: none;
  color: var(--text-muted);
  font-size: 0.9rem;
  font-weight: 600;
  padding: 0.5rem 0.8rem;
  border-radius: 8px;
}

.btn-signup {
  background-color: var(--primary);
  color: white;
  text-decoration: none;
  padding: 0.5rem 1.25rem;
  border-radius: 8px;
  font-weight: 600;
}

.btn-logout {
  background: #fff1f2;
  border: 1px solid #fecaca;
  color: #e11d48;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-weight: 600;
}
</style>