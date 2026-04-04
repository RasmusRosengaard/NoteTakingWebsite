# Project Overview

This project uses JWT tokens for authorization instead of saving the user's email directly in localStorage.

It still uses localStorage to store the token, to minimize backend requests.

---

## IMPORTANT

Make sure your frontend runs on the port defined in `Program.cs` to allow it to send HTTP requests to the backend (default: `:5173`).

---

## To Run

### Backend
Navigate to the `webapi/` folder and run:
dotnet run

### Backend
Navigate to the `frontend/` folder and run:
npm run dev



# Future Work & Roadmap

## Core Note Features

### Snapping Logic
Implement grid-snapping for cleaner note alignment and organization.

### Note Titles
Add the ability to toggle optional titles/headers for individual notes.

### Visual Assets
Support for uploading custom cover images or selecting from a preset gallery for each canvas.

---

## Dashboard & Organization

### Search based on name (already implemented)
Search functionality to search by canvas name.
### Advanced Filtering
Sort canvases by name (alphabetical) or by the number of notes they contain.

### Metadata Indexing
Improved search functionality to scan note content across the dashboard.