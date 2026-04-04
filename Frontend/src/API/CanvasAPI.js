const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5237/api';

const getHeaders = (token) => ({
  'Authorization': `Bearer ${token}`,
  'Content-Type': 'application/json'
});

export async function getUserCanvases(token) {
  const response = await fetch(`${API_BASE}/canvas`, { headers: getHeaders(token) });
  return await response.json();
}

export async function getCanvasById(token, canvasId) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}`, { headers: getHeaders(token) });
  return await response.json();
}

export async function createCanvas(token, title) {
  const response = await fetch(`${API_BASE}/canvas`, {
    method: 'POST',
    headers: getHeaders(token),
    body: JSON.stringify({ title })
  });
  return await response.json();
}

export async function deleteCanvas(token, canvasId) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}`, {
    method: 'DELETE',
    headers: getHeaders(token)
  });
  return response.ok;
}

export async function syncCanvasNotes(token, canvasId, notes) {
  const response = await fetch(`${API_BASE}/canvas/${canvasId}/sync`, {
    method: 'PUT',
    headers: getHeaders(token),
    body: JSON.stringify({ notes })
  });
  if (!response.ok) throw new Error('Failed to sync canvas');
  return await response.json();
}