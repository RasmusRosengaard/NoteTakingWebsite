<template>
  <div class="canvas-toolbar">
    <div class="toolbar-left">
      <button class="icon-btn back-btn" @click="handleBack" title="Back to Canvases">
        <span class="icon">&larr;</span>
      </button>
      <div class="divider"></div>
      <h1>{{ title }}</h1>
    </div>

    <div class="toolbar-actions">
      <button class="icon-btn add-note-btn" @click="onAddNote" title="Add Note">
        <span class="icon">+</span>
      </button>
      <button 
        class="icon-btn save-btn" 
        @click="onSave" 
        :disabled="isSaving"
        :title="isSaving ? 'Saving...' : 'Save Changes'"
      >
        <span class="icon">💾</span>
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';

defineProps({
  title: { type: String, default: '' },
  isSaving: { type: Boolean, default: false }
});

const emit = defineEmits(['add-note', 'save', 'back']);
const router = useRouter();

const onAddNote = () => emit('add-note');
const onSave = () => emit('save');

/**
 * Save before navigating back
 */
const handleBack = async () => {
  await emit('save');   // trigger save in parent
  emit('back');         // notify parent to navigate
};
</script>

<style scoped>
.canvas-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.5rem;
  height: 64px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(8px);
  border-bottom: 1px solid var(--border);
  z-index: 100;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.toolbar-left h1 {
  font-size: 1rem;
  font-weight: 700;
  color: var(--text-main);
  margin: 0;
}

.divider {
  width: 1px;
  height: 24px;
  background: var(--border);
}

/* Icon button styling */
.icon-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background: white;
  color: var(--text-main);
  font-size: 1.1rem;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.icon-btn:hover {
  background: #f8fafc;
  color: var(--primary);
  transform: translateY(-1px);
}

.icon-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.toolbar-actions {
  display: flex;
}

.add-note-btn .icon { font-size: 1.5rem; }
.save-btn .icon { font-size: 1.1rem; color: var(--primary); }
.back-btn .icon { font-size: 1.2rem; color: var(--text-muted); }
.back-btn:hover .icon { color: var(--primary); }
</style>