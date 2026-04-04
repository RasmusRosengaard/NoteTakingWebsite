<template>
  <div class="dashboard-wrapper">
    <header class="dash-header">
      <div class="header-left">
        <h1>My Canvases</h1>
        <p v-if="email" class="user-badge">Logged in as {{ email }}</p>
      </div>
      
      <div class="header-right">
        <div class="search-container">
          <span class="search-icon">🔍</span>
          <input 
            v-model="searchQuery" 
            placeholder="Search canvases..." 
            class="search-input"
          />
        </div>

        <button @click="showModal = true" class="btn-create">
          <span class="icon">+</span> New Canvas
        </button>
      </div>
    </header>

    <main class="content-area">
      <div v-if="loading" class="state-msg">
        <div class="spinner"></div>
        <p>Loading your creative space...</p>
      </div>

      <div v-else-if="filteredCanvases.length === 0" class="empty-state">
        <p>No canvases found. Create your first one to get started!</p>
      </div>

      <div v-else class="canvas-grid">
        <CanvasCard
          v-for="canvas in filteredCanvases"
          :key="canvas.id"
          :canvas="canvas"
          @delete="handleDeleteCanvas"
        />
      </div>
    </main>

    <CreateCanvasModal
      :show="showModal"
      @close="showModal = false"
      @create="createNewCanvas"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { parseJwt } from '@/Services/JWTService';
import { getUserCanvases, createCanvas, deleteCanvas } from '@/API/CanvasAPI';

import CanvasCard from '../Components/CanvasCard.vue';
import CreateCanvasModal from '../Components/CreateCanvasModal.vue';

const email = ref('');
const canvases = ref([]);
const loading = ref(true);
const showModal = ref(false);
const searchQuery = ref('');

const filteredCanvases = computed(() => {
  return canvases.value.filter(c => 
    c.title.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

onMounted(async () => {
  const token = localStorage.getItem('token');
  if (token) {
    const decoded = parseJwt(token);
    email.value = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '';
    try {
      canvases.value = await getUserCanvases(token);
    } catch (err) {
      console.error(err);
    }
  }
  loading.value = false;
});

async function createNewCanvas(title) {
  const token = localStorage.getItem('token');
  try {
    const newCanvas = await createCanvas(token, title);
    canvases.value.push(newCanvas);
    showModal.value = false;
  } catch {
    alert('Failed to create canvas');
  }
}

async function handleDeleteCanvas(id) {
  if (!confirm("Delete this canvas and all its notes?")) return;

  const token = localStorage.getItem('token');
  if (await deleteCanvas(token, id)) {
    canvases.value = canvases.value.filter(c => c.id !== id);
  }
}
</script>

<style scoped>
.dashboard-wrapper {
  padding: 2rem 4rem;
  min-height: 100vh;
}

.dash-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid var(--border);
}

.user-badge {
  font-size: 0.85rem;
  color: var(--text-muted);
  margin-top: 4px;
}

.header-right {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.search-container {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 12px;
  opacity: 0.5;
}

.search-input {
  padding-left: 2.5rem;
  width: 250px;
  background: white;
  height: 40px; /* fixed height to match button */
  border-radius: 10px; /* same as button for visual consistency */
  border: 1px solid var(--border);
  box-sizing: border-box; /* include padding in height */
  font-size: 0.95rem;
}

.btn-create {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0 1.5rem;
  height: 40px; /* match input */
  border-radius: 10px;
  font-weight: 500;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.canvas-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.state-msg { text-align: center; padding: 5rem; }

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e2e8f0;
  border-top-color: var(--primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

.btn-create:hover {
  opacity: 0.9;
}

.icon {
  font-size: 1.2rem;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>