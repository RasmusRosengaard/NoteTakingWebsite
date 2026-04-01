<template>
  <Navbar></Navbar>
  <h1>Notes</h1>
  <p v-if="email">Logged in as: {{ email }}</p>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import Navbar from '@/Components/Navbar.vue';
import { parseJwt } from '@/Services/JWTService';
const email = ref('');

onMounted(() => {
  const token = localStorage.getItem('token');
  if (token) {
    const decoded = parseJwt(token);
    if (decoded) {
      
      email.value = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '';
    }
  }
});
</script>