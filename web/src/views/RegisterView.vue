<template>
  <section class="auth-form">
    <h2>Register</h2>
    <form @submit.prevent="onRegister">
      <div>
        <label for="name">Name</label>
        <input id="name" v-model="name" type="text" required />
      </div>
      <div>
        <label for="email">Email</label>
        <input id="email" v-model="email" type="email" required />
      </div>
      <div>
        <label for="password">Password</label>
        <input id="password" v-model="password" type="password" required />
      </div>
      <button type="submit" :disabled="auth.loading">{{ auth.loading ? 'Registering...' : 'Register' }}</button>
      <p v-if="auth.error" class="error">{{ auth.error }}</p>
      <p v-if="success" class="success">Registration successful! Redirecting to login...</p>
    </form>
    <p>Already have an account? <RouterLink to="/login">Login</RouterLink></p>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const name = ref('')
const email = ref('')
const password = ref('')
const auth = useAuthStore()
const router = useRouter()
const success = ref(false)

async function onRegister() {
  await auth.register({ name: name.value, email: email.value, password: password.value })
  if (!auth.error) {
    success.value = true
    setTimeout(() => router.push('/login'), 1200)
  }
}
</script>

<style scoped>
.auth-form {
  max-width: 400px;
  margin: 4rem auto;
  padding: 2rem;
  border: 1px solid #eee;
  border-radius: 8px;
}
.auth-form h2 {
  margin-bottom: 1.5rem;
}
.auth-form form > div {
  margin-bottom: 1rem;
}
.auth-form label {
  display: block;
  margin-bottom: 0.5rem;
}
.auth-form input {
  width: 100%;
  padding: 0.5rem;
  border-radius: 4px;
  border: 1px solid #ccc;
}
.auth-form button {
  width: 100%;
  padding: 0.75rem;
  background: #42b983;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
}
.auth-form p {
  margin-top: 1rem;
  text-align: center;
}
.error {
  color: #d32f2f;
  margin-top: 1rem;
  text-align: center;
}
.success {
  color: #388e3c;
  margin-top: 1rem;
  text-align: center;
}
</style> 