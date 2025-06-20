<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { ref, computed } from 'vue'

// Mock auth state (replace with Pinia store later)
const isAuthenticated = computed(() => !!localStorage.getItem('auth_token'))
const router = useRouter()

function logout() {
  localStorage.removeItem('auth_token')
  router.push('/login')
}
</script>

<template>
  <header>
    <nav class="navbar">
      <div class="navbar-content">
        <RouterLink to="/">TaskManager</RouterLink>
        <div class="nav-links">
          <RouterLink v-if="!isAuthenticated" to="/login">Login</RouterLink>
          <RouterLink v-if="!isAuthenticated" to="/register">Register</RouterLink>
          <RouterLink v-if="isAuthenticated" to="/dashboard">Dashboard</RouterLink>
          <button v-if="isAuthenticated" @click="logout">Logout</button>
        </div>
      </div>
    </nav>
  </header>
  <main>
    <RouterView />
  </main>
</template>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 2rem;
  background: #42b983;
  color: white;
}
.navbar a {
  color: white;
  text-decoration: none;
  font-weight: bold;
  margin-right: 1.5rem;
}
.navbar a:last-child {
  margin-right: 0;
}
.nav-links {
  display: flex;
  align-items: center;
}
.nav-links button {
  background: transparent;
  border: none;
  color: white;
  font-weight: bold;
  cursor: pointer;
  margin-left: 1rem;
}
main {
  min-height: 80vh;
}
</style>
