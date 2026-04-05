<template>
  <div class="app-layout">
    <Navbar />
    <router-view />
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { checkToken } from './Services/JWTService';
import { useRouter } from 'vue-router';
import Navbar from './Components/Navbar.vue';

const router = useRouter(); 

onMounted(() => {
  checkToken(router); 
});
</script>

<style>
:root {
  --primary: #6366f1;        
  --primary-hover: #4f46e5;   
  --bg-app: #f1f5f9;         
  --bg-card: #ffffff;        
  --text-main: #1e293b;      
  --text-muted: #64748b;   
  --border: #e2e8f0;        
  --shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
}

body {
  margin: 0;
  padding: 0;
  font-family: 'Inter', system-ui, -apple-system, sans-serif;
  background-color: var(--bg-app);
  color: var(--text-main);
  line-height: 1.5;
  -webkit-font-smoothing: antialiased;
}

h1 { font-size: 1.8rem; font-weight: 800; color: var(--text-main); letter-spacing: -0.025em; }
p { color: var(--text-muted); }

button { cursor: pointer; transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); }

input { 
  border-radius: 8px; 
  border: 1px solid var(--border); 
  padding: 10px 14px; 
  font-size: 0.95rem;
  transition: border-color 0.2s;
}

input:focus {
  outline: none;
  border-color: var(--primary);
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.1);
}

/* Auth Pages Layout */
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 64px); 
  background-color: var(--bg-app);
}

.auth-card {
  background: var(--bg-card);
  padding: 2.5rem;
  border-radius: 16px;
  box-shadow: var(--shadow);
  width: 100%;
  max-width: 400px;
  border: 1px solid var(--border);
}

.auth-card h1 { margin: 0 0 0.5rem 0; text-align: center; }

.auth-subtitle {
  text-align: center;
  font-size: 0.9rem;
  color: var(--text-muted);
  margin-bottom: 2rem;
}

.form-group {
  margin-bottom: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--text-main);
}

.auth-btn {
  width: 100%;
  background-color: var(--primary);
  color: white;
  border: none;
  padding: 0.75rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1rem;
}

.auth-btn:hover { background-color: var(--primary-hover); transform: translateY(-1px); }

.auth-footer {
  margin-top: 1.5rem;
  text-align: center;
  font-size: 0.85rem;
  color: var(--text-muted);
}

.auth-footer a {
  color: var(--primary);
  font-weight: 600;
  text-decoration: none;
}
</style>