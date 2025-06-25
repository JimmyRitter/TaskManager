<script setup lang="ts">
import { RouterView, useRouter, useRoute } from 'vue-router'
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Navbar from './components/Navbar.vue'
import PomodoroWidget from './components/PomodoroWidget.vue'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()

onMounted(() => {
  // Define redirect function for token expiration
  const redirectToLogin = () => {
    // Only redirect if not already on login/register/landing pages
    if (!['login', 'register', 'landing'].includes(route.name as string)) {
      console.log('Redirecting to login due to expired token...')
      router.push({ name: 'login' })
    }
  }

  // Initialize auth system with redirect callback
  auth.initializeAuth(redirectToLogin)
})
</script>

<template>
  <Navbar />
  <main>
    <RouterView />
  </main>
  <PomodoroWidget />
</template>

<style scoped>
main {
  min-height: calc(100vh - 4rem);
  /* padding: 2rem 1rem; */
}
</style>
