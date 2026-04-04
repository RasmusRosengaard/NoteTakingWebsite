<template>
  <div class="canvas-card-wrapper">
    <router-link
      :to="{ name: 'CanvasView', params: { canvasId: canvas.id } }"
      class="canvas-card"
    >
      <div class="card-icon">📁</div>
      <h3>{{ canvas.title }}</h3>
      <span class="card-link">Open Project →</span>
    </router-link>

    <button
      @click="$emit('delete', canvas.id)"
      class="delete-card-btn"
    >
      ×
    </button>
  </div>
</template>

<script setup>
defineProps({ canvas: Object });
defineEmits(['delete']);
</script>

<style scoped>
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

.canvas-card:hover {
  transform: translateY(-5px);
  border-color: var(--primary);
}

.card-icon { font-size: 2rem; }

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
  cursor: pointer;
  opacity: 0;
}

.canvas-card-wrapper:hover .delete-card-btn {
  opacity: 1;
}
</style>