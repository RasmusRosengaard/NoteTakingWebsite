<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Login</h1>
      <p class="auth-subtitle">Welcome back! Please enter your details.</p>
      
      <div class="form-group">
        <label>Email Address</label>
        <input v-model="email" type="email" placeholder="name@company.com" />
      </div>

      <div class="form-group">
        <label>Password</label>
        <input v-model="password" type="password" placeholder="••••••••" />
      </div>

      <button @click="login" class="btn-primary auth-btn">Login</button>
      
      <p class="auth-footer">
        Don't have an account? <router-link to="/register">Register here</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { LoginAPI } from '../API/Backend';
import { useRouter } from 'vue-router';

const email = ref('');
const password = ref('');
const router = useRouter();

function login() {
    LoginAPI(email.value, password.value)
        .then(data => {
            if (data?.token) {
                localStorage.setItem('token', data.token);
                router.push('/canvas');
            }
        })
        .catch(err => alert('Login failed. Please check your credentials.'));
}
</script>