<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

const dropdownOpen = ref(false)

function logout() {
  auth.logout()
  dropdownOpen.value = false
  router.push('/login')
}

function toggleDropdown() {
  dropdownOpen.value = !dropdownOpen.value
}
function closeDropdown() {
  dropdownOpen.value = false
}
</script>

<template>
  <header>
    <nav class="navbar">
      <div class="navbar-content">
        <RouterLink to="/">TaskManager</RouterLink>
        <div class="nav-links">
          <RouterLink v-if="auth.token" to="/dashboard">Todo Lists</RouterLink>
          <RouterLink v-if="auth.token" to="/pomodoro">Pomodoro Timer</RouterLink>
          <RouterLink v-if="!auth.token" to="/login">Login</RouterLink>
          <RouterLink v-if="!auth.token" to="/register">Register</RouterLink>
          <div v-if="auth.token && auth.user" class="user-dropdown" @mouseleave="closeDropdown">
            <button class="user-btn" @click="toggleDropdown">
              Welcome, {{ auth.user.name }}
              <span class="arrow" :class="{ open: dropdownOpen }">▼</span>
            </button>
            <div v-if="dropdownOpen" class="dropdown-menu">
              <RouterLink to="/profile" @click="closeDropdown">Profile</RouterLink>
              <RouterLink to="/change-password" @click="closeDropdown">Change Password</RouterLink>
              <button @click="logout">Log out</button>
            </div>
          </div>
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
.user-dropdown {
  position: relative;
  display: flex;
  align-items: center;
}
.user-btn {
  background: transparent;
  border: none;
  color: white;
  font-weight: bold;
  cursor: pointer;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 0.3rem;
}
.arrow {
  font-size: 0.9em;
  transition: transform 0.2s;
}
.arrow.open {
  transform: rotate(180deg);
}
.dropdown-menu {
  position: absolute;
  right: 0;
  top: 2.2rem;
  background: white;
  color: #222;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(60,60,60,0.12);
  min-width: 160px;
  z-index: 100;
  display: flex;
  flex-direction: column;
  padding: 0.5rem 0;
}
.dropdown-menu a,
.dropdown-menu button {
  padding: 0.7rem 1.2rem;
  background: none;
  border: none;
  color: inherit;
  text-align: left;
  font-size: 1rem;
  cursor: pointer;
  text-decoration: none;
}
.dropdown-menu a:hover,
.dropdown-menu button:hover {
  background: #f2f2f2;
}
</style>
