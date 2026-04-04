<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Register</h1>
      <p class="auth-subtitle">Create an account to start managing your canvases.</p>

      <div class="form-group">
        <label>Email Address</label>
        <input v-model="email" type="email" placeholder="name@company.com" />
      </div>

      <div class="form-group">
        <label>Password</label>
        <input v-model="password" type="password" placeholder="Min. 8 characters" />
      </div>

      <button @click="register" class="btn-primary auth-btn">Create Account</button>

      <p class="auth-footer">
        Already have an account? <router-link to="/login">Login here</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { LoginAPI, RegisterAPI } from '../API/Backend';
import { useRouter } from 'vue-router';

const email = ref('');
const password = ref('');
const router = useRouter();

function register() {
    RegisterAPI(email.value, password.value)
        .then(() => LoginAPI(email.value, password.value)) 
        .then(data => {
            if (data?.token) {
                localStorage.setItem('token', data.token);
                router.push('/');
            }
        })
        .catch(err => alert('Registration failed. Email might already be in use.'));
}
</script>