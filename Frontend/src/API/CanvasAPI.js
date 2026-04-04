const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5237/api';

export async function getUserCanvases(token) {
  const response = await fetch(`${API_BASE}/canvas`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return await response.json();
}

export async function getCanvasById(token, canvasId) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return await response.json();
}

export async function createCanvas(token, title) {
  const response = await fetch(`${API_BASE}/canvas`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
    body: JSON.stringify({ title })
  });
  return await response.json();
}

export async function deleteCanvas(token, canvasId) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` }
  });
  return response.ok;
}

// --- NEW BULK SYNC ---
export async function syncCanvasNotes(token, canvasId, notes) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}/sync`, {
    method: 'PUT',
    headers: { 
      'Content-Type': 'application/json', 
      'Authorization': `Bearer ${token}` 
    },
    body: JSON.stringify({ notes })
  });
  if (!response.ok) throw new Error('Failed to sync canvas');
  return await response.json();
}