import { createRouter, createWebHistory } from 'vue-router';

import Login from '../Pages/Login.vue';
import Register from '@/Pages/Register.vue';
import FrontPage from '@/Pages/FrontPage.vue';
import CanvasDashboard from '../Pages/CanvasDashboard.vue';
import CanvasView from '../Pages/CanvasView.vue'; 
import { parseJwt, isTokenExpired } from '@/Services/JWTService';

const routes = [
  {
    path: '/canvas',
    name: 'CanvasDashboard',
    component: CanvasDashboard,
    meta: { requiresAuth: true } 
  },
  {
    path: '/canvas/:canvasId',      
    name: 'CanvasView',
    component: CanvasView,
    meta: { requiresAuth: true }
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { guestOnly: true } 
  },
  {
    path: '/register',
    name: 'Register',
    component: Register,
    meta: { guestOnly: true }
  },
  {
    path: '/',
    name: 'FrontPage',
    component: FrontPage
  }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    const isAuthenticated = !!token && !isTokenExpired(token);


    if (!isAuthenticated && token) {
        localStorage.removeItem('token');
    }

    if (to.meta.guestOnly && isAuthenticated) {
        return next('/canvas'); // redirect logged-in users
    }

    if (to.meta.requiresAuth && !isAuthenticated) {
        return next('/login'); // redirect non-authenticated users
    }

    // Optional: role-based route check if you store role in token
    if (to.meta.requiredRole && isAuthenticated) {
        const payload = parseJwt(token);
        if (payload?.role !== to.meta.requiredRole) {
            return next('/'); // redirect if role mismatch
        }
    }

    next(); 
    
});

export default router;