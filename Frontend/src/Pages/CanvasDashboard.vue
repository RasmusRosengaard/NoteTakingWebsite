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
        <div v-for="canvas in filteredCanvases" :key="canvas.id" class="canvas-card-wrapper">
          <router-link :to="{ name: 'CanvasView', params: { canvasId: canvas.id } }" class="canvas-card">
            <div class="card-icon">📁</div>
            <h3>{{ canvas.title }}</h3>
            <span class="card-link">Open Project &rarr;</span>
          </router-link>
          <button @click="handleDeleteCanvas(canvas.id)" class="delete-card-btn" title="Delete Canvas">×</button>
        </div>
      </div>
    </main>

    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
        <div class="modal-card">
          <h2>Create New Canvas</h2>
          <p>Give your new project a name to get started.</p>
          <form @submit.prevent="createNewCanvas">
            <input 
              v-model="newTitle" 
              placeholder="e.g. Brainstorming Session" 
              ref="modalInput"
              required 
              autofocus
            />
            <div class="modal-actions">
              <button type="button" @click="showModal = false" class="btn-cancel">Cancel</button>
              <button type="submit" class="btn-confirm">Create Project</button>
            </div>
          </form>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { parseJwt } from '@/Services/JWTService';
import { getUserCanvases, createCanvas, deleteCanvas } from '@/API/CanvasAPI';

const email = ref('');
const canvases = ref([]);
const loading = ref(true);
const showModal = ref(false);
const newTitle = ref('');
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
    try { canvases.value = await getUserCanvases(token); } catch (err) { console.error(err); }
  }
  loading.value = false;
});

async function createNewCanvas() {
  const token = localStorage.getItem('token');
  try {
    const newCanvas = await createCanvas(token, newTitle.value);
    canvases.value.push(newCanvas);
    newTitle.value = '';
    showModal.value = false;
  } catch (err) { alert('Failed to create canvas'); }
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
/* Full-Width Layout Fix */
.dashboard-wrapper {
  padding: 2rem 4rem; /* Wide padding for 1920px screens */
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

/* Search Bar Styling */
.search-container {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 12px;
  font-size: 0.9rem;
  opacity: 0.5;
}

.search-input {
  padding-left: 2.5rem;
  width: 250px;
  background: white;
}

/* Grid Layout */
.canvas-grid {
  display: grid;
  /* Larger cards for 1080p screens */
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.canvas-card-wrapper { position: relative; }

.canvas-card {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  background: var(--bg-card);
  padding: 2rem;
  border-radius: 16px;
  border: 1px solid var(--border);
  text-decoration: none;
  color: inherit;
  transition: all 0.3s ease;
  box-shadow: var(--shadow);
}

.card-icon { font-size: 2rem; }

.canvas-card:hover {
  transform: translateY(-5px);
  border-color: var(--primary);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.canvas-card h3 { margin: 0; font-size: 1.2rem; }
.card-link { font-size: 0.85rem; font-weight: 600; color: var(--primary); }

.delete-card-btn {
  position: absolute;
  top: 15px;
  right: 15px;
  background: #fee2e2;
  color: #ef4444;
  border: none;
  border-radius: 8px;
  width: 32px;
  height: 32px;
  font-size: 1.2rem;
  cursor: pointer;
  opacity: 0;
  transition: opacity 0.2s;
}

.canvas-card-wrapper:hover .delete-card-btn { opacity: 1; }

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.modal-card {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.modal-card h2 { margin-top: 0; }
.modal-card input { width: 100%; margin: 1.5rem 0; box-sizing: border-box; }

.modal-actions { display: flex; justify-content: flex-end; gap: 1rem; }

.btn-create {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 10px;
  font-weight: 700;
}

.btn-confirm { background: var(--primary); color: white; border: none; padding: 0.7rem 1.2rem; border-radius: 8px; font-weight: 600; }
.btn-cancel { background: #f1f5f9; border: none; padding: 0.7rem 1.2rem; border-radius: 8px; font-weight: 600; color: var(--text-muted); }

/* Transitions */
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

.state-msg { text-align: center; padding: 5rem; }
.spinner { width: 40px; height: 40px; border: 4px solid #e2e8f0; border-top-color: var(--primary); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 1rem; }
@keyframes spin { to { transform: rotate(360deg); } }

/* Media Query for very wide screens */
@media (min-width: 1900px) {
  .dashboard-wrapper { padding: 4rem 10rem; }
}
</style>