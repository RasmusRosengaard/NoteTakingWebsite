<template>
  <Transition name="fade">
    <div v-if="show" class="modal-overlay" @click.self="$emit('close')">
      <div class="modal-card">
        <h2>Create New Canvas</h2>
        <p class="modal-subtitle">Give your new project a name to get started.</p>

        <form @submit.prevent="handleSubmit">
          <input
            v-model="title"
            placeholder="e.g. Mathematical Concepts"
            required
            class="modal-input"
            autofocus
          />

          <div class="modal-actions">
            <button type="button" @click="$emit('close')" class="btn-cancel">
              Cancel
            </button>
            <button type="submit" class="btn-confirm">
              Create Project
            </button>
          </div>
        </form>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref } from 'vue';

defineProps({ show: Boolean });

const emit = defineEmits(['close', 'create']);
const title = ref('');

function handleSubmit() {
  if (!title.value.trim()) return;
  emit('create', title.value);
  title.value = '';
}
</script>

<style scoped>
/* Overlay background */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

/* Card container */
.modal-card {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  width: 100%;
  max-width: 450px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.modal-card h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
}

.modal-subtitle {
  font-size: 0.95rem;
  color: var(--text-muted);
  margin-bottom: 1rem;
}

/* Input styling */
.modal-input {
  width: 100%;
  padding: 0.8rem 1rem;
  font-size: 1rem;
  border: 1px solid var(--border);
  border-radius: 10px;
  box-sizing: border-box;
}

/* Actions row */
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

/* Buttons */
.btn-confirm {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0.7rem 1.2rem;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.btn-confirm:hover {
  opacity: 0.9;
}

.btn-cancel {
  background: #f1f5f9;
  border: none;
  padding: 0.7rem 1.2rem;
  border-radius: 8px;
  font-weight: 600;
  color: var(--text-muted);
  cursor: pointer;
}

.btn-cancel:hover {
  background: #e2e8f0;
}

/* Fade animation */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>