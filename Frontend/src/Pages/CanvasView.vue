<template>
  <div class="canvas-wrapper">
    <!-- Toolbar component -->
    <CanvasToolbar
      :title="canvas?.title || 'Loading...'"
      :isSaving="isSaving"
      @add-note="addNewNoteLocal"
      @save="saveAllNotes"
      @back="goBack"
    />

    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
      <p>Opening your canvas...</p>
    </div>

    <div v-else class="canvas-viewport">
      <div class="canvas-area">
        <div
          v-for="note in canvas.notes"
          :key="note.id || note.tempId"
          :ref="(el) => setInteractable(el, note)"
          class="note-box"
          :style="{
            transform: `translate(${note.x}px, ${note.y}px)`,
            width: `${note.width}px`,
            height: `${note.height}px`,
            zIndex: note.zIndex || 10
          }"
          @mousedown="bringToFront(note)"
        >
          <button class="delete-note-btn" @click.stop="removeNoteLocal(note)">×</button>

          <div class="note-handle"></div>
          <textarea
            v-model="note.content"
            class="note-textarea"
            placeholder="Notes..."
          ></textarea>

          <div class="resize-handle-br"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import interact from 'interactjs';
import { getCanvasById, syncCanvasNotes } from '@/API/CanvasAPI';
import CanvasToolbar from '@/Components/CanvasToolbar.vue';

const route = useRoute();
const router = useRouter();
const canvasId = route.params.canvasId;

const canvas = ref({ notes: [] });
const loading = ref(true);
const isSaving = ref(false);
const maxZIndex = ref(100);

const bringToFront = (note) => { maxZIndex.value++; note.zIndex = maxZIndex.value; };

const setInteractable = (el, note) => {
  if (!el) return;
  interact(el)
    .draggable({
      allowFrom: '.note-handle',
      listeners: { move(event) { note.x += event.dx; note.y += event.dy; } },
      modifiers: [interact.modifiers.restrictRect({ restriction: 'parent', endOnly: true })]
    })
    .resizable({
      edges: { right: '.resize-handle-br', bottom: '.resize-handle-br' },
      listeners: { move(event) { note.width = event.rect.width; note.height = event.rect.height; } },
      modifiers: [
        interact.modifiers.restrictSize({ min: { width: 160, height: 120 } }),
        interact.modifiers.restrictRect({ restriction: 'parent', endOnly: true })
      ]
    });
};

onMounted(async () => {
  const token = localStorage.getItem('token');
  try {
    const data = await getCanvasById(token, canvasId);
    canvas.value = {
      ...data,
      notes: data.notes.map((n, i) => ({ ...n, zIndex: 10 + i }))
    };
    maxZIndex.value = 10 + data.notes.length;
  } catch (err) {
    console.error(err);
  } finally {
    loading.value = false;
  }
});


function addNewNoteLocal() {
  maxZIndex.value++;
  canvas.value.notes.push({
    tempId: Date.now(),
    content: '',
    x: 150, y: 150, width: 240, height: 180,
    zIndex: maxZIndex.value
  });
}


function removeNoteLocal(note) {
  canvas.value.notes = canvas.value.notes.filter(n => note.id ? n.id !== note.id : n.tempId !== note.tempId);
}

async function saveAllNotes() {
  if (isSaving.value) return;
  const token = localStorage.getItem('token');
  isSaving.value = true;
  try {
    const freshNotes = await syncCanvasNotes(token, canvasId, canvas.value.notes);
    canvas.value.notes = freshNotes.map((n, i) => ({ ...n, zIndex: 10 + i }));
  } catch (err) {
    console.error('Failed to sync canvas');
  } finally {
    isSaving.value = false;
  }
}


function goBack() {
  router.push('/canvas');
}
</script>

<style scoped>
.canvas-wrapper {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: var(--bg-app);
  overflow: hidden;
}

.canvas-viewport {
  flex-grow: 1;
  position: relative;
  overflow: auto;
}

.canvas-area {
  position: relative;
  width: 5000px;
  height: 5000px;
  background-color: transparent;
}

/* Notes styling */
.note-box {
  position: absolute;
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.05),
              0 4px 6px -2px rgba(0, 0, 0, 0.05);
  border: 1px solid var(--border);
  display: flex;
  flex-direction: column;
  overflow: visible;
  transition: box-shadow 0.2s, border-color 0.2s;
}
.note-box:hover {
  border-color: var(--primary);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.note-handle {
  height: 32px;
  width: 100%;
  cursor: grab;
  border-radius: 12px 12px 0 0;
}
.note-handle:active { cursor: grabbing; }

.note-textarea {
  flex-grow: 1;
  padding: 0 1.25rem 1.25rem;
  border: none;
  resize: none;
  outline: none;
  font-size: 1rem;
  line-height: 1.6;
  color: var(--text-main);
  background: transparent;
  font-family: inherit;
}

/* Delete button */
.delete-note-btn {
  position: absolute;
  top: -10px;
  right: -10px;
  width: 28px;
  height: 28px;
  background: #fff;
  color: #ef4444;
  border: 1px solid #fee2e2;
  border-radius: 50%;
  cursor: pointer;
  display: none;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  box-shadow: var(--shadow);
  z-index: 50;
}
.note-box:hover .delete-note-btn { display: flex; }
.delete-note-btn:hover { background: #fee2e2; }

/* Resize handle */
.resize-handle-br {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 20px;
  height: 20px;
  cursor: nwse-resize;
  background: linear-gradient(135deg, transparent 50%, var(--border) 50%);
  border-radius: 0 0 12px 0;
}

/* Buttons */
.btn-primary {
  background: var(--primary);
  color: white;
  border: none;
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-weight: 700;
}
.btn-secondary {
  background: white;
  color: var(--text-main);
  border: 1px solid var(--border);
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-weight: 700;
  margin-right: 10px;
}

/* Loading spinner */
.spinner {
  width: 40px;
  height: 40px;
  border: 3px solid var(--border);
  border-top-color: var(--primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 20px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }
</style>