import { createRouter, createWebHistory } from 'vue-router';

import Login from '../Pages/Login.vue';
import Register from '@/Pages/Register.vue';
import FrontPage from '@/Pages/FrontPage.vue';
import CanvasDashboard from '../Pages/CanvasDashboard.vue';
import CanvasView from '../Pages/CanvasView.vue'; // new page for individual canvas

const routes = [
  {
    path: '/canvas',
    name: 'CanvasDashboard',
    component: CanvasDashboard,
    meta: { requiresAuth: true } 
  },
  {
    path: '/canvas/:canvasId',      // <--- dynamic route
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

    const isAuthenticated = !!token;

    if (to.meta.guestOnly && isAuthenticated) {
        return next('/dashboard');
    }

    if (to.meta.requiresAuth && !isAuthenticated) {
        return next('/login');
    }

    next();
});

export default router;